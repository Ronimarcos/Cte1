namespace Emissor.Dominio.Enums;

/// <summary>
/// Indicador da Inscrição Estadual do participante (tag <c>ide/indIEToma</c> quando tomador).
/// Valores conforme <c>cteTiposBasico_v4.00.xsd</c> do pacote PL_CTe_400_NT2026.005.
/// </summary>
public enum IndicadorIe
{
    /// <summary>1 - Contribuinte do ICMS.</summary>
    Contribuinte = 1,

    /// <summary>2 - Contribuinte isento de inscrição.</summary>
    Isento = 2,

    /// <summary>9 - Não contribuinte.</summary>
    NaoContribuinte = 9,
}
