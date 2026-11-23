namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Lista em memória com inclusão, alteração e exclusão, usada pelo protótipo no lugar do banco de dados.
/// Os dados se perdem ao fechar o sistema.
/// </summary>
/// <typeparam name="T">Tipo de registro armazenado.</typeparam>
public sealed class RepositorioMemoria<T> where T : RegistroPrototipo
{
    private readonly List<T> _itens = [];
    private int _ultimoId;

    /// <summary>Cria o repositório já com os registros iniciais (o <c>Id</c> informado é mantido).</summary>
    /// <param name="iniciais">Registros fictícios de partida.</param>
    public RepositorioMemoria(IEnumerable<T>? iniciais = null)
    {
        foreach (var item in iniciais ?? [])
        {
            Incluir(item);
        }
    }

    /// <summary>Lista os registros que atendem ao filtro, na ordem de inclusão. Os itens retornados são só para exibição.</summary>
    /// <param name="filtro">Condição opcional.</param>
    public IReadOnlyList<T> Listar(Func<T, bool>? filtro = null) =>
        (filtro is null ? _itens : _itens.Where(filtro)).ToList();

    /// <summary>Retorna uma cópia do registro, pronta para edição, ou <c>null</c> se não existir.</summary>
    /// <param name="id">Identificador do registro.</param>
    public T? ObterPorId(int id)
    {
        var item = _itens.FirstOrDefault(i => i.Id == id);
        return item is null ? null : (T)item.Clonar();
    }

    /// <summary>Indica se algum registro atende à condição.</summary>
    /// <param name="condicao">Condição a verificar.</param>
    public bool Existe(Func<T, bool> condicao) => _itens.Any(condicao);

    /// <summary>Inclui o registro; gera o <c>Id</c> quando ele não vier preenchido.</summary>
    /// <param name="item">Registro a incluir.</param>
    public void Incluir(T item)
    {
        if (item.Id <= 0)
        {
            item.Id = ++_ultimoId;
        }
        else
        {
            _ultimoId = Math.Max(_ultimoId, item.Id);
        }

        item.CriadoEm = DateTime.Now;
        item.AlteradoEm = item.CriadoEm;
        _itens.Add((T)item.Clonar());
    }

    /// <summary>Substitui o registro de mesmo <c>Id</c>, preservando a data de inclusão.</summary>
    /// <param name="item">Registro já editado.</param>
    public void Alterar(T item)
    {
        var indice = _itens.FindIndex(i => i.Id == item.Id);
        if (indice < 0)
        {
            throw new InvalidOperationException($"Registro {item.Id} não encontrado.");
        }

        item.CriadoEm = _itens[indice].CriadoEm;
        item.AlteradoEm = DateTime.Now;
        _itens[indice] = (T)item.Clonar();
    }

    /// <summary>Remove o registro de mesmo <c>Id</c>, se existir.</summary>
    /// <param name="id">Identificador do registro.</param>
    public void Excluir(int id) => _itens.RemoveAll(i => i.Id == id);
}
