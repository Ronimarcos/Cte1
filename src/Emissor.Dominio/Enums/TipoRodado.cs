namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo de rodado do veículo de tração (tag <c>veicTracao/tpRod</c>, gravada com 2 dígitos).
/// Valores conforme <c>mdfeModalRodoviario_v3.00.xsd</c> do pacote PL_MDFe_300b_NT2025.001_1.05.
/// </summary>
public enum TipoRodado
{
    /// <summary>01 - Truck.</summary>
    Truck = 1,

    /// <summary>02 - Toco.</summary>
    Toco = 2,

    /// <summary>03 - Cavalo mecânico.</summary>
    CavaloMecanico = 3,

    /// <summary>04 - Van.</summary>
    Van = 4,

    /// <summary>05 - Utilitário.</summary>
    Utilitario = 5,

    /// <summary>06 - Outros.</summary>
    Outros = 6,
}
