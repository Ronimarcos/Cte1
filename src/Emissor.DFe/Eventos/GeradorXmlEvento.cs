using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Emissor.DFe.Assinatura;
using Emissor.DFe.Validacao;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Eventos;

/// <summary>
/// Gera o XML do evento sem transmitir: monta pelas classes do XSD, assina o <c>infEvento</c> com o certificado da
/// empresa e valida o envelope e o detalhe (que o envelope aceita sem validar) contra os XSD de cada um.
/// </summary>
public static class GeradorXmlEvento
{
    /// <summary>Monta, assina e valida.</summary>
    /// <param name="dados">Evento, empresa, chave e protocolo do documento.</param>
    /// <param name="certificado">Certificado da empresa, com chave privada.</param>
    /// <returns>XML assinado, chave do documento e pendências (vazia quando válido).</returns>
    /// <exception cref="RegraNegocioException">Falha ao assinar (certificado sem chave, PIN recusado...).</exception>
    public static XmlGerado Gerar(DadosEvento dados, X509Certificate2 certificado)
    {
        var montado = MontadorEvento.Montar(dados);
        if (montado.Documento is null)
        {
            return new XmlGerado(string.Empty, dados.Chave, montado.Pendencias);
        }

        var documento = montado.Documento;
        AssinadorXml.Assinar(documento, "infEvento", certificado);

        var erros = new List<string>(ValidadorXsd.Validar(documento.DocumentElement!, montado.SchemaEnvelope));
        var detalhe = documento.GetElementsByTagName("detEvento").OfType<XmlElement>().FirstOrDefault()?.ChildNodes.OfType<XmlElement>().FirstOrDefault();
        if (detalhe is null)
        {
            erros.Add("O detalhe do evento (detEvento) ficou vazio.");
        }
        else
        {
            erros.AddRange(ValidadorXsd.Validar(detalhe, montado.SchemaDetalhe));
        }

        return new XmlGerado(XmlGerado.Texto(documento), dados.Chave, erros);
    }
}
