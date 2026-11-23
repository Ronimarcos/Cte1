using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição da apólice de seguro (§5.8).
/// O CNPJ/CPF do responsável só fica disponível quando o responsável é o contratante.
/// </summary>
public partial class FormEdicaoApolice : Form
{
    private readonly ApolicePrototipo _apolice;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="apolice">Cópia editável do registro.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    internal FormEdicaoApolice(ApolicePrototipo apolice, bool novo)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _apolice = apolice;
        Text = novo ? "Nova apólice de seguro" : "Editar apólice de seguro";
        cboResponsavel.Carregar(OpcoesCadastro.ResponsaveisSeguro);
        CarregarCampos();
        AtualizarCampoResponsavel();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    private void cboResponsavel_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarCampoResponsavel();

    /// <summary>Habilita o CNPJ/CPF do responsável apenas para o contratante.</summary>
    private void AtualizarCampoResponsavel()
    {
        var contratante = cboResponsavel.ValorSelecionado<ResponsavelSeguro>() == ResponsavelSeguro.Contratante;
        lblCnpjCpfResponsavel.Enabled = contratante;
        txtCnpjCpfResponsavel.Enabled = contratante;
    }

    private void CarregarCampos()
    {
        cboResponsavel.Selecionar(_apolice.Responsavel);
        txtCnpjCpfResponsavel.Text = _apolice.CnpjCpfResponsavel;
        txtNomeSeguradora.Text = _apolice.NomeSeguradora;
        txtCnpjSeguradora.Text = _apolice.CnpjSeguradora;
        txtNumeroApolice.Text = _apolice.NumeroApolice;
    }

    private void LerCampos()
    {
        _apolice.Responsavel = cboResponsavel.ValorSelecionado<ResponsavelSeguro>();

        // Para o emitente o documento não vai no XML: não guarda o que tiver sobrado no campo desabilitado.
        _apolice.CnpjCpfResponsavel = _apolice.Responsavel == ResponsavelSeguro.Contratante
            ? txtCnpjCpfResponsavel.Text.Trim()
            : string.Empty;
        _apolice.NomeSeguradora = txtNomeSeguradora.Text.Trim();
        _apolice.CnpjSeguradora = txtCnpjSeguradora.Text.Trim();
        _apolice.NumeroApolice = txtNumeroApolice.Text.Trim();
    }

    /// <summary>Copia os campos para o registro e fecha com OK se os obrigatórios estiverem preenchidos.</summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _apolice.ValidarObrigatorios()))
        {
            return;
        }

        DialogResult = DialogResult.OK;
    }
}
