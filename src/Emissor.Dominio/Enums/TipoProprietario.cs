namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo do proprietário ou possuidor do veículo de terceiro (tag <c>prop/tpProp</c>).
/// Valores conforme <c>mdfeModalRodoviario_v3.00.xsd</c> do pacote PL_MDFe_300b_NT2025.001_1.05.
/// </summary>
public enum TipoProprietario
{
    /// <summary>0 - TAC agregado.</summary>
    TacAgregado = 0,

    /// <summary>1 - TAC independente.</summary>
    TacIndependente = 1,

    /// <summary>2 - Outros.</summary>
    Outros = 2,
}
