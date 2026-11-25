using Emissor.Dominio.Regras;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Acesso;

/// <summary>Troca da senha do usuário logado (por escolha ou obrigatória após senha definida pelo administrador).</summary>
public partial class FormTrocarSenha : Form
{
    private readonly ServicoAcesso _acesso;

    /// <summary>Cria a tela.</summary>
    /// <param name="acesso">Login e sessão.</param>
    /// <param name="obrigatoria">Verdadeiro quando o administrador exigiu a troca: cancelar não entra no sistema.</param>
    internal FormTrocarSenha(ServicoAcesso acesso, bool obrigatoria)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _acesso = acesso;
        if (obrigatoria)
        {
            lblAviso.Text = "A senha foi definida pelo administrador. Troque-a para continuar.";
            btnCancelar.Text = "Sai&r";
        }
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        if (txtNova.Text != txtConfirmar.Text)
        {
            Avisar(["A confirmação não confere com a nova senha."]);
            return;
        }

        btnSalvar.Enabled = false;
        UseWaitCursor = true;
        try
        {
            await _acesso.TrocarSenhaAsync(txtAtual.Text, txtNova.Text);
            UseWaitCursor = false;
            MessageBox.Show(this, "Senha alterada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            btnSalvar.Enabled = true;
        }
    }

    private void Avisar(IReadOnlyList<string> pendencias) =>
        MessageBox.Show(this, string.Join(Environment.NewLine, pendencias.Select(p => $"• {p}")), Text,
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
