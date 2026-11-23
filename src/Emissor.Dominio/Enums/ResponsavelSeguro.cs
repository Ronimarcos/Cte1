namespace Emissor.Dominio.Enums;

/// <summary>
/// Responsável pelo seguro da carga no MDF-e (tag <c>seg/infResp/respSeg</c>).
/// Valores conforme <c>mdfeTiposBasico_v3.00.xsd</c> do pacote PL_MDFe_300b_NT2025.001_1.05.
/// </summary>
public enum ResponsavelSeguro
{
    /// <summary>1 - Emitente do MDF-e.</summary>
    Emitente = 1,

    /// <summary>2 - Responsável pela contratação do serviço de transporte (contratante).</summary>
    Contratante = 2,
}
