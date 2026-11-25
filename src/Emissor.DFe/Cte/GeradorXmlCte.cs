using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Emissor.DFe.Assinatura;
using Emissor.DFe.Validacao;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Cte;

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

        return new XmlGerado(XmlGerado.Texto(documento), montado.Chave, erros);
    }

}
