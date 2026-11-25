using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Testes;

/// <summary>
/// Repositório e banco com SQLite em memória, aplicando a migração real (<c>Inicial</c>).
/// Cada teste cria um banco novo, que some quando a conexão é fechada.
/// </summary>
public sealed class RepositorioCadastroTests : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly EmissorDbContext _contexto;

    public RepositorioCadastroTests()
    {
        _conexao = new SqliteConnection("Data Source=:memory:");
        _conexao.Open();
        _contexto = new EmissorDbContext(new DbContextOptionsBuilder<EmissorDbContext>().UseSqlite(_conexao).Options);
        _contexto.Database.Migrate();
        _contexto.Municipios.Add(new Municipio { Codigo = 5103403, Nome = "Cuiabá", Uf = "MT" });
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    [Fact]
    public async Task IncluirEObter_PreencheIdEDatas()
    {
        var empresas = new RepositorioCadastro<Empresa>(_contexto);
        var empresa = RegrasDominioTests.EmpresaValida();

        await empresas.IncluirAsync(empresa);
        var lida = await empresas.ObterAsync(empresa.Id);

        Assert.NotNull(lida);
        Assert.True(lida.Id > 0);
        Assert.NotEqual(default, lida.CriadoEm);
        Assert.Equal("AVENIDA EXEMPLO", lida.Endereco.Logradouro);
        Assert.Equal(TipoAmbiente.Homologacao, lida.AmbientePadrao);
    }

    [Fact]
    public async Task Alterar_PreservaDataDeInclusao()
    {
        var empresas = new RepositorioCadastro<Empresa>(_contexto);
        var empresa = RegrasDominioTests.EmpresaValida();
        await empresas.IncluirAsync(empresa);
        var criadoEm = (await empresas.ObterAsync(empresa.Id))!.CriadoEm;

        var copia = (await empresas.ObterAsync(empresa.Id))!;
        copia.NomeFantasia = "ALFA";
        copia.CriadoEm = DateTime.MinValue;
        await empresas.AlterarAsync(copia);

        var lida = (await empresas.ObterAsync(empresa.Id))!;
        Assert.Equal("ALFA", lida.NomeFantasia);
        Assert.Equal(criadoEm, lida.CriadoEm);
    }

    [Fact]
    public async Task Incluir_ComPendencias_LancaRegraNegocio()
    {
        var motoristas = new RepositorioCadastro<Motorista>(_contexto);
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => motoristas.IncluirAsync(new Motorista { Nome = "SEM CPF" }));
        Assert.Contains(erro.Pendencias, p => p.Contains("CPF"));
    }

    [Fact]
    public async Task Serie_Repetida_NaEmpresaModeloEAmbiente_ERecusada()
    {
        var empresaId = await IncluirEmpresaAsync();
        var series = new RepositorioCadastro<SerieDocumento>(_contexto);
        await series.IncluirAsync(new SerieDocumento { EmpresaId = empresaId, Modelo = ModeloDocumento.Cte, Serie = 1 });

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            series.IncluirAsync(new SerieDocumento { EmpresaId = empresaId, Modelo = ModeloDocumento.Cte, Serie = 1 }));
        Assert.Contains("Já existe", erro.Message);

        // Mesma série em produção é outra numeração: aceita.
        await series.IncluirAsync(new SerieDocumento { EmpresaId = empresaId, Modelo = ModeloDocumento.Cte, Serie = 1, Ambiente = TipoAmbiente.Producao });
        Assert.Equal(2, (await series.ListarAsync(s => s.EmpresaId == empresaId)).Count);
    }

    [Fact]
    public async Task Excluir_ProprietarioVinculadoAVeiculo_ERecusado()
    {
        var empresaId = await IncluirEmpresaAsync();
        var proprietarios = new RepositorioCadastro<ProprietarioVeiculo>(_contexto);
        var proprietario = new ProprietarioVeiculo
        {
            EmpresaId = empresaId, CnpjCpf = "22222222222", Rntrc = "44444444", Nome = "AGREGADO", TipoProprietario = TipoProprietario.TacAgregado,
            Uf = "MT",
        };
        await proprietarios.IncluirAsync(proprietario);
        await new RepositorioCadastro<Veiculo>(_contexto).IncluirAsync(new Veiculo
        {
            EmpresaId = empresaId, Tipo = TipoVeiculo.Tracao, Placa = "AAA3C33", TaraKg = 6000, TipoRodado = TipoRodado.Truck,
            TipoCarroceria = TipoCarroceria.FechadaBau, Proprio = false, ProprietarioId = proprietario.Id,
        });

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => proprietarios.ExcluirAsync(proprietario.Id));
        Assert.Contains("em uso", erro.Message);
        Assert.True(await proprietarios.ExisteAsync(p => p.Id == proprietario.Id));
    }

    [Fact]
    public async Task Excluir_EmpresaComCadastros_ERecusado()
    {
        var empresaId = await IncluirEmpresaAsync();
        await new RepositorioCadastro<Motorista>(_contexto).IncluirAsync(new Motorista { EmpresaId = empresaId, Nome = "JOSE", Cpf = "33333333333" });

        var empresas = new RepositorioCadastro<Empresa>(_contexto);
        await Assert.ThrowsAsync<RegraNegocioException>(() => empresas.ExcluirAsync(empresaId));
        Assert.NotNull(await empresas.ObterAsync(empresaId));
    }

    [Fact]
    public async Task Listar_FiltraPorEmpresa()
    {
        var alfa = await IncluirEmpresaAsync();
        var beta = RegrasDominioTests.EmpresaValida();
        beta.Cnpj = "22222222000122";
        await new RepositorioCadastro<Empresa>(_contexto).IncluirAsync(beta);

        var motoristas = new RepositorioCadastro<Motorista>(_contexto);
        await motoristas.IncluirAsync(new Motorista { EmpresaId = alfa, Nome = "JOSE", Cpf = "33333333333" });
        await motoristas.IncluirAsync(new Motorista { EmpresaId = beta.Id, Nome = "JOAO", Cpf = "55555555555" });

        var daAlfa = await motoristas.ListarAsync(m => m.EmpresaId == alfa);
        Assert.Single(daAlfa);
        Assert.Equal("JOSE", daAlfa[0].Nome);
    }

    [Fact]
    public async Task Empresa_ComMunicipioInexistente_ERecusada()
    {
        var empresa = RegrasDominioTests.EmpresaValida();
        empresa.Endereco.MunicipioCodigo = 9999999;
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioCadastro<Empresa>(_contexto).IncluirAsync(empresa));
        Assert.Contains("não existe", erro.Message);
    }

    [Fact]
    public async Task PrepararAsync_CriaPastaEBancoComAsTabelas()
    {
        var pasta = Path.Combine(Path.GetTempPath(), "Cte1Testes", Guid.NewGuid().ToString("N"));
        var caminho = Path.Combine(pasta, "emissor.db");
        try
        {
            await ConfiguracaoBanco.PrepararAsync(caminho);

            Assert.True(File.Exists(caminho));
            await using var contexto = new EmissorDbContext(ConfiguracaoBanco.Opcoes(caminho));
            Assert.Empty(await contexto.Database.GetPendingMigrationsAsync());
            Assert.Equal(0, await contexto.Empresas.CountAsync());
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            Directory.Delete(pasta, recursive: true);
        }
    }

    private async Task<int> IncluirEmpresaAsync()
    {
        var empresa = RegrasDominioTests.EmpresaValida();
        await new RepositorioCadastro<Empresa>(_contexto).IncluirAsync(empresa);
        return empresa.Id;
    }
}
