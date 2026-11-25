using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>
/// Assistente de emissão do MDF-e em 5 etapas. Cada etapa valida o que é dela antes de avançar;
/// "Concluir" confere tudo e grava o MDF-e como "Pronto para transmitir". O documento fica gravado
/// no banco; transmissão, chave, protocolo e DAMDFE entram com o motor DF-e (P4b e P4c).
/// </summary>
public partial class FormEmissaoMdfe : Form
{
    private readonly ManifestoEletronico _mdfe;
    private readonly CadastrosEmissao _cadastros;
    private readonly UserControl[] _telas;
    private readonly IEtapaMdfe[] _etapas;
    private int _atual;
    private bool _ajustandoLista;
    private bool _gravado;
    private bool _gravando;

    /// <summary>Cria o assistente sobre a cópia do MDF-e.</summary>
    /// <param name="mdfe">Cópia editável do MDF-e (novo ou existente).</param>
    /// <param name="cadastros">Empresa emitente e cadastros lidos do banco ao abrir o assistente.</param>
    internal FormEmissaoMdfe(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
    {
        InitializeComponent();
        _mdfe = mdfe;
        _cadastros = cadastros;
        Text = mdfe.Id == 0
            ? $"Emitir MDF-e — {cadastros.Empresa.RazaoSocial}"
            : $"MDF-e de {mdfe.DataEmissao:dd/MM/yyyy} ({mdfe.SituacaoDescricao}) — {cadastros.Empresa.RazaoSocial}";

        _telas = [ucIdentificacao, ucCarregamentoDescarga, ucVeiculos, ucCargaSeguro, ucConferencia];
        _etapas = _telas.Cast<IEtapaMdfe>().ToArray();
        foreach (var etapa in _etapas)
        {
            etapa.Carregar(_mdfe, cadastros);
            lstEtapas.Items.Add(etapa.Titulo);
        }

        IrPara(0);
    }

    /// <summary>
    /// Atalhos do assistente: Ctrl+→/Ctrl+← avançam/voltam, Ctrl+S salva o rascunho,
    /// Ctrl+Enter conclui (só na última etapa), Esc cancela (com confirmação) e F1 mostra a ajuda.
    /// </summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.Right when btnAvancar.Visible:
                Avancar();
                return true;
            case Keys.Control | Keys.Left when btnVoltar.Enabled:
                Voltar();
                return true;
            case Keys.Control | Keys.S:
                _ = SalvarRascunhoAsync();
                return true;
            case Keys.Control | Keys.Enter when btnConcluir.Visible:
                _ = ConcluirAsync();
                return true;
            case Keys.Escape when !EscPertenceAoControle():
                Close();
                return true;
            case Keys.F1:
                AtalhosTeclado.MostrarAjuda(this);
                return true;
            default:
                return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    /// <summary>
    /// Esc deve primeiro desfazer a edição de uma célula da grade ou fechar uma lista aberta,
    /// e só depois cancelar o assistente.
    /// </summary>
    private bool EscPertenceAoControle()
    {
        Control? atual = ActiveControl;
        while (atual is ContainerControl { ActiveControl: { } interno })
        {
            atual = interno;
        }

        for (var controle = atual; controle is not null; controle = controle.Parent)
        {
            if (controle is DataGridView { IsCurrentCellInEditMode: true } or ComboBox { DroppedDown: true })
            {
                return true;
            }
        }

        return false;
    }

    private void btnAvancar_Click(object? sender, EventArgs e) => Avancar();

    private void btnVoltar_Click(object? sender, EventArgs e) => Voltar();

    private async void btnSalvarRascunho_Click(object? sender, EventArgs e) => await SalvarRascunhoAsync();

    private async void btnConcluir_Click(object? sender, EventArgs e) => await ConcluirAsync();

    private void btnCancelar_Click(object? sender, EventArgs e) => Close();

    /// <summary>Clicar numa etapa anterior volta para ela; as seguintes só pelo botão Avançar, que valida.</summary>
    private void lstEtapas_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_ajustandoLista || lstEtapas.SelectedIndex < 0 || lstEtapas.SelectedIndex == _atual)
        {
            return;
        }

