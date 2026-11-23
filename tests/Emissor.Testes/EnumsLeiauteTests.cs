using Emissor.Dominio.Enums;

namespace Emissor.Testes;

/// <summary>
/// Garante que os enums do domínio carregam exatamente os códigos dos XSD (CT-e 4.00 NT 2026.005 e MDF-e 3.00b).
/// Se um teste falhar, o leiaute mudou ou o enum foi alterado por engano.
/// </summary>
public class EnumsLeiauteTests
{
    [Theory]
    [InlineData(Crt.SimplesNacional, 1)]
    [InlineData(Crt.SimplesNacionalExcessoSublimite, 2)]
    [InlineData(Crt.RegimeNormal, 3)]
    [InlineData(Crt.SimplesNacionalMei, 4)]
    public void Crt_TemCodigosDoLeiaute(Crt valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Theory]
    [InlineData(RegimeIbsCbs.SimplesNacionalComRegimeRegular, 1)]
    [InlineData(RegimeIbsCbs.NormalOpcaoPendenteSimplesNacional, 2)]
    [InlineData(RegimeIbsCbs.NormalOpcaoPendenteSimplesNacionalExcesso, 3)]
    [InlineData(RegimeIbsCbs.NormalOpcaoPendenteComRegimeRegular, 4)]
    public void RegimeIbsCbs_TemCodigosDoLeiaute(RegimeIbsCbs valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Theory]
    [InlineData(IndicadorIe.Contribuinte, 1)]
    [InlineData(IndicadorIe.Isento, 2)]
    [InlineData(IndicadorIe.NaoContribuinte, 9)]
    public void IndicadorIe_TemCodigosDoLeiaute(IndicadorIe valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Theory]
    [InlineData(TipoRodado.Truck, 1)]
    [InlineData(TipoRodado.Toco, 2)]
    [InlineData(TipoRodado.CavaloMecanico, 3)]
    [InlineData(TipoRodado.Van, 4)]
    [InlineData(TipoRodado.Utilitario, 5)]
    [InlineData(TipoRodado.Outros, 6)]
    public void TipoRodado_TemCodigosDoLeiaute(TipoRodado valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Theory]
    [InlineData(TipoCarroceria.NaoAplicavel, 0)]
    [InlineData(TipoCarroceria.Aberta, 1)]
    [InlineData(TipoCarroceria.FechadaBau, 2)]
    [InlineData(TipoCarroceria.Graneleira, 3)]
    [InlineData(TipoCarroceria.PortaConteiner, 4)]
    [InlineData(TipoCarroceria.Sider, 5)]
    public void TipoCarroceria_TemCodigosDoLeiaute(TipoCarroceria valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Theory]
    [InlineData(TipoProprietario.TacAgregado, 0)]
    [InlineData(TipoProprietario.TacIndependente, 1)]
    [InlineData(TipoProprietario.Outros, 2)]
    public void TipoProprietario_TemCodigosDoLeiaute(TipoProprietario valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Theory]
    [InlineData(ModeloDocumento.Cte, 57)]
    [InlineData(ModeloDocumento.Mdfe, 58)]
    public void ModeloDocumento_TemCodigosDoLeiaute(ModeloDocumento valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Theory]
    [InlineData(ResponsavelSeguro.Emitente, 1)]
    [InlineData(ResponsavelSeguro.Contratante, 2)]
    public void ResponsavelSeguro_TemCodigosDoLeiaute(ResponsavelSeguro valor, int codigo) => Assert.Equal(codigo, (int)valor);

    [Fact]
    public void Enums_NaoTemValoresAlemDoLeiaute()
    {
        Assert.Equal(4, Enum.GetValues<Crt>().Length);
        Assert.Equal(4, Enum.GetValues<RegimeIbsCbs>().Length);
        Assert.Equal(3, Enum.GetValues<IndicadorIe>().Length);
        Assert.Equal(6, Enum.GetValues<TipoRodado>().Length);
        Assert.Equal(6, Enum.GetValues<TipoCarroceria>().Length);
        Assert.Equal(3, Enum.GetValues<TipoProprietario>().Length);
        Assert.Equal(2, Enum.GetValues<ModeloDocumento>().Length);
        Assert.Equal(2, Enum.GetValues<ResponsavelSeguro>().Length);
    }
}
