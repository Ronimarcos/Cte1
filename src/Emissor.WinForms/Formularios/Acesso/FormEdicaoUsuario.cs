using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Acesso;

/// <summary>
/// Inclusão e edição de usuário (só administrador): login, nome, perfil, situação, senha e empresas liberadas. Na
/// inclusão a senha inicial é obrigatória; na edição, informar uma senha nova a redefine. Nos dois casos o usuário
/// troca a senha no próximo login.
/// </summary>
public partial class FormEdicaoUsuario : Form
{
    private static readonly IReadOnlyList<Opcao<PerfilUsuario>> Perfis =
    [
        new(PerfilUsuario.Administrador, "Administrador"),
        new(PerfilUsuario.Operador, "Operador"),
    ];

    private readonly Usuario _usuario;
    private readonly bool _novo;
    private readonly Func<Usuario, string?, Task> _gravar;

    /// <summary>Cria o diálogo.</summary>
    /// <param name="usuario">Cópia editável do usuário.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="empresas">Todas as empresas (o administrador vê todas).</param>
    /// <param name="gravar">Inclui ou altera, recebendo a senha (inicial ou nova; vazia mantém a atual na edição).</param>
    internal FormEdicaoUsuario(Usuario usuario, bool novo, IReadOnlyList<EmpresaResumo> empresas, Func<Usuario, string?, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _usuario = usuario;
        _novo = novo;
        _gravar = gravar;
        Text = novo ? "Novo usuário" : "Editar usuário";
        cboPerfil.Carregar(Perfis);
        foreach (var empresa in empresas)
        {
            clbEmpresas.Items.Add(new Opcao<int>(empresa.Id, $"{empresa.RazaoSocial} - {empresa.Cnpj}"),
                usuario.Empresas.Any(e => e.EmpresaId == empresa.Id));
        }

        lblSenha.Text = novo ? "&Senha inicial *" : "Nova &senha (deixe em branco para manter)";
        lblRegraSenha.Text = $"Mínimo de {HashSenha.TamanhoMinimo} caracteres, com letras e números; troca obrigatória no próximo login.";
        txtLogin.Text = usuario.Login;
        txtNome.Text = usuario.Nome;
        cboPerfil.Selecionar(usuario.Perfil);
        chkAtivo.Checked = usuario.Ativo;
        lblSituacao.Text = Situacao(usuario);
        AtualizarEmpresas();
    }

    private void cboPerfil_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarEmpresas();

    /// <summary>Administrador vê todas as empresas: a lista só vale para o operador.</summary>
    private void AtualizarEmpresas()
    {
        var operador = cboPerfil.ValorSelecionado<PerfilUsuario>() == PerfilUsuario.Operador;
        clbEmpresas.Enabled = operador;
        lblEmpresas.Text = operador ? "&Empresas liberadas *" : "&Empresas liberadas (o administrador vê todas)";
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        var senha = txtSenha.Text;
        var pendencias = new List<string>();
        if (_novo && senha.Length == 0) pendencias.Add("Senha inicial");
        if (senha != txtConfirmar.Text) pendencias.Add("A confirmação não confere com a senha");
        if (AvisosCadastro.HaPendencias(this, Text, pendencias))
        {
            return;
        }

        _usuario.Login = Usuario.NormalizarLogin(txtLogin.Text);
        _usuario.Nome = txtNome.Text.Trim();
        _usuario.Perfil = cboPerfil.ValorSelecionado<PerfilUsuario>();
        _usuario.Ativo = chkAtivo.Checked;
        _usuario.Empresas = _usuario.Perfil == PerfilUsuario.Operador
            ? clbEmpresas.CheckedItems.OfType<Opcao<int>>().Select(o => new UsuarioEmpresa { EmpresaId = o.Valor }).ToList()
            : [];

        btnSalvar.Enabled = false;
        UseWaitCursor = true;
        try
        {
            await _gravar(_usuario, senha.Length > 0 ? senha : null);
            DialogResult = DialogResult.OK;
        }
        catch (RegraNegocioException erro)
        {
            UseWaitCursor = false;
            AvisosCadastro.HaPendencias(this, Text, erro.Pendencias);
        }
        finally
        {
            UseWaitCursor = false;
            btnSalvar.Enabled = true;
        }
    }

    private static string Situacao(Usuario usuario)
    {
        if (usuario.Id == 0)
        {
            return string.Empty;
        }

        var partes = new List<string>
        {
            usuario.UltimoAcesso is { } acesso ? $"Último acesso: {acesso:dd/MM/yyyy HH:mm}" : "Ainda não entrou no sistema",
        };
        if (usuario.BloqueadoEm(DateTime.Now)) partes.Add($"bloqueado até {usuario.BloqueadoAte:HH:mm} (redefinir a senha desbloqueia)");
        if (usuario.DeveTrocarSenha) partes.Add("deve trocar a senha no próximo login");
        return string.Join(" — ", partes);
    }
}
