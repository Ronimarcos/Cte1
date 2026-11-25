using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>Filtro da consulta de auditoria.</summary>
/// <param name="De">Primeiro dia.</param>
/// <param name="Ate">Último dia.</param>
/// <param name="UsuarioId">Usuário, ou <c>null</c> para todos.</param>
/// <param name="EmpresaId">Empresa, ou <c>null</c> para todas.</param>
/// <param name="Acao">Ação, ou <c>null</c> para todas.</param>
/// <param name="Texto">Texto procurado em login, tipo do registro e descrição, ou vazio.</param>
public sealed record FiltroAuditoria(DateTime De, DateTime Ate, int? UsuarioId = null, int? EmpresaId = null, AcaoAuditoria? Acao = null, string Texto = "");

/// <summary>
/// Eventos e consulta da auditoria. As inclusões, alterações e exclusões entram sozinhas (pelo
/// <see cref="EmissorDbContext"/>); aqui ficam os eventos de acesso e de uso (login, troca de empresa, XML de teste,
/// SEFAZ) e a consulta, que só o administrador faz.
/// </summary>
public static class Auditoria
{
    /// <summary>Limite de linhas por consulta.</summary>
    public const int LimiteConsulta = 2000;

    /// <summary>Monta a linha de um evento.</summary>
    /// <param name="acao">Evento.</param>
    /// <param name="usuario">Usuário, ou <c>null</c> (tentativa com login inexistente).</param>
    /// <param name="login">Login a registrar (o do usuário, ou o digitado).</param>
    /// <param name="empresaId">Empresa, quando houver.</param>
    /// <param name="descricao">Detalhe do evento.</param>
    /// <param name="agora">Data e hora.</param>
    public static RegistroAuditoria Evento(AcaoAuditoria acao, Usuario? usuario, string login, int? empresaId, string descricao, DateTime agora)
    {
        var loginLimitado = (usuario?.Login ?? login).Trim();
        return new RegistroAuditoria
        {
            DataHora = agora,
            UsuarioId = usuario?.Id,
            UsuarioLogin = loginLimitado.Length > 30 ? loginLimitado[..30] : loginLimitado,
            EmpresaId = empresaId,
            Acao = acao,
            Descricao = descricao.Length > RegistroAuditoria.TamanhoDescricao ? descricao[..RegistroAuditoria.TamanhoDescricao] : descricao,
        };
    }

    /// <summary>Grava um evento do usuário logado.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    /// <param name="acesso">Sessão do usuário (sem usuário, nada é gravado).</param>
    /// <param name="acao">Evento.</param>
    /// <param name="empresaId">Empresa, quando houver.</param>
    /// <param name="descricao">Detalhe.</param>
    /// <param name="cancelamento">Cancelamento.</param>
    public static async Task RegistrarAsync(EmissorDbContext contexto, IContextoAcesso acesso, AcaoAuditoria acao, int? empresaId, string descricao,
        CancellationToken cancelamento = default)
    {
        if (acesso.Usuario is not { } usuario)
        {
            return;
        }

        contexto.Auditoria.Add(Evento(acao, usuario, usuario.Login, empresaId, descricao, DateTime.Now));
        await contexto.SaveChangesAsync(cancelamento);
        contexto.ChangeTracker.Clear();
    }

    /// <summary>Consulta a auditoria (só administrador), das mais recentes para as mais antigas.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    /// <param name="acesso">Sessão do usuário.</param>
    /// <param name="filtro">Filtro.</param>
    /// <param name="cancelamento">Cancelamento.</param>
    /// <exception cref="RegraNegocioException">Usuário sem acesso total.</exception>
    public static async Task<IReadOnlyList<RegistroAuditoria>> ConsultarAsync(EmissorDbContext contexto, IContextoAcesso acesso, FiltroAuditoria filtro,
        CancellationToken cancelamento = default)
    {
        if (!acesso.AcessoTotal)
        {
            throw new RegraNegocioException("Somente o administrador consulta a auditoria.");
        }

        var inicio = filtro.De.Date;
        var fim = filtro.Ate.Date.AddDays(1);
        var consulta = contexto.Auditoria.AsNoTracking().Where(a => a.DataHora >= inicio && a.DataHora < fim);
        if (filtro.UsuarioId is { } usuarioId)
        {
            consulta = consulta.Where(a => a.UsuarioId == usuarioId);
        }

        if (filtro.EmpresaId is { } empresaId)
        {
            consulta = consulta.Where(a => a.EmpresaId == empresaId);
        }

        if (filtro.Acao is { } acao)
        {
            consulta = consulta.Where(a => a.Acao == acao);
        }

        if (filtro.Texto.Trim() is { Length: > 0 } texto)
        {
            consulta = consulta.Where(a => a.UsuarioLogin.Contains(texto) || a.Entidade.Contains(texto) || a.Descricao.Contains(texto));
        }

        return await consulta.OrderByDescending(a => a.DataHora).ThenByDescending(a => a.Id).Take(LimiteConsulta).ToListAsync(cancelamento);
    }
}
