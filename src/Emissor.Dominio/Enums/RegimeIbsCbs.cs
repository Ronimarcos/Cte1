namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo de regime de apuração do IBS e da CBS (tag <c>emit/tpRegimeIBSCBS</c>).
/// Valores conforme <c>cteTiposBasico_v4.00.xsd</c> do pacote PL_CTe_400_NT2026.005.
/// </summary>
public enum RegimeIbsCbs
{
    /// <summary>1 - Simples Nacional (CRT 1 ou 2) com opção pelo regime regular do IBS e da CBS.</summary>
    SimplesNacionalComRegimeRegular = 1,

    /// <summary>2 - Regime Normal (CRT 3) com opção pendente pelo CRT 1, sem regime regular do IBS e da CBS.</summary>
    NormalOpcaoPendenteSimplesNacional = 2,

    /// <summary>3 - Regime Normal (CRT 3) com opção pendente pelo CRT 2, sem regime regular do IBS e da CBS.</summary>
    NormalOpcaoPendenteSimplesNacionalExcesso = 3,

    /// <summary>4 - Regime Normal (CRT 3) com opção pendente pelo Simples Nacional (CRT 1 ou 2), com regime regular do IBS e da CBS.</summary>
    NormalOpcaoPendenteComRegimeRegular = 4,
}
