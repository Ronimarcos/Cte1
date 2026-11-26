namespace Emissor.Dominio.Enums;

/// <summary>
/// Situação do CT-e e do MDF-e no emissor. Autorizado entra com a transmissão (P4c2 e P4c3); cancelado e encerrado,
/// com o registro dos eventos (P4d2). Não há inutilizado: o CT-e 4.00 e o MDF-e 3.00 não têm mais esse serviço.
/// </summary>
public enum SituacaoDocumento
{
    /// <summary>Rascunho salvo; pode faltar informação.</summary>
    EmDigitacao = 0,

    /// <summary>Todas as etapas conferidas; aguarda a transmissão.</summary>
    ProntoParaTransmitir = 1,

    /// <summary>Autorizado pela SEFAZ, com chave e protocolo.</summary>
    Autorizado = 2,

    /// <summary>Cancelado pelo evento de cancelamento registrado.</summary>
    Cancelado = 3,

    /// <summary>MDF-e encerrado pelo evento de encerramento registrado.</summary>
    Encerrado = 4,
}
