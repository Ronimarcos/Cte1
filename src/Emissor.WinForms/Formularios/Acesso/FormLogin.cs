using Emissor.Dados;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Acesso;

/// <summary>Login do emissor (usuário e senha do sistema). Retorna OK com a sessão já iniciada.</summary>
public partial class FormLogin : Form
{
    private readonly ServicoAcesso _acesso;

    /// <summary>Cria a tela de login.</summary>
    /// <param name="acesso">Login e sessão.</param>
    /// <param name="loginSugerido">Login já preenchido (troca de usuário), ou vazio.</param>
    internal FormLogin(ServicoAcesso acesso, string loginSugerido = "")
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnEntrar);
        _acesso = acesso;
        txtLogin.Text = loginSugerido;
        Shown += (_, _) => (loginSugerido.Length > 0 ? txtSenha : txtLogin).Focus();
    }

    private async void btnEntrar_Click(object? sender, EventArgs e)
    {
        if (txtLogin.Text.Trim().Length == 0 || txtSenha.Text.Length == 0)
        {
            lblMensagem.Text = "Informe o login e a senha.";
            return;
        }

        btnEntrar.Enabled = false;
        UseWaitCursor = true;
        try
        {
            ResultadoLogin resultado = await _acesso.EntrarAsync(txtLogin.Text, txtSenha.Text);
            if (resultado.Aceito)
            {
                DialogResult = DialogResult.OK;
                return;
            }

            lblMensagem.Text = resultado.Mensagem;
            txtSenha.Clear();
            txtSenha.Focus();
        }
        catch (Exception erro)
        {
            lblMensagem.Text = $"Não foi possível entrar: {erro.GetBaseException().Message}";
        }
        finally
        {
            UseWaitCursor = false;
            btnEntrar.Enabled = true;
        }
    }
}
