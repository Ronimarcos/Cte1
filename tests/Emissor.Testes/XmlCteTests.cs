using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Emissor.Dados;
using Emissor.DFe;
using Emissor.DFe.Assinatura;
using Emissor.DFe.Cte;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;

namespace Emissor.Testes;

/// <summary>
/// Motor DF-e do CT-e sem transmissão: chave de acesso, montagem, assinatura e validação contra os XSD oficiais.
/// O certificado é autoassinado e criado em memória.
/// </summary>
public class XmlCteTests
{
    private static readonly IReadOnlyDictionary<int, Municipio> Municipios =
        CargaMunicipios.LerListaOficial().ToDictionary(m => m.Codigo);

    // ---- Chave de acesso ----

    [Fact]
    public void Digito_ModuloOnzeComPesosDeDoisANove()
    {
        // 43 posições "1": soma = 1 x (pesos 2..9 repetidos) = 5 x 44 + (2+3+4) = 229; 229 % 11 = 9; dígito = 11 - 9 = 2.
        Assert.Equal(2, ChaveAcesso.CalcularDigito(new string('1', 43)));
        // Soma zero: resto 0, dígito 0.
        Assert.Equal(0, ChaveAcesso.CalcularDigito(new string('0', 43)));
    }

    [Fact]
    public void Digito_LetraDoCnpjAlfanumericoValeAsciiMenos48()
    {
        // "A" (65 - 48 = 17) na última posição, com peso 2: soma 34; 34 % 11 = 1; resto menor que 2, dígito 0.
        Assert.Equal(0, ChaveAcesso.CalcularDigito(new string('0', 42) + "A"));
        // "B" (18) com peso 2: soma 36; 36 % 11 = 3; dígito 8.
        Assert.Equal(8, ChaveAcesso.CalcularDigito(new string('0', 42) + "B"));
    }

    [Fact]
    public void Montar_GeraChaveDe44PosicoesComOsCampos()
    {
        var chave = ChaveAcesso.Montar(51, new DateTime(2026, 11, 25), "11111111000111", 57, 1, 15, 1, 12345678);

        Assert.Equal(44, chave.Length);
        Assert.StartsWith("512611" + "11111111000111" + "57" + "001" + "000000015" + "1" + "12345678", chave);
        Assert.True(ChaveAcesso.Valida(chave));
        Assert.False(ChaveAcesso.Valida(chave[..43] + ((chave[43] - '0' + 1) % 10)));
    }

    [Fact]
    public void Montar_AceitaCnpjAlfanumerico()
    {
        var chave = ChaveAcesso.Montar(51, new DateTime(2026, 11, 25), "12ABC34501DE35", 57, 1, 1, 1, 1);
        Assert.True(ChaveAcesso.Valida(chave));
        Assert.Equal("12ABC34501DE35", chave[6..20]);
    }

    // ---- Montagem, assinatura e XSD ----

    [Fact]
    public void Demonstracao_CteNormalGeraXmlValidoEAssinado()
    {
        using var certificado = CriarCertificado();
        var resultado = GeradorXmlCte.Gerar(Dados(DadosDemonstracao.Ctes()[0]), certificado);

        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        Assert.True(ChaveAcesso.Valida(resultado.Chave));

        var xml = Carregar(resultado.Xml);
        Assert.True(AssinadorXml.Conferir(xml));
        Assert.Equal("CTe" + resultado.Chave, ((XmlElement)xml.GetElementsByTagName("infCte")[0]!).GetAttribute("Id"));
        Assert.Equal("2", Texto(xml, "tpAmb"));
        Assert.Equal("15", Texto(xml, "nCT"));
        Assert.Equal("8820.00", Texto(xml, "vTPrest"));
        Assert.Equal("90", Texto(xml, "CST"));
        Assert.Equal("11111111", Texto(xml, "RNTRC"));
        Assert.Empty(xml.GetElementsByTagName("IBSCBS"));
        Assert.Empty(xml.GetElementsByTagName("infCTeSupl"));
    }

