using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Emissor.Dados;
using Emissor.DFe;
using Emissor.DFe.Assinatura;
using Emissor.DFe.Mdfe;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;

namespace Emissor.Testes;

/// <summary>
/// Motor DF-e do MDF-e sem transmissão: montagem, assinatura e validação contra os XSD oficiais do MDF-e 3.00b.
/// O certificado é autoassinado e criado em memória.
/// </summary>
public class XmlMdfeTests
{
    private static readonly IReadOnlyDictionary<int, Municipio> Municipios =
        CargaMunicipios.LerListaOficial().ToDictionary(m => m.Codigo);

    [Fact]
    public void ComChaveDeCteDigitada_GeraXmlValidoEAssinado()
    {
        using var certificado = CriarCertificado();
        var resultado = GeradorXmlMdfe.Gerar(Dados(MdfeComChave()), certificado);

        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        Assert.True(ChaveAcesso.Valida(resultado.Chave));
        Assert.Equal("58", resultado.Chave[20..22]);

        var xml = Carregar(resultado.Xml);
        Assert.True(AssinadorXml.Conferir(xml));
        Assert.Equal("MDFe" + resultado.Chave, ((XmlElement)xml.GetElementsByTagName("infMDFe")[0]!).GetAttribute("Id"));
        Assert.Equal("2", Texto(xml, "tpAmb"));
        Assert.Equal("4", Texto(xml, "nMDF"));
        Assert.Equal("MS", Texto(xml, "UFPer"));
        Assert.Equal("123456789012", Texto(xml, "CIOT"));
        Assert.Equal("AAA1A11", Texto(xml, "placa"));
        Assert.Single(xml.GetElementsByTagName("veicReboque"));
        Assert.Equal("1", Texto(xml, "qCTe"));
        Assert.Equal("AVB-000123", Texto(xml, "nAver"));
        Assert.Empty(xml.GetElementsByTagName("infMDFeSupl"));
    }

    [Fact]
    public void VeiculoDeTerceiro_LevaOProprietarioComIeEUf()
    {
        using var certificado = CriarCertificado();
        var mdfe = MdfeComChave();
        mdfe.VeiculoTracaoId = 3;
        mdfe.ReboquesIds = [];

        var resultado = GeradorXmlMdfe.Gerar(Dados(mdfe), certificado);

        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        var xml = Carregar(resultado.Xml);
        var prop = (XmlElement)xml.GetElementsByTagName("prop").Cast<XmlNode>().Single();
        Assert.Equal("ISENTO", prop.GetElementsByTagName("IE")[0]!.InnerText);
        Assert.Equal("MT", prop.GetElementsByTagName("UF")[0]!.InnerText);
    }

    [Fact]
    public void CargaPropriaComNfe_SemSeguroGeraXmlValido()
    {
        using var certificado = CriarCertificado();
        var mdfe = MdfeComChave();
        mdfe.TipoEmitente = TipoEmitenteMdfe.CargaPropria;
        mdfe.TipoTransportador = null;
        mdfe.Ciots = [];
        mdfe.ApoliceId = null;
        mdfe.Averbacoes = [];
        mdfe.Descargas[0].Documentos = [new() { Tipo = TipoDocumentoMdfe.Nfe, Chave = ChaveAcesso.Montar(51, DateTime.Today, "44444444000144", 55, 1, 1501, 1, 1) }];

        var resultado = GeradorXmlMdfe.Gerar(Dados(mdfe), certificado);

        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        var xml = Carregar(resultado.Xml);
        Assert.Single(xml.GetElementsByTagName("infNFe"));
        Assert.Empty(xml.GetElementsByTagName("seg"));
    }

    [Fact]
    public void CteDoSistemaSemChave_EhPendencia()
    {
        using var certificado = CriarCertificado();
        var resultado = GeradorXmlMdfe.Gerar(Dados(DadosDemonstracao.Mdfes()[0]), certificado);

        Assert.False(resultado.Valido);
        Assert.Empty(resultado.Xml);
        Assert.Contains(resultado.Pendencias, p => p.StartsWith("CT-e nº 1 emitido neste sistema ainda não tem chave", StringComparison.Ordinal));
    }

