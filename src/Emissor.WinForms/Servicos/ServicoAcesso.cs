using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Login, primeiro acesso, troca de senha e cadastro de usuários para as telas. A conferência da senha (PBKDF2) roda
/// fora da thread da tela. O cadastro de usuários passa pela alçada: só o administrador lista e grava.
/// </summary>
public sealed class ServicoAcesso
{
    private readonly IDbContextFactory<EmissorDbContext> _fabrica;
    private readonly Sessao _sessao;

    /// <summary>Cria o serviço.</summary>
    /// <param name="fabrica">Fábrica de contextos do banco.</param>
    /// <param name="sessao">Sessão do aplicativo, preenchida no login.</param>
    public ServicoAcesso(IDbContextFactory<EmissorDbContext> fabrica, Sessao sessao)
    {
        _fabrica = fabrica;
        _sessao = sessao;
    }

    /// <summary>Sessão do usuário logado.</summary>
    public Sessao Sessao => _sessao;

    /// <summary>Indica se ainda não há usuários (primeiro acesso).</summary>
    public Task<bool> SemUsuariosAsync() => ExecutarAsync(a => a.SemUsuariosAsync());

    /// <summary>Cria o primeiro administrador e já inicia a sessão com ele.</summary>
    /// <param name="login">Login.</param>
    /// <param name="nome">Nome.</param>
    /// <param name="senha">Senha.</param>
    public async Task CriarPrimeiroAdministradorAsync(string login, string nome, string senha)
    {
        var usuario = await Task.Run(() => ExecutarAsync(a => a.CriarPrimeiroAdministradorAsync(login, nome, senha)));
        _sessao.Entrar(usuario);
    }

    /// <summary>Confere login e senha; se aceito, inicia a sessão.</summary>
    /// <param name="login">Login.</param>
    /// <param name="senha">Senha.</param>
    public async Task<ResultadoLogin> EntrarAsync(string login, string senha)
    {
        var resultado = await Task.Run(() => ExecutarAsync(a => a.AutenticarAsync(login, senha)));
        if (resultado.Usuario is { } usuario)
        {
            _sessao.Entrar(usuario);
        }

        return resultado;
    }

    /// <summary>Troca a senha do usuário logado.</summary>
    /// <param name="senhaAtual">Senha atual.</param>
    /// <param name="senhaNova">Senha nova.</param>
    /// <exception cref="RegraNegocioException">Sem login, senha atual errada ou senha nova fraca.</exception>
    public async Task TrocarSenhaAsync(string senhaAtual, string senhaNova)
    {
        var usuario = _sessao.Usuario ?? throw new RegraNegocioException("Nenhum usuário logado.");
        await Task.Run(() => ExecutarAsync(async a => { await a.TrocarSenhaAsync(usuario.Id, senhaAtual, senhaNova); return true; }));
        usuario.DeveTrocarSenha = false;
    }

    /// <summary>Encerra a sessão (trocar usuário).</summary>
    public void Sair() => _sessao.Sair();

    /// <summary>Usuários cadastrados (só administrador; para o operador a lista vem vazia).</summary>
    public Task<IReadOnlyList<Usuario>> ListarUsuariosAsync() => UsuariosAsync(r => r.ListarAsync());

    /// <summary>Cópia do usuário para edição, ou <c>null</c>.</summary>
    /// <param name="id">Identificador.</param>
    public Task<Usuario?> ObterUsuarioAsync(int id) => UsuariosAsync(r => r.ObterAsync(id));

    /// <summary>Inclui o usuário com a senha inicial, que ele deverá trocar no primeiro login.</summary>
    /// <param name="usuario">Usuário novo.</param>
    /// <param name="senhaInicial">Senha inicial.</param>
    public Task IncluirUsuarioAsync(Usuario usuario, string senhaInicial) => UsuariosAsync(async r =>
    {
        Preparar(usuario, senhaInicial);
        usuario.DeveTrocarSenha = true;
        await r.IncluirAsync(usuario);
        return true;
    });

    /// <summary>Altera o usuário; com senha nova informada, redefine e exige a troca no próximo login.</summary>
    /// <param name="usuario">Usuário editado.</param>
    /// <param name="senhaNova">Senha nova, ou vazio para manter a atual.</param>
    public Task AlterarUsuarioAsync(Usuario usuario, string? senhaNova) => UsuariosAsync(async r =>
    {
        if (!string.IsNullOrEmpty(senhaNova))
        {
            Preparar(usuario, senhaNova);
            usuario.DeveTrocarSenha = true;
            usuario.TentativasFalhas = 0;
            usuario.BloqueadoAte = null;
        }

        await r.AlterarAsync(usuario);
        return true;
    });

    /// <summary>Exclui o usuário (não o próprio nem o último administrador ativo).</summary>
    /// <param name="id">Identificador.</param>
    public Task ExcluirUsuarioAsync(int id) => UsuariosAsync(async r => { await r.ExcluirAsync(id); return true; });

    /// <summary>Normaliza o login, aplica a senha (validada) e limpa as empresas do administrador.</summary>
    private static void Preparar(Usuario usuario, string senha)
    {
        var pendencias = HashSenha.ValidarForca(senha);
        if (pendencias.Count > 0)
        {
            throw new RegraNegocioException(pendencias);
        }

        usuario.Login = Usuario.NormalizarLogin(usuario.Login);
        usuario.SenhaHash = HashSenha.Gerar(senha);
        if (usuario.Perfil == PerfilUsuario.Administrador)
        {
            usuario.Empresas = [];
        }
    }

    private async Task<TResultado> ExecutarAsync<TResultado>(Func<AutenticacaoUsuarios, Task<TResultado>> operacao)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await operacao(new AutenticacaoUsuarios(contexto));
    }

    private async Task<TResultado> UsuariosAsync<TResultado>(Func<RepositorioUsuarios, Task<TResultado>> operacao)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await operacao(new RepositorioUsuarios(contexto, _sessao));
    }
}
