namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo do transportador no MDF-e (tag <c>ide/tpTransp</c>, tipo <c>TTransp</c>, opcional).
/// Valores conforme <c>mdfeTiposBasico_v3.00.xsd</c> do pacote PL_MDFe_300b_NT2025.001_1.05.
/// </summary>
public enum TipoTransportador
{
    /// <summary>1 - ETC (Empresa de Transporte Rodoviário de Cargas).</summary>
    Etc = 1,

    /// <summary>2 - TAC (Transportador Autônomo de Cargas).</summary>
    Tac = 2,

    /// <summary>3 - CTC (Cooperativa de Transporte Rodoviário de Cargas).</summary>
    Ctc = 3,
}
