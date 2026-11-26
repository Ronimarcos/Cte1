using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Schema;

namespace Emissor.DFe.Validacao;

/// <summary>
/// Validação dos XML contra os XSD oficiais copiados para a pasta <c>schemas</c> da saída (sem edição).
/// Os conjuntos de schemas são carregados uma vez por arquivo principal.
/// </summary>
public static class ValidadorXsd
{
    /// <summary>Schema principal do CT-e 4.00 (NT 2026.005).</summary>
    public const string SchemaCte = @"schemas\cte\PL_CTe_400_NT2026.005\cte_v4.00.xsd";

    /// <summary>Schema do modal rodoviário do CT-e (validado à parte: <c>infModal</c> aceita qualquer conteúdo).</summary>
    public const string SchemaCteRodoviario = @"schemas\cte\PL_CTe_400_NT2026.005\cteModalRodoviario_v4.00.xsd";

    /// <summary>Schema principal do MDF-e 3.00b.</summary>
    public const string SchemaMdfe = @"schemas\mdfe\PL_MDFe_300b_NT2025.001_1.05\mdfe_v3.00.xsd";

    /// <summary>Schema do modal rodoviário do MDF-e (validado à parte: <c>infModal</c> aceita qualquer conteúdo).</summary>
    public const string SchemaMdfeRodoviario = @"schemas\mdfe\PL_MDFe_300b_NT2025.001_1.05\mdfeModalRodoviario_v3.00.xsd";

    /// <summary>Consulta de status do serviço do CT-e.</summary>
    public const string SchemaConsStatServCte = @"schemas\cte\PL_CTe_400_NT2026.005\consStatServCTe_v4.00.xsd";

    /// <summary>Consulta de status do serviço do MDF-e.</summary>
    public const string SchemaConsStatServMdfe = @"schemas\mdfe\PL_MDFe_300b_NT2025.001_1.05\consStatServMDFe_v3.00.xsd";

    /// <summary>Envelope do evento do CT-e (o detalhe em <c>detEvento</c> é validado à parte).</summary>
    public const string SchemaEventoCte = @"schemas\cte\PL_CTe_400_NT2026.005\eventoCTe_v4.00.xsd";

    /// <summary>Detalhe do cancelamento do CT-e.</summary>
    public const string SchemaEvCancCte = @"schemas\cte\PL_CTe_400_NT2026.005\evCancCTe_v4.00.xsd";

    /// <summary>Detalhe da carta de correção do CT-e.</summary>
    public const string SchemaEvCceCte = @"schemas\cte\PL_CTe_400_NT2026.005\evCCeCTe_v4.00.xsd";

    /// <summary>Envelope do evento do MDF-e (o detalhe em <c>detEvento</c> é validado à parte).</summary>
    public const string SchemaEventoMdfe = @"schemas\mdfe\PL_MDFe_300b_NT2025.001_1.05\eventoMDFe_v3.00.xsd";

    /// <summary>Detalhe do cancelamento do MDF-e.</summary>
    public const string SchemaEvCancMdfe = @"schemas\mdfe\PL_MDFe_300b_NT2025.001_1.05\evCancMDFe_v3.00.xsd";

    /// <summary>Detalhe do encerramento do MDF-e.</summary>
    public const string SchemaEvEncMdfe = @"schemas\mdfe\PL_MDFe_300b_NT2025.001_1.05\evEncMDFe_v3.00.xsd";

    /// <summary>Detalhe da inclusão de condutor no MDF-e.</summary>
    public const string SchemaEvIncCondutorMdfe = @"schemas\mdfe\PL_MDFe_300b_NT2025.001_1.05\evIncCondutorMDFe_v3.00.xsd";

    private static readonly ConcurrentDictionary<string, XmlSchemaSet> Conjuntos = new();

    /// <summary>Valida o elemento contra o schema e devolve os erros com o caminho do campo; vazio se válido.</summary>
    /// <param name="elemento">Elemento raiz a validar (ex.: <c>CTe</c> ou <c>rodo</c>).</param>
    /// <param name="schema">Caminho do schema principal, relativo à pasta do aplicativo.</param>
    public static IReadOnlyList<string> Validar(XmlElement elemento, string schema)
    {
        var documento = new XmlDocument { PreserveWhitespace = true, Schemas = Obter(schema) };
        documento.AppendChild(documento.ImportNode(elemento, deep: true));

        var erros = new List<string>();
        documento.Validate((_, argumentos) =>
        {
            var caminho = argumentos.Exception is XmlSchemaValidationException { SourceObject: XmlNode no } ? Caminho(no) : string.Empty;
            erros.Add(caminho.Length > 0 ? $"{caminho}: {argumentos.Message}" : argumentos.Message);
        });
        return erros;
    }

    private static XmlSchemaSet Obter(string schema) => Conjuntos.GetOrAdd(schema, caminho =>
    {
        var completo = Path.Combine(AppContext.BaseDirectory, caminho);
        if (!File.Exists(completo))
        {
            throw new FileNotFoundException("Schema do leiaute não encontrado na pasta do aplicativo.", completo);
        }

        var conjunto = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        using (var leitor = XmlReader.Create(completo, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse }))
        {
            conjunto.Add(null, leitor);
        }

        conjunto.Compile();
        return conjunto;
    });

    /// <summary>Caminho do nó para a mensagem, ex.: <c>CTe/infCte/ide/CFOP</c>.</summary>
    private static string Caminho(XmlNode no)
    {
        var partes = new Stack<string>();
        for (var atual = no; atual is not null and not XmlDocument; atual = atual is XmlAttribute atributo ? atributo.OwnerElement : atual.ParentNode)
        {
            partes.Push(atual is XmlAttribute ? "@" + atual.LocalName : atual.LocalName);
        }

        return string.Join("/", partes);
    }
}
