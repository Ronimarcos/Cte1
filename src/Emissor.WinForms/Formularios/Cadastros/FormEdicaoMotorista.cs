using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Diálogo de inclusão e edição do motorista (§5.7), gravado no banco.</summary>
public partial class FormEdicaoMotorista : Form
{
    private readonly Motorista _motorista;
    private readonly Func<Motorista, Task> _gravar;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="motorista">Cópia editável do motorista.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="gravar">Inclui ou altera no banco.</param>
    internal FormEdicaoMotorista(Motorista motorista, bool novo, Func<Motorista, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _motorista = motorista;
        _gravar = gravar;
        Text = novo ? "Novo motorista" : "Editar motorista";
        CarregarCampos();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        LerCampos();
        if (await GravacaoDialogo.GravarAsync(this, btnSalvar, _motorista, _gravar))
        {
            DialogResult = DialogResult.OK;
        }
    }

    private void CarregarCampos()
    {
        txtNome.Text = _motorista.Nome;
        txtCpf.Text = Formatador.Cpf(_motorista.Cpf);
        txtTelefone.Text = _motorista.Telefone;
    }

    private void LerCampos()
    {
        _motorista.Nome = txtNome.Text.Trim();
        _motorista.Cpf = Formatos.SoDigitos(txtCpf.Text);
        _motorista.Telefone = Formatos.SoDigitos(txtTelefone.Text);
    }
}
