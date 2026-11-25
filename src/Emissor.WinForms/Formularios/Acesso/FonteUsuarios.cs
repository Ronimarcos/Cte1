using System.Collections;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Acesso;

/// <summary>Linha da lista de usuários.</summary>
internal sealed class LinhaUsuario
{
    /// <summary>Identificador.</summary>
    public int Id { get; init; }

    /// <summary>Login.</summary>
    public string Login { get; init; } = string.Empty;

    /// <summary>Nome.</summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>Perfil para exibição.</summary>
    public string PerfilDescricao { get; init; } = string.Empty;

    /// <summary>Empresas liberadas para exibição.</summary>
    public string EmpresasDescricao { get; init; } = string.Empty;

    /// <summary>Situação para exibição.</summary>
    public string SituacaoDescricao { get; init; } = string.Empty;

    /// <summary>Inativo ou bloqueado (cor da linha).</summary>
    public bool Destacar { get; init; }
}

/// <summary>Cadastro de usuários (só administrador), no padrão das listas de cadastro.</summary>
internal sealed class FonteUsuarios : IFonteCadastro
{
    private readonly ServicoAcesso _acesso;
    private readonly ServicoEmpresas _empresas;

    /// <summary>Cria o cadastro.</summary>
    /// <param name="acesso">Usuários e sessão.</param>
    /// <param name="empresas">Empresas (para a lista de liberadas).</param>
    public FonteUsuarios(ServicoAcesso acesso, ServicoEmpresas empresas)
    {
        _acesso = acesso;
        _empresas = empresas;
    }

    /// <inheritdoc />
    public string Titulo => "Usuários";

    /// <inheritdoc />
    public IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaUsuario.Login), "Login", 14),
        new(nameof(LinhaUsuario.Nome), "Nome", 26),
        new(nameof(LinhaUsuario.PerfilDescricao), "Perfil", 12),
        new(nameof(LinhaUsuario.EmpresasDescricao), "Empresas liberadas", 34),
        new(nameof(LinhaUsuario.SituacaoDescricao), "Situação", 14),
    ];

    /// <inheritdoc />
    public async Task<IList> ListarAsync(string pesquisa)
    {
        var termo = pesquisa.Trim();
        var empresas = (await _empresas.ListarResumosAsync()).ToDictionary(e => e.Id, e => e.NomeFantasia.Length > 0 ? e.NomeFantasia : e.RazaoSocial);
        var agora = DateTime.Now;
        return (await _acesso.ListarUsuariosAsync())
            .OrderBy(u => u.Login)
            .Select(u => new LinhaUsuario
            {
                Id = u.Id,
                Login = u.Login,
                Nome = u.Nome,
                PerfilDescricao = u.Administrador ? "Administrador" : "Operador",
                EmpresasDescricao = u.Administrador ? "Todas" : string.Join(", ", u.Empresas.Select(e => empresas.GetValueOrDefault(e.EmpresaId, $"#{e.EmpresaId}"))),
                SituacaoDescricao = !u.Ativo ? "Inativo" : u.BloqueadoEm(agora) ? "Bloqueado" : "Ativo",
                Destacar = !u.Ativo || u.BloqueadoEm(agora),
            })
            .Where(l => termo.Length == 0
                || $"{l.Login} {l.Nome} {l.PerfilDescricao} {l.EmpresasDescricao}".Contains(termo, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    /// <inheritdoc />
    public int IdDe(object registro) => ((LinhaUsuario)registro).Id;

    /// <inheritdoc />
    public async Task<int?> NovoAsync(IWin32Window dono)
    {
        var usuario = new Usuario { Perfil = PerfilUsuario.Operador };
        using var tela = new FormEdicaoUsuario(usuario, novo: true, await _empresas.ListarResumosAsync(),
            (u, senha) => _acesso.IncluirUsuarioAsync(u, senha ?? string.Empty));
        return tela.ShowDialog(dono) == DialogResult.OK ? usuario.Id : null;
    }

    /// <inheritdoc />
    public async Task<bool> EditarAsync(IWin32Window dono, object registro)
    {
        if (await _acesso.ObterUsuarioAsync(IdDe(registro)) is not { } usuario)
        {
            MessageBox.Show(dono, "O usuário não existe mais. A lista será atualizada.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        using var tela = new FormEdicaoUsuario(usuario, novo: false, await _empresas.ListarResumosAsync(), _acesso.AlterarUsuarioAsync);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }

    /// <inheritdoc />
    public async Task<bool> ExcluirAsync(IWin32Window dono, object registro)
    {
        var linha = (LinhaUsuario)registro;
        var resposta = MessageBox.Show(dono, $"Excluir o usuário \"{linha.Login}\"? Para só impedir o acesso, prefira desmarcar \"Ativo\".",
            Titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta != DialogResult.Yes)
        {
            return false;
        }

        try
        {
            await _acesso.ExcluirUsuarioAsync(linha.Id);
            return true;
        }
        catch (RegraNegocioException erro)
        {
            MessageBox.Show(dono, string.Join(Environment.NewLine, erro.Pendencias), Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }
    }

    /// <inheritdoc />
    public Color? CorDaLinha(object registro) => registro is LinhaUsuario { Destacar: true } ? Color.MistyRose : null;
}
