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
    public void Demonstracao_DemaisCadastrosPassamNasRegras()
    {
        Assert.All(DadosDemonstracao.Participantes(), p => Assert.Empty(p.Validar()));
        Assert.All(DadosDemonstracao.Proprietarios(), p => Assert.Empty(p.Validar()));
        Assert.All(DadosDemonstracao.Veiculos(), v => Assert.Empty(v.Validar()));
        Assert.All(DadosDemonstracao.Motoristas(), m => Assert.Empty(m.Validar()));
        Assert.All(DadosDemonstracao.Apolices(), a => Assert.Empty(a.Validar()));
        Assert.All(DadosDemonstracao.Series(), s => Assert.Empty(s.Validar()));
    }

    [Fact]
    public async Task Demonstracao_CarregaTodosOsCadastrosComOsCodigosEsperados()
    {
        await CargaMunicipios.CarregarSeVazioAsync(_contexto);
        await DadosDemonstracao.CarregarAsync(_contexto);

        Assert.Equal([1, 2, 3, 4], await _contexto.Participantes.OrderBy(p => p.Id).Select(p => p.Id).ToListAsync());
        Assert.Equal(3, await _contexto.Veiculos.CountAsync());
        Assert.Equal(1, (await _contexto.Veiculos.SingleAsync(v => v.Id == 3)).ProprietarioId);
        Assert.Equal(2, await _contexto.Proprietarios.CountAsync());
        Assert.Equal(3, await _contexto.Motoristas.CountAsync());
        Assert.Equal(2, await _contexto.Apolices.CountAsync());
        Assert.Equal(3, await _contexto.Series.CountAsync());
        Assert.False(await DadosDemonstracao.CadastrosDemonstracaoPendentesAsync(_contexto));
    }

    [Fact]
    public async Task Demonstracao_CompletaQuemSoTinhaAsEmpresas()
    {
        await CargaMunicipios.CarregarSeVazioAsync(_contexto);
        _contexto.Empresas.AddRange(DadosDemonstracao.Empresas());
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();

        Assert.True(await DadosDemonstracao.CadastrosDemonstracaoPendentesAsync(_contexto));
        await DadosDemonstracao.CarregarAsync(_contexto);
        Assert.Equal(4, await _contexto.Participantes.CountAsync());
    }

    [Fact]
    public async Task Demonstracao_NaoCompletaBancoComEmpresaPropria()
    {
        await CargaMunicipios.CarregarSeVazioAsync(_contexto);
        var propria = RegrasDominioTests.EmpresaValida();
        propria.Cnpj = "12ABC34501DE35";
        _contexto.Empresas.Add(propria);
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();

        Assert.False(await DadosDemonstracao.CadastrosDemonstracaoPendentesAsync(_contexto));
        await DadosDemonstracao.CarregarAsync(_contexto);
        Assert.Equal(1, await _contexto.Empresas.CountAsync());
        Assert.Equal(0, await _contexto.Participantes.CountAsync());
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
