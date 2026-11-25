using System.IO.Compression;
using System.Net;
using System.Text;
using System.Xml;
using Emissor.DFe.Webservices;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Testes;

/// <summary>
/// Envelope SOAP 1.2 e status do serviço, sem rede: um manipulador HTTP falso guarda o pedido e devolve respostas no
/// formato descrito no MOC.
/// </summary>
public class SoapTests
{
    [Fact]
    public void Envelope_SemHeaderComAMensagemNoElementoDeDados()
    {
        var servico = new ServicoSoap(new Uri("https://exemplo.invalid/ws"), ServicoStatus.NamespaceStatusMdfe, "mdfeDadosMsg", FormatoMensagem.Xml);
        var envelope = Carregar(ClienteSoap.MontarEnvelope(servico, ServicoStatus.MensagemMdfe(TipoAmbiente.Homologacao)));

        Assert.Equal(ClienteSoap.NamespaceSoap12, envelope.DocumentElement!.NamespaceURI);
        Assert.Empty(envelope.GetElementsByTagName("Header", ClienteSoap.NamespaceSoap12));
        var dados = (XmlElement)envelope.GetElementsByTagName("mdfeDadosMsg", ServicoStatus.NamespaceStatusMdfe)[0]!;
        var consulta = (XmlElement)dados.FirstChild!;
        Assert.Equal("consStatServMDFe", consulta.LocalName);
        Assert.Equal("http://www.portalfiscal.inf.br/mdfe", consulta.NamespaceURI);
        Assert.Equal("2", consulta["tpAmb"]!.InnerText);
        Assert.Equal("STATUS", consulta["xServ"]!.InnerText);
    }

    [Fact]
    public void Envelope_RecepcaoVaiCompactadaEmGzipBase64()
    {
        var mensagem = ServicoStatus.MensagemCte(51, TipoAmbiente.Homologacao);
        var servico = new ServicoSoap(new Uri("https://exemplo.invalid/ws"), "http://www.portalfiscal.inf.br/cte/wsdl/Teste", "cteDadosMsg", FormatoMensagem.GzipBase64);
        var envelope = Carregar(ClienteSoap.MontarEnvelope(servico, mensagem));

        var base64 = envelope.GetElementsByTagName("cteDadosMsg")[0]!.InnerText;
        using var gzip = new GZipStream(new MemoryStream(Convert.FromBase64String(base64)), CompressionMode.Decompress);
        using var leitor = new StreamReader(gzip, Encoding.UTF8);
        Assert.Equal(mensagem.OuterXml, leitor.ReadToEnd());
    }

    [Fact]
    public void MensagemCte_TemUfVersaoEServico()
    {
        var consulta = ServicoStatus.MensagemCte(51, TipoAmbiente.Homologacao);
        Assert.Equal("consStatServCTe", consulta.LocalName);
        Assert.Equal("4.00", consulta.GetAttribute("versao"));
        Assert.Equal("51", consulta["cUF"]!.InnerText);
        Assert.Equal("STATUS", consulta["xServ"]!.InnerText);
    }

    [Fact]
    public async Task StatusMdfe_LeORetornoDaSefaz()
    {
        var falso = new ManipuladorFalso(HttpStatusCode.OK, Resposta("""
            <mdfeStatusServicoMDFResult xmlns="http://www.portalfiscal.inf.br/mdfe/wsdl/MDFeStatusServico">
              <retConsStatServMDFe versao="3.00" xmlns="http://www.portalfiscal.inf.br/mdfe">
                <tpAmb>2</tpAmb><verAplic>RS20261125</verAplic><cStat>107</cStat><xMotivo>Servico em Operacao</xMotivo>
                <cUF>43</cUF><dhRecbto>2026-11-25T14:00:00-03:00</dhRecbto><tMed>1</tMed>
              </retConsStatServMDFe>
            </mdfeStatusServicoMDFResult>
            """));
        using var cliente = new HttpClient(falso);

        var status = await ServicoStatus.ConsultarMdfeAsync(cliente, "MT", TipoAmbiente.Homologacao);

        Assert.True(status.EmOperacao);
        Assert.Equal(107, status.Codigo);
        Assert.Equal("43", status.UfAtendida);
        Assert.Equal("1", status.TempoMedio);
        Assert.Equal("mdfe-homologacao.svrs.rs.gov.br", falso.Pedido!.RequestUri!.Host);
        Assert.Equal("application/soap+xml", falso.Pedido.Content!.Headers.ContentType!.MediaType);
        Assert.Contains("consStatServMDFe", falso.Corpo, StringComparison.Ordinal);
    }

