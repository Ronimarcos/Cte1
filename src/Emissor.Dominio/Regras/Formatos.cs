using System.Text.RegularExpressions;

namespace Emissor.Dominio.Regras;

/// <summary>
/// Normalização e conferência de formato dos documentos, conforme os tipos dos XSD
/// (<c>tiposGeralCTe_v4.00.xsd</c> e <c>tiposGeralMDFe_v3.00.xsd</c>). Os valores são gravados sem pontuação;
/// a formatação é responsabilidade da tela. O dígito verificador de CNPJ/CPF ainda não é conferido (§8.1).
/// </summary>
public static partial class Formatos
{
    /// <summary>Mantém só os dígitos (CPF, CEP, RNTRC, telefone, chave de acesso).</summary>
    /// <param name="texto">Texto digitado, com ou sem máscara.</param>
    public static string SoDigitos(string? texto) => new((texto ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

    /// <summary>
    /// Mantém letras e dígitos em maiúsculas: o CNPJ passou a ser alfanumérico
    /// (<c>TCnpj</c>: 12 letras ou dígitos seguidos de 2 dígitos).
    /// </summary>
    /// <param name="texto">CNPJ digitado, com ou sem máscara.</param>
    public static string SoAlfanumericos(string? texto) =>
        new((texto ?? string.Empty).Where(char.IsAsciiLetterOrDigit).Select(char.ToUpperInvariant).ToArray());

    /// <summary>CNPJ no formato do leiaute (<c>TCnpj</c>), já normalizado.</summary>
    /// <param name="cnpj">CNPJ sem máscara.</param>
    public static bool CnpjValido(string? cnpj) => cnpj is not null && Cnpj().IsMatch(cnpj);

    /// <summary>CPF no formato do leiaute (<c>TCpf</c>), já normalizado.</summary>
    /// <param name="cpf">CPF sem máscara.</param>
    public static bool CpfValido(string? cpf) => cpf is not null && Cpf().IsMatch(cpf);

    /// <summary>CNPJ ou CPF válido no formato do leiaute.</summary>
    /// <param name="documento">Documento sem máscara.</param>
    public static bool CnpjOuCpfValido(string? documento) => CnpjValido(documento) || CpfValido(documento);

    /// <summary>Inscrição Estadual numérica do leiaute (<c>TIe</c>: 2 a 14 dígitos).</summary>
    /// <param name="ie">IE sem máscara.</param>
    public static bool IeValida(string? ie) => ie is not null && Ie().IsMatch(ie);

    /// <summary>CEP com 8 dígitos.</summary>
    /// <param name="cep">CEP sem máscara.</param>
    public static bool CepValido(string? cep) => cep is not null && Cep().IsMatch(cep);

    /// <summary>Telefone do leiaute (<c>TFone</c>: 6 a 14 dígitos).</summary>
    /// <param name="fone">Telefone sem máscara.</param>
    public static bool FoneValido(string? fone) => fone is not null && Fone().IsMatch(fone);

    /// <summary>E-mail no formato aceito pelo leiaute do CT-e (<c>TEmail</c>).</summary>
    /// <param name="email">E-mail.</param>
    public static bool EmailValido(string? email) => email is not null && Email().IsMatch(email);

    /// <summary>RNTRC com 8 dígitos (<c>TRNTRC</c> do MDF-e).</summary>
    /// <param name="rntrc">RNTRC sem máscara.</param>
    public static bool RntrcValido(string? rntrc) => rntrc is not null && Rntrc().IsMatch(rntrc);

    /// <summary>Placa no formato do MDF-e (<c>TPlaca</c>), incluindo o padrão Mercosul.</summary>
    /// <param name="placa">Placa sem hífen, em maiúsculas.</param>
    public static bool PlacaValida(string? placa) => placa is not null && Placa().IsMatch(placa);

    /// <summary>Sigla de uma das 27 UFs.</summary>
    /// <param name="uf">Sigla.</param>
    public static bool UfValida(string? uf) => uf is not null && Ufs.Contains(uf);

    /// <summary>Siglas das 27 UFs.</summary>
    public static IReadOnlySet<string> Ufs { get; } = new HashSet<string>(
    [
        "AC", "AL", "AM", "AP", "BA", "CE", "DF", "ES", "GO", "MA", "MG", "MS", "MT", "PA",
        "PB", "PE", "PI", "PR", "RJ", "RN", "RO", "RR", "RS", "SC", "SE", "SP", "TO",
    ]);

    [GeneratedRegex("^[A-Z0-9]{12}[0-9]{2}$")]
    private static partial Regex Cnpj();

    [GeneratedRegex("^[0-9]{11}$")]
    private static partial Regex Cpf();

    [GeneratedRegex("^[0-9]{2,14}$")]
    private static partial Regex Ie();

    [GeneratedRegex("^[0-9]{8}$")]
    private static partial Regex Cep();

    [GeneratedRegex("^[0-9]{6,14}$")]
    private static partial Regex Fone();

    [GeneratedRegex(@"^[^@]+@[^\.]+\..+$")]
    private static partial Regex Email();

    [GeneratedRegex("^[0-9]{8}$")]
    private static partial Regex Rntrc();

    [GeneratedRegex("^([A-Z]{2,3}[0-9]{4}|[A-Z]{3,4}[0-9]{3}|[A-Z0-9]{7})$")]
    private static partial Regex Placa();
}
