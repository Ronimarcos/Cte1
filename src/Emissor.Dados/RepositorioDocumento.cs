using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Repositório do CT-e e do MDF-e. Além das regras do <see cref="RepositorioCadastro{T}"/>, troca os itens
/// gravados (tabelas filhas) pelos editados na alteração e só exclui documentos em digitação.
/// </summary>
/// <typeparam name="T">CT-e ou MDF-e.</typeparam>
public class RepositorioDocumento<T> : RepositorioCadastro<T> where T : DocumentoFiscal
{
    /// <summary>Cria o repositório sobre o contexto informado.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    public RepositorioDocumento(EmissorDbContext contexto)
        : base(contexto)
    {
    }

    /// <summary>Cria o repositório aplicando a alçada do usuário.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    /// <param name="acesso">Quem acessa e em quais empresas.</param>
    public RepositorioDocumento(EmissorDbContext contexto, IContextoAcesso acesso)
        : base(contexto, acesso)
    {
    }

    /// <summary>
    /// Valida e altera o documento: lê o gravado, copia os campos e substitui as listas de itens, para o banco
    /// apagar os itens antigos e incluir os editados.
    /// </summary>
    /// <param name="registro">Documento editado.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <exception cref="RegraNegocioException">Pendências, documento inexistente ou de outra empresa.</exception>
    public override async Task AlterarAsync(T registro, CancellationToken cancelamento = default)
    {
        GarantirEscrita(registro);
        Validar(registro);
        var gravado = await Consulta().FirstOrDefaultAsync(d => d.Id == registro.Id, cancelamento)
            ?? throw new RegraNegocioException("O documento não existe mais. Atualize a lista.");
        if (gravado.EmpresaId != registro.EmpresaId)
        {
            Contexto.ChangeTracker.Clear();
            throw new RegraNegocioException("O documento é de outra empresa.");
        }

        Contexto.Entry(gravado).CurrentValues.SetValues(registro);
        gravado.CopiarItensDe(registro);
        await GravarAsync(cancelamento, "alterar");
    }

    /// <summary>Exclui o documento; só rascunhos (em digitação) podem ser excluídos.</summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <exception cref="RegraNegocioException">Documento que não está em digitação ou vinculado a outro.</exception>
    public override async Task ExcluirAsync(int id, CancellationToken cancelamento = default)
    {
        var situacao = await Consulta().Where(d => d.Id == id).Select(d => (SituacaoDocumento?)d.Situacao).FirstOrDefaultAsync(cancelamento);
        if (situacao is null)
        {
            return;
        }

        if (situacao != SituacaoDocumento.EmDigitacao)
        {
            throw new RegraNegocioException("Só documentos em digitação podem ser excluídos.");
        }

        await base.ExcluirAsync(id, cancelamento);
    }
}
