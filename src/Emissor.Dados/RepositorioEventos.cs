using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Eventos dos documentos, sempre pela alçada do usuário (operador só vê e grava eventos das empresas liberadas).
/// Eventos não são excluídos: o histórico fiscal fica no banco, inclusive o dos rejeitados.
/// </summary>
public sealed class RepositorioEventos : RepositorioCadastro<EventoDocumento>
{
    /// <summary>Cria o repositório aplicando a alçada do usuário.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    /// <param name="acesso">Quem acessa e em quais empresas.</param>
    public RepositorioEventos(EmissorDbContext contexto, IContextoAcesso acesso)
        : base(contexto, acesso)
    {
    }

    /// <summary>Eventos do documento, do mais antigo para o mais recente.</summary>
    /// <param name="modelo">CT-e ou MDF-e.</param>
    /// <param name="documentoId">Código interno do documento.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public async Task<IReadOnlyList<EventoDocumento>> ListarDoDocumentoAsync(ModeloDocumento modelo, int documentoId,
        CancellationToken cancelamento = default)
    {
        var tipos = Enum.GetValues<TipoEvento>().Where(t => t.Modelo() == modelo).ToList();
        return await Consulta().AsNoTracking()
            .Where(e => e.DocumentoId == documentoId && tipos.Contains(e.Tipo))
            .OrderBy(e => e.DataHora).ThenBy(e => e.Id)
            .ToListAsync(cancelamento);
    }

    /// <summary>Próximo número sequencial do tipo de evento para o documento (rejeitados não contam).</summary>
    /// <param name="tipo">Tipo do evento.</param>
    /// <param name="documentoId">Código interno do documento.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public async Task<int> ProximaSequenciaAsync(TipoEvento tipo, int documentoId, CancellationToken cancelamento = default) =>
        RegrasEvento.ProximaSequencia(tipo, await ListarDoDocumentoAsync(tipo.Modelo(), documentoId, cancelamento));

    /// <summary>Eventos não são excluídos.</summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <exception cref="RegraNegocioException">Sempre.</exception>
    public override Task ExcluirAsync(int id, CancellationToken cancelamento = default) =>
        throw new RegraNegocioException("Eventos não podem ser excluídos: fazem parte do histórico fiscal do documento.");
}
