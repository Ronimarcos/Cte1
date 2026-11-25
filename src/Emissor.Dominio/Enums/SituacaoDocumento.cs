namespace Emissor.Dominio.Enums;

/// <summary>
/// Situação do CT-e e do MDF-e no emissor. Por enquanto só as situações anteriores à transmissão; autorizado,
/// rejeitado, cancelado, encerrado e inutilizado entram com a transmissão e os eventos (P4c e P4d).
/// </summary>
public enum SituacaoDocumento
{
    /// <summary>Rascunho salvo; pode faltar informação.</summary>
    EmDigitacao = 0,

    /// <summary>Todas as etapas conferidas; aguarda a transmissão.</summary>
    ProntoParaTransmitir = 1,
}
