using System.Collections;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Coluna exibida na lista de um cadastro.</summary>
/// <param name="Propriedade">Nome da propriedade do registro exibida na coluna.</param>
/// <param name="Titulo">Título do cabeçalho.</param>
/// <param name="Peso">Largura relativa (FillWeight).</param>
internal sealed record ColunaCadastro(string Propriedade, string Titulo, float Peso);

/// <summary>
/// O que a lista genérica (<see cref="FormListaCadastro"/>) precisa saber de um cadastro:
/// título, colunas, pesquisa e as ações de incluir, editar e excluir. As operações são assíncronas porque
/// os cadastros passam a ler e gravar no banco; os que ainda estão em memória respondem na hora.
/// </summary>
internal interface IFonteCadastro
{
    /// <summary>Título da janela (também usado para não abrir a mesma lista duas vezes).</summary>
    string Titulo { get; }

    /// <summary>Colunas da grade.</summary>
    IReadOnlyList<ColunaCadastro> Colunas { get; }

    /// <summary>Registros que contêm o texto pesquisado (vazio lista todos).</summary>
    /// <param name="pesquisa">Texto digitado pelo usuário.</param>
    Task<IList> ListarAsync(string pesquisa);

    /// <summary>Identificador do registro de uma linha da grade.</summary>
    /// <param name="registro">Objeto da linha.</param>
    int IdDe(object registro);

    /// <summary>Abre a edição de um registro novo; retorna o <c>Id</c> incluído ou <c>null</c> se cancelado.</summary>
    /// <param name="dono">Janela que abre o diálogo.</param>
    Task<int?> NovoAsync(IWin32Window dono);

    /// <summary>Abre a edição do registro; retorna verdadeiro se foi salvo.</summary>
    /// <param name="dono">Janela que abre o diálogo.</param>
    /// <param name="registro">Registro da linha selecionada.</param>
    Task<bool> EditarAsync(IWin32Window dono, object registro);

    /// <summary>Pede confirmação e exclui o registro; retorna verdadeiro se excluiu.</summary>
    /// <param name="dono">Janela que abre a confirmação.</param>
    /// <param name="registro">Registro da linha selecionada.</param>
    Task<bool> ExcluirAsync(IWin32Window dono, object registro);

    /// <summary>Cor de fundo para destacar a linha do registro (ex.: certificado vencido), ou <c>null</c> para a cor padrão.</summary>
    /// <param name="registro">Registro da linha.</param>
    Color? CorDaLinha(object registro);
}
