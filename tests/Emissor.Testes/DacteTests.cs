using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Emissor.Dados;
using Emissor.DFe.Cte;
using Emissor.DFe.Impressao;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using QuestPDF.Fluent;

namespace Emissor.Testes;

/// <summary>
/// DACTE (P5a): código de barras CODE-128C pelo exemplo do Anexo II do MOC, leitura dos quadros a partir do XML do
/// CT-e e geração do PDF. Com a variável de ambiente <c>CTE1_DACTE_SAIDA</c>, o PDF de exemplo é gravado nesse
/// caminho para conferência visual.
/// </summary>
public class DacteTests
{
    private static readonly IReadOnlyDictionary<int, Municipio> Municipios =
        CargaMunicipios.LerListaOficial().ToDictionary(m => m.Codigo);

    [Fact]
    public void Codigo128C_ExemploDoManual()
    {
        // Anexo II, item 2.16.1: chave "09758364", soma ponderada 769, 769 % 103 = 48.
        Assert.Equal(48, CodigoBarras128C.DigitoVerificador("09758364"));

        // Item 2.16.2: START, 09, 75, 83, 64, DV 48 e STOP.
        Assert.Equal("211232" + "221213" + "241211" + "114212" + "111422" + "313121" + "2331112", CodigoBarras128C.Larguras("09758364"));
    }

    [Fact]
    public void Codigo128C_ChaveDe44DigitosTem297ModulosComAMargem()
    {
        var chave = new string('1', 44);
        var modulos = CodigoBarras128C.Larguras(chave).Sum(c => c - '0') + 2 * CodigoBarras128C.MargemClara;
        Assert.Equal(297, modulos);
        Assert.StartsWith("<svg", CodigoBarras128C.Svg(chave), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12A4")]
    [InlineData("")]
    public void Codigo128C_RecusaTextoImparOuComLetra(string texto)
    {
        Assert.Throws<ArgumentException>(() => CodigoBarras128C.Larguras(texto));
    }

    [Fact]
    public void Leitura_PreencheOsQuadrosDoXmlDeTeste()
    {
        var (dados, chave) = DacteDaDemonstracao();

        Assert.Equal(TipoAmbiente.Homologacao, dados.Ambiente);
        Assert.Equal(chave, dados.Chave);
        Assert.Equal("RODOVIÁRIO", dados.Modal);
        Assert.Equal("57", dados.Modelo);
        Assert.Equal("15", dados.Numero);
        Assert.Equal("8.820,00", dados.ValorTotalPrestacao);
        Assert.StartsWith("90", dados.SituacaoTributaria, StringComparison.Ordinal);
        Assert.Equal("11111111", dados.Rntrc);
        Assert.NotEmpty(dados.Emitente.Nome);
        Assert.NotEmpty(dados.Remetente.Nome);
        Assert.NotEmpty(dados.TomadorPapel);
        Assert.NotEmpty(dados.Componentes);
        Assert.NotEmpty(dados.Documentos);
        Assert.Empty(dados.Protocolo);
        Assert.Equal($"https://homologacao.sefaz.mt.gov.br/cte/qrcode?chCTe={chave}&tpAmb=2", dados.QrCode);
        Assert.Equal(11, dados.ChaveFormatada.Split(' ').Length);
    }

    [Fact]
    public void Leitura_ConvertePesoEmToneladaParaQuilos()
    {
        const string xml = """
            <CTe xmlns="http://www.portalfiscal.inf.br/cte"><infCte Id="CTe51261111111111000111570010000000151123456780" versao="4.00">
            <ide><tpAmb>2</tpAmb><modal>01</modal></ide>
            <infCTeNorm><infCarga><infQ><cUnid>02</cUnid><tpMed>PESO BRUTO</tpMed><qCarga>1.5000</qCarga></infQ></infCarga></infCTeNorm>
            </infCte></CTe>
            """;
        var quantidade = Assert.Single(DadosDacte.Ler(xml).Quantidades);
        Assert.Equal("PESO BRUTO", quantidade.Descricao);
        Assert.Equal("1.500,0000 KG", quantidade.Valor);
    }

    [Fact]
    public void Leitura_ComProtocoloAutorizado()
    {
        const string xml = """
            <cteProc xmlns="http://www.portalfiscal.inf.br/cte" versao="4.00"><CTe><infCte Id="CTe51261111111111000111570010000000151123456780" versao="4.00">
            <ide><tpAmb>2</tpAmb><modal>01</modal></ide></infCte></CTe>
            <protCTe versao="4.00"><infProt><cStat>100</cStat><nProt>151260000012345</nProt><dhRecbto>2026-11-26T10:00:00-04:00</dhRecbto></infProt></protCTe></cteProc>
            """;
        Assert.Equal("151260000012345 26/11/2026 10:00:00", DadosDacte.Ler(xml).Protocolo);
    }

    [Fact]
    public void Leitura_RecusaXmlQueNaoEDeCte()
    {
        Assert.Throws<RegraNegocioException>(() => DadosDacte.Ler("<MDFe/>"));
        Assert.Throws<RegraNegocioException>(() => DadosDacte.Ler("não é xml"));
    }

    [Fact]
    public void Pdf_GeradoComCabecalhoEPdfValido()
    {
        var (dados, _) = DacteDaDemonstracao();

        var pdf = GeradorDacte.Gerar(dados);

        Assert.True(pdf.Length > 10_000);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));
        if (Environment.GetEnvironmentVariable("CTE1_DACTE_SAIDA") is { Length: > 0 } saida)
        {
            File.WriteAllBytes(saida, pdf);
            var imagens = GeradorDacte.Documento(dados).GenerateImages(new QuestPDF.Infrastructure.ImageGenerationSettings { RasterDpi = 110 }).ToList();
            File.WriteAllBytes(Path.ChangeExtension(saida, ".png"), imagens[0]);
        }
    }

    [Fact]
    public void Pdf_MuitosDocumentosSeguemNaFolhaSeguinte()
    {
        var (dados, _) = DacteDaDemonstracao();
        var muitos = new DadosDacte
        {
            Ambiente = dados.Ambiente,
            Chave = dados.Chave,
            Emitente = dados.Emitente,
            Documentos = Enumerable.Range(1, 400).Select(i => new DocumentoOriginarioDacte("NF-e", dados.Chave)).ToList(),
        };

        var pdf = Encoding.ASCII.GetString(GeradorDacte.Gerar(muitos));

        Assert.True(System.Text.RegularExpressions.Regex.Matches(pdf, @"/Type\s*/Page[^s]").Count >= 2);
    }

    private static (DadosDacte Dados, string Chave) DacteDaDemonstracao()
    {
        using var certificado = CriarCertificado();
        var resultado = GeradorXmlCte.Gerar(new DadosEmissaoCte(
            DadosDemonstracao.Ctes()[0],
            DadosDemonstracao.Empresas()[0],
            DadosDemonstracao.Series()[0],
            DadosDemonstracao.Series()[0].ProximoNumero,
            DadosDemonstracao.Participantes().ToDictionary(p => p.Id),
            codigo => Municipios.GetValueOrDefault(codigo),
            TipoAmbiente.Homologacao,
            new DateTimeOffset(2026, 11, 25, 10, 30, 0, TimeSpan.FromHours(-4)),
            "Cte1 teste"), certificado);
        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        return (DadosDacte.Ler(resultado.Xml), resultado.Chave);
    }

    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=TRANSPORTADORA EXEMPLO ALFA LTDA:11111111000111", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return pedido.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(30));
    }
}
