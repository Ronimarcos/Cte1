using System.Collections;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Implementação base de <see cref="IFonteCadastro"/> sobre um <see cref="RepositorioMemoria{T}"/>:
/// cuida da pesquisa, da cópia para edição e da confirmação de exclusão.
/// </summary>
/// <typeparam name="T">Tipo de registro do cadastro.</typeparam>
internal abstract class FonteCadastro<T> : IFonteCadastro where T : RegistroPrototipo
{
    /// <summary>Cria a fonte sobre o repositório informado.</summary>
    /// <param name="repositorio">Repositório em memória do cadastro.</param>
    protected FonteCadastro(RepositorioMemoria<T> repositorio) => Repositorio = repositorio;

    /// <summary>Repositório do cadastro.</summary>
    protected RepositorioMemoria<T> Repositorio { get; }

    /// <inheritdoc />
    public abstract string Titulo { get; }

    /// <inheritdoc />
    public abstract IReadOnlyList<ColunaCadastro> Colunas { get; }

    /// <summary>Nome do registro no singular, usado nas mensagens (ex.: "o participante").</summary>
    protected abstract string NomeRegistro { get; }

    /// <inheritdoc />
    public IList Listar(string pesquisa)
    {
        var termo = pesquisa.Trim();
        return Repositorio
            .Listar(item => PertenceALista(item)
                && (termo.Length == 0 || TextoPesquisa(item).Contains(termo, StringComparison.CurrentCultureIgnoreCase)))
            .ToList();
    }

    /// <inheritdoc />
    public int? Novo(IWin32Window dono)
    {
        var item = CriarNovo();
        if (!AbrirEdicao(dono, item, novo: true))
        {
            return null;
        }

        Repositorio.Incluir(item);
        AoAlterar();
        return item.Id;
    }

    /// <inheritdoc />
    public bool Editar(IWin32Window dono, object registro)
    {
        if (registro is not T atual || Repositorio.ObterPorId(atual.Id) is not { } copia)
        {
            return false;
        }

        if (!AbrirEdicao(dono, copia, novo: false))
        {
            return false;
        }

        Repositorio.Alterar(copia);
        AoAlterar();
        return true;
    }

    /// <inheritdoc />
    public bool Excluir(IWin32Window dono, object registro)
    {
        if (registro is not T item)
        {
            return false;
        }

        var impedimento = MotivoParaNaoExcluir(item);
        if (impedimento is not null)
        {
            MessageBox.Show(dono, impedimento, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        var resposta = MessageBox.Show(dono, $"Excluir {NomeRegistro} \"{Descrever(item)}\"?", Titulo,
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta != DialogResult.Yes)
        {
            return false;
        }

        Repositorio.Excluir(item.Id);
        AoAlterar();
        return true;
    }

    /// <inheritdoc />
    public Color? CorDaLinha(object registro) => registro is T item ? CorDoRegistro(item) : null;

    /// <summary>Cor de fundo da linha do registro; o padrão é não destacar.</summary>
    /// <param name="item">Registro da linha.</param>
    protected virtual Color? CorDoRegistro(T item) => null;

    /// <summary>Indica se o registro aparece nesta lista (ex.: só os da empresa ativa).</summary>
    /// <param name="item">Registro avaliado.</param>
    protected virtual bool PertenceALista(T item) => true;

    /// <summary>Texto em que a pesquisa procura (nome, documento etc.).</summary>
    /// <param name="item">Registro avaliado.</param>
    protected abstract string TextoPesquisa(T item);

    /// <summary>Descrição curta do registro para as mensagens.</summary>
    /// <param name="item">Registro descrito.</param>
    protected abstract string Descrever(T item);

    /// <summary>Cria um registro novo com os valores padrão.</summary>
    protected abstract T CriarNovo();

    /// <summary>Abre o diálogo de edição; retorna verdadeiro se o usuário salvou.</summary>
    /// <param name="dono">Janela que abre o diálogo.</param>
    /// <param name="item">Cópia do registro a editar.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    protected abstract bool AbrirEdicao(IWin32Window dono, T item, bool novo);

    /// <summary>Mensagem explicando por que o registro não pode ser excluído, ou <c>null</c> se pode.</summary>
    /// <param name="item">Registro a excluir.</param>
    protected virtual string? MotivoParaNaoExcluir(T item) => null;

    /// <summary>Chamado depois de incluir, alterar ou excluir.</summary>
    protected virtual void AoAlterar()
    {
    }
}
