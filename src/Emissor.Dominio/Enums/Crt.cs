namespace Emissor.Dominio.Enums;

/// <summary>
/// Código de Regime Tributário do emitente (tag <c>emit/CRT</c>).
/// Valores conforme <c>cteTiposBasico_v4.00.xsd</c> do pacote PL_CTe_400_NT2026.005.
/// </summary>
public enum Crt
{
    /// <summary>1 - Simples Nacional.</summary>
    SimplesNacional = 1,

    /// <summary>2 - Simples Nacional, excesso de sublimite de receita bruta.</summary>
    SimplesNacionalExcessoSublimite = 2,

    /// <summary>3 - Regime Normal.</summary>
    RegimeNormal = 3,

    /// <summary>4 - Simples Nacional, Microempreendedor Individual (MEI).</summary>
    SimplesNacionalMei = 4,
}
