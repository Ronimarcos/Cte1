using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Testes;

/// <summary>
/// Controle de acesso (fatia A1): hash de senha, login com bloqueio e, principalmente, a alçada por empresa aplicada na
/// camada de dados. Banco SQLite em memória com a migração real e os dados de demonstração (empresas 1, 2 e 3).
/// </summary>
public sealed class AcessoTests : IDisposable
{
    private const string SenhaBoa = "Senha123forte";

    private readonly SqliteConnection _conexao;
    private readonly EmissorDbContext _contexto;

    public AcessoTests()
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

    // ---- Senha ----

    [Fact]
    public void HashSenha_ConfereSoASenhaCertaENaoGuardaASenha()
    {
        var hash = HashSenha.Gerar(SenhaBoa);

        Assert.StartsWith("PBKDF2-SHA256$600000$", hash, StringComparison.Ordinal);
        Assert.DoesNotContain(SenhaBoa, hash, StringComparison.Ordinal);
        Assert.True(HashSenha.Conferir(SenhaBoa, hash));
        Assert.False(HashSenha.Conferir("senha123forte", hash));
        Assert.NotEqual(hash, HashSenha.Gerar(SenhaBoa));
        Assert.False(HashSenha.Conferir(SenhaBoa, "texto-qualquer"));
    }

    [Theory]
    [InlineData("curta1", false)]
    [InlineData("somenteletras", false)]
    [InlineData("12345678", false)]
    [InlineData("letras123", true)]
    public void HashSenha_ExigeTamanhoLetrasENumeros(string senha, bool aceita)
    {
        Assert.Equal(aceita, HashSenha.ValidarForca(senha).Count == 0);
    }

    [Fact]
    public void Usuario_OperadorPrecisaDeEmpresaELoginNoFormato()
    {
        var usuario = new Usuario { Login = "Joao Silva", Nome = "João", SenhaHash = "x", Perfil = PerfilUsuario.Operador };
        var pendencias = usuario.Validar();
        Assert.Contains(pendencias, p => p.StartsWith("Login", StringComparison.Ordinal));
        Assert.Contains("Ao menos uma empresa liberada para o operador", pendencias);
        Assert.Equal("joao.silva", Usuario.NormalizarLogin("  Joao.Silva "));
    }

    // ---- Login ----

    [Fact]
    public async Task PrimeiroAdministrador_SoComOBancoSemUsuarios()
    {
        var autenticacao = new AutenticacaoUsuarios(_contexto);
        Assert.True(await autenticacao.SemUsuariosAsync());

        var admin = await autenticacao.CriarPrimeiroAdministradorAsync(" Admin ", "Administrador", SenhaBoa);

        Assert.Equal("admin", admin.Login);
        Assert.True(admin.Administrador);
        Assert.False(await autenticacao.SemUsuariosAsync());
        await Assert.ThrowsAsync<RegraNegocioException>(() => autenticacao.CriarPrimeiroAdministradorAsync("outro", "Outro", SenhaBoa));
    }

    [Fact]
    public async Task Login_MensagemIgualParaLoginOuSenhaErrados()
    {
        var autenticacao = new AutenticacaoUsuarios(_contexto);
        await autenticacao.CriarPrimeiroAdministradorAsync("admin", "Administrador", SenhaBoa);

        Assert.True((await autenticacao.AutenticarAsync("ADMIN", SenhaBoa)).Aceito);
        Assert.Equal(AutenticacaoUsuarios.MensagemInvalido, (await autenticacao.AutenticarAsync("admin", "errada123")).Mensagem);
        Assert.Equal(AutenticacaoUsuarios.MensagemInvalido, (await autenticacao.AutenticarAsync("ninguem", SenhaBoa)).Mensagem);
    }

    [Fact]
    public async Task Login_BloqueiaDepoisDeCincoErrosELiberaDepoisDoTempo()
    {
        var agora = new DateTime(2026, 11, 25, 10, 0, 0);
        var autenticacao = new AutenticacaoUsuarios(_contexto, () => agora);
        await autenticacao.CriarPrimeiroAdministradorAsync("admin", "Administrador", SenhaBoa);

        for (var i = 0; i < Usuario.TentativasAteBloquear; i++)
        {
            Assert.False((await autenticacao.AutenticarAsync("admin", "errada123")).Aceito);
        }

        var bloqueado = await autenticacao.AutenticarAsync("admin", SenhaBoa);
        Assert.False(bloqueado.Aceito);
        Assert.Contains("bloqueado", bloqueado.Mensagem, StringComparison.Ordinal);

        agora = agora + Usuario.TempoBloqueio + TimeSpan.FromSeconds(1);
        Assert.True((await autenticacao.AutenticarAsync("admin", SenhaBoa)).Aceito);
    }

