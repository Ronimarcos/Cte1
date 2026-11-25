using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Testes;

/// <summary>Regras do CT-e e do MDF-e e gravação no banco (SQLite em memória, com a migração real).</summary>
public sealed class DocumentosTests : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly EmissorDbContext _contexto;

    public DocumentosTests()
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

    // ---- Regras do domínio ----

    [Fact]
    public void Rascunho_GravaMesmoIncompleto()
    {
        var cte = new ConhecimentoTransporte { EmpresaId = 1 };
        Assert.NotEmpty(cte.ValidarObrigatorios());
        Assert.Empty(cte.Validar());
    }

    [Fact]
    public void ProntoParaTransmitir_ExigeTudoPreenchido()
    {
        var cte = new ConhecimentoTransporte { EmpresaId = 1, Situacao = SituacaoDocumento.ProntoParaTransmitir };
        Assert.Contains("Remetente", cte.Validar());

        var mdfe = new ManifestoEletronico { EmpresaId = 1, Situacao = SituacaoDocumento.ProntoParaTransmitir };
        Assert.Contains("Veículo de tração", mdfe.Validar());
    }

    [Theory]
    [InlineData(TipoEmitenteMdfe.PrestadorServico, null, true)]
    [InlineData(TipoEmitenteMdfe.CteGlobalizado, null, true)]
    [InlineData(TipoEmitenteMdfe.CargaPropria, null, false)]
    [InlineData(TipoEmitenteMdfe.CargaPropria, TipoTransportador.Etc, true)]
    public void Ciot_ObrigatorioConformeNt2026001(TipoEmitenteMdfe emitente, TipoTransportador? transportador, bool exige)
    {
        var mdfe = new ManifestoEletronico { TipoEmitente = emitente, TipoTransportador = transportador };
        Assert.Equal(exige, mdfe.ExigeCiot);
        Assert.Equal(exige, mdfe.ValidarVeiculos().Any(p => p.StartsWith("CIOT (obrigatório", StringComparison.Ordinal)));
    }

    [Fact]
    public void ValorAReceber_NaoPassaDoTotal()
    {
        var cte = new ConhecimentoTransporte { Componentes = [new() { Nome = "FRETE", Valor = 100m }], ValorReceber = 150m };
        Assert.Contains("Valor a receber não pode passar do total da prestação", cte.ValidarValores());
    }

    [Fact]
    public void CopiarItensDe_GeraListasIndependentes()
    {
        var origem = DadosDemonstracao.Mdfes()[0];
        var destino = new ManifestoEletronico();
        destino.CopiarItensDe(origem);

        origem.Descargas[0].Documentos.Clear();
        origem.Percurso.Add("GO");
        Assert.Single(destino.Descargas[0].Documentos);
        Assert.Equal(["MS"], destino.Percurso);
    }

    [Fact]
    public void Demonstracao_DocumentosPassamNasRegras()
    {
        Assert.All(DadosDemonstracao.Ctes(), c => Assert.Empty(c.Validar()));
        Assert.All(DadosDemonstracao.Mdfes(), m => Assert.Empty(m.Validar()));
    }

    // ---- Banco ----

    [Fact]
    public async Task Demonstracao_GravaDocumentosComOsItens()
    {
        await CarregarDemonstracaoAsync();

        var ctes = await new RepositorioDocumento<ConhecimentoTransporte>(_contexto).ListarAsync();
        Assert.Equal([1, 2, 3], ctes.Select(c => c.Id));
        var cte = ctes[0];
        Assert.Equal(2, cte.Componentes.Count);
        Assert.Equal(8_820m, cte.ValorTotalPrestacao);
        Assert.Single(cte.Quantidades);
        Assert.Equal(44, cte.DocumentosNfe.Single().Chave.Length);

        var mdfe = await new RepositorioDocumento<ManifestoEletronico>(_contexto).ObterAsync(1);
        Assert.NotNull(mdfe);
        Assert.Equal(1, mdfe.Descargas.Single().Documentos.Single().CteId);
        Assert.Equal(["MS"], mdfe.Percurso);
        Assert.Equal([2], mdfe.ReboquesIds);
        Assert.Equal("123456789012", mdfe.Ciots.Single().Ciot);
        Assert.Equal(["AVB-000123"], mdfe.Averbacoes);
        Assert.False(await DadosDemonstracao.DocumentosDemonstracaoPendentesAsync(_contexto));
    }

    [Fact]
    public async Task Alterar_TrocaOsItensGravados()
    {
        await CarregarDemonstracaoAsync();
        var repositorio = new RepositorioDocumento<ConhecimentoTransporte>(_contexto);
        var cte = (await repositorio.ObterAsync(1))!;

        cte.Componentes = [new() { Nome = "FRETE UNICO", Valor = 9_000m }];
        cte.ValorReceber = 9_000m;
        cte.Observacoes = "ALTERADO";
        await repositorio.AlterarAsync(cte);

        var relido = (await repositorio.ObterAsync(1))!;
        Assert.Equal("ALTERADO", relido.Observacoes);
        Assert.Equal("FRETE UNICO", relido.Componentes.Single().Nome);
        Assert.Equal(9_000m, relido.ValorTotalPrestacao);
        Assert.Single(relido.Quantidades);
        Assert.Equal(1 + 1 + 1, await ContarLinhasAsync("CteComponentes")); // 1 do CT-e 1, 1 do CT-e 2 e 1 do CT-e 3
    }

    [Fact]
    public async Task Alterar_MdfeTrocaDescargasEDocumentos()
    {
        await CarregarDemonstracaoAsync();
        var repositorio = new RepositorioDocumento<ManifestoEletronico>(_contexto);
        var mdfe = (await repositorio.ObterAsync(2))!;

        mdfe.Descargas =
        [
            new() { MunicipioCodigo = 5108402, Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, CteId = 3 }] },
            new() { MunicipioCodigo = 5103403, Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, CteId = 1 }, new() { Tipo = TipoDocumentoMdfe.Cte, CteId = 2 }] },
        ];
        mdfe.CondutoresIds = [1, 2];
        await repositorio.AlterarAsync(mdfe);

        var relido = (await repositorio.ObterAsync(2))!;
        Assert.Equal(2, relido.Descargas.Count);
        Assert.Equal(3, relido.QuantidadeCte);
        Assert.Equal([1, 2], relido.CondutoresIds);
        Assert.Equal(1 + 2, await ContarLinhasAsync("MdfeDescargas")); // 1 do MDF-e 1 e 2 do MDF-e 2
        Assert.Equal(1 + 3, await ContarLinhasAsync("MdfeDocumentos"));
    }

    [Fact]
    public async Task Gravar_ProntoComPendenciasEhRecusado()
    {
        await CarregarDemonstracaoAsync();
        var repositorio = new RepositorioDocumento<ConhecimentoTransporte>(_contexto);
        var cte = (await repositorio.ObterAsync(2))!;
        cte.Situacao = SituacaoDocumento.ProntoParaTransmitir;
        cte.ProdutoPredominante = string.Empty;

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => repositorio.AlterarAsync(cte));
        Assert.Contains("Produto predominante", erro.Pendencias);
        Assert.Equal(SituacaoDocumento.EmDigitacao, (await repositorio.ObterAsync(2))!.Situacao);
    }

    [Fact]
    public async Task Excluir_SoRascunho()
    {
        await CarregarDemonstracaoAsync();
        var repositorio = new RepositorioDocumento<ConhecimentoTransporte>(_contexto);

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => repositorio.ExcluirAsync(1));
        Assert.Contains("em digitação", erro.Message);

        await repositorio.ExcluirAsync(2);
        Assert.Null(await repositorio.ObterAsync(2));
        Assert.Equal(1, await ContarLinhasAsync("CteNfes")); // só a NF-e do CT-e 1 (o CT-e 3 é complemento, sem NF-e)
    }

    [Fact]
    public async Task Excluir_CteVinculadoEmMdfeEhRecusado()
    {
        await CarregarDemonstracaoAsync();
        // O CT-e 2 (rascunho) passa a constar no MDF-e 2.
        var mdfes = new RepositorioDocumento<ManifestoEletronico>(_contexto);
        var mdfe = (await mdfes.ObterAsync(2))!;
        mdfe.Descargas[0].Documentos.Add(new() { Tipo = TipoDocumentoMdfe.Cte, CteId = 2 });
        await mdfes.AlterarAsync(mdfe);

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioDocumento<ConhecimentoTransporte>(_contexto).ExcluirAsync(2));
        Assert.Contains("em uso", erro.Message);
    }

    [Fact]
    public async Task Excluir_CadastroUsadoEmDocumentoEhRecusado()
    {
        await CarregarDemonstracaoAsync();
        await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioCadastro<Veiculo>(_contexto).ExcluirAsync(1));
        await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioCadastro<Participante>(_contexto).ExcluirAsync(1));
    }

    [Fact]
    public async Task Alterar_DocumentoDeOutraEmpresaEhRecusado()
    {
        await CarregarDemonstracaoAsync();
        var repositorio = new RepositorioDocumento<ConhecimentoTransporte>(_contexto);
        var cte = (await repositorio.ObterAsync(2))!;
        cte.EmpresaId = 2;

        await Assert.ThrowsAsync<RegraNegocioException>(() => repositorio.AlterarAsync(cte));
    }

    [Fact]
    public async Task Demonstracao_CompletaDocumentosDeQuemTinhaSoOsCadastros()
    {
        await CargaMunicipios.CarregarSeVazioAsync(_contexto);
        _contexto.Empresas.AddRange(DadosDemonstracao.Empresas());
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();
        await DadosDemonstracao.CarregarAsync(_contexto);
        _contexto.Mdfes.RemoveRange(await _contexto.Mdfes.ToListAsync());
        await _contexto.SaveChangesAsync();
        _contexto.Ctes.RemoveRange(await _contexto.Ctes.ToListAsync());
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();

        Assert.False(await DadosDemonstracao.CadastrosDemonstracaoPendentesAsync(_contexto));
        Assert.True(await DadosDemonstracao.DemonstracaoIncompletaAsync(_contexto));
        await DadosDemonstracao.CarregarAsync(_contexto);

        Assert.Equal(3, await _contexto.Ctes.CountAsync());
        Assert.Equal(2, await _contexto.Mdfes.CountAsync());
        Assert.False(await DadosDemonstracao.DemonstracaoIncompletaAsync(_contexto));
    }

    private async Task CarregarDemonstracaoAsync()
    {
        await CargaMunicipios.CarregarSeVazioAsync(_contexto);
        await DadosDemonstracao.CarregarAsync(_contexto);
    }

    private async Task<long> ContarLinhasAsync(string tabela)
    {
        await using var comando = _conexao.CreateCommand();
        comando.CommandText = $"SELECT COUNT(*) FROM \"{tabela}\"";
        return (long)(await comando.ExecuteScalarAsync())!;
    }
}