    [Fact]
    public void ProprietarioSemUfEReboqueSemCapacidade_SaoPendencias()
    {
        using var certificado = CriarCertificado();
        var mdfe = MdfeComChave();
        mdfe.VeiculoTracaoId = 3;
        var proprietarios = DadosDemonstracao.Proprietarios().ToDictionary(p => p.Id);
        proprietarios[1].Uf = string.Empty;
        var veiculos = DadosDemonstracao.Veiculos().ToDictionary(v => v.Id);
        veiculos[2].CapacidadeKg = null;

        var resultado = GeradorXmlMdfe.Gerar(Dados(mdfe) with { Proprietarios = proprietarios, Veiculos = veiculos }, certificado);

        Assert.False(resultado.Valido);
        Assert.Contains("UF do proprietário MOTORISTA AGREGADO EXEMPLO (complete o cadastro do proprietário)", resultado.Pendencias);
        Assert.Contains("Capacidade em kg do reboque AAA2B22 (obrigatória no MDF-e)", resultado.Pendencias);
    }

    [Fact]
    public void Validacao_ApontaOCampoForaDoXsd()
    {
        using var certificado = CriarCertificado();
        var mdfe = MdfeComChave();
        mdfe.ProdutoPredominante = new string('X', 130); // xProd aceita até 120 caracteres

        var resultado = GeradorXmlMdfe.Gerar(Dados(mdfe), certificado);

        Assert.False(resultado.Valido);
        Assert.Contains(resultado.Pendencias, p => p.Contains("xProd", StringComparison.Ordinal));
    }

    [Fact]
    public void Proprietario_ValidaIeEUf()
    {
        var proprietario = DadosDemonstracao.Proprietarios()[0];
        Assert.Empty(proprietario.Validar());

        proprietario.Ie = "12A";
        proprietario.Uf = "XX";
        Assert.Contains("IE com 2 a 14 dígitos ou ISENTO", proprietario.Validar());
        Assert.Contains("UF", proprietario.Validar());
        Assert.Equal("ISENTO", ProprietarioVeiculo.NormalizarIe(" isento "));
        Assert.Equal("123456", ProprietarioVeiculo.NormalizarIe("123.456"));
    }

    /// <summary>MDF-e 1 da demonstração com a chave do CT-e digitada (o CT-e do sistema ainda não tem chave).</summary>
    private static ManifestoEletronico MdfeComChave()
    {
        var mdfe = DadosDemonstracao.Mdfes()[0];
        mdfe.Descargas[0].Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, Chave = ChaveAcesso.Montar(51, DateTime.Today, "11111111000111", 57, 1, 14, 1, 1) }];
        return mdfe;
    }

    private static DadosEmissaoMdfe Dados(ManifestoEletronico mdfe) => new(
        mdfe,
        DadosDemonstracao.Empresas()[0],
        DadosDemonstracao.Series()[1],
        DadosDemonstracao.Series()[1].ProximoNumero,
        DadosDemonstracao.Veiculos().ToDictionary(v => v.Id),
        DadosDemonstracao.Proprietarios().ToDictionary(p => p.Id),
        DadosDemonstracao.Motoristas().ToDictionary(m => m.Id),
        DadosDemonstracao.Apolices().ToDictionary(a => a.Id),
        codigo => Municipios.GetValueOrDefault(codigo),
        TipoAmbiente.Homologacao,
        new DateTimeOffset(DateTime.Today.AddDays(-2).AddHours(7), TimeSpan.FromHours(-4)),
        "Cte1 teste");

    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=TRANSPORTADORA EXEMPLO ALFA LTDA:11111111000111", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return pedido.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(30));
    }

    private static XmlDocument Carregar(string xml)
    {
        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(xml);
        return documento;
    }

    private static string Texto(XmlDocument xml, string elemento) => xml.GetElementsByTagName(elemento)[0]!.InnerText;
}
