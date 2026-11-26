using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Emissor.Dados;
using Emissor.DFe;
using Emissor.DFe.Assinatura;
using Emissor.DFe.Eventos;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;

namespace Emissor.Testes;

/// <summary>
/// Eventos sem transmissão (P4d1): montagem, assinatura e validação contra os XSD oficiais dos eventos do CT-e 4.00 e
/// do MDF-e 3.00b (envelope e detalhe). O certificado é autoassinado e criado em memória.
/// </summary>
public class EventosXmlTests
{
    private const string Protocolo = "151260000012345";
    private static readonly DateTimeOffset Momento = new(2026, 11, 26, 9, 15, 0, TimeSpan.FromHours(-4));

    private static readonly Empresa Alfa = DadosDemonstracao.Empresas()[0];
    private static readonly string ChaveCte = ChaveAcesso.Montar(51, new DateTime(2026, 11, 1), Alfa.Cnpj, 57, 1, 10, 1, 12345678);
    private static readonly string ChaveMdfe = ChaveAcesso.Montar(51, new DateTime(2026, 11, 1), Alfa.Cnpj, 58, 1, 7, 1, 87654321);

    [Fact]
    public void CancelamentoCte_ValidoEAssinado()
    {
        var evento = new EventoDocumento { Tipo = TipoEvento.CancelamentoCte, DocumentoId = 1, Justificativa = "  Valor do frete   informado errado  " };

        var xml = GerarValido(evento, ChaveCte);

        var info = (XmlElement)xml.GetElementsByTagName("infEvento")[0]!;
        Assert.Equal("ID110111" + ChaveCte + "001", info.GetAttribute("Id"));
        Assert.Equal("eventoCTe", xml.DocumentElement!.LocalName);
        Assert.Equal("4.00", xml.DocumentElement.GetAttribute("versao"));
        Assert.Equal("51", Texto(xml, "cOrgao"));
        Assert.Equal("2", Texto(xml, "tpAmb"));
        Assert.Equal(Alfa.Cnpj, Texto(xml, "CNPJ"));
        Assert.Equal("2026-11-26T09:15:00-04:00", Texto(xml, "dhEvento"));
        Assert.Equal(Protocolo, Texto(xml, "nProt"));
        Assert.Equal("Valor do frete informado errado", Texto(xml, "xJust"));
        Assert.Equal("Cancelamento", Texto(xml, "descEvento"));
    }

    [Fact]
    public void CartaCorrecao_LevaAsCorrecoesEAsCondicoesDeUso()
    {
        var evento = new EventoDocumento
        {
            Tipo = TipoEvento.CartaCorrecaoCte,
            DocumentoId = 1,
            Sequencia = 2,
            Correcoes =
            [
                new() { Grupo = "compl", Campo = "xObs", Valor = "Entrega no período da manhã" },
                new() { Grupo = "infQ", Campo = "qCarga", Valor = "1500.0000", NumeroItem = 1 },
            ],
        };

        var xml = GerarValido(evento, ChaveCte);

        Assert.Equal("ID110110" + ChaveCte + "002", ((XmlElement)xml.GetElementsByTagName("infEvento")[0]!).GetAttribute("Id"));
        Assert.Equal(2, xml.GetElementsByTagName("infCorrecao").Count);
        Assert.Equal("01", Texto(xml, "nroItemAlterado"));
        Assert.StartsWith("A Carta de Correção é disciplinada", Texto(xml, "xCondUso"), StringComparison.Ordinal);
        Assert.Empty(xml.GetElementsByTagName("nProt"));
    }

    [Fact]
    public void CancelamentoMdfe_Valido()
    {
        var evento = new EventoDocumento { Tipo = TipoEvento.CancelamentoMdfe, DocumentoId = 1, Justificativa = "Viagem cancelada pelo cliente" };

        var xml = GerarValido(evento, ChaveMdfe);

        Assert.Equal("eventoMDFe", xml.DocumentElement!.LocalName);
        Assert.Equal("3.00", xml.DocumentElement.GetAttribute("versao"));
        Assert.Equal("ID110111" + ChaveMdfe + "01", ((XmlElement)xml.GetElementsByTagName("infEvento")[0]!).GetAttribute("Id"));
        Assert.Equal(ChaveMdfe, Texto(xml, "chMDFe"));
    }

    [Fact]
    public void EncerramentoMdfe_LevaUfEMunicipio()
    {
        var evento = new EventoDocumento
        {
            Tipo = TipoEvento.EncerramentoMdfe,
            DocumentoId = 1,
            DataEncerramento = new DateTime(2026, 11, 25),
            MunicipioEncerramentoCodigo = 5002704,
        };

        var xml = GerarValido(evento, ChaveMdfe);

        Assert.Equal("2026-11-25", Texto(xml, "dtEnc"));
        Assert.Equal("50", Texto(xml, "cUF"));
        Assert.Equal("5002704", Texto(xml, "cMun"));
        Assert.Equal(Protocolo, Texto(xml, "nProt"));
    }

    [Fact]
    public void InclusaoCondutor_LevaNomeECpf()
    {
        var evento = new EventoDocumento
        {
            Tipo = TipoEvento.InclusaoCondutorMdfe,
            DocumentoId = 1,
            Sequencia = 3,
            NomeCondutor = " JOSE   DA SILVA ",
            CpfCondutor = "12345678909",
        };

        var xml = GerarValido(evento, ChaveMdfe);

        Assert.Equal("ID110114" + ChaveMdfe + "03", ((XmlElement)xml.GetElementsByTagName("infEvento")[0]!).GetAttribute("Id"));
        Assert.Equal("JOSE DA SILVA", Texto(xml, "xNome"));
        Assert.Equal("Inclusao Condutor", Texto(xml, "descEvento"));
    }

    [Fact]
    public void DadosInvalidos_NaoMontam()
    {
        using var certificado = CriarCertificado();
        var evento = new EventoDocumento { Tipo = TipoEvento.CancelamentoCte, DocumentoId = 1, Justificativa = "curta" };

        var resultado = GeradorXmlEvento.Gerar(Dados(evento, ChaveCte, protocolo: "123"), certificado);

        Assert.False(resultado.Valido);
        Assert.Contains(resultado.Pendencias, p => p.Contains("justificativa", StringComparison.Ordinal));
        Assert.Contains(resultado.Pendencias, p => p.Contains("15 dígitos", StringComparison.Ordinal));
    }

    private static XmlDocument GerarValido(EventoDocumento evento, string chave)
    {
        using var certificado = CriarCertificado();
        var resultado = GeradorXmlEvento.Gerar(Dados(evento, chave), certificado);
        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));

        var xml = new XmlDocument { PreserveWhitespace = true };
        xml.LoadXml(resultado.Xml);
        Assert.True(AssinadorXml.Conferir(xml));
        return xml;
    }

    private static DadosEvento Dados(EventoDocumento evento, string chave, string protocolo = Protocolo) =>
        new(evento, Alfa, chave, protocolo, TipoAmbiente.Homologacao, Momento);

    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=TRANSPORTADORA EXEMPLO ALFA LTDA:11111111000111", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return pedido.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(30));
    }

    private static string Texto(XmlDocument xml, string elemento) => xml.GetElementsByTagName(elemento)[0]!.InnerText;
}
