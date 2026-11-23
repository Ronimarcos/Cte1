using System.Collections;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Coluna exibida na lista de um cadastro.</summary>
/// <param name="Propriedade">Nome da propriedade do registro exibida na coluna.</param>
/// <param name="Titulo">Título do cabeçalho.</param>
/// <param name="Peso">Largura relativa (FillWeight).</param>
internal sealed record ColunaCadastro(string Propriedade, string Titulo, float Peso);

/// <summary>
/// O que a lista genérica (<see cref="FormListaCadastro"/>) precisa saber de um cadastro:
/// título, colunas, pesquisa e as ações de incluir, editar e excluir.
/// </summary>
internal interface IFonteCadastro
{
    /// <summary>Título da janela (também usado para não abrir a mesma lista duas vezes).</summary>
    string Titulo { get; }

    /// <summary>Colunas da grade.</summary>
    IReadOnlyList<ColunaCadastro> Colunas { get; }

    /// <summary>Registros que contêm o texto pesquisado (vazio lista todos).</summary>
    /// <param name="pesquisa">Texto digitado pelo usuário.</param>
    IList Listar(string pesquisa);

    /// <summary>Abre a edição de um registro novo; retorna o <c>Id</c> incluído ou <c>null</c> se cancelado.</summary>
    /// <param name="dono">Janela que abre o diálogo.</param>
    int? Novo(IWin32Window dono);

    /// <summary>Abre a edição do registro; retorna verdadeiro se foi salvo.</summary>
    /// <param name="dono">Janela que abre o diálogo.</param>
    /// <param name="registro">Registro da linha selecionada.</param>
    bool Editar(IWin32Window dono, object registro);

    /// <summary>Pede confirmação e exclui o registro; retorna verdadeiro se excluiu.</summary>
    /// <param name="dono">Janela que abre a confirmação.</param>
    /// <param name="registro">Registro da linha selecionada.</param>
    bool Excluir(IWin32Window dono, object registro);
}
