namespace Emissor.Dominio.Enums;

/// <summary>
/// Unidade do peso bruto da carga no MDF-e (tag <c>tot/cUnid</c>, gravada com 2 dígitos).
/// Valores conforme <c>mdfeTiposBasico_v3.00.xsd</c> do pacote PL_MDFe_300b_NT2025.001_1.05.
/// </summary>
public enum UnidadePesoMdfe
{
    /// <summary>01 - KG.</summary>
    Quilograma = 1,

    /// <summary>02 - TON.</summary>
    Tonelada = 2,
}
