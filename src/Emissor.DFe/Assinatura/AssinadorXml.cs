using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Assinatura;

/// <summary>
/// Assinatura digital dos documentos (XMLDSig enveloped), com os algoritmos fixados no
/// <c>xmldsig-core-schema_v1.01.xsd</c> dos pacotes de leiaute: canonicalização C14N, RSA-SHA1 e digest SHA1.
/// A referência aponta para o <c>Id</c> do grupo assinado (ex.: <c>infCte</c>) e a assinatura entra como último
/// filho do elemento raiz do documento.
/// </summary>
public static class AssinadorXml
{
    /// <summary>Algoritmo de assinatura fixado no XSD.</summary>
    public const string AlgoritmoAssinatura = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

    /// <summary>Algoritmo de digest fixado no XSD.</summary>
    public const string AlgoritmoDigest = "http://www.w3.org/2000/09/xmldsig#sha1";

    /// <summary>Assina o grupo de nome informado (pelo atributo <c>Id</c>) dentro do documento.</summary>
    /// <param name="documento">Documento já montado; recebe o elemento <c>Signature</c>.</param>
    /// <param name="grupo">Nome do elemento assinado (ex.: <c>infCte</c>).</param>
    /// <param name="certificado">Certificado com chave privada (A1 aberto ou A3 do Windows).</param>
    /// <exception cref="RegraNegocioException">Grupo sem <c>Id</c> ou certificado sem chave privada RSA.</exception>
    public static void Assinar(XmlDocument documento, string grupo, X509Certificate2 certificado)
    {
        var elemento = documento.GetElementsByTagName(grupo).OfType<XmlElement>().FirstOrDefault()
            ?? throw new RegraNegocioException($"O grupo {grupo} não foi encontrado no XML.");
        var id = elemento.GetAttribute("Id");
        if (id.Length == 0)
        {
            throw new RegraNegocioException($"O grupo {grupo} não tem o atributo Id.");
        }

        var chave = certificado.GetRSAPrivateKey()
            ?? throw new RegraNegocioException("O certificado não tem chave privada RSA para assinar.");

        var assinatura = new SignedXml(documento) { SigningKey = chave };
        assinatura.SignedInfo!.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
        assinatura.SignedInfo.SignatureMethod = AlgoritmoAssinatura;

        var referencia = new Reference("#" + id) { DigestMethod = AlgoritmoDigest };
        referencia.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        referencia.AddTransform(new XmlDsigC14NTransform());
        assinatura.AddReference(referencia);

        var informacoesChave = new KeyInfo();
        informacoesChave.AddClause(new KeyInfoX509Data(certificado));
        assinatura.KeyInfo = informacoesChave;

        try
        {
            assinatura.ComputeSignature();
        }
        catch (CryptographicException erro)
        {
            throw new RegraNegocioException($"Não foi possível assinar com o certificado: {erro.Message}");
        }

        documento.DocumentElement!.AppendChild(documento.ImportNode(assinatura.GetXml(), deep: true));
    }

    /// <summary>Confere a assinatura do documento com o certificado que está nela.</summary>
    /// <param name="documento">Documento assinado.</param>
    /// <returns>Verdadeiro se a assinatura confere.</returns>
    public static bool Conferir(XmlDocument documento)
    {
        var no = documento.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl).OfType<XmlElement>().FirstOrDefault();
        if (no is null)
        {
            return false;
        }

        var assinatura = new SignedXml(documento);
        assinatura.LoadXml(no);
        return assinatura.CheckSignature();
    }
}
