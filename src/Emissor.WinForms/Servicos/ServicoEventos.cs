using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Eventos gravados dos documentos, pela alçada do usuário logado. Na P4d1 os eventos só geram o XML de teste (não
/// são gravados); a gravação entra com a transmissão (P4d2). A sequência já considera o que estiver gravado.
/// </summary>
public sealed class ServicoEventos
{
    private readonly IDbContextFactory<EmissorDbContext> _fabrica;
    private readonly Sessao _sessao;

    /// <summary>Cria o serviço sobre a fábrica de contextos.</summary>
    /// <param name="fabrica">Fábrica de contextos do banco.</param>
    /// <param name="sessao">Usuário logado.</param>
    public ServicoEventos(IDbContextFactory<EmissorDbContext> fabrica, Sessao sessao)
    {
        _fabrica = fabrica;
        _sessao = sessao;
    }

    /// <summary>Eventos gravados do documento.</summary>
    /// <param name="modelo">CT-e ou MDF-e.</param>
    /// <param name="documentoId">Código interno do documento.</param>
    public async Task<IReadOnlyList<EventoDocumento>> ListarAsync(ModeloDocumento modelo, int documentoId)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await new RepositorioEventos(contexto, _sessao).ListarDoDocumentoAsync(modelo, documentoId);
    }

    /// <summary>Próximo número sequencial do tipo de evento para o documento.</summary>
    /// <param name="tipo">Tipo do evento.</param>
    /// <param name="documentoId">Código interno do documento.</param>
    public async Task<int> ProximaSequenciaAsync(TipoEvento tipo, int documentoId)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await new RepositorioEventos(contexto, _sessao).ProximaSequenciaAsync(tipo, documentoId);
    }
}