    [Fact]
    public async Task Login_UsuarioInativoNaoEntra()
    {
        await CarregarDemonstracaoAsync();
        _contexto.Usuarios.Add(Operador("maria", 2, ativo: false));
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();

        var resultado = await new AutenticacaoUsuarios(_contexto).AutenticarAsync("maria", SenhaBoa);
        Assert.False(resultado.Aceito);
        Assert.Contains("inativo", resultado.Mensagem, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TrocarSenha_ConfereAAtualELimpaATrocaObrigatoria()
    {
        await CarregarDemonstracaoAsync();
        var maria = Operador("maria", 2);
        maria.DeveTrocarSenha = true;
        _contexto.Usuarios.Add(maria);
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();
        var autenticacao = new AutenticacaoUsuarios(_contexto);

        await Assert.ThrowsAsync<RegraNegocioException>(() => autenticacao.TrocarSenhaAsync(maria.Id, "errada123", "Nova12345"));
        await autenticacao.TrocarSenhaAsync(maria.Id, SenhaBoa, "Nova12345");

        var relido = await _contexto.Usuarios.AsNoTracking().SingleAsync(u => u.Id == maria.Id);
        Assert.False(relido.DeveTrocarSenha);
        Assert.True((await autenticacao.AutenticarAsync("maria", "Nova12345")).Aceito);
    }

    // ---- Alçada por empresa ----

    [Fact]
    public async Task Operador_SoVeAsEmpresasLiberadas()
    {
        await CarregarDemonstracaoAsync();
        var acesso = ContextoAcesso.De(Operador("maria", 2));

        var empresas = await new RepositorioCadastro<Empresa>(_contexto, acesso).ListarAsync();

        Assert.Equal([2], empresas.Select(e => e.Id));
        Assert.Null(await new RepositorioCadastro<Empresa>(_contexto, acesso).ObterAsync(1));
    }

    [Fact]
    public async Task Operador_NaoLeNemAbreCadastroDeOutraEmpresa()
    {
        await CarregarDemonstracaoAsync();
        var participantes = new RepositorioCadastro<Participante>(_contexto, ContextoAcesso.De(Operador("maria", 2)));

        Assert.All(await participantes.ListarAsync(), p => Assert.Equal(2, p.EmpresaId));
        Assert.Null(await participantes.ObterAsync(1)); // participante 1 é da empresa 1
        Assert.False(await participantes.ExisteAsync(p => p.Id == 1));
        Assert.NotNull(await participantes.ObterAsync(4)); // participante 4 é da empresa 2
    }

    [Fact]
    public async Task Operador_NaoGravaNemExcluiEmOutraEmpresa()
    {
        await CarregarDemonstracaoAsync();
        var participantes = new RepositorioCadastro<Participante>(_contexto, ContextoAcesso.De(Operador("maria", 2)));

        var novo = DadosDemonstracao.Participantes().First(p => p.EmpresaId == 1);
        novo.Id = 0;
        novo.CnpjCpf = "99999999000199";
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => participantes.IncluirAsync(novo));
        Assert.Contains("não tem acesso", erro.Message, StringComparison.Ordinal);

        var proprio = (await participantes.ObterAsync(4))!;
        proprio.EmpresaId = 1;
        await Assert.ThrowsAsync<RegraNegocioException>(() => participantes.AlterarAsync(proprio));

        await participantes.ExcluirAsync(1);
        Assert.NotNull(await new RepositorioCadastro<Participante>(_contexto).ObterAsync(1));
    }

    [Fact]
    public async Task Operador_NaoVeDocumentosDeOutraEmpresa()
    {
        await CarregarDemonstracaoAsync();
        var operador = ContextoAcesso.De(Operador("maria", 2));

        Assert.Empty(await new RepositorioDocumento<ConhecimentoTransporte>(_contexto, operador).ListarAsync());
        Assert.Null(await new RepositorioDocumento<ManifestoEletronico>(_contexto, operador).ObterAsync(1));
        await new RepositorioDocumento<ConhecimentoTransporte>(_contexto, operador).ExcluirAsync(2);
        Assert.NotNull(await new RepositorioDocumento<ConhecimentoTransporte>(_contexto).ObterAsync(2));
    }

    [Fact]
    public async Task Operador_NaoGravaEmpresaCertificadoNemUsuario()
    {
        await CarregarDemonstracaoAsync();
        var operador = ContextoAcesso.De(Operador("maria", 2));

        var empresa = (await new RepositorioCadastro<Empresa>(_contexto, operador).ObterAsync(2))!;
        empresa.NomeFantasia = "ALTERADA";
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioCadastro<Empresa>(_contexto, operador).AlterarAsync(empresa));
        Assert.Contains("Somente o administrador", erro.Message, StringComparison.Ordinal);

        var certificado = new CertificadoDigital { EmpresaId = 2, Tipo = TipoCertificado.A3, Thumbprint = new string('A', 40), Titular = "X", CnpjTitular = "22222222000122" };
        await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioCadastro<CertificadoDigital>(_contexto, operador).IncluirAsync(certificado));

