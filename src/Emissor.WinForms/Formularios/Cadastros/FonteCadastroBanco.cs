using System.Collections;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de um cadastro gravado no banco: o identificador e os textos de exibição.</summary>
internal abstract class LinhaCadastro
{
    /// <summary>Identificador do registro.</summary>
    public int Id { get; init; }

    /// <summary>Texto em que a pesquisa procura.</summary>
    public abstract string TextoPesquisa { get; }

    /// <summary>Descrição curta para as mensagens (ex.: a placa, o nome).</summary>
    public abstract string Descricao { get; }
}

/// <summary>
/// Base dos cadastros da empresa gravados no banco. Lê os registros da empresa ativa, monta as linhas com os
/// textos de exibição e abre os diálogos de edição, que gravam pelo delegado recebido — assim uma recusa do banco
/// mantém o diálogo aberto com o que foi digitado.
/// </summary>
/// <typeparam name="T">Entidade do cadastro.</typeparam>
/// <typeparam name="TLinha">Linha exibida na lista.</typeparam>
internal abstract class FonteCadastroBanco<T, TLinha> : IFonteCadastro
    where T : EntidadeDaEmpresa
    where TLinha : LinhaCadastro
{
    /// <summary>Cria a fonte.</summary>
    /// <param name="servico">Operações do cadastro no banco.</param>
    /// <param name="empresaId">Empresa ativa.</param>
    protected FonteCadastroBanco(ServicoCadastro<T> servico, int empresaId)
    {
        Servico = servico;
        EmpresaId = empresaId;
    }

    /// <summary>Operações do cadastro no banco.</summary>
    protected ServicoCadastro<T> Servico { get; }

    /// <summary>Empresa ativa.</summary>
    protected int EmpresaId { get; }

    /// <inheritdoc />
    public abstract string Titulo { get; }

    /// <inheritdoc />
    public abstract IReadOnlyList<ColunaCadastro> Colunas { get; }

    /// <summary>Nome do registro no singular, usado nas mensagens (ex.: "o participante").</summary>
    protected abstract string NomeRegistro { get; }

    /// <inheritdoc />
    public async Task<IList> ListarAsync(string pesquisa)
    {
        var termo = pesquisa.Trim();
        var registros = await Servico.ListarAsync(EmpresaId);
        var linhas = await CriarLinhasAsync(registros);
        return linhas
            .Where(l => termo.Length == 0 || l.TextoPesquisa.Contains(termo, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    /// <inheritdoc />
    public int IdDe(object registro) => ((LinhaCadastro)registro).Id;

    /// <inheritdoc />
    public async Task<int?> NovoAsync(IWin32Window dono)
    {
        var registro = CriarNovo();
        registro.EmpresaId = EmpresaId;
        return await AbrirEdicaoAsync(dono, registro, novo: true, Servico.IncluirAsync) ? registro.Id : null;
    }

    /// <inheritdoc />
    public async Task<bool> EditarAsync(IWin32Window dono, object registro)
    {
        if (await Servico.ObterAsync(IdDe(registro)) is not { } copia)
        {
            MessageBox.Show(dono, $"O registro não existe mais. A lista será atualizada.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        return await AbrirEdicaoAsync(dono, copia, novo: false, Servico.AlterarAsync);
    }

    /// <inheritdoc />
    public async Task<bool> ExcluirAsync(IWin32Window dono, object registro)
    {
        var linha = (TLinha)registro;
        var resposta = MessageBox.Show(dono, $"Excluir {NomeRegistro} \"{linha.Descricao}\"?", Titulo,
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta != DialogResult.Yes)
        {
            return false;
        }

        try
        {
            await Servico.ExcluirAsync(linha.Id);
            return true;
        }
        catch (RegraNegocioException erro)
        {
            MessageBox.Show(dono, string.Join(Environment.NewLine, erro.Pendencias), Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }
    }

    /// <inheritdoc />
    public Color? CorDaLinha(object registro) => null;

    /// <summary>Monta as linhas da lista (pode consultar outros cadastros para os textos).</summary>
    /// <param name="registros">Registros da empresa.</param>
    protected abstract Task<IReadOnlyList<TLinha>> CriarLinhasAsync(IReadOnlyList<T> registros);

    /// <summary>Registro novo com os valores padrão (a empresa é preenchida pela base).</summary>
    protected abstract T CriarNovo();

    /// <summary>Abre o diálogo de edição; retorna verdadeiro se o registro foi gravado.</summary>
    /// <param name="dono">Janela que abre o diálogo.</param>
    /// <param name="registro">Cópia editável.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="gravar">Inclui ou altera no banco.</param>
    protected abstract Task<bool> AbrirEdicaoAsync(IWin32Window dono, T registro, bool novo, Func<T, Task> gravar);
}
