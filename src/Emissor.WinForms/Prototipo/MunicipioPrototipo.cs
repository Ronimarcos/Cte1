namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Município da lista reduzida do protótipo. A tabela completa virá da carga oficial do IBGE (§5.9).
/// </summary>
/// <param name="Codigo">Código IBGE (7 dígitos).</param>
/// <param name="Nome">Nome do município.</param>
/// <param name="Uf">Sigla da UF.</param>
public sealed record MunicipioPrototipo(int Codigo, string Nome, string Uf)
{
    /// <summary>Texto de exibição no formato "Nome/UF".</summary>
    public string Descricao => $"{Nome}/{Uf}";
}