        Assert.Empty(await new RepositorioUsuarios(_contexto, operador).ListarAsync());
        await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioUsuarios(_contexto, operador).IncluirAsync(Operador("joao", 2)));
    }

    [Fact]
    public async Task Administrador_VeTodasAsEmpresas()
    {
        await CarregarDemonstracaoAsync();
        var admin = new Usuario { Login = "admin", Nome = "Admin", Perfil = PerfilUsuario.Administrador, SenhaHash = "x" };

        var empresas = await new RepositorioCadastro<Empresa>(_contexto, ContextoAcesso.De(admin)).ListarAsync();
        Assert.Equal([1, 2, 3], empresas.Select(e => e.Id));
    }

    [Fact]
    public void SessaoSemLogin_NaoAcessaNada()
    {
        var sessao = new Sessao();
        Assert.False(sessao.Autenticada);
        Assert.False(sessao.PodeAcessar(1));

        sessao.Entrar(Operador("maria", 2));
        Assert.True(sessao.PodeAcessar(2));
        Assert.False(sessao.PodeAcessar(1));

        sessao.Sair();
        Assert.False(sessao.PodeAcessar(2));
    }

    // ---- Cadastro de usuários ----

    [Fact]
    public async Task Usuarios_AlteracaoTrocaAsEmpresasLiberadas()
    {
        await CarregarDemonstracaoAsync();
        var admin = await new AutenticacaoUsuarios(_contexto).CriarPrimeiroAdministradorAsync("admin", "Administrador", SenhaBoa);
        var repositorio = new RepositorioUsuarios(_contexto, ContextoAcesso.De(admin));
        var maria = Operador("maria", 2);
        await repositorio.IncluirAsync(maria);

        var editado = (await repositorio.ObterAsync(maria.Id))!;
        editado.Empresas = [new() { EmpresaId = 1 }, new() { EmpresaId = 3 }];
        await repositorio.AlterarAsync(editado);

        var relido = (await repositorio.ObterAsync(maria.Id))!;
        Assert.Equal([1, 3], relido.Empresas.Select(e => e.EmpresaId).Order());
    }

    [Fact]
    public async Task Usuarios_NaoFicaSemAdministradorAtivo()
    {
        await CarregarDemonstracaoAsync();
        var admin = await new AutenticacaoUsuarios(_contexto).CriarPrimeiroAdministradorAsync("admin", "Administrador", SenhaBoa);
        var repositorio = new RepositorioUsuarios(_contexto, ContextoAcesso.De(admin));

        var proprio = (await repositorio.ObterAsync(admin.Id))!;
        proprio.Ativo = false;
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => repositorio.AlterarAsync(proprio));
        Assert.Contains("ao menos um administrador ativo", erro.Message, StringComparison.Ordinal);

        await Assert.ThrowsAsync<RegraNegocioException>(() => repositorio.ExcluirAsync(admin.Id));
    }

    [Fact]
    public async Task Usuarios_EmpresaLiberadaNaoPodeSerExcluida()
    {
        await CarregarDemonstracaoAsync();
        _contexto.Usuarios.Add(Operador("maria", 3));
        await _contexto.SaveChangesAsync();
        _contexto.ChangeTracker.Clear();

        await Assert.ThrowsAsync<RegraNegocioException>(() => new RepositorioCadastro<Empresa>(_contexto).ExcluirAsync(3));
    }

    private static Usuario Operador(string login, int empresaId, bool ativo = true) => new()
    {
        Login = login,
        Nome = login.ToUpperInvariant(),
        Perfil = PerfilUsuario.Operador,
        SenhaHash = HashSenha.Gerar(SenhaBoa),
        Ativo = ativo,
        Empresas = [new() { EmpresaId = empresaId }],
    };

    private async Task CarregarDemonstracaoAsync()
    {
        await CargaMunicipios.CarregarSeVazioAsync(_contexto);
        await DadosDemonstracao.CarregarAsync(_contexto);
    }
}
