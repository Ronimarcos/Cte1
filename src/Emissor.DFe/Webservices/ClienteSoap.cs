using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Webservices;

/// <summary>Como a mensagem vai no elemento de dados do SOAP.</summary>
public enum FormatoMensagem
{
    /// <summary>XML sem compactação (status, consulta, eventos).</summary>
    Xml,

    /// <summary>XML compactado em GZip e convertido para Base64 (recepção do CT-e e do MDF-e).</summary>
    GzipBase64,
}

/// <summary>Um serviço da SEFAZ do ponto de vista do envelope SOAP.</summary>
/// <param name="Endereco">URL do serviço (da <see cref="TabelaWebservices"/>).</param>
/// <param name="NamespaceWsdl">Namespace do elemento de dados (ex.: <c>http://www.portalfiscal.inf.br/mdfe/wsdl/MDFeStatusServico</c>).</param>
/// <param name="ElementoDados">Elemento de dados: <c>cteDadosMsg</c> ou <c>mdfeDadosMsg</c>.</param>
/// <param name="Formato">Compactação da mensagem.</param>
public sealed record ServicoSoap(Uri Endereco, string NamespaceWsdl, string ElementoDados, FormatoMensagem Formato);

/// <summary>
/// Envelope SOAP 1.2 dos webservices do CT-e e do MDF-e conforme o MOC (§3.4 de cada manual): Document/Literal, sem
/// SOAP Header (descontinuado), mensagem no elemento de dados do <c>Body</c>. O retorno é localizado pelo nome do
/// elemento XML esperado dentro do <c>Body</c>, sem depender do elemento SOAP que o envolve.
/// </summary>
public static class ClienteSoap
{
    /// <summary>Namespace do envelope SOAP 1.2.</summary>
    public const string NamespaceSoap12 = "http://www.w3.org/2003/05/soap-envelope";

    /// <summary>Monta o envelope com a mensagem.</summary>
    /// <param name="servico">Serviço chamado.</param>
    /// <param name="mensagem">XML da mensagem (ex.: <c>consStatServMDFe</c>), já validado contra o XSD.</param>
    public static string MontarEnvelope(ServicoSoap servico, XmlElement mensagem)
    {
        var xml = new XmlDocument();
        var envelope = xml.CreateElement("soap12", "Envelope", NamespaceSoap12);
        xml.AppendChild(envelope);
        var corpo = xml.CreateElement("soap12", "Body", NamespaceSoap12);
        envelope.AppendChild(corpo);
        var dados = xml.CreateElement(servico.ElementoDados, servico.NamespaceWsdl);
        corpo.AppendChild(dados);

        if (servico.Formato == FormatoMensagem.GzipBase64)
        {
            dados.InnerText = CompactarBase64(mensagem.OuterXml);
        }
        else
        {
            dados.AppendChild(xml.ImportNode(mensagem, deep: true));
        }

        return xml.OuterXml;
    }

    /// <summary>Compacta o texto em GZip (UTF-8) e converte para Base64.</summary>
    /// <param name="texto">XML da mensagem.</param>
    public static string CompactarBase64(string texto)
    {
        using var memoria = new MemoryStream();
        using (var gzip = new GZipStream(memoria, CompressionLevel.Optimal, leaveOpen: true))
        {
            var bytes = Encoding.UTF8.GetBytes(texto);
            gzip.Write(bytes, 0, bytes.Length);
        }

        return Convert.ToBase64String(memoria.ToArray());
    }

    /// <summary>Envia a mensagem e devolve o elemento de retorno esperado.</summary>
    /// <param name="cliente">Cliente HTTPS com o certificado da empresa (<see cref="ClienteHttpSefaz"/>).</param>
    /// <param name="servico">Serviço chamado.</param>
    /// <param name="mensagem">XML da mensagem.</param>
    /// <param name="elementoRetorno">Nome do elemento de retorno (ex.: <c>retConsStatServMDFe</c>).</param>
    /// <param name="cancelamento">Cancelamento da chamada.</param>
    /// <exception cref="RegraNegocioException">Falha de comunicação, recusa da SEFAZ (SOAP Fault, HTTP) ou retorno inesperado.</exception>
    public static async Task<XmlElement> EnviarAsync(
        HttpClient cliente, ServicoSoap servico, XmlElement mensagem, string elementoRetorno, CancellationToken cancelamento = default)
    {
        using var conteudo = new StringContent(MontarEnvelope(servico, mensagem), Encoding.UTF8);
        conteudo.Headers.ContentType = new MediaTypeHeaderValue("application/soap+xml") { CharSet = "utf-8" };

        string resposta;
        HttpStatusCode situacao;
        try
        {
            using var retorno = await cliente.PostAsync(servico.Endereco, conteudo, cancelamento);
            situacao = retorno.StatusCode;
            resposta = await retorno.Content.ReadAsStringAsync(cancelamento);
        }
        catch (TaskCanceledException) when (!cancelamento.IsCancellationRequested)
        {
            throw new RegraNegocioException($"A SEFAZ não respondeu no tempo limite ({servico.Endereco.Host}). Tente de novo em instantes.");
        }
        catch (HttpRequestException erro)
        {
            throw new RegraNegocioException($"Não foi possível conectar à SEFAZ ({servico.Endereco.Host}): {erro.GetBaseException().Message}");
        }

        return LerRetorno(resposta, situacao, elementoRetorno, servico.Endereco.Host);
    }

    /// <summary>Lê a resposta SOAP: devolve o elemento de retorno ou explica a recusa.</summary>
    /// <param name="resposta">Corpo da resposta HTTP.</param>
    /// <param name="situacao">Código HTTP.</param>
    /// <param name="elementoRetorno">Nome do elemento de retorno.</param>
    /// <param name="servidor">Servidor, para a mensagem.</param>
    /// <exception cref="RegraNegocioException">SOAP Fault, HTTP de erro sem retorno ou retorno inesperado.</exception>
    public static XmlElement LerRetorno(string resposta, HttpStatusCode situacao, string elementoRetorno, string servidor)
    {
        XmlDocument? xml = null;
        if (resposta.TrimStart().StartsWith('<'))
        {
            try
            {
                xml = new XmlDocument { XmlResolver = null };
                xml.LoadXml(resposta);
            }
            catch (XmlException)
            {
                xml = null;
            }
        }

        var retorno = xml?.GetElementsByTagName(elementoRetorno).OfType<XmlElement>().FirstOrDefault();
        if (retorno is not null)
        {
            return retorno;
        }

        var falha = xml?.GetElementsByTagName("Fault", NamespaceSoap12).OfType<XmlElement>().FirstOrDefault();
        if (falha is not null)
        {
            var motivo = falha.GetElementsByTagName("Text", NamespaceSoap12).OfType<XmlElement>().FirstOrDefault()?.InnerText;
            throw new RegraNegocioException($"A SEFAZ recusou a mensagem ({servidor}): {motivo ?? falha.InnerText}".Trim());
        }

        if (situacao is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
        {
            throw new RegraNegocioException(
                $"A SEFAZ recusou o acesso ({servidor}, HTTP {(int)situacao}). Confira se o certificado da empresa é ICP-Brasil válido e de quem emite o documento.");
        }

        throw new RegraNegocioException(
            $"Resposta inesperada da SEFAZ ({servidor}, HTTP {(int)situacao}): o retorno {elementoRetorno} não veio na mensagem.");
    }
}
