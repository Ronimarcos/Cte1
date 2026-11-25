using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.WinForms.Prototipo;
using Microsoft.EntityFrameworkCore;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Municípios do IBGE lidos do banco uma vez, na abertura, e mantidos em memória para as listas de escolha
/// e para mostrar "Nome/UF" a partir do código. A tabela é global e não muda durante o uso.
/// </summary>
public sealed class CatalogoMunicipios
{
    private readonly IDbContextFactory<EmissorDbContext> _fabrica;
    private Dictionary<int, Municipio> _porCodigo = [];
    private IReadOnlyList<Opcao<int>> _opcoes = [];

    /// <summary>Cria o catálogo sobre a fábrica de contextos do banco.</summary>
    /// <param name="fabrica">Fábrica de contextos.</param>
    public CatalogoMunicipios(IDbContextFactory<EmissorDbContext> fabrica) => _fabrica = fabrica;

    /// <summary>Quantidade de municípios carregados.</summary>
    public int Quantidade => _porCodigo.Count;

    /// <summary>Lista de escolha "Nome/UF", em ordem alfabética.</summary>
    public IReadOnlyList<Opcao<int>> Opcoes => _opcoes;

    /// <summary>Lê os municípios do banco.</summary>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public async Task CarregarAsync(CancellationToken cancelamento = default)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync(cancelamento);
        var municipios = await contexto.Municipios.AsNoTracking().ToListAsync(cancelamento);
        _porCodigo = municipios.ToDictionary(m => m.Codigo);
        _opcoes = municipios
            .OrderBy(m => m.Nome, StringComparer.CurrentCulture)
            .ThenBy(m => m.Uf)
            .Select(m => new Opcao<int>(m.Codigo, m.Descricao))
            .ToList();
    }

    /// <summary>Municípios de uma UF como opções "Nome/UF", em ordem alfabética.</summary>
    /// <param name="uf">Sigla da UF.</param>
    public IEnumerable<Opcao<int>> OpcoesDaUf(string uf) =>
        _opcoes.Where(o => _porCodigo.TryGetValue(o.Valor, out var municipio) && municipio.Uf == uf);

    /// <summary>Município do código, ou <c>null</c>.</summary>
    /// <param name="codigo">Código IBGE.</param>
    public Municipio? Obter(int? codigo) => codigo is { } valor && _porCodigo.TryGetValue(valor, out var municipio) ? municipio : null;

    /// <summary>Texto "Nome/UF" do código, ou vazio.</summary>
    /// <param name="codigo">Código IBGE.</param>
    public string Descricao(int? codigo) => Obter(codigo)?.Descricao ?? string.Empty;
}
