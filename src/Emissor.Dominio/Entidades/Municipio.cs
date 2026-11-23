namespace Emissor.Dominio.Entidades;

/// <summary>
/// Município (tabela global, sem empresa — §5.9). A chave é o código IBGE de 7 dígitos (<c>TCodMunIBGE</c>);
/// a carga vem da tabela oficial do IBGE (fatia P2 da persistência).
/// </summary>
public class Municipio
{
    /// <summary>Código IBGE (7 dígitos), usado em <c>cMun</c>, <c>cMunIni</c>, <c>cMunCarrega</c> etc.</summary>
    public int Codigo { get; set; }

    /// <summary>Nome do município (<c>xMun</c>).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Sigla da UF.</summary>
    public string Uf { get; set; } = string.Empty;

    /// <summary>Texto de exibição no formato "Nome/UF".</summary>
    public string Descricao => $"{Nome}/{Uf}";
}
