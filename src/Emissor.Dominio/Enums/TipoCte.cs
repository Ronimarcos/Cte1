namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo do CT-e (tag <c>ide/tpCTe</c>, tipo <c>TFinCTe</c>).
/// Valores conforme <c>cteTiposBasico_v4.00.xsd</c> do pacote PL_CTe_400_NT2026.005.
/// </summary>
public enum TipoCte
{
    /// <summary>0 - CT-e Normal.</summary>
    Normal = 0,

    /// <summary>1 - CT-e de Complemento de Valores.</summary>
    Complemento = 1,

    /// <summary>3 - CT-e de Substituição.</summary>
    Substituicao = 3,
}
