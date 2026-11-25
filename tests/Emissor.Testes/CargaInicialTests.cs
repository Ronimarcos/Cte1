using Emissor.Dados;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Testes;

/// <summary>Carga dos municípios do IBGE e dos dados de demonstração.</summary>
public sealed class CargaInicialTests : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly EmissorDbContext _contexto;

    public CargaInicialTests()
    {
        _conexao = new SqliteConnection("Data Source=:memory:");
        _conexao.Open();
        _contexto = new EmissorDbContext(new DbContextOptionsBuilder<EmissorDbContext>().UseSqlite(_conexao).Options);
        _contexto.Database.Migrate();
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    [Fact]
    public void ListaOficial_TemTodosOsMunicipiosComCodigoNomeEUf()
    {
        var municipios = CargaMunicipios.LerListaOficial();

        Assert.Equal(5571, municipios.Count);
        Assert.Equal(municipios.Count, municipios.Select(m => m.Codigo).Distinct().Count());
        Assert.Equal(27, municipios.Select(m => m.Uf).Distinct().Count());
        Assert.All(municipios, m => Assert.InRange(m.Codigo, 1_000_000, 9_999_999));
        Assert.All(municipios, m => Assert.True(Formatos.UfValida(m.Uf)));
    }

    [Theory]
    [InlineData(5103403, "Cuiabá", "MT")]
    [InlineData(5107602, "Rondonópolis", "MT")]
    [InlineData(5107909, "Sinop", "MT")]
    [InlineData(5101837, "Boa Esperança do Norte", "MT")]
    [InlineData(3550308, "São Paulo", "SP")]
    public void ListaOficial_TemCodigosConhecidos(int codigo, string nome, string uf)
    {
        var municipio = CargaMunicipios.LerListaOficial().Single(m => m.Codigo == codigo);
        Assert.Equal(nome, municipio.Nome);
        Assert.Equal(uf, municipio.Uf);
    }

    [Fact]
    public async Task CarregarSeVazio_CarregaUmaVezSo()
    {
        Assert.Equal(5571, await CargaMunicipios.CarregarSeVazioAsync(_contexto));
        Assert.Equal(0, await CargaMunicipios.CarregarSeVazioAsync(_contexto));
        Assert.Equal(5571, await _contexto.Municipios.CountAsync());
    }

    [Fact]
    public void Demonstracao_EmpresasSaoValidasComCodigos1a3()
    {
        var empresas = DadosDemonstracao.Empresas();
        Assert.Equal([1, 2, 3], empresas.Select(e => e.Id));
        Assert.All(empresas, e => Assert.Empty(e.Validar()));
        Assert.Equal(TipoAmbiente.Producao, empresas.Single(e => e.Id == 3).AmbientePadrao);
    }

    [Fact]
    public async Task Demonstracao_GravaSoComBancoSemEmpresas()
    {
        await CargaMunicipios.CarregarSeVazioAsync(_contexto);
        Assert.True(await DadosDemonstracao.BancoSemEmpresasAsync(_contexto));

        await DadosDemonstracao.CarregarAsync(_contexto);
        await DadosDemonstracao.CarregarAsync(_contexto);

        Assert.Equal([1, 2, 3], await _contexto.Empresas.OrderBy(e => e.Id).Select(e => e.Id).ToListAsync());
        Assert.False(await DadosDemonstracao.BancoSemEmpresasAsync(_contexto));
    }
}
