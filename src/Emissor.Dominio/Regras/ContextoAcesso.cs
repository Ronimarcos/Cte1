using Emissor.Dominio.Entidades;

namespace Emissor.Dominio.Regras;

/// <summary>
/// Quem está acessando os dados e em quais empresas. Aplicado na camada de dados (repositórios), para que nenhuma
/// tela consiga ler ou gravar registro de empresa fora da alçada do usuário.
/// </summary>
public interface IContextoAcesso
{
    /// <summary>Usuário logado (<c>null</c> no acesso interno do sistema ou sem login).</summary>
    Usuario? Usuario { get; }

    /// <summary>Indica se tem acesso total (administrador ou acesso interno do sistema).</summary>
    bool AcessoTotal { get; }

    /// <summary>Empresas liberadas (ignoradas com <see cref="AcessoTotal"/>).</summary>
    IReadOnlySet<int> Empresas { get; }

    /// <summary>Indica se pode ver e alterar dados da empresa.</summary>
    /// <param name="empresaId">Empresa.</param>
    bool PodeAcessar(int empresaId);
}

/// <summary>Contexto de acesso fixo.</summary>
public sealed class ContextoAcesso : IContextoAcesso
{
    private ContextoAcesso(Usuario? usuario, bool acessoTotal, IReadOnlySet<int> empresas)
    {
        Usuario = usuario;
        AcessoTotal = acessoTotal;
        Empresas = empresas;
    }

    /// <summary>
    /// Acesso interno do sistema, sem usuário (preparação do banco, carga de demonstração, testes). Nunca usado pelas
    /// telas depois do login.
    /// </summary>
    public static ContextoAcesso Sistema { get; } = new(null, acessoTotal: true, new HashSet<int>());

    /// <summary>Sem acesso a nada (antes do login).</summary>
    public static ContextoAcesso Nenhum { get; } = new(null, acessoTotal: false, new HashSet<int>());

    /// <inheritdoc />
    public Usuario? Usuario { get; }

    /// <inheritdoc />
    public bool AcessoTotal { get; }

    /// <inheritdoc />
    public IReadOnlySet<int> Empresas { get; }

    /// <summary>Contexto do usuário: administrador com acesso total; operador só nas empresas liberadas.</summary>
    /// <param name="usuario">Usuário autenticado.</param>
    public static ContextoAcesso De(Usuario usuario) =>
        new(usuario, usuario.Administrador, usuario.Empresas.Select(e => e.EmpresaId).ToHashSet());

    /// <inheritdoc />
    public bool PodeAcessar(int empresaId) => AcessoTotal || Empresas.Contains(empresaId);
}

/// <summary>
/// Sessão do aplicativo desktop: começa sem acesso e passa a valer o contexto do usuário depois do login. Os serviços
/// das telas recebem esta sessão; trocar de usuário troca o acesso de todos eles.
/// </summary>
public sealed class Sessao : IContextoAcesso
{
    private ContextoAcesso _atual = ContextoAcesso.Nenhum;

    /// <inheritdoc />
    public Usuario? Usuario => _atual.Usuario;

    /// <inheritdoc />
    public bool AcessoTotal => _atual.AcessoTotal;

    /// <inheritdoc />
    public IReadOnlySet<int> Empresas => _atual.Empresas;

    /// <summary>Indica se há usuário logado.</summary>
    public bool Autenticada => _atual.Usuario is not null;

    /// <summary>Inicia a sessão do usuário autenticado.</summary>
    /// <param name="usuario">Usuário.</param>
    public void Entrar(Usuario usuario) => _atual = ContextoAcesso.De(usuario);

    /// <summary>Encerra a sessão (troca de usuário).</summary>
    public void Sair() => _atual = ContextoAcesso.Nenhum;

    /// <inheritdoc />
    public bool PodeAcessar(int empresaId) => _atual.PodeAcessar(empresaId);
}
