using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Emissor.DFe.Assinatura;
using Emissor.DFe.Validacao;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Cte;

/// <summary>Resultado da geração do XML do CT-e.</summary>
/// <param name="Xml">XML assinado (vazio se a montagem não foi possível).</param>
/// <param name="Chave">Chave de acesso.</param>
/// <param name="Pendencias">Pendências da montagem ou erros da validação contra o XSD; vazia se o XML está válido.</param>
public sealed record XmlGerado(string Xml, string Chave, IReadOnlyList<string> Pendencias)
{
    /// <summary>Indica se o XML foi gerado e passou na validação.</summary>
    public bool Valido => Xml.Length > 0 && Pendencias.Count == 0;
}

/// <summary>
/// Gera o XML do CT-e sem transmitir: monta pelas classes do XSD, assina com o certificado da empresa e valida contra
/// os XSD do CT-e e do modal rodoviário.
/// </summary>
public static class GeradorXmlCte
{
    /// <summary>Monta, assina e valida.</summary>
    /// <param name="dados">CT-e, empresa e cadastros.</param>
    /// <param name="certificado">Certificado da empresa, com chave privada.</param>
    /// <exception cref="RegraNegocioException">Falha ao assinar (certificado sem chave, PIN recusado...).</exception>
    public static XmlGerado Gerar(DadosEmissaoCte dados, X509Certificate2 certificado)
    {
        var montado = MontadorCte.Montar(dados);
        if (montado.Documento is null)
        {
            return new XmlGerado(string.Empty, string.Empty, montado.Pendencias);
        }

        var documento = MontadorCte.Serializar(montado.Documento);
        AssinadorXml.Assinar(documento, "infCte", certificado);

        var erros = new List<string>(ValidadorXsd.Validar(documento.DocumentElement!, ValidadorXsd.SchemaCte));
        var rodo = documento.GetElementsByTagName("rodo").OfType<XmlElement>().FirstOrDefault();
        if (rodo is not null)
        {
            erros.AddRange(ValidadorXsd.Validar(rodo, ValidadorXsd.SchemaCteRodoviario));
        }

        return new XmlGerado(Texto(documento), montado.Chave, erros);
    }

    /// <summary>XML em UTF-8, sem quebras nem indentação (que mudariam a assinatura), com a declaração.</summary>
    private static string Texto(XmlDocument documento)
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
