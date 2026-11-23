namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo de carroceria do veículo (tag <c>tpCar</c>, gravada com 2 dígitos).
/// Valores conforme <c>mdfeModalRodoviario_v3.00.xsd</c> do pacote PL_MDFe_300b_NT2025.001_1.05.
/// </summary>
public enum TipoCarroceria
{
    /// <summary>00 - Não aplicável.</summary>
    NaoAplicavel = 0,

    /// <summary>01 - Aberta.</summary>
    Aberta = 1,

    /// <summary>02 - Fechada/baú.</summary>
    FechadaBau = 2,

    /// <summary>03 - Graneleira.</summary>
    Graneleira = 3,

    /// <summary>04 - Porta-contêiner.</summary>
    PortaConteiner = 4,

    /// <summary>05 - Sider.</summary>
    Sider = 5,
}
