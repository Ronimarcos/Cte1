using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Auditoria para as telas: eventos de uso do usuário logado (entrada na empresa, troca de usuário, XML de teste,
/// SEFAZ) e consulta, que só o administrador faz. Inclusões, alterações e exclusões entram sozinhas pelo banco.
/// </summary>
public sealed class ServicoAuditoria
{
    private readonly IDbContextFactory<EmissorDbContext> _fabrica;
    private readonly Sessao _sessao;

    /// <summary>Cria o serviço.</summary>
    /// <param name="fabrica">Fábrica de contextos do banco.</param>
    /// <param name="sessao">Usuário logado.</param>
    public ServicoAuditoria(IDbContextFactory<EmissorDbContext> fabrica, Sessao sessao)
    {
        _fabrica = fabrica;
        _sessao = sessao;
    }

    /// <summary>Registra um evento do usuário logado.</summary>
    /// <param name="acao">Evento.</param>
    /// <param name="empresaId">Empresa, quando houver.</param>
    /// <param name="descricao">Detalhe.</param>
    public async Task RegistrarAsync(AcaoAuditoria acao, int? empresaId, string descricao)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        await Auditoria.RegistrarAsync(contexto, _sessao, acao, empresaId, descricao);
    }

    /// <summary>Consulta a auditoria (só administrador).</summary>
    /// <param name="filtro">Filtro.</param>
    /// <exception cref="RegraNegocioException">Usuário sem acesso total.</exception>
    public async Task<IReadOnlyList<RegistroAuditoria>> ConsultarAsync(FiltroAuditoria filtro)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await Auditoria.ConsultarAsync(contexto, _sessao, filtro);
    }
}
