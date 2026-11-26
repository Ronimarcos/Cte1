using System.Globalization;
using System.Xml;
using QRCoder;

namespace Emissor.DFe.Impressao;

/// <summary>
/// Auxiliares comuns do DACTE e do DAMDFE: leitura do XML por nome de elemento (sem depender do namespace),
/// formatação pt-BR e imagem do QR Code (nível M, UTF-8, conforme os MOC do CT-e e do MDF-e).
/// </summary>
internal static class ImpressaoComum
{
    /// <summary>Cultura das telas e dos documentos auxiliares.</summary>
    public static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Imagem do QR Code (nível M e UTF-8); nula se não houver conteúdo.</summary>
    /// <param name="conteudo">Texto do QR Code.</param>
    public static byte[]? QrCodePng(string conteudo)
    {
        if (conteudo.Length == 0)
        {
            return null;
        }

        using var gerador = new QRCodeGenerator();
        using var dados = gerador.CreateQrCode(conteudo, QRCodeGenerator.ECCLevel.M, forceUtf8: true);
        return new PngByteQRCode(dados).GetGraphic(10);
    }

    /// <summary>Sigla da UF pelos dois primeiros dígitos da chave de acesso; vazio se desconhecida.</summary>
    /// <param name="chave">Chave de acesso.</param>
    public static string UfDaChave(string chave) =>
        chave.Length >= 2 && Ufs.TryGetValue(chave[..2], out var sigla) ? sigla : string.Empty;

    public static string DataHora(string texto) =>
        DateTimeOffset.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data)
            ? data.ToString("dd/MM/yyyy HH:mm:ss", Brasil)
            : texto;

    public static string Dinheiro(string texto) => texto.Length == 0 ? string.Empty : Decimal(texto).ToString("N2", Brasil);

    public static string Percentual(string texto) => texto.Length == 0 ? string.Empty : Decimal(texto).ToString("N2", Brasil) + "%";

    public static decimal Decimal(string texto) =>
        decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var valor) ? valor : 0m;

    public static string Cep(string cep) => cep.Length == 8 ? $"{cep[..5]}-{cep[5..]}" : cep;

    public static string FormatarCnpj(string cnpj) =>
        cnpj.Length == 14 ? $"{cnpj[..2]}.{cnpj[2..5]}.{cnpj[5..8]}/{cnpj[8..12]}-{cnpj[12..]}" : cnpj;

    public static string FormatarCpf(string cpf) =>
        cpf.Length == 11 ? $"{cpf[..3]}.{cpf[3..6]}.{cpf[6..9]}-{cpf[9..]}" : cpf;

    public static string Primeiro(params string[] textos) => textos.FirstOrDefault(t => t.Length > 0) ?? string.Empty;

    public static XmlElement? Primeiro(XmlNode? pai, string nome) =>
        pai switch
        {
            null => null,
            XmlDocument documento => documento.GetElementsByTagName(nome).OfType<XmlElement>().FirstOrDefault(),
            _ => pai.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.LocalName == nome),
        };

    public static IEnumerable<XmlElement> Filhos(XmlElement? pai, string nome) =>
        pai?.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == nome) ?? [];

    public static string Texto(XmlElement? pai, string nome) => Primeiro(pai, nome)?.InnerText.Trim() ?? string.Empty;

    /// <summary>Código IBGE da UF (dois primeiros dígitos da chave) para a sigla.</summary>
    private static readonly IReadOnlyDictionary<string, string> Ufs = new Dictionary<string, string>
    {
        ["11"] = "RO", ["12"] = "AC", ["13"] = "AM", ["14"] = "RR", ["15"] = "PA", ["16"] = "AP", ["17"] = "TO",
        ["21"] = "MA", ["22"] = "PI", ["23"] = "CE", ["24"] = "RN", ["25"] = "PB", ["26"] = "PE", ["27"] = "AL", ["28"] = "SE", ["29"] = "BA",
        ["31"] = "MG", ["32"] = "ES", ["33"] = "RJ", ["35"] = "SP", ["41"] = "PR", ["42"] = "SC", ["43"] = "RS",
        ["50"] = "MS", ["51"] = "MT", ["52"] = "GO", ["53"] = "DF",
    };
}
