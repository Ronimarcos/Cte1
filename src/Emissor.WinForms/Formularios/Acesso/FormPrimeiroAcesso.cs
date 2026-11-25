using Emissor.Dominio.Regras;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Acesso;

/// <summary>Primeiro acesso: cria o usuário administrador quando o banco ainda não tem usuários.</summary>
public partial class FormPrimeiroAcesso : Form
{
    private readonly ServicoAcesso _acesso;

    /// <summary>Cria a tela.</summary>
    /// <param name="acesso">Login e sessão.</param>
    internal FormPrimeiroAcesso(ServicoAcesso acesso)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnCriar);
        _acesso = acesso;
    }

    private async void btnCriar_Click(object? sender, EventArgs e)
    {
        if (txtSenha.Text != txtConfirmar.Text)
        {
            Avisar(["A confirmação não confere com a senha."]);
            return;
        }

        btnCriar.Enabled = false;
        UseWaitCursor = true;
        try
        {
            await _acesso.CriarPrimeiroAdministradorAsync(txtLogin.Text, txtNome.Text, txtSenha.Text);
            DialogResult = DialogResult.OK;
        }
        catch (RegraNegocioException erro)
        {
            UseWaitCursor = false;
            Avisar(erro.Pendencias);
        }
        finally
        {
            UseWaitCursor = false;
            btnCriar.Enabled = true;
        }
    }

    private void Avisar(IReadOnlyList<string> pendencias) =>
        MessageBox.Show(this, string.Join(Environment.NewLine, pendencias.Select(p => $"• {p}")), Text,
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
