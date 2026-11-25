using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Testes;

/// <summary>
/// Auditoria (fatia A2): linhas automáticas nas gravações do usuário logado, eventos de login, consulta só do
/// administrador e a proteção do banco contra alteração e exclusão do histórico.
/// </summary>
public sealed class AuditoriaTests : IDisposable
{
    private const string SenhaBoa = "Senha123forte";

    private readonly SqliteConnection _conexao;
    private readonly EmissorDbContext _contexto;

    public AuditoriaTests()
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

    [Fact]
    public async Task OperacoesInternas_NaoSaoAuditadas()
    {
        // A carga da demonstração (feita no construtor) não tem usuário.
        Assert.Equal(0, await _contexto.Auditoria.CountAsync());
    }

    [Fact]
    public async Task Inclusao_RegistraUsuarioEmpresaECodigo()
    {
        var maria = await OperadoraAsync();
        var participante = DadosDemonstracao.Participantes().First(p => p.Id == 4);
        participante.Id = 0;
        participante.CnpjCpf = "99999999000199";

        await new RepositorioCadastro<Participante>(_contexto, ContextoAcesso.De(maria)).IncluirAsync(participante);

        var linha = await _contexto.Auditoria.AsNoTracking().SingleAsync(a => a.Acao == AcaoAuditoria.Inclusao);
        Assert.Equal(maria.Id, linha.UsuarioId);
        Assert.Equal("maria", linha.UsuarioLogin);
        Assert.Equal(2, linha.EmpresaId);
        Assert.Equal("Participante", linha.Entidade);
        Assert.Equal(participante.Id, linha.RegistroId);
        Assert.Contains("CnpjCpf: \"99999999000199\"", linha.Descricao, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Alteracao_RegistraSoOsCamposQueMudaram()
    {
        var maria = await OperadoraAsync();
        var repositorio = new RepositorioCadastro<Participante>(_contexto, ContextoAcesso.De(maria));
        var participante = (await repositorio.ObterAsync(4))!;
        var nomeAnterior = participante.RazaoSocial;
        participante.RazaoSocial = "NOVA RAZAO";
        participante.Endereco.Bairro = "BAIRRO NOVO";

        await repositorio.AlterarAsync(participante);

        var linha = await _contexto.Auditoria.AsNoTracking().SingleAsync(a => a.Acao == AcaoAuditoria.Alteracao);
        Assert.Contains($"RazaoSocial: \"{nomeAnterior}\" → \"NOVA RAZAO\"", linha.Descricao, StringComparison.Ordinal);
        Assert.Contains("Endereco.Bairro:", linha.Descricao, StringComparison.Ordinal);
        Assert.DoesNotContain("CnpjCpf", linha.Descricao, StringComparison.Ordinal);
        Assert.DoesNotContain("AlteradoEm", linha.Descricao, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AlteracaoSemMudanca_NaoGeraLinha()
    {
        var maria = await OperadoraAsync();
        var repositorio = new RepositorioCadastro<Participante>(_contexto, ContextoAcesso.De(maria));
        await repositorio.AlterarAsync((await repositorio.ObterAsync(4))!);
        Assert.False(await _contexto.Auditoria.AnyAsync(a => a.Acao == AcaoAuditoria.Alteracao));
    }

    [Fact]
    public async Task Exclusao_Registrada()
    {
        var admin = await AdministradorAsync();
        await new RepositorioDocumento<ConhecimentoTransporte>(_contexto, ContextoAcesso.De(admin)).ExcluirAsync(2);

        var linha = await _contexto.Auditoria.AsNoTracking().SingleAsync(a => a.Acao == AcaoAuditoria.Exclusao);
        Assert.Equal("CT-e", linha.Entidade);
        Assert.Equal(2, linha.RegistroId);
        Assert.Equal(1, linha.EmpresaId);
    }

    [Fact]
    public async Task ItensDoDocumento_ViramItensAlterados()
    {
        var admin = await AdministradorAsync();
        var repositorio = new RepositorioDocumento<ConhecimentoTransporte>(_contexto, ContextoAcesso.De(admin));
        var cte = (await repositorio.ObterAsync(2))!;
        cte.Componentes = [new() { Nome = "FRETE NOVO", Valor = 500m }];

        await repositorio.AlterarAsync(cte);

        var linha = await _contexto.Auditoria.AsNoTracking().SingleAsync(a => a.Acao == AcaoAuditoria.Alteracao);
        Assert.Equal("CT-e", linha.Entidade);
        Assert.Contains("Itens alterados: Componentes", linha.Descricao, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SenhaECertificado_NuncaVaoParaAAuditoria()
    {
        var admin = await AdministradorAsync();
        var maria = await OperadoraAsync();
        var repositorio = new RepositorioUsuarios(_contexto, ContextoAcesso.De(admin));
        var editada = (await repositorio.ObterAsync(maria.Id))!;
        var hashAnterior = editada.SenhaHash;
        editada.SenhaHash = HashSenha.Gerar("Outra12345");

        await repositorio.AlterarAsync(editada);

        var linha = await _contexto.Auditoria.AsNoTracking().SingleAsync(a => a.Acao == AcaoAuditoria.Alteracao);
        Assert.Contains("SenhaHash: alterado", linha.Descricao, StringComparison.Ordinal);
        Assert.DoesNotContain("PBKDF2", linha.Descricao, StringComparison.Ordinal);
        Assert.DoesNotContain(hashAnterior, linha.Descricao, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Login_RegistraAceitoRecusadoEBloqueio()
    {
        await AdministradorAsync();
        var autenticacao = new AutenticacaoUsuarios(_contexto);

        await autenticacao.AutenticarAsync("admin", SenhaBoa);
        await autenticacao.AutenticarAsync("ninguem", SenhaBoa);
        for (var i = 0; i < Usuario.TentativasAteBloquear; i++)
        {
            await autenticacao.AutenticarAsync("admin", "errada123");
        }

        var linhas = await _contexto.Auditoria.AsNoTracking().ToListAsync();
        Assert.Contains(linhas, a => a.Acao == AcaoAuditoria.PrimeiroAdministrador);
        Assert.Contains(linhas, a => a.Acao == AcaoAuditoria.LoginAceito && a.UsuarioLogin == "admin");
        Assert.Contains(linhas, a => a.Acao == AcaoAuditoria.LoginRecusado && a.UsuarioId == null && a.UsuarioLogin == "ninguem");
        Assert.Equal(Usuario.TentativasAteBloquear, linhas.Count(a => a.Acao == AcaoAuditoria.LoginRecusado && a.UsuarioLogin == "admin"));
        Assert.Single(linhas, a => a.Acao == AcaoAuditoria.LoginBloqueado);
        Assert.DoesNotContain(linhas, a => a.Descricao.Contains("errada123", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Consulta_SoAdministradorComFiltros()
    {
        var admin = await AdministradorAsync();
        var maria = await OperadoraAsync();
        await Auditoria.RegistrarAsync(_contexto, ContextoAcesso.De(maria), AcaoAuditoria.EntradaEmpresa, 2, "Entrada na empresa BETA");
        await Auditoria.RegistrarAsync(_contexto, ContextoAcesso.De(admin), AcaoAuditoria.EntradaEmpresa, 1, "Entrada na empresa ALFA");

        await Assert.ThrowsAsync<RegraNegocioException>(() =>
            Auditoria.ConsultarAsync(_contexto, ContextoAcesso.De(maria), new FiltroAuditoria(DateTime.Today, DateTime.Today)));

        var daMaria = await Auditoria.ConsultarAsync(_contexto, ContextoAcesso.De(admin),
            new FiltroAuditoria(DateTime.Today, DateTime.Today, UsuarioId: maria.Id, Acao: AcaoAuditoria.EntradaEmpresa));
        Assert.Equal("Entrada na empresa BETA", Assert.Single(daMaria).Descricao);

        var porTexto = await Auditoria.ConsultarAsync(_contexto, ContextoAcesso.De(admin), new FiltroAuditoria(DateTime.Today, DateTime.Today, Texto: "ALFA"));
        Assert.Equal(1, Assert.Single(porTexto).EmpresaId);
    }

    [Fact]
    public async Task Banco_RecusaAlterarOuExcluirAuditoria()
    {
        await AdministradorAsync();
        Assert.True(await _contexto.Auditoria.AnyAsync());

        var alterar = await Assert.ThrowsAsync<SqliteException>(() => _contexto.Database.ExecuteSqlRawAsync("UPDATE \"Auditoria\" SET \"Descricao\" = 'x'"));
        Assert.Contains("não pode ser alterada", alterar.Message, StringComparison.Ordinal);
        var excluir = await Assert.ThrowsAsync<SqliteException>(() => _contexto.Database.ExecuteSqlRawAsync("DELETE FROM \"Auditoria\""));
        Assert.Contains("não pode ser excluída", excluir.Message, StringComparison.Ordinal);
    }

    private async Task<Usuario> AdministradorAsync() =>
        await new AutenticacaoUsuarios(_contexto).CriarPrimeiroAdministradorAsync("admin", "Administrador", SenhaBoa);

    private async Task<Usuario> OperadoraAsync()
    {
        var maria = new Usuario
        {
            Login = "maria",
            Nome = "MARIA",
            Perfil = PerfilUsuario.Operador,
            SenhaHash = HashSenha.Gerar(SenhaBoa),
            Empresas = [new() { EmpresaId = 2 }],
        };
        _contexto.Usuarios.Add(maria);
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();
        return maria;
    }
}
