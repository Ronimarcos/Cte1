using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Testes;

/// <summary>Regras de formato e de validação das entidades do domínio.</summary>
public class RegrasDominioTests
{
    [Theory]
    [InlineData("11111111000111", true)]
    [InlineData("12ABC34501DE35", true)]
    [InlineData("12abc34501de35", false)]
    [InlineData("12ABC34501DEXX", false)]
    [InlineData("1111111100011", false)]
    public void CnpjValido_AceitaCnpjAlfanumericoDoLeiaute(string cnpj, bool esperado) =>
        Assert.Equal(esperado, Formatos.CnpjValido(cnpj));

    [Fact]
    public void SoAlfanumericos_RemoveMascaraEPassaParaMaiusculas() =>
        Assert.Equal("12ABC34501DE35", Formatos.SoAlfanumericos("12.abc.345/01de-35"));

    [Theory]
    [InlineData("ABC1234", true)]
    [InlineData("ABC1D23", true)]
    [InlineData("AB-1234", false)]
    [InlineData("ABCD12", false)]
    public void PlacaValida_AceitaPadraoAntigoEMercosul(string placa, bool esperado) =>
        Assert.Equal(esperado, Formatos.PlacaValida(placa));

    [Fact]
    public void Empresa_ExigeCnpjOuCpfMasNaoOsDois()
    {
        var empresa = EmpresaValida();
        Assert.Empty(empresa.Validar());

        empresa.Cpf = "11111111111";
        Assert.Contains(empresa.Validar(), p => p.Contains("CNPJ ou o CPF"));

        empresa.Cnpj = string.Empty;
        Assert.Empty(empresa.Validar());
    }

    [Fact]
    public void Veiculo_ReboqueExigeCapacidadeETracaoExigeRodado()
    {
        var reboque = new Veiculo { Tipo = TipoVeiculo.Reboque, Placa = "ABC1D23", TaraKg = 7000, TipoCarroceria = TipoCarroceria.Sider };
        Assert.Contains(reboque.Validar(), p => p.Contains("Capacidade em kg"));

        var tracao = new Veiculo { Tipo = TipoVeiculo.Tracao, Placa = "ABC1234", TaraKg = 8000, TipoCarroceria = TipoCarroceria.NaoAplicavel };
        Assert.Contains(tracao.Validar(), p => p.Contains("rodado"));
    }

    [Fact]
    public void Veiculo_DeTerceiroExigeProprietario()
    {
        var veiculo = new Veiculo
        {
            Tipo = TipoVeiculo.Tracao, Placa = "ABC1234", TaraKg = 8000, TipoRodado = TipoRodado.Truck,
            TipoCarroceria = TipoCarroceria.Aberta, Proprio = false,
        };
        Assert.Contains(veiculo.Validar(), p => p.Contains("Proprietário"));
    }

    [Fact]
    public void Apolice_DocumentoDoResponsavelSoParaContratante()
    {
        var apolice = new ApoliceSeguro
        {
            Responsavel = ResponsavelSeguro.Contratante, NomeSeguradora = "SEGURADORA", CnpjSeguradora = "88888888000188",
            NumeroApolice = "123",
        };
        Assert.Contains(apolice.Validar(), p => p.Contains("contratante"));

        apolice.Responsavel = ResponsavelSeguro.Emitente;
        Assert.Empty(apolice.Validar());
    }

    [Theory]
    [InlineData(-1, SituacaoCertificado.Vencido)]
    [InlineData(10, SituacaoCertificado.VencendoEmBreve)]
    [InlineData(31, SituacaoCertificado.Valido)]
    public void Certificado_SituacaoConformeDiasParaVencer(int diasParaVencer, SituacaoCertificado esperada)
    {
        var hoje = new DateTime(2026, 11, 23);
        var certificado = new CertificadoDigital { ValidoDe = hoje.AddYears(-1), ValidoAte = hoje.AddDays(diasParaVencer) };
        Assert.Equal(esperada, certificado.SituacaoEm(hoje));
    }

    [Fact]
    public void Serie_ForaDosLimitesDoLeiaute()
    {
        var serie = new SerieDocumento { Modelo = ModeloDocumento.Cte, Serie = 1000, ProximoNumero = 0 };
        var pendencias = serie.Validar();
        Assert.Contains(pendencias, p => p.StartsWith("Série"));
        Assert.Contains(pendencias, p => p.StartsWith("Próximo número"));
    }

    [Theory]
    [InlineData(TipoAmbiente.Producao, 1)]
    [InlineData(TipoAmbiente.Homologacao, 2)]
    public void TipoAmbiente_TemCodigosDoLeiaute(TipoAmbiente valor, int codigo) => Assert.Equal(codigo, (int)valor);

    /// <summary>Empresa fictícia válida usada nos testes.</summary>
    internal static Empresa EmpresaValida() => new()
    {
        RazaoSocial = "TRANSPORTADORA EXEMPLO ALFA LTDA", Cnpj = "11111111000111", Ie = "111111111", Rntrc = "11111111",
        Endereco = new Endereco { Logradouro = "AVENIDA EXEMPLO", Numero = "100", Bairro = "CENTRO", MunicipioCodigo = 5103403, Cep = "78000000" },
    };
}