        if (lstEtapas.SelectedIndex < _atual)
        {
            _etapas[_atual].Ler(_mdfe);
            IrPara(lstEtapas.SelectedIndex);
        }
        else
        {
            SelecionarNaLista(_atual);
        }
    }

    /// <summary>Pede confirmação antes de descartar o que foi digitado (botão Cancelar ou fechar a janela).</summary>
    private void FormEmissaoMdfe_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_gravado)
        {
            return;
        }

        var resposta = MessageBox.Show(this, "Fechar sem salvar? O que foi digitado será descartado.", Text,
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        e.Cancel = resposta != DialogResult.Yes;
    }

    private void Avancar()
    {
        var etapa = _etapas[_atual];
        etapa.Ler(_mdfe);
        if (AvisosCadastro.HaPendencias(this, etapa.Titulo, etapa.Validar(_mdfe)))
        {
            return;
        }

        IrPara(_atual + 1);
    }

    private void Voltar()
    {
        _etapas[_atual].Ler(_mdfe);
        IrPara(_atual - 1);
    }

    /// <summary>Grava como está, sem exigir os campos obrigatórios.</summary>
    private async Task SalvarRascunhoAsync()
    {
        _etapas[_atual].Ler(_mdfe);
        if (!await GravarAsync(SituacaoDocumento.EmDigitacao))
        {
            return;
        }

        MessageBox.Show(this, "Rascunho salvo. O MDF-e fica \"Em digitação\" na consulta.", Text,
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }

    /// <summary>Confere todas as etapas; na primeira com pendência, avisa e mostra a etapa.</summary>
    private async Task ConcluirAsync()
    {
        _etapas[_atual].Ler(_mdfe);
        for (var i = 0; i < _etapas.Length; i++)
        {
            if (i != _atual)
            {
                _etapas[i].Ler(_mdfe);
            }
        }

        for (var i = 0; i < _etapas.Length; i++)
        {
            if (AvisosCadastro.HaPendencias(this, _etapas[i].Titulo, _etapas[i].Validar(_mdfe)))
            {
                IrPara(i);
                return;
            }
        }

        if (!await GravarAsync(SituacaoDocumento.ProntoParaTransmitir))
        {
            return;
        }

        MessageBox.Show(this,
            "MDF-e conferido e salvo como \"Pronto para transmitir\".\n\nA transmissão para a SEFAZ (homologação) entra com a implementação do motor DF-e.",
            Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }

    /// <summary>
    /// Grava o MDF-e no banco com a situação informada. Se o banco recusar, mostra o motivo, devolve a situação
    /// anterior e mantém o assistente aberto.
    /// </summary>
    /// <param name="situacao">Em digitação (rascunho) ou pronto para transmitir.</param>
    /// <returns>Verdadeiro se gravou.</returns>
    private async Task<bool> GravarAsync(SituacaoDocumento situacao)
    {
        if (_gravando)
        {
            return false;
        }

        var anterior = _mdfe.Situacao;
        _mdfe.Situacao = situacao;
        _gravando = true;
        UseWaitCursor = true;
        try
        {
            await _cadastros.Mdfes.SalvarAsync(_mdfe);
            _gravado = true;
            DialogResult = DialogResult.OK;
            return true;
        }
        catch (RegraNegocioException erro)
        {
            _mdfe.Situacao = anterior;
            AvisosCadastro.HaPendencias(this, Text, erro.Pendencias);
            return false;
        }
        catch (Exception erro)
        {
            _mdfe.Situacao = anterior;
            MessageBox.Show(this, $"Não foi possível gravar o MDF-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        finally
        {
            UseWaitCursor = false;
            _gravando = false;
        }
    }

    /// <summary>Exibe a etapa informada e ajusta título, lista e botões.</summary>
    private void IrPara(int indice)
    {
        _atual = indice;
        for (var i = 0; i < _telas.Length; i++)
        {
            _telas[i].Visible = i == indice;
        }

        _etapas[indice].AoEntrar(_mdfe);
        lblTituloEtapa.Text = _etapas[indice].Titulo;
        SelecionarNaLista(indice);

        var ultima = indice == _etapas.Length - 1;
        btnVoltar.Enabled = indice > 0;
        btnAvancar.Visible = !ultima;
        btnConcluir.Visible = ultima;
        AcceptButton = null;
    }

    private void SelecionarNaLista(int indice)
    {
        _ajustandoLista = true;
        lstEtapas.SelectedIndex = indice;
        _ajustandoLista = false;
    }
}
