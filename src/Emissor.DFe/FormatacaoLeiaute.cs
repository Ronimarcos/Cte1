using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Emissor.DFe;

/// <summary>
/// Formatação comum dos campos dos leiautes do CT-e e do MDF-e e serialização das classes geradas dos XSD.
/// </summary>
public static class FormatacaoLeiaute
{
    private static readonly CultureInfo Invariante = CultureInfo.InvariantCulture;

    /// <summary>Texto sem espaços nas pontas e sem espaços repetidos (o leiaute não aceita).</summary>
    /// <param name="texto">Texto digitado.</param>
    public static string Texto(string texto) =>
        string.Join(' ', texto.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    /// <summary>Texto opcional: <c>null</c> (campo omitido) quando vazio.</summary>
    /// <param name="texto">Texto digitado.</param>
    public static string? Opcional(string? texto) => string.IsNullOrWhiteSpace(texto) ? null : Texto(texto);

    /// <summary>Valor com duas casas e ponto decimal (<c>TDec_1302</c>).</summary>
    /// <param name="valor">Valor.</param>
    public static string Valor(decimal valor) => valor.ToString("0.00", Invariante);

    /// <summary>Quantidade com quatro casas e ponto decimal (<c>TDec_1104</c>).</summary>
    /// <param name="quantidade">Quantidade.</param>
    public static string Quantidade(decimal quantidade) => quantidade.ToString("0.0000", Invariante);

    /// <summary>Número inteiro sem separadores.</summary>
    /// <param name="numero">Número.</param>
    public static string Inteiro(long numero) => numero.ToString(Invariante);

    /// <summary>Data e hora com o fuso, no formato <c>TDateTimeUTC</c> (ex.: 2026-11-25T10:30:00-04:00).</summary>
    /// <param name="dataHora">Data e hora.</param>
    public static string DataHora(DateTimeOffset dataHora) => dataHora.ToString("yyyy-MM-dd'T'HH:mm:sszzz", Invariante);

    /// <summary>Serializa o documento num <see cref="XmlDocument"/>, com o namespace padrão e sem xsi/xsd.</summary>
    /// <typeparam name="T">Classe gerada do XSD.</typeparam>
    /// <param name="documento">Documento montado.</param>
    /// <param name="namespaceXml">Namespace do leiaute.</param>
    public static XmlDocument Serializar<T>(T documento, string namespaceXml)
    {
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, namespaceXml);
        var xml = new XmlDocument { PreserveWhitespace = false };
        using (var escritor = xml.CreateNavigator()!.AppendChild())
        {
            new XmlSerializer(typeof(T)).Serialize(escritor, documento, namespaces);
        }

        return xml;
    }

    /// <summary>
    /// Serializa um grupo à parte (ex.: o modal em <c>infModal</c>), sem a declaração de namespace repetida, que já
    /// vem do documento em que ele entra.
    /// </summary>
    /// <typeparam name="T">Classe gerada do XSD.</typeparam>
    /// <param name="grupo">Grupo montado.</param>
    /// <param name="namespaceXml">Namespace do leiaute.</param>
    public static XmlElement Elemento<T>(T grupo, string namespaceXml)
    {
        var elemento = Serializar(grupo, namespaceXml).DocumentElement!;
        elemento.RemoveAttribute("xmlns");
        return elemento;
    }
}
