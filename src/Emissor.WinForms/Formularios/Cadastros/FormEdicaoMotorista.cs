using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Diálogo de inclusão e edição do motorista (§5.7).</summary>
public partial class FormEdicaoMotorista : Form
{
    private readonly MotoristaPrototipo _motorista;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="motorista">Cópia editável do registro.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    internal FormEdicaoMotorista(MotoristaPrototipo motorista, bool novo)
    {
        InitializeComponent();
        _motorista = motorista;
        Text = novo ? "Novo motorista" : "Editar motorista";
        CarregarCampos();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    private void CarregarCampos()
    {
        txtNome.Text = _motorista.Nome;
        txtCpf.Text = _motorista.Cpf;
        txtTelefone.Text = _motorista.Telefone;
    }

    private void LerCampos()
    {
        _motorista.Nome = txtNome.Text.Trim();
        _motorista.Cpf = txtCpf.Text.Trim();
        _motorista.Telefone = txtTelefone.Text.Trim();
    }

    /// <summary>Copia os campos para o registro e fecha com OK se os obrigatórios estiverem preenchidos.</summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _motorista.ValidarObrigatorios()))
        {
            return;
        }

        DialogResult = DialogResult.OK;
    }
}
