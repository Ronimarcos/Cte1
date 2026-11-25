using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>Resultado da tentativa de login.</summary>
/// <param name="Usuario">Usuário autenticado, ou <c>null</c>.</param>
/// <param name="Mensagem">Motivo da recusa (genérico para não revelar se o login existe), ou vazio.</param>
public sealed record ResultadoLogin(Usuario? Usuario, string Mensagem)
{
    /// <summary>Indica se o login foi aceito.</summary>
    public bool Aceito => Usuario is not null;
}

/// <summary>
/// Login e senha dos usuários do emissor: criação do primeiro administrador, autenticação com bloqueio após tentativas
/// erradas e troca de senha. Não depende de tela, para servir também à versão web.
/// </summary>
public sealed class AutenticacaoUsuarios
{
    /// <summary>Mensagem para login ou senha errados (a mesma nos dois casos).</summary>
    public const string MensagemInvalido = "Login ou senha inválidos.";

    private readonly EmissorDbContext _contexto;
    private readonly Func<DateTime> _agora;

    /// <summary>Cria o serviço sobre o contexto.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    /// <param name="agora">Relógio (para os testes do bloqueio); padrão <see cref="DateTime.Now"/>.</param>
    public AutenticacaoUsuarios(EmissorDbContext contexto, Func<DateTime>? agora = null)
    {
        _contexto = contexto;
        _agora = agora ?? (() => DateTime.Now);
    }

    /// <summary>Indica se ainda não há nenhum usuário (primeiro acesso).</summary>
    /// <param name="cancelamento">Cancelamento.</param>
    public async Task<bool> SemUsuariosAsync(CancellationToken cancelamento = default) =>
        !await _contexto.Usuarios.AnyAsync(cancelamento);

    /// <summary>Cria o primeiro administrador; só funciona com o banco sem usuários.</summary>
    /// <param name="login">Login.</param>
    /// <param name="nome">Nome.</param>
    /// <param name="senha">Senha (validada pela força mínima).</param>
    /// <param name="cancelamento">Cancelamento.</param>
    /// <exception cref="RegraNegocioException">Já há usuários, senha fraca ou dados inválidos.</exception>
    public async Task<Usuario> CriarPrimeiroAdministradorAsync(string login, string nome, string senha, CancellationToken cancelamento = default)
    {
        if (!await SemUsuariosAsync(cancelamento))
        {
            throw new RegraNegocioException("O primeiro administrador já foi criado. Entre com um usuário existente.");
        }

        GarantirForca(senha);
        var usuario = new Usuario
        {
            Login = Usuario.NormalizarLogin(login),
            Nome = nome.Trim(),
            Perfil = PerfilUsuario.Administrador,
            SenhaHash = HashSenha.Gerar(senha),
            UltimoAcesso = _agora(),
        };
        await new RepositorioCadastro<Usuario>(_contexto).IncluirAsync(usuario, cancelamento);
        await RegistrarAsync(AcaoAuditoria.PrimeiroAdministrador, usuario, usuario.Login, "Primeiro administrador criado", cancelamento);
        return usuario;
    }

    /// <summary>
    /// Confere login e senha. Após <see cref="Usuario.TentativasAteBloquear"/> erros seguidos, bloqueia por
    /// <see cref="Usuario.TempoBloqueio"/>. Usuário inativo não entra.
    /// </summary>
    /// <param name="login">Login digitado.</param>
    /// <param name="senha">Senha digitada.</param>
    /// <param name="cancelamento">Cancelamento.</param>
    public async Task<ResultadoLogin> AutenticarAsync(string login, string senha, CancellationToken cancelamento = default)
    {
        var normalizado = Usuario.NormalizarLogin(login);
        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Login == normalizado, cancelamento);
        var agora = _agora();
        if (usuario is null)
        {
            // Mesmo custo de uma conferência real, para não indicar pelo tempo de resposta que o login não existe.
            HashSenha.Conferir(senha, HashFicticio);
            await RegistrarAsync(AcaoAuditoria.LoginRecusado, null, normalizado, "Login inexistente", cancelamento);
            return new ResultadoLogin(null, MensagemInvalido);
        }

