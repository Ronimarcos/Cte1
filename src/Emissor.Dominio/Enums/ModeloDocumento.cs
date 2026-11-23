namespace Emissor.Dominio.Enums;

/// <summary>
/// Modelo do documento fiscal (tag <c>ide/mod</c>).
/// Valores conforme <c>TModCT</c> de <c>tiposGeralCTe_v4.00.xsd</c> (PL_CTe_400_NT2026.005)
/// e <c>TModMD</c> de <c>tiposGeralMDFe_v3.00.xsd</c> (PL_MDFe_300b_NT2025.001_1.05).
/// </summary>
public enum ModeloDocumento
{
    /// <summary>57 - Conhecimento de Transporte eletrônico (CT-e).</summary>
    Cte = 57,

    /// <summary>58 - Manifesto Eletrônico de Documentos Fiscais (MDF-e).</summary>
    Mdfe = 58,
}