    [Fact]
    public void Demonstracao_CteDeComplementoESubcontratacaoGeramXmlValido()
    {
        using var certificado = CriarCertificado();
        var complemento = GeradorXmlCte.Gerar(Dados(DadosDemonstracao.Ctes()[2]), certificado);
        Assert.True(complemento.Valido, string.Join(Environment.NewLine, complemento.Pendencias));
        Assert.Single(Carregar(complemento.Xml).GetElementsByTagName("infCteComp"));

        var subcontratacao = DadosDemonstracao.Ctes()[1];
        subcontratacao.DocumentosAnteriores =
        [
            new() { CnpjCpfEmitente = "77777777000177", NomeEmitente = "TRANSPORTADORA ANTERIOR", ChaveCte = ChaveAcesso.Montar(51, DateTime.Today, "77777777000177", 57, 1, 9, 1, 1) },
        ];
        var resultado = GeradorXmlCte.Gerar(Dados(subcontratacao), certificado);
        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        Assert.Single(Carregar(resultado.Xml).GetElementsByTagName("emiDocAnt"));
    }

    [Fact]
    public void TomadorOutros_UsaToma4ComEndereco()
    {
        using var certificado = CriarCertificado();
        var cte = DadosDemonstracao.Ctes()[0];
        cte.Tomador = TomadorServico.Outros;
        cte.TomadorOutrosId = 3;

        var resultado = GeradorXmlCte.Gerar(Dados(cte), certificado);

        Assert.True(resultado.Valido, string.Join(Environment.NewLine, resultado.Pendencias));
        var xml = Carregar(resultado.Xml);
        Assert.Single(xml.GetElementsByTagName("toma4"));
        Assert.Single(xml.GetElementsByTagName("enderToma"));
    }

    [Fact]
    public void XmlAlterado_FalhaNaConferenciaDaAssinatura()
    {
        using var certificado = CriarCertificado();
        var resultado = GeradorXmlCte.Gerar(Dados(DadosDemonstracao.Ctes()[0]), certificado);
        var xml = Carregar(resultado.Xml.Replace("8820.00", "1.00", StringComparison.Ordinal));
        Assert.False(AssinadorXml.Conferir(xml));
    }

    [Fact]
    public void FaltandoDados_DevolvePendenciasSemXml()
    {
        using var certificado = CriarCertificado();
        var cte = DadosDemonstracao.Ctes()[0];
        cte.RemetenteId = 99;
        cte.Cfop = string.Empty;

        var resultado = GeradorXmlCte.Gerar(Dados(cte), certificado);

        Assert.False(resultado.Valido);
        Assert.Empty(resultado.Xml);
        Assert.Contains("CFOP (4 dígitos)", resultado.Pendencias);
        Assert.Contains("Remetente: participante 99 não encontrado no cadastro", resultado.Pendencias);
    }

    [Fact]
    public void Validacao_ApontaOCampoForaDoXsd()
    {
        using var certificado = CriarCertificado();
        var cte = DadosDemonstracao.Ctes()[0];
        cte.Quantidades[0].TipoMedida = new string('X', 30); // tpMed aceita até 20 caracteres

        var resultado = GeradorXmlCte.Gerar(Dados(cte), certificado);

        Assert.False(resultado.Valido);
        Assert.Contains(resultado.Pendencias, p => p.Contains("tpMed", StringComparison.Ordinal));
    }

    private static DadosEmissaoCte Dados(ConhecimentoTransporte cte) => new(
        cte,
        DadosDemonstracao.Empresas()[0],
        DadosDemonstracao.Series()[0],
        DadosDemonstracao.Series()[0].ProximoNumero,
        DadosDemonstracao.Participantes().ToDictionary(p => p.Id),
        codigo => Municipios.GetValueOrDefault(codigo),
        TipoAmbiente.Homologacao,
        new DateTimeOffset(2026, 11, 25, 10, 30, 0, TimeSpan.FromHours(-4)),
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
