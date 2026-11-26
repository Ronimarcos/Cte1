using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Emissor.Dados;
using Emissor.DFe;
using Emissor.DFe.Impressao;
using Emissor.DFe.Mdfe;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using QuestPDF.Fluent;

namespace Emissor.Testes;

/// <summary>
/// DAMDFE (P5b): leitura dos quadros do modelo rodoviário a partir do XML do MDF-e, campos adicionais (documentos
/// vinculados e CIOT) e geração do PDF. Com a variável de ambiente <c>CTE1_DAMDFE_SAIDA</c>, o PDF e a imagem da
/// primeira folha são gravados para conferência visual.
/// </summary>
public class DamdfeTests
{
    private static readonly IReadOnlyDictionary<int, Municipio> Municipios =
        CargaMunicipios.LerListaOficial().ToDictionary(m => m.Codigo);

    private static readonly string ChaveCteVinculado = ChaveAcesso.Montar(51, new DateTime(2026, 11, 1), "11111111000111", 57, 1, 14, 1, 1);

    [Fact]
    public void Leitura_PreencheOsQuadrosDoModeloRodoviario()
    {
        var (dados, chave) = DamdfeDaDemonstracao();

        Assert.Equal(TipoAmbiente.Homologacao, dados.Ambiente);
        Assert.False(dados.Contingencia);
        Assert.Equal(chave, dados.Chave);
        Assert.Equal("58", dados.Modelo);
        Assert.NotEmpty(dados.Numero);
        Assert.NotEmpty(dados.EmitenteNome);
        Assert.Equal("11111111", dados.EmitenteRntrc);
        Assert.Equal("MT", dados.UfCarregamento);
        Assert.Equal("SP", dados.UfDescarregamento);
        Assert.Equal("1", dados.QuantidadeCte);
        Assert.Equal("0", dados.QuantidadeNfe);
        Assert.NotEmpty(dados.PesoTotalKg);
        Assert.Equal(2, dados.Veiculos.Count);
        Assert.Equal("AAA1A11", dados.Veiculos[0].Placa);
        Assert.NotEmpty(dados.Condutores);
        Assert.Equal(14, dados.Condutores[0].Cpf.Length);
        Assert.Empty(dados.Protocolo);
        Assert.Equal($"https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe={chave}&tpAmb=2", dados.QrCode);
    }

    [Fact]
    public void Leitura_TrazDocumentosVinculadosECiot()
    {
        var (dados, _) = DamdfeDaDemonstracao();

        var documento = Assert.Single(dados.Documentos);
        Assert.Equal("CT-e", documento.Tipo);
        Assert.Equal(ChaveCteVinculado, documento.Chave);
        Assert.NotEmpty(documento.MunicipioDescarga);
        Assert.Equal("123456789012", Assert.Single(dados.Ciots).Ciot);
    }

    [Fact]
    public void Leitura_ConvertePesoEmToneladaEContingencia()
    {
        const string xml = """
            <MDFe xmlns="http://www.portalfiscal.inf.br/mdfe"><infMDFe Id="MDFe51261111111111000111580010000000071876543210" versao="3.00">
            <ide><tpAmb>2</tpAmb><tpEmis>2</tpEmis></ide>
            <tot><qCTe>3</qCTe><cUnid>02</cUnid><qCarga>2.8900</qCarga></tot>
            </infMDFe></MDFe>
            """;
        var dados = DadosDamdfe.Ler(xml);

        Assert.Equal("2.890,00", dados.PesoTotalKg);
        Assert.True(dados.Contingencia);
        Assert.Equal("3", dados.QuantidadeCte);
    }

    [Fact]
    public void Leitura_ComProtocoloAutorizado()
    {
        const string xml = """
            <mdfeProc xmlns="http://www.portalfiscal.inf.br/mdfe" versao="3.00"><MDFe><infMDFe Id="MDFe51261111111111000111580010000000071876543210" versao="3.00">
            <ide><tpAmb>2</tpAmb></ide></infMDFe></MDFe>
            <protMDFe versao="3.00"><infProt><cStat>100</cStat><nProt>951260000012345</nProt><dhRecbto>2026-11-26T10:00:00-04:00</dhRecbto></infProt></protMDFe></mdfeProc>
            """;
        Assert.Equal("951260000012345 - 26/11/2026 10:00:00", DadosDamdfe.Ler(xml).Protocolo);
    }

    [Fact]
    public void Leitura_RecusaXmlQueNaoEDeMdfe()
    {
        Assert.Throws<RegraNegocioException>(() => DadosDamdfe.Ler("<CTe/>"));
        Assert.Throws<RegraNegocioException>(() => DadosDamdfe.Ler("não é xml"));
    }

    [Fact]
    public void Pdf_GeradoValido()
    {
        var (dados, _) = DamdfeDaDemonstracao();

        var pdf = GeradorDamdfe.Gerar(dados);

        Assert.True(pdf.Length > 10_000);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));
        if (Environment.GetEnvironmentVariable("CTE1_DAMDFE_SAIDA") is { Length: > 0 } saida)
        {
            File.WriteAllBytes(saida, pdf);
            var imagens = GeradorDamdfe.Documento(dados).GenerateImages(new QuestPDF.Infrastructure.ImageGenerationSettings { RasterDpi = 110 }).ToList();
            File.WriteAllBytes(Path.ChangeExtension(saida, ".png"), imagens[0]);
        }
    }

    [Fact]
    public void Pdf_MuitosDocumentosSeguemNaFolhaSeguinte()
    {
        var (dados, _) = DamdfeDaDemonstracao();
        var muitos = new DadosDamdfe
        {
            Ambiente = dados.Ambiente,
            Chave = dados.Chave,
            EmitenteNome = dados.EmitenteNome,
            Documentos = Enumerable.Range(1, 300).Select(_ => new DocumentoDamdfe("CT-e", ChaveCteVinculado, "Campo Grande")).ToList(),
        };

        var pdf = Encoding.ASCII.GetString(GeradorDamdfe.Gerar(muitos));

        Assert.True(System.Text.RegularExpressions.Regex.Matches(pdf, @"/Type\s*/Page[^s]").Count >= 2);
    }

    private static (DadosDamdfe Dados, string Chave) DamdfeDaDemonstracao()
    {
        var mdfe = DadosDemonstracao.Mdfes()[0];
        mdfe.Descargas[0].Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, Chave = ChaveCteVinculado }];
        using var certificado = CriarCertificado();
        var resultado = GeradorXmlMdfe.Gerar(new DadosEmissaoMdfe(
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
            "Cte1 teste"), certificado);
        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        return (DadosDamdfe.Ler(resultado.Xml), resultado.Chave);
    }

    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=TRANSPORTADORA EXEMPLO ALFA LTDA:11111111000111", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return pedido.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(30));
    }
}
