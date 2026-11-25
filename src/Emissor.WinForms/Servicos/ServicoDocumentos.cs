using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Operações do CT-e ou do MDF-e no banco, para as telas. Cada operação abre um contexto novo e usa o
/// <see cref="RepositorioDocumento{T}"/>, que valida, troca os itens na alteração e só exclui rascunhos.
/// </summary>
/// <typeparam name="T">CT-e ou MDF-e.</typeparam>
public sealed class ServicoDocumentos<T> where T : DocumentoFiscal
{
    private readonly IDbContextFactory<EmissorDbContext> _fabrica;
    private readonly Sessao _sessao;

    /// <summary>Cria o serviço sobre a fábrica de contextos.</summary>
    /// <param name="fabrica">Fábrica de contextos do banco.</param>
    /// <param name="sessao">Usuário logado: o repositório só deixa ver e gravar as empresas da alçada dele.</param>
    public ServicoDocumentos(IDbContextFactory<EmissorDbContext> fabrica, Sessao sessao)
    {
        _fabrica = fabrica;
        _sessao = sessao;
    }

    /// <summary>Documentos da empresa emitidos no período, com os itens, opcionalmente só de uma situação.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    /// <param name="de">Primeiro dia.</param>
    /// <param name="ate">Último dia.</param>
    /// <param name="situacao">Situação, ou <c>null</c> para todas.</param>
    public Task<IReadOnlyList<T>> ListarAsync(int empresaId, DateTime de, DateTime ate, SituacaoDocumento? situacao = null)
    {
        var inicio = de.Date;
        var fim = ate.Date.AddDays(1);
        return ExecutarAsync(r => r.ListarAsync(d => d.EmpresaId == empresaId
            && d.DataEmissao >= inicio && d.DataEmissao < fim
            && (situacao == null || d.Situacao == situacao)));
    }

    /// <summary>Documentos da empresa numa situação, de qualquer data (ex.: CT-e prontos para vincular no MDF-e).</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    /// <param name="situacao">Situação.</param>
    public Task<IReadOnlyList<T>> ListarPorSituacaoAsync(int empresaId, SituacaoDocumento situacao) =>
        ExecutarAsync(r => r.ListarAsync(d => d.EmpresaId == empresaId && d.Situacao == situacao));

    /// <summary>Documentos da empresa pelos códigos (ex.: CT-e vinculados a um MDF-e).</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    /// <param name="ids">Códigos.</param>
    public Task<IReadOnlyList<T>> ListarPorIdsAsync(int empresaId, IReadOnlyCollection<int> ids) =>
        ExecutarAsync(r => r.ListarAsync(d => d.EmpresaId == empresaId && ids.Contains(d.Id)));

    /// <summary>Cópia do documento, com os itens, para edição; ou <c>null</c>.</summary>
    /// <param name="id">Identificador.</param>
    public Task<T?> ObterAsync(int id) => ExecutarAsync(r => r.ObterAsync(id));

    /// <summary>Inclui (sem código) ou altera o documento; o código é preenchido na inclusão.</summary>
    /// <param name="documento">Documento editado.</param>
    public Task SalvarAsync(T documento) => ExecutarAsync(async r =>
    {
        if (documento.Id == 0)
        {
            await r.IncluirAsync(documento);
        }
        else
        {
            await r.AlterarAsync(documento);
        }

        return true;
    });

    /// <summary>Exclui o rascunho; o banco recusa documento vinculado a outro (ex.: CT-e num MDF-e).</summary>
    /// <param name="id">Identificador.</param>
    public Task ExcluirAsync(int id) => ExecutarAsync(async r => { await r.ExcluirAsync(id); return true; });

    private async Task<TResultado> ExecutarAsync<TResultado>(Func<RepositorioDocumento<T>, Task<TResultado>> operacao)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await operacao(new RepositorioDocumento<T>(contexto, _sessao));
    }
}
