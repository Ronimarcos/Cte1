using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Emissor.DFe.Assinatura;
using Emissor.DFe.Validacao;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Mdfe;

/// <summary>
/// Gera o XML do MDF-e sem transmitir: monta pelas classes do XSD, assina com o certificado da empresa e valida contra
/// os XSD do MDF-e e do modal rodoviário.
/// </summary>
public static class GeradorXmlMdfe
{
    /// <summary>Monta, assina e valida.</summary>
    /// <param name="dados">MDF-e, empresa e cadastros.</param>
    /// <param name="certificado">Certificado da empresa, com chave privada.</param>
    /// <exception cref="RegraNegocioException">Falha ao assinar (certificado sem chave, PIN recusado...).</exception>
    public static XmlGerado Gerar(DadosEmissaoMdfe dados, X509Certificate2 certificado)
    {
        var montado = MontadorMdfe.Montar(dados);
        if (montado.Documento is null)
        {
            return new XmlGerado(string.Empty, string.Empty, montado.Pendencias);
        }

        var documento = MontadorMdfe.Serializar(montado.Documento);
        AssinadorXml.Assinar(documento, "infMDFe", certificado);

        var erros = new List<string>(ValidadorXsd.Validar(documento.DocumentElement!, ValidadorXsd.SchemaMdfe));
        var rodo = documento.GetElementsByTagName("rodo").OfType<XmlElement>().FirstOrDefault();
        if (rodo is not null)
        {
            erros.AddRange(ValidadorXsd.Validar(rodo, ValidadorXsd.SchemaMdfeRodoviario));
        }

        return new XmlGerado(XmlGerado.Texto(documento), montado.Chave, erros);
    }
}
