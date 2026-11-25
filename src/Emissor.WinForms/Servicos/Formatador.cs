namespace Emissor.WinForms.Servicos;

/// <summary>
/// Máscaras de exibição dos documentos. O banco guarda os valores sem pontuação (ver <c>Formatos</c> no domínio);
/// a formatação é só para a tela.
/// </summary>
public static class Formatador
{
    /// <summary>CNPJ (inclusive alfanumérico) como 00.000.000/0000-00; outros tamanhos ficam como estão.</summary>
    /// <param name="cnpj">CNPJ sem máscara.</param>
    public static string Cnpj(string? cnpj) =>
        cnpj is { Length: 14 } c ? $"{c[..2]}.{c[2..5]}.{c[5..8]}/{c[8..12]}-{c[12..]}" : cnpj ?? string.Empty;

    /// <summary>CPF como 000.000.000-00.</summary>
    /// <param name="cpf">CPF sem máscara.</param>
    public static string Cpf(string? cpf) =>
        cpf is { Length: 11 } c ? $"{c[..3]}.{c[3..6]}.{c[6..9]}-{c[9..]}" : cpf ?? string.Empty;

    /// <summary>CNPJ ou CPF, conforme o tamanho.</summary>
    /// <param name="documento">Documento sem máscara.</param>
    public static string CnpjCpf(string? documento) => documento?.Length == 11 ? Cpf(documento) : Cnpj(documento);

    /// <summary>CEP como 00000-000.</summary>
    /// <param name="cep">CEP sem máscara.</param>
    public static string Cep(string? cep) => cep is { Length: 8 } c ? $"{c[..5]}-{c[5..]}" : cep ?? string.Empty;
}
