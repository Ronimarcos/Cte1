namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo do serviço do CT-e (tag <c>ide/tpServ</c>).
/// Valores conforme <c>cteTiposBasico_v4.00.xsd</c> do pacote PL_CTe_400_NT2026.005.
/// O valor 4 (serviço vinculado a multimodal) não entra porque o multimodal está fora da v1 (§1.1).
/// </summary>
public enum TipoServicoCte
{
    /// <summary>0 - Normal.</summary>
    Normal = 0,

    /// <summary>1 - Subcontratação.</summary>
    Subcontratacao = 1,

    /// <summary>2 - Redespacho.</summary>
    Redespacho = 2,

    /// <summary>3 - Redespacho intermediário.</summary>
    RedespachoIntermediario = 3,
}
