using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>
/// Assistente de emissão do MDF-e em 5 etapas. Cada etapa valida o que é dela antes de avançar;
/// "Concluir" confere tudo e grava o MDF-e como "Pronto para transmitir". O protótipo não transmite,
/// não gera chave, protocolo nem DACTE.
/// </summary>
public partial class FormEmissaoMdfe : Form
{
    private readonly MdfePrototipo _mdfe;
    private readonly UserControl[] _telas;
    private readonly IEtapaMdfe[] _etapas;
    private int _atual;
    private bool _ajustandoLista;
    private bool _gravado;

    /// <summary>Cria o assistente sobre a cópia do MDF-e.</summary>
    /// <param name="mdfe">Cópia editável do MDF-e (novo ou existente).</param>
    /// <param name="cadastros">Empresa emitente e cadastros lidos do banco ao abrir o assistente.</param>
    internal FormEmissaoMdfe(MdfePrototipo mdfe, CadastrosEmissao cadastros)
    {
        InitializeComponent();
        _mdfe = mdfe;
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
                SalvarRascunho();
                return true;
            case Keys.Control | Keys.Enter when btnConcluir.Visible:
                Concluir();
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

    private void btnSalvarRascunho_Click(object? sender, EventArgs e) => SalvarRascunho();

    private void btnConcluir_Click(object? sender, EventArgs e) => Concluir();

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
    private void SalvarRascunho()
    {
        _etapas[_atual].Ler(_mdfe);
        _mdfe.Situacao = SituacaoMdfe.EmDigitacao;
        Gravar();
        MessageBox.Show(this, "Rascunho salvo. O MDF-e fica \"Em digitação\" na consulta.", Text,
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }

    /// <summary>Confere todas as etapas; na primeira com pendência, avisa e mostra a etapa.</summary>
    private void Concluir()
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

        _mdfe.Situacao = SituacaoMdfe.ProntoParaTransmitir;
        Gravar();
        MessageBox.Show(this,
            "MDF-e conferido e salvo como \"Pronto para transmitir\".\n\nA transmissão para a SEFAZ (homologação) entra com a implementação do motor DF-e.",
            Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }

    /// <summary>Inclui ou altera o MDF-e no repositório em memória.</summary>
    private void Gravar()
    {
        if (_mdfe.Id == 0)
        {
            DadosFicticios.Mdfes.Incluir(_mdfe);
        }
        else
        {
            DadosFicticios.Mdfes.Alterar(_mdfe);
        }

        _gravado = true;
        DialogResult = DialogResult.OK;
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
