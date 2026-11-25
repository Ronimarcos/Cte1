using System.Text;
using System.Xml;

namespace Emissor.DFe;

/// <summary>Resultado da geração do XML (CT-e ou MDF-e) sem transmissão.</summary>
/// <param name="Xml">XML assinado (vazio se a montagem não foi possível).</param>
/// <param name="Chave">Chave de acesso.</param>
/// <param name="Pendencias">Pendências da montagem ou erros da validação contra o XSD; vazia se o XML está válido.</param>
public sealed record XmlGerado(string Xml, string Chave, IReadOnlyList<string> Pendencias)
{
    /// <summary>Indica se o XML foi gerado e passou na validação.</summary>
    public bool Valido => Xml.Length > 0 && Pendencias.Count == 0;

    /// <summary>XML em UTF-8, sem quebras nem indentação (que mudariam a assinatura), com a declaração.</summary>
    /// <param name="documento">Documento assinado.</param>
    public static string Texto(XmlDocument documento)
    {
        var configuracao = new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = false, OmitXmlDeclaration = false };
        using var memoria = new MemoryStream();
        using (var escritor = XmlWriter.Create(memoria, configuracao))
        {
            documento.Save(escritor);
        }

        return Encoding.UTF8.GetString(memoria.ToArray());
    }
}