    [Fact]
    public async Task StatusCte_UsaOAutorizadorDeMt()
    {
        var falso = new ManipuladorFalso(HttpStatusCode.OK, Resposta("""
            <retConsStatServCTe versao="4.00" xmlns="http://www.portalfiscal.inf.br/cte">
              <tpAmb>2</tpAmb><verAplic>MT-1.0</verAplic><cStat>107</cStat><xMotivo>Servico em Operacao</xMotivo>
              <cUF>51</cUF><dhRecbto>2026-11-25T13:00:00-04:00</dhRecbto>
            </retConsStatServCTe>
            """));
        using var cliente = new HttpClient(falso);

        var status = await ServicoStatus.ConsultarCteAsync(cliente, "MT", 51, TipoAmbiente.Homologacao);

        Assert.True(status.EmOperacao);
        Assert.Equal("51", status.UfAtendida);
        Assert.Null(status.TempoMedio);
        Assert.Equal("homologacao.sefaz.mt.gov.br", falso.Pedido!.RequestUri!.Host);
        Assert.Contains(ServicoStatus.NamespaceStatusCte, falso.Corpo, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SoapFault_ViraMensagemComOMotivo()
    {
        var falso = new ManipuladorFalso(HttpStatusCode.InternalServerError, $"""
            <soap12:Envelope xmlns:soap12="{ClienteSoap.NamespaceSoap12}"><soap12:Body><soap12:Fault>
              <soap12:Code><soap12:Value>soap12:Receiver</soap12:Value></soap12:Code>
              <soap12:Reason><soap12:Text xml:lang="pt-BR">Namespace nao reconhecido</soap12:Text></soap12:Reason>
            </soap12:Fault></soap12:Body></soap12:Envelope>
            """);
        using var cliente = new HttpClient(falso);

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => ServicoStatus.ConsultarMdfeAsync(cliente, "MT", TipoAmbiente.Homologacao));
        Assert.Contains("Namespace nao reconhecido", erro.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Http403_ExplicaOCertificado()
    {
        using var cliente = new HttpClient(new ManipuladorFalso(HttpStatusCode.Forbidden, "<html>403 - Forbidden</html>"));

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => ServicoStatus.ConsultarMdfeAsync(cliente, "MT", TipoAmbiente.Homologacao));
        Assert.Contains("certificado", erro.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Producao_NaoChegaAEnviar()
    {
        var falso = new ManipuladorFalso(HttpStatusCode.OK, string.Empty);
        using var cliente = new HttpClient(falso);

        await Assert.ThrowsAsync<RegraNegocioException>(() => ServicoStatus.ConsultarMdfeAsync(cliente, "MT", TipoAmbiente.Producao));
        Assert.Null(falso.Pedido);
    }

    private static string Resposta(string corpo) =>
        $"""<soap12:Envelope xmlns:soap12="{ClienteSoap.NamespaceSoap12}"><soap12:Body>{corpo}</soap12:Body></soap12:Envelope>""";

    private static XmlDocument Carregar(string xml)
    {
        var documento = new XmlDocument();
        documento.LoadXml(xml);
        return documento;
    }

    /// <summary>Servidor HTTP falso: guarda o pedido e devolve a resposta fixa.</summary>
    private sealed class ManipuladorFalso(HttpStatusCode situacao, string resposta) : HttpMessageHandler
    {
        public HttpRequestMessage? Pedido { get; private set; }

        public string Corpo { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Pedido = request;
            Corpo = request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(situacao) { Content = new StringContent(resposta, Encoding.UTF8, "application/soap+xml") };
        }
    }
}
