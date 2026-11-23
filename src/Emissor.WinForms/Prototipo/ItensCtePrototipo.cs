using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>Quantidade da carga (grupo <c>infCarga/infQ</c>). Propriedades com setter público para edição em grade.</summary>
public sealed class QuantidadeCargaPrototipo
{
    /// <summary>Unidade de medida (<c>cUnid</c>).</summary>
    public UnidadeMedidaCarga Unidade { get; set; } = UnidadeMedidaCarga.Quilograma;

    /// <summary>Tipo da medida (<c>tpMed</c>), ex.: "PESO BRUTO".</summary>
    public string TipoMedida { get; set; } = string.Empty;

    /// <summary>Quantidade (<c>qCarga</c>).</summary>
    public decimal Quantidade { get; set; }

    /// <summary>Cópia independente da linha.</summary>
    public QuantidadeCargaPrototipo Copiar() => (QuantidadeCargaPrototipo)MemberwiseClone();
}

/// <summary>NF-e transportada (grupo <c>infDoc/infNFe</c>).</summary>
public sealed class DocumentoNfePrototipo
{
    /// <summary>Chave de acesso da NF-e com 44 dígitos (<c>chave</c>).</summary>
    public string Chave { get; set; } = string.Empty;

    /// <summary>Cópia independente da linha.</summary>
    public DocumentoNfePrototipo Copiar() => (DocumentoNfePrototipo)MemberwiseClone();
}

/// <summary>
/// Documento de transporte anterior (grupo <c>docAnt/emiDocAnt</c>), usado em subcontratação e redespacho.
/// O protótipo considera só o CT-e eletrônico do emitente anterior.
/// </summary>
public sealed class DocumentoAnteriorPrototipo
{
    /// <summary>CNPJ ou CPF do emitente do documento anterior.</summary>
    public string CnpjCpfEmitente { get; set; } = string.Empty;

    /// <summary>Razão social ou nome do emitente (<c>xNome</c>).</summary>
    public string NomeEmitente { get; set; } = string.Empty;

    /// <summary>Chave do CT-e anterior com 44 dígitos (<c>idDocAntEle/chCTe</c>).</summary>
    public string ChaveCte { get; set; } = string.Empty;

    /// <summary>Cópia independente da linha.</summary>
    public DocumentoAnteriorPrototipo Copiar() => (DocumentoAnteriorPrototipo)MemberwiseClone();
}

/// <summary>Componente do valor da prestação (grupo <c>vPrest/Comp</c>).</summary>
public sealed class ComponenteFretePrototipo
{
    /// <summary>Nome do componente (<c>xNome</c>, até 15 caracteres), ex.: "FRETE PESO".</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Valor do componente (<c>vComp</c>).</summary>
    public decimal Valor { get; set; }

    /// <summary>Cópia independente da linha.</summary>
    public ComponenteFretePrototipo Copiar() => (ComponenteFretePrototipo)MemberwiseClone();
}
