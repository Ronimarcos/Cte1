using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Diálogo de inclusão e edição do proprietário de veículo de terceiro (§5.6), gravado no banco.</summary>
public partial class FormEdicaoProprietarioVeiculo : Form
{
    private readonly ProprietarioVeiculo _proprietario;
    private readonly Func<ProprietarioVeiculo, Task> _gravar;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="proprietario">Cópia editável do proprietário.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="gravar">Inclui ou altera no banco.</param>
    internal FormEdicaoProprietarioVeiculo(ProprietarioVeiculo proprietario, bool novo, Func<ProprietarioVeiculo, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _proprietario = proprietario;
        _gravar = gravar;
        Text = novo ? "Novo proprietário de veículo" : "Editar proprietário de veículo";
        cboTipoProprietario.Carregar(OpcoesCadastro.TiposProprietario);
        cboUf.Items.AddRange([.. OpcoesCadastro.Ufs]);
        CarregarCampos();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        LerCampos();
        if (await GravacaoDialogo.GravarAsync(this, btnSalvar, _proprietario, _gravar))
        {
            DialogResult = DialogResult.OK;
        }
    }

    private void CarregarCampos()
    {
        txtCnpjCpf.Text = Formatador.CnpjCpf(_proprietario.CnpjCpf);
        txtRntrc.Text = _proprietario.Rntrc;
        txtNome.Text = _proprietario.Nome;
        cboTipoProprietario.Selecionar(_proprietario.TipoProprietario);
        txtIe.Text = _proprietario.Ie;
        cboUf.SelectedItem = string.IsNullOrEmpty(_proprietario.Uf) ? null : _proprietario.Uf;
    }

    private void LerCampos()
    {
        _proprietario.CnpjCpf = Formatos.SoAlfanumericos(txtCnpjCpf.Text);
        _proprietario.Rntrc = Formatos.SoDigitos(txtRntrc.Text);
        _proprietario.Nome = txtNome.Text.Trim();
        _proprietario.TipoProprietario = cboTipoProprietario.ValorSelecionado<TipoProprietario>();
        _proprietario.Ie = ProprietarioVeiculo.NormalizarIe(txtIe.Text);
        _proprietario.Uf = cboUf.SelectedItem as string ?? string.Empty;
    }
}
