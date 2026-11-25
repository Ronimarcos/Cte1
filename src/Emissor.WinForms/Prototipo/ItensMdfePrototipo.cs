namespace Emissor.WinForms.Prototipo;

/// <summary>Situação do MDF-e no protótipo, que não transmite (sem autorizado, encerrado ou cancelado).</summary>
public enum SituacaoMdfe
{
    /// <summary>Rascunho salvo; pode faltar informação.</summary>
    EmDigitacao,

    /// <summary>Todas as etapas conferidas; aguarda a transmissão (ainda não implementada).</summary>
    ProntoParaTransmitir,
}

/// <summary>Tipo do documento vinculado a um município de descarga.</summary>
public enum TipoDocumentoMdfe
{
    /// <summary>CT-e (<c>infMunDescarga/infCTe</c>).</summary>
    Cte,

    /// <summary>NF-e (<c>infMunDescarga/infNFe</c>), usada por quem transporta carga própria.</summary>
    Nfe,
}

/// <summary>Documento transportado, vinculado a um município de descarga.</summary>
public sealed class DocumentoMdfePrototipo
{
    /// <summary>Tipo do documento.</summary>
    public TipoDocumentoMdfe Tipo { get; set; } = TipoDocumentoMdfe.Cte;

    /// <summary>Chave de acesso com 44 dígitos (<c>chCTe</c> ou <c>chNFe</c>).</summary>
    public string Chave { get; set; } = string.Empty;

    /// <summary>
    /// CT-e emitido por este sistema, quando o documento foi escolhido na lista de CT-e. No protótipo esses CT-e
    /// não têm chave (não são transmitidos); na versão real só CT-e autorizados poderão ser vinculados.
    /// </summary>
    public int? CteId { get; set; }

    /// <summary>Texto da chave para exibição.</summary>
    public string ChaveDescricao => CteId is null ? Chave : $"nº {CteId} emitido neste sistema (chave após a autorização)";

    /// <summary>Cópia independente.</summary>
    public DocumentoMdfePrototipo Copiar() => (DocumentoMdfePrototipo)MemberwiseClone();
}

/// <summary>Município de descarga (<c>infDoc/infMunDescarga</c>) com seus documentos.</summary>
public sealed class DescargaMdfePrototipo
{
    /// <summary>Código IBGE do município (<c>cMunDescarga</c>).</summary>
    public int MunicipioCodigo { get; set; }

    /// <summary>Documentos descarregados neste município.</summary>
    public List<DocumentoMdfePrototipo> Documentos { get; set; } = [];

    /// <summary>Cópia independente, incluindo os documentos.</summary>
    public DescargaMdfePrototipo Copiar() =>
        new() { MunicipioCodigo = MunicipioCodigo, Documentos = Documentos.Select(d => d.Copiar()).ToList() };
}

/// <summary>CIOT da viagem (<c>infANTT/infCIOT</c>).</summary>
public sealed class CiotPrototipo
{
    /// <summary>Código do CIOT com 12 dígitos (tipo <c>TCIOT</c>).</summary>
    public string Ciot { get; set; } = string.Empty;

    /// <summary>CPF ou CNPJ do responsável pela geração do CIOT.</summary>
    public string CpfCnpjResponsavel { get; set; } = string.Empty;

    /// <summary>Cópia independente.</summary>
    public CiotPrototipo Copiar() => (CiotPrototipo)MemberwiseClone();
}
