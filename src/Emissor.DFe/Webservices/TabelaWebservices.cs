using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Webservices;

/// <summary>Serviços web usados pelo emissor.</summary>
public enum ServicoSefaz
{
    /// <summary>Status do serviço do CT-e (<c>CTeStatusServicoV4</c>).</summary>
    CteStatusServico,

    /// <summary>Recepção síncrona do CT-e (<c>CTeRecepcaoSincV4</c>).</summary>
    CteRecepcaoSinc,

    /// <summary>Consulta do CT-e (<c>CTeConsultaV4</c>).</summary>
    CteConsulta,

    /// <summary>Eventos do CT-e (<c>CTeRecepcaoEventoV4</c>).</summary>
    CteRecepcaoEvento,

    /// <summary>Página do QR Code do CT-e.</summary>
    CteQrCode,

    /// <summary>Status do serviço do MDF-e (<c>MDFeStatusServico</c>).</summary>
    MdfeStatusServico,

    /// <summary>Recepção síncrona do MDF-e (<c>MDFeRecepcaoSinc</c>).</summary>
    MdfeRecepcaoSinc,

    /// <summary>Consulta do MDF-e (<c>MDFeConsulta</c>).</summary>
    MdfeConsulta,

    /// <summary>Eventos do MDF-e (<c>MDFeRecepcaoEvento</c>).</summary>
    MdfeRecepcaoEvento,

    /// <summary>Consulta de MDF-e não encerrados (<c>MDFeConsNaoEnc</c>).</summary>
    MdfeConsultaNaoEncerrados,

    /// <summary>Página do QR Code do MDF-e.</summary>
    MdfeQrCode,
}

/// <summary>
/// Endereços dos webservices de homologação conferidos nas tabelas oficiais em 25/11/2026 (§8.3 do levantamento):
/// CT-e de MT no autorizador próprio da SEFAZ-MT; MDF-e na SVRS (Ambiente Nacional). Produção não é atendida: o
/// emissor só transmite em homologação até haver confirmação explícita do responsável (CLAUDE.md).
/// </summary>
public static class TabelaWebservices
{
    private static readonly IReadOnlyDictionary<(string Uf, ServicoSefaz Servico), string> Homologacao =
        new Dictionary<(string, ServicoSefaz), string>
        {
            [("MT", ServicoSefaz.CteStatusServico)] = "https://homologacao.sefaz.mt.gov.br/ctews2/services/CTeStatusServicoV4",
            [("MT", ServicoSefaz.CteRecepcaoSinc)] = "https://homologacao.sefaz.mt.gov.br/ctews2/services/CTeRecepcaoSincV4",
            [("MT", ServicoSefaz.CteConsulta)] = "https://homologacao.sefaz.mt.gov.br/ctews2/services/CTeConsultaV4",
            [("MT", ServicoSefaz.CteRecepcaoEvento)] = "https://homologacao.sefaz.mt.gov.br/ctews2/services/CTeRecepcaoEventoV4",
            [("MT", ServicoSefaz.CteQrCode)] = "https://homologacao.sefaz.mt.gov.br/cte/qrcode",
        };

    /// <summary>MDF-e: um autorizador para todas as UF (SVRS, Ambiente Nacional).</summary>
    private static readonly IReadOnlyDictionary<ServicoSefaz, string> MdfeHomologacao = new Dictionary<ServicoSefaz, string>
    {
        [ServicoSefaz.MdfeStatusServico] = "https://mdfe-homologacao.svrs.rs.gov.br/ws/MDFeStatusServico/MDFeStatusServico.asmx",
        [ServicoSefaz.MdfeRecepcaoSinc] = "https://mdfe-homologacao.svrs.rs.gov.br/ws/MDFeRecepcaoSinc/MDFeRecepcaoSinc.asmx",
        [ServicoSefaz.MdfeConsulta] = "https://mdfe-homologacao.svrs.rs.gov.br/ws/MDFeConsulta/MDFeConsulta.asmx",
        [ServicoSefaz.MdfeRecepcaoEvento] = "https://mdfe-homologacao.svrs.rs.gov.br/ws/MDFeRecepcaoEvento/MDFeRecepcaoEvento.asmx",
        [ServicoSefaz.MdfeConsultaNaoEncerrados] = "https://mdfe-homologacao.svrs.rs.gov.br/ws/MDFeConsNaoEnc/MDFeConsNaoEnc.asmx",
        [ServicoSefaz.MdfeQrCode] = "https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode",
    };

    /// <summary>Endereço do serviço para a UF do emitente.</summary>
    /// <param name="uf">UF do emitente (o MDF-e ignora: autorizador único).</param>
    /// <param name="servico">Serviço.</param>
    /// <param name="ambiente">Ambiente; produção é recusada.</param>
    /// <exception cref="RegraNegocioException">Produção, ou UF/serviço sem endereço conferido.</exception>
    public static Uri Endereco(string uf, ServicoSefaz servico, TipoAmbiente ambiente)
    {
        if (ambiente != TipoAmbiente.Homologacao)
        {
            throw new RegraNegocioException(
                "A transmissão em produção não está liberada neste emissor. Use o ambiente de homologação.");
        }

        if (MdfeHomologacao.TryGetValue(servico, out var mdfe))
        {
            return new Uri(mdfe);
        }

        return Homologacao.TryGetValue((uf, servico), out var cte)
            ? new Uri(cte)
            : throw new RegraNegocioException(
                $"Não há endereço de homologação conferido para o serviço {servico} da UF {uf}. Confira a tabela oficial do portal.");
    }
}
