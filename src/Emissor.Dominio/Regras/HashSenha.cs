using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Emissor.Dominio.Regras;

/// <summary>
/// Senhas guardadas só como hash: PBKDF2 com SHA-256, sal aleatório de 16 bytes e 600.000 iterações.
/// Formato gravado: <c>PBKDF2-SHA256$iterações$sal(Base64)$hash(Base64)</c>, o que permite aumentar as iterações no
/// futuro sem perder as senhas já gravadas.
/// </summary>
public static class HashSenha
{
    /// <summary>Iterações usadas nas senhas novas.</summary>
    public const int Iteracoes = 600_000;

    /// <summary>Tamanho mínimo da senha.</summary>
    public const int TamanhoMinimo = 8;

    private const string Algoritmo = "PBKDF2-SHA256";
    private const int TamanhoSal = 16;
    private const int TamanhoHash = 32;

    /// <summary>Gera o hash da senha (com sal novo).</summary>
    /// <param name="senha">Senha digitada.</param>
    public static string Gerar(string senha)
    {
        var sal = RandomNumberGenerator.GetBytes(TamanhoSal);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(senha), sal, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
        return string.Join('$', Algoritmo, Iteracoes.ToString(CultureInfo.InvariantCulture), Convert.ToBase64String(sal), Convert.ToBase64String(hash));
    }

    /// <summary>Confere a senha com o hash gravado (comparação em tempo constante).</summary>
    /// <param name="senha">Senha digitada.</param>
    /// <param name="gravado">Hash gravado.</param>
    /// <returns>Verdadeiro se a senha confere.</returns>
    public static bool Conferir(string senha, string gravado)
    {
        var partes = gravado.Split('$');
        if (partes.Length != 4 || partes[0] != Algoritmo
            || !int.TryParse(partes[1], NumberStyles.None, CultureInfo.InvariantCulture, out var iteracoes) || iteracoes < 1)
        {
            return false;
        }

        try
        {
            var sal = Convert.FromBase64String(partes[2]);
            var esperado = Convert.FromBase64String(partes[3]);
            var calculado = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(senha), sal, iteracoes, HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>Pendências da senha nova: tamanho mínimo, letras e números.</summary>
    /// <param name="senha">Senha nova.</param>
    public static IReadOnlyList<string> ValidarForca(string? senha)
    {
        var faltando = new List<string>();
        if (string.IsNullOrEmpty(senha) || senha.Length < TamanhoMinimo) faltando.Add($"Senha com pelo menos {TamanhoMinimo} caracteres");
        if (senha is null || !senha.Any(char.IsLetter) || !senha.Any(char.IsDigit)) faltando.Add("Senha com letras e números");
        return faltando;
    }
}
