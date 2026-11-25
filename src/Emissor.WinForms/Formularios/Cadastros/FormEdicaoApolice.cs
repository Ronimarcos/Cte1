using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição da apólice de seguro (§5.8), gravada no banco.
/// O CNPJ/CPF do responsável só fica disponível quando o responsável é o contratante.
/// </summary>
public partial class FormEdicaoApolice : Form
{
    private readonly ApoliceSeguro _apolice;
    private readonly Func<ApoliceSeguro, Task> _gravar;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="apolice">Cópia editável da apólice.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="gravar">Inclui ou altera no banco.</param>
    internal FormEdicaoApolice(ApoliceSeguro apolice, bool novo, Func<ApoliceSeguro, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _apolice = apolice;
        _gravar = gravar;
        Text = novo ? "Nova apólice de seguro" : "Editar apólice de seguro";
        cboResponsavel.Carregar(OpcoesCadastro.ResponsaveisSeguro);
        CarregarCampos();
        AtualizarCampoResponsavel();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        LerCampos();
        if (await GravacaoDialogo.GravarAsync(this, btnSalvar, _apolice, _gravar))
        {
            DialogResult = DialogResult.OK;
        }
    }

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
        txtCnpjCpfResponsavel.Text = Formatador.CnpjCpf(_apolice.CnpjCpfResponsavel);
        txtNomeSeguradora.Text = _apolice.NomeSeguradora;
        txtCnpjSeguradora.Text = Formatador.Cnpj(_apolice.CnpjSeguradora);
        txtNumeroApolice.Text = _apolice.NumeroApolice;
    }

    private void LerCampos()
    {
        _apolice.Responsavel = cboResponsavel.ValorSelecionado<ResponsavelSeguro>();

        // Para o emitente o documento não vai no XML: não guarda o que tiver sobrado no campo desabilitado.
        _apolice.CnpjCpfResponsavel = _apolice.Responsavel == ResponsavelSeguro.Contratante
            ? Formatos.SoAlfanumericos(txtCnpjCpfResponsavel.Text)
            : string.Empty;
        _apolice.NomeSeguradora = txtNomeSeguradora.Text.Trim();
        _apolice.CnpjSeguradora = Formatos.SoAlfanumericos(txtCnpjSeguradora.Text);
        _apolice.NumeroApolice = txtNumeroApolice.Text.Trim();
    }
}
