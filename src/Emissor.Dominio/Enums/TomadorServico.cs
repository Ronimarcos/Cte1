namespace Emissor.Dominio.Enums;

/// <summary>
/// Tomador do serviço (tag <c>ide/toma3/toma</c> para 0 a 3 e <c>ide/toma4/toma</c> para 4).
/// Valores conforme <c>cteTiposBasico_v4.00.xsd</c> do pacote PL_CTe_400_NT2026.005.
/// </summary>
public enum TomadorServico
{
    /// <summary>0 - Remetente.</summary>
    Remetente = 0,

    /// <summary>1 - Expedidor.</summary>
    Expedidor = 1,

    /// <summary>2 - Recebedor.</summary>
    Recebedor = 2,

    /// <summary>3 - Destinatário.</summary>
    Destinatario = 3,

    /// <summary>4 - Outros (dados do tomador informados no grupo <c>toma4</c>).</summary>
    Outros = 4,
}
