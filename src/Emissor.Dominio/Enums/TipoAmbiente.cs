namespace Emissor.Dominio.Enums;

/// <summary>
/// Ambiente de emissão (tag <c>ide/tpAmb</c>), igual no CT-e e no MDF-e.
/// Valores conforme <c>tiposGeralCTe_v4.00.xsd</c> (PL_CTe_400_NT2026.005) e <c>tiposGeralMDFe_v3.00.xsd</c>.
/// O padrão de desenvolvimento e testes é sempre homologação.
/// </summary>
public enum TipoAmbiente
{
    /// <summary>1 - Produção (documentos com validade jurídica).</summary>
    Producao = 1,

    /// <summary>2 - Homologação (testes, sem validade jurídica).</summary>
    Homologacao = 2,
}
