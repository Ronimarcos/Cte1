namespace Emissor.Dominio.Enums;

/// <summary>
/// Unidade de medida da quantidade de carga (tag <c>infCarga/infQ/cUnid</c>, gravada com 2 dígitos).
/// Valores conforme <c>cteTiposBasico_v4.00.xsd</c> do pacote PL_CTe_400_NT2026.005.
/// </summary>
public enum UnidadeMedidaCarga
{
    /// <summary>00 - M3.</summary>
    MetroCubico = 0,

    /// <summary>01 - KG.</summary>
    Quilograma = 1,

    /// <summary>02 - TON.</summary>
    Tonelada = 2,

    /// <summary>03 - UNIDADE.</summary>
    Unidade = 3,

    /// <summary>04 - LITROS.</summary>
    Litros = 4,

    /// <summary>05 - MMBTU.</summary>
    Mmbtu = 5,
}
