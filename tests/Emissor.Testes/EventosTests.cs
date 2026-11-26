using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Testes;

/// <summary>
/// Eventos (P4d1): regras do MOC conferidas no emissor, gravação com alçada, sequência por tipo e documento e
/// histórico sem exclusão.
/// </summary>
public sealed class EventosTests : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly EmissorDbContext _contexto;

    public EventosTests()
    {
        _conexao = new SqliteConnection("Data Source=:memory:");
        _conexao.Open();
        _contexto = new EmissorDbContext(new DbContextOptionsBuilder<EmissorDbContext>().UseSqlite(_conexao).Options);
        _contexto.Database.Migrate();
        CargaMunicipios.CarregarSeVazioAsync(_contexto).GetAwaiter().GetResult();
        DadosDemonstracao.CarregarAsync(_contexto).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    [Theory]
    [InlineData(TipoEvento.CancelamentoCte, "110111", ModeloDocumento.Cte)]
    [InlineData(TipoEvento.CartaCorrecaoCte, "110110", ModeloDocumento.Cte)]
    [InlineData(TipoEvento.CancelamentoMdfe, "110111", ModeloDocumento.Mdfe)]
    [InlineData(TipoEvento.EncerramentoMdfe, "110112", ModeloDocumento.Mdfe)]
    [InlineData(TipoEvento.InclusaoCondutorMdfe, "110114", ModeloDocumento.Mdfe)]
    public void Tipos_TemOCodigoDoLeiauteEOModelo(TipoEvento tipo, string codigo, ModeloDocumento modelo)
    {
        Assert.Equal(codigo, tipo.Codigo());
        Assert.Equal(modelo, tipo.Modelo());
    }

    [Theory]
    [InlineData("curta demais", false)]
    [InlineData("   quinze  caracteres   ", true)]
    [InlineData("Frete digitado com valor errado", true)]
    public void Justificativa_De15A255Caracteres(string justificativa, bool aceita)
    {
        var evento = new EventoDocumento { Tipo = TipoEvento.CancelamentoCte, DocumentoId = 1, Justificativa = justificativa };
        Assert.Equal(aceita, RegrasEvento.ValidarDados(evento).Count == 0);
    }

    [Fact]
    public void Justificativa_AcimaDe255_Recusada()
    {
        var evento = new EventoDocumento { Tipo = TipoEvento.CancelamentoMdfe, DocumentoId = 1, Justificativa = new string('x', 256) };
        Assert.Contains(RegrasEvento.ValidarDados(evento), p => p.Contains("tem 256", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("ide", "dhEmi", false)]
    [InlineData("IDE", "DHEMI", false)]
    [InlineData("vPrest", "vTPrest", false)]
    [InlineData("ICMSSN", "CST", false)]
    [InlineData("rem", "CNPJ", false)]
    [InlineData("compl", "xObs", true)]
    [InlineData("rem", "xNome", true)]
    [InlineData("infQ", "qCarga", true)]
    public void CartaCorrecao_RecusaOsCamposDoItem13DoMoc(string grupo, string campo, bool permitido)
    {
        Assert.Equal(permitido, RegrasEvento.CampoPermitidoNaCce(grupo, campo));

        var evento = new EventoDocumento
        {
            Tipo = TipoEvento.CartaCorrecaoCte,
            DocumentoId = 1,
            Correcoes = [new() { Grupo = grupo, Campo = campo, Valor = "NOVO VALOR" }],
        };
        Assert.Equal(permitido, RegrasEvento.ValidarDados(evento).Count == 0);
    }

    [Fact]
    public void CartaCorrecao_ExigeCorrecaoItemValidoESequenciaAte20()
    {
        var semCorrecao = new EventoDocumento { Tipo = TipoEvento.CartaCorrecaoCte, DocumentoId = 1 };
        Assert.Contains(RegrasEvento.ValidarDados(semCorrecao), p => p.Contains("ao menos uma", StringComparison.Ordinal));

        var itemInvalido = new EventoDocumento
        {
            Tipo = TipoEvento.CartaCorrecaoCte,
            DocumentoId = 1,
            Sequencia = 21,
            Correcoes = [new() { Grupo = "infQ", Campo = "qCarga", Valor = "10", NumeroItem = 100 }],
        };
        var pendencias = RegrasEvento.ValidarDados(itemInvalido);
        Assert.Contains(pendencias, p => p.Contains("rejeição 522", StringComparison.Ordinal));
        Assert.Contains(pendencias, p => p.Contains("entre 1 e 20", StringComparison.Ordinal));
    }

    [Fact]
    public void Encerramento_ExigeDataNaoFuturaEMunicipio()
    {
        var evento = new EventoDocumento { Tipo = TipoEvento.EncerramentoMdfe, DocumentoId = 1, DataEncerramento = DateTime.Today.AddDays(1) };
        var pendencias = RegrasEvento.ValidarDados(evento);
        Assert.Contains(pendencias, p => p.Contains("futura", StringComparison.Ordinal));
        Assert.Contains(pendencias, p => p.Contains("município", StringComparison.Ordinal));
    }

    [Fact]
    public void Condutor_ExigeNomeECpf()
    {
        var evento = new EventoDocumento { Tipo = TipoEvento.InclusaoCondutorMdfe, DocumentoId = 1, NomeCondutor = "X", CpfCondutor = "123" };
        Assert.Equal(2, RegrasEvento.ValidarDados(evento).Count);
    }

    [Fact]
    public void Documento_ExigeAutorizadoEPrazo()
    {
        var agora = new DateTime(2026, 11, 26, 10, 0, 0);
        var cte = new ConhecimentoTransporte { Situacao = SituacaoDocumento.ProntoParaTransmitir };
        Assert.Contains(RegrasEvento.ValidarDocumento(TipoEvento.CancelamentoCte, cte, [], agora), p => p.Contains("autorizado", StringComparison.Ordinal));

        cte.Situacao = SituacaoDocumento.Autorizado;
        cte.Chave = new string('1', 44);
        cte.Protocolo = new string('2', 15);
        cte.AutorizadoEm = agora.AddHours(-167);
        Assert.Empty(RegrasEvento.ValidarDocumento(TipoEvento.CancelamentoCte, cte, [], agora));

        cte.AutorizadoEm = agora.AddHours(-169);
        Assert.Contains(RegrasEvento.ValidarDocumento(TipoEvento.CancelamentoCte, cte, [], agora), p => p.Contains("rejeição 220", StringComparison.Ordinal));

        var mdfe = new ManifestoEletronico
        {
            Situacao = SituacaoDocumento.Autorizado,
            Chave = new string('1', 44),
            Protocolo = new string('2', 15),
            AutorizadoEm = agora.AddHours(-25),
        };
        Assert.Contains(RegrasEvento.ValidarDocumento(TipoEvento.CancelamentoMdfe, mdfe, [], agora), p => p.Contains("24 horas", StringComparison.Ordinal));
        Assert.Empty(RegrasEvento.ValidarDocumento(TipoEvento.EncerramentoMdfe, mdfe, [], agora));

        mdfe.Situacao = SituacaoDocumento.Encerrado;
        Assert.Contains(RegrasEvento.ValidarDocumento(TipoEvento.CancelamentoMdfe, mdfe, [], agora), p => p.Contains("encerrado", StringComparison.Ordinal));
    }

    [Fact]
    public void CancelamentoCte_BloqueadoComCartaDeCorrecaoRegistrada()
    {
        var cte = new ConhecimentoTransporte
        {
            Situacao = SituacaoDocumento.Autorizado,
            Chave = new string('1', 44),
            Protocolo = new string('2', 15),
            AutorizadoEm = DateTime.Now,
        };
        var carta = new EventoDocumento { Tipo = TipoEvento.CartaCorrecaoCte, Situacao = SituacaoEvento.Registrado };

        Assert.Contains(RegrasEvento.ValidarDocumento(TipoEvento.CancelamentoCte, cte, [carta], DateTime.Now),
            p => p.Contains("rejeição 523", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Repositorio_SequenciaPorTipoEDocumento_SemExclusao()
    {
        var acesso = ContextoAcesso.Sistema;
        var repositorio = new RepositorioEventos(_contexto, acesso);
        Assert.Equal(1, await repositorio.ProximaSequenciaAsync(TipoEvento.CartaCorrecaoCte, 2));

        await repositorio.IncluirAsync(Carta(sequencia: 1, SituacaoEvento.Registrado));
        await repositorio.IncluirAsync(Carta(sequencia: 2, SituacaoEvento.Rejeitado));
        Assert.Equal(2, await repositorio.ProximaSequenciaAsync(TipoEvento.CartaCorrecaoCte, 2));
        Assert.Equal(1, await repositorio.ProximaSequenciaAsync(TipoEvento.CancelamentoCte, 2));
        Assert.Equal(1, await repositorio.ProximaSequenciaAsync(TipoEvento.CartaCorrecaoCte, 3));

        var lidos = await repositorio.ListarDoDocumentoAsync(ModeloDocumento.Cte, 2);
        Assert.Equal(2, lidos.Count);
        Assert.Equal("compl", lidos[0].Correcoes.Single().Grupo);
        Assert.Empty(await repositorio.ListarDoDocumentoAsync(ModeloDocumento.Mdfe, 2));

        var id = lidos[0].Id;
        await Assert.ThrowsAsync<RegraNegocioException>(() => repositorio.ExcluirAsync(id));
        Assert.True(await _contexto.Eventos.AnyAsync(e => e.Id == id));
    }

    [Fact]
    public async Task Repositorio_OperadorSoVeEventosDasEmpresasLiberadas()
    {
        await new RepositorioEventos(_contexto, ContextoAcesso.Sistema).IncluirAsync(Carta(sequencia: 1, SituacaoEvento.Registrado));
        _contexto.ChangeTracker.Clear();

        var operador = new Usuario
        {
            Id = 99,
            Login = "maria",
            Perfil = PerfilUsuario.Operador,
            Empresas = [new() { EmpresaId = 2 }],
        };
        var repositorio = new RepositorioEventos(_contexto, ContextoAcesso.De(operador));

        Assert.Empty(await repositorio.ListarDoDocumentoAsync(ModeloDocumento.Cte, 2));
        await Assert.ThrowsAsync<RegraNegocioException>(() => repositorio.IncluirAsync(Carta(sequencia: 2, SituacaoEvento.Gerado)));
    }

    [Fact]
    public async Task Inclusao_EntraNaAuditoriaSemOXml()
    {
        var admin = await new AutenticacaoUsuarios(_contexto).CriarPrimeiroAdministradorAsync("admin", "Administrador", "Senha123forte");
        var carta = Carta(sequencia: 1, SituacaoEvento.Gerado);
        carta.Xml = "<eventoCTe>conteudo</eventoCTe>";

        await new RepositorioEventos(_contexto, ContextoAcesso.De(admin)).IncluirAsync(carta);

        var linha = await _contexto.Auditoria.AsNoTracking().SingleAsync(a => a.Acao == AcaoAuditoria.Inclusao);
        Assert.Equal("Evento", linha.Entidade);
        Assert.Equal(1, linha.EmpresaId);
        Assert.DoesNotContain("conteudo", linha.Descricao, StringComparison.Ordinal);
    }

    private static EventoDocumento Carta(int sequencia, SituacaoEvento situacao) => new()
    {
        EmpresaId = 1,
        Tipo = TipoEvento.CartaCorrecaoCte,
        DocumentoId = 2,
        Sequencia = sequencia,
        DataHora = DateTime.Now,
        Situacao = situacao,
        Correcoes = [new() { Grupo = "compl", Campo = "xObs", Valor = "Entrega pela manhã" }],
    };
}
