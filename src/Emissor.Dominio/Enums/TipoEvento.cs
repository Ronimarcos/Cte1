namespace Emissor.Dominio.Enums;

/// <summary>
/// Eventos tratados pelo emissor (P4d). O código do leiaute (<c>tpEvento</c>) é o mesmo no CT-e e no MDF-e para o
/// cancelamento (110111), por isso o tipo interno separa os dois documentos.
/// </summary>
public enum TipoEvento
{
    /// <summary>Cancelamento do CT-e (110111).</summary>
    CancelamentoCte = 1,

    /// <summary>Carta de correção do CT-e (110110).</summary>
    CartaCorrecaoCte = 2,

    /// <summary>Cancelamento do MDF-e (110111).</summary>
    CancelamentoMdfe = 3,

    /// <summary>Encerramento do MDF-e (110112).</summary>
    EncerramentoMdfe = 4,

    /// <summary>Inclusão de condutor no MDF-e (110114).</summary>
    InclusaoCondutorMdfe = 5,
}

/// <summary>Código do leiaute, descrição e modelo de cada tipo de evento.</summary>
public static class TiposEvento
{
    /// <summary>Código do leiaute (<c>tpEvento</c>), conforme a tabela de eventos do MOC.</summary>
    /// <param name="tipo">Tipo de evento.</param>
    public static string Codigo(this TipoEvento tipo) => tipo switch
    {
        TipoEvento.CancelamentoCte or TipoEvento.CancelamentoMdfe => "110111",
        TipoEvento.CartaCorrecaoCte => "110110",
        TipoEvento.EncerramentoMdfe => "110112",
        TipoEvento.InclusaoCondutorMdfe => "110114",
        _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de evento desconhecido."),
    };

    /// <summary>Descrição para as telas.</summary>
    /// <param name="tipo">Tipo de evento.</param>
    public static string Descricao(this TipoEvento tipo) => tipo switch
    {
        TipoEvento.CancelamentoCte => "Cancelamento do CT-e",
        TipoEvento.CartaCorrecaoCte => "Carta de correção",
        TipoEvento.CancelamentoMdfe => "Cancelamento do MDF-e",
        TipoEvento.EncerramentoMdfe => "Encerramento do MDF-e",
        TipoEvento.InclusaoCondutorMdfe => "Inclusão de condutor",
        _ => tipo.ToString(),
    };

    /// <summary>Modelo do documento a que o evento se refere.</summary>
    /// <param name="tipo">Tipo de evento.</param>
    public static ModeloDocumento Modelo(this TipoEvento tipo) =>
        tipo is TipoEvento.CancelamentoCte or TipoEvento.CartaCorrecaoCte ? ModeloDocumento.Cte : ModeloDocumento.Mdfe;

    /// <summary>Maior número sequencial aceito pela SEFAZ para o tipo (CC-e: 20; condutor: 99; demais: 1).</summary>
    /// <param name="tipo">Tipo de evento.</param>
    public static int SequenciaMaxima(this TipoEvento tipo) => tipo switch
    {
        TipoEvento.CartaCorrecaoCte => 20,
        TipoEvento.InclusaoCondutorMdfe => 99,
        _ => 1,
    };
}
