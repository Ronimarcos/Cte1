namespace Emissor.Dominio.Enums;

/// <summary>Tipo do documento vinculado a um município de descarga do MDF-e.</summary>
public enum TipoDocumentoMdfe
{
    /// <summary>CT-e (<c>infMunDescarga/infCTe</c>).</summary>
    Cte = 0,

    /// <summary>NF-e (<c>infMunDescarga/infNFe</c>), usada por quem transporta carga própria.</summary>
    Nfe = 1,
}
