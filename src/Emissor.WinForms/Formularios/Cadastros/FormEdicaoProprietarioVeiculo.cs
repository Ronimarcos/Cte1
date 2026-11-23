using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Diálogo de inclusão e edição do proprietário de veículo de terceiro (§5.6).</summary>
public partial class FormEdicaoProprietarioVeiculo : Form
{
    private readonly ProprietarioVeiculoPrototipo _proprietario;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="proprietario">Cópia editável do registro.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    internal FormEdicaoProprietarioVeiculo(ProprietarioVeiculoPrototipo proprietario, bool novo)
    {
        InitializeComponent();
        _proprietario = proprietario;
        Text = novo ? "Novo proprietário de veículo" : "Editar proprietário de veículo";
        cboTipoProprietario.Carregar(OpcoesCadastro.TiposProprietario);
        CarregarCampos();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    private void CarregarCampos()
    {
        txtCnpjCpf.Text = _proprietario.CnpjCpf;
        txtRntrc.Text = _proprietario.Rntrc;
        txtNome.Text = _proprietario.Nome;
        cboTipoProprietario.Selecionar(_proprietario.TipoProprietario);
    }

    private void LerCampos()
    {
        _proprietario.CnpjCpf = txtCnpjCpf.Text.Trim();
        _proprietario.Rntrc = txtRntrc.Text.Trim();
        _proprietario.Nome = txtNome.Text.Trim();
        _proprietario.TipoProprietario = cboTipoProprietario.ValorSelecionado<TipoProprietario>();
    }

    /// <summary>Copia os campos para o registro e fecha com OK se os obrigatórios estiverem preenchidos.</summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _proprietario.ValidarObrigatorios()))
        {
            return;
        }

        DialogResult = DialogResult.OK;
    }
}
