namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo do emitente do MDF-e (tag <c>ide/tpEmit</c>, tipo <c>TEmit</c>).
/// Valores conforme <c>mdfeTiposBasico_v3.00.xsd</c> do pacote PL_MDFe_300b_NT2025.001_1.05.
/// </summary>
public enum TipoEmitenteMdfe
{
    /// <summary>1 - Prestador de serviço de transporte.</summary>
    PrestadorServico = 1,

    /// <summary>2 - Transportador de carga própria.</summary>
    CargaPropria = 2,

    /// <summary>3 - Prestador de serviço de transporte que emitirá CT-e globalizado.</summary>
    CteGlobalizado = 3,
}
