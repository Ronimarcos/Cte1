using System.Text.RegularExpressions;
using Emissor.Dominio.Enums;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// Usuário do emissor (login próprio do sistema). O administrador vê todas as empresas; o operador só as liberadas em
/// <see cref="Empresas"/>. A senha fica só como hash (<see cref="Regras.HashSenha"/>).
/// </summary>
public partial class Usuario : EntidadeBase
{
    /// <summary>Tentativas erradas seguidas que bloqueiam o login.</summary>
    public const int TentativasAteBloquear = 5;

    /// <summary>Tempo do bloqueio após as tentativas erradas.</summary>
    public static readonly TimeSpan TempoBloqueio = TimeSpan.FromMinutes(15);

    /// <summary>Login (minúsculas, 3 a 30 caracteres: letras, números, ponto, hífen ou sublinhado).</summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>Nome da pessoa.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Hash da senha.</summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>Perfil de acesso.</summary>
    public PerfilUsuario? Perfil { get; set; } = PerfilUsuario.Operador;

    /// <summary>Exige trocar a senha no próximo login (senha criada ou redefinida pelo administrador).</summary>
    public bool DeveTrocarSenha { get; set; }

    /// <summary>Tentativas erradas seguidas desde o último login correto.</summary>
    public int TentativasFalhas { get; set; }

    /// <summary>Login bloqueado até este momento, após tentativas erradas.</summary>
    public DateTime? BloqueadoAte { get; set; }

    /// <summary>Último login correto.</summary>
    public DateTime? UltimoAcesso { get; set; }

    /// <summary>Empresas liberadas (só para o operador; o administrador vê todas).</summary>
    public List<UsuarioEmpresa> Empresas { get; set; } = [];

    /// <summary>Indica se é administrador.</summary>
    public bool Administrador => Perfil == PerfilUsuario.Administrador;

    /// <summary>Login no formato gravado: sem espaços nas pontas e em minúsculas.</summary>
    /// <param name="login">Login digitado.</param>
    public static string NormalizarLogin(string? login) => (login ?? string.Empty).Trim().ToLowerInvariant();

    /// <summary>Indica se o login está bloqueado no momento informado.</summary>
    /// <param name="agora">Momento da tentativa.</param>
    public bool BloqueadoEm(DateTime agora) => BloqueadoAte is { } ate && ate > agora;

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (!LoginValido().IsMatch(Login)) faltando.Add("Login com 3 a 30 caracteres (letras minúsculas, números, ponto, hífen ou sublinhado)");
        if (string.IsNullOrWhiteSpace(Nome)) faltando.Add("Nome");
        if (Perfil is null) faltando.Add("Perfil");
        if (string.IsNullOrWhiteSpace(SenhaHash)) faltando.Add("Senha");
        if (Perfil == PerfilUsuario.Operador && Empresas.Count == 0) faltando.Add("Ao menos uma empresa liberada para o operador");
        if (Empresas.Select(e => e.EmpresaId).Distinct().Count() != Empresas.Count) faltando.Add("Empresa repetida na lista de liberadas");
        return faltando;
    }

    [GeneratedRegex("^[a-z0-9._-]{3,30}$")]
    private static partial Regex LoginValido();
}

/// <summary>Empresa liberada para um usuário operador.</summary>
public class UsuarioEmpresa
{
    /// <summary>Empresa liberada.</summary>
    public int EmpresaId { get; set; }
}
