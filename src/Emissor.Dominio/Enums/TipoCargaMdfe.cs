namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo de carga do produto predominante (tag <c>prodPred/tpCarga</c>, gravada com 2 dígitos),
/// conforme a Resolução ANTT nº 5.849/2019 citada em <c>mdfeTiposBasico_v3.00.xsd</c> (PL_MDFe_300b_NT2025.001_1.05).
/// </summary>
public enum TipoCargaMdfe
{
    /// <summary>01 - Granel sólido.</summary>
    GranelSolido = 1,

    /// <summary>02 - Granel líquido.</summary>
    GranelLiquido = 2,

    /// <summary>03 - Frigorificada.</summary>
    Frigorificada = 3,

    /// <summary>04 - Conteinerizada.</summary>
    Conteinerizada = 4,

    /// <summary>05 - Carga geral.</summary>
    CargaGeral = 5,

    /// <summary>06 - Neogranel.</summary>
    Neogranel = 6,

    /// <summary>07 - Perigosa (granel sólido).</summary>
    PerigosaGranelSolido = 7,

    /// <summary>08 - Perigosa (granel líquido).</summary>
    PerigosaGranelLiquido = 8,

    /// <summary>09 - Perigosa (carga frigorificada).</summary>
    PerigosaFrigorificada = 9,

    /// <summary>10 - Perigosa (conteinerizada).</summary>
    PerigosaConteinerizada = 10,

    /// <summary>11 - Perigosa (carga geral).</summary>
    PerigosaCargaGeral = 11,

    /// <summary>12 - Granel pressurizada.</summary>
    GranelPressurizada = 12,
}
