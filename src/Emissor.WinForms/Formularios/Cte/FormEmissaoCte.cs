using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>
/// Assistente de emissão do CT-e em 5 etapas. Cada etapa valida o que é dela antes de avançar;
/// "Concluir" confere tudo e grava o CT-e como "Pronto para transmitir". O protótipo não transmite,
/// não gera chave, protocolo nem DACTE.
/// </summary>
public partial class FormEmissaoCte : Form
{
    private readonly CtePrototipo _cte;
    private readonly UserControl[] _telas;
    private readonly IEtapaCte[] _etapas;
    private int _atual;
    private bool _ajustandoLista;
    private bool _gravado;

    /// <summary>Cria o assistente sobre a cópia do CT-e.</summary>
    /// <param name="cte">Cópia editável do CT-e (novo ou existente).</param>
    /// <param name="empresa">Empresa emitente.</param>
    internal FormEmissaoCte(CtePrototipo cte, EmpresaPrototipo empresa)
    {
        InitializeComponent();
        _cte = cte;
        Text = cte.Id == 0
            ? $"Emitir CT-e — {empresa.RazaoSocial}"
            : $"CT-e de {cte.DataEmissao:dd/MM/yyyy} ({cte.SituacaoDescricao}) — {empresa.RazaoSocial}";

        _telas = [ucIdentificacao, ucParticipantes, ucCargaDocumentos, ucValores, ucConferencia];
        _etapas = _telas.Cast<IEtapaCte>().ToArray();
        foreach (var etapa in _etapas)
        {
            etapa.Carregar(_cte, empresa);
            lstEtapas.Items.Add(etapa.Titulo);
        }

        IrPara(0);
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
            _etapas[_atual].Ler(_cte);
            IrPara(lstEtapas.SelectedIndex);
        }
        else
        {
            SelecionarNaLista(_atual);
        }
    }

    /// <summary>Pede confirmação antes de descartar o que foi digitado (botão Cancelar ou fechar a janela).</summary>
    private void FormEmissaoCte_FormClosing(object? sender, FormClosingEventArgs e)
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
        etapa.Ler(_cte);
        if (AvisosCadastro.HaPendencias(this, etapa.Titulo, etapa.Validar(_cte)))
        {
            return;
        }

        IrPara(_atual + 1);
    }

    private void Voltar()
    {
        _etapas[_atual].Ler(_cte);
        IrPara(_atual - 1);
    }

    /// <summary>Grava como está, sem exigir os campos obrigatórios.</summary>
    private void SalvarRascunho()
    {
        _etapas[_atual].Ler(_cte);
        _cte.Situacao = SituacaoCte.EmDigitacao;
        Gravar();
        MessageBox.Show(this, "Rascunho salvo. O CT-e fica \"Em digitação\" na consulta.", Text,
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }

    /// <summary>Confere todas as etapas; na primeira com pendência, avisa e mostra a etapa.</summary>
    private void Concluir()
    {
        _etapas[_atual].Ler(_cte);
        for (var i = 0; i < _etapas.Length; i++)
        {
            if (i != _atual)
            {
                _etapas[i].Ler(_cte);
            }
        }

        for (var i = 0; i < _etapas.Length; i++)
        {
            if (AvisosCadastro.HaPendencias(this, _etapas[i].Titulo, _etapas[i].Validar(_cte)))
            {
                IrPara(i);
                return;
            }
        }

        _cte.Situacao = SituacaoCte.ProntoParaTransmitir;
        Gravar();
        MessageBox.Show(this,
            "CT-e conferido e salvo como \"Pronto para transmitir\".\n\nA transmissão para a SEFAZ (homologação) entra com a implementação do motor DF-e.",
            Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }

    /// <summary>Inclui ou altera o CT-e no repositório em memória.</summary>
    private void Gravar()
    {
        if (_cte.Id == 0)
        {
            DadosFicticios.Ctes.Incluir(_cte);
        }
        else
        {
            DadosFicticios.Ctes.Alterar(_cte);
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

        _etapas[indice].AoEntrar(_cte);
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