        try
        {
            if (usuario.BloqueadoEm(agora))
            {
                await RegistrarAsync(AcaoAuditoria.LoginBloqueado, usuario, usuario.Login, $"Tentativa durante o bloqueio (até {usuario.BloqueadoAte:dd/MM/yyyy HH:mm})", cancelamento);
                return new ResultadoLogin(null, $"Login bloqueado por excesso de tentativas. Tente de novo depois de {usuario.BloqueadoAte:HH:mm}.");
            }

            if (!HashSenha.Conferir(senha, usuario.SenhaHash))
            {
                usuario.TentativasFalhas++;
                _contexto.Auditoria.Add(Auditoria.Evento(AcaoAuditoria.LoginRecusado, usuario, usuario.Login, null,
                    $"Senha errada (tentativa {usuario.TentativasFalhas} de {Usuario.TentativasAteBloquear})", agora));
                if (usuario.TentativasFalhas >= Usuario.TentativasAteBloquear)
                {
                    usuario.BloqueadoAte = agora + Usuario.TempoBloqueio;
                    usuario.TentativasFalhas = 0;
                    _contexto.Auditoria.Add(Auditoria.Evento(AcaoAuditoria.LoginBloqueado, usuario, usuario.Login, null,
                        $"Login bloqueado até {usuario.BloqueadoAte:dd/MM/yyyy HH:mm}", agora));
                }

                await _contexto.SaveChangesAsync(cancelamento);
                return new ResultadoLogin(null, MensagemInvalido);
            }

            if (!usuario.Ativo)
            {
                await RegistrarAsync(AcaoAuditoria.LoginInativo, usuario, usuario.Login, "Tentativa de usuário inativo", cancelamento);
                return new ResultadoLogin(null, "Usuário inativo. Procure o administrador.");
            }

            usuario.TentativasFalhas = 0;
            usuario.BloqueadoAte = null;
            usuario.UltimoAcesso = agora;
            _contexto.Auditoria.Add(Auditoria.Evento(AcaoAuditoria.LoginAceito, usuario, usuario.Login, null, "Login aceito", agora));
            await _contexto.SaveChangesAsync(cancelamento);
            return new ResultadoLogin(usuario, string.Empty);
        }
        finally
        {
            _contexto.ChangeTracker.Clear();
        }
    }

    /// <summary>Troca a senha do próprio usuário, conferindo a senha atual.</summary>
    /// <param name="usuarioId">Usuário logado.</param>
    /// <param name="senhaAtual">Senha atual.</param>
    /// <param name="senhaNova">Senha nova.</param>
    /// <param name="cancelamento">Cancelamento.</param>
    /// <exception cref="RegraNegocioException">Senha atual errada, senha nova fraca ou igual à atual.</exception>
    public async Task TrocarSenhaAsync(int usuarioId, string senhaAtual, string senhaNova, CancellationToken cancelamento = default)
    {
        try
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId, cancelamento)
                ?? throw new RegraNegocioException("Usuário não encontrado.");
            if (!HashSenha.Conferir(senhaAtual, usuario.SenhaHash))
            {
                throw new RegraNegocioException("A senha atual não confere.");
            }

            if (senhaNova == senhaAtual)
            {
                throw new RegraNegocioException("A senha nova precisa ser diferente da atual.");
            }

            GarantirForca(senhaNova);
            usuario.SenhaHash = HashSenha.Gerar(senhaNova);
            usuario.DeveTrocarSenha = false;
            _contexto.Auditoria.Add(Auditoria.Evento(AcaoAuditoria.TrocaSenha, usuario, usuario.Login, null, "Senha trocada pelo próprio usuário", _agora()));
            await _contexto.SaveChangesAsync(cancelamento);
        }
        finally
        {
            _contexto.ChangeTracker.Clear();
        }
    }

    /// <summary>Grava um evento de acesso (login, bloqueio, primeiro administrador) na hora.</summary>
    private async Task RegistrarAsync(AcaoAuditoria acao, Usuario? usuario, string login, string descricao, CancellationToken cancelamento)
    {
        _contexto.Auditoria.Add(Auditoria.Evento(acao, usuario, login, null, descricao, _agora()));
        await _contexto.SaveChangesAsync(cancelamento);
        _contexto.ChangeTracker.Clear();
    }

    private static void GarantirForca(string senha)
    {
        var pendencias = HashSenha.ValidarForca(senha);
        if (pendencias.Count > 0)
        {
            throw new RegraNegocioException(pendencias);
        }
    }

    private static readonly string HashFicticio = HashSenha.Gerar(Guid.NewGuid().ToString());
}
