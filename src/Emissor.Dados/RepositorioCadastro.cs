using System.Linq.Expressions;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Repositório assíncrono dos cadastros. Valida o registro com as regras do domínio antes de gravar e traduz
/// as recusas do banco (registro repetido, registro em uso) em <see cref="RegraNegocioException"/> com mensagem
/// em português para a tela.
/// <para>
/// Aplica a alçada do usuário (<see cref="IContextoAcesso"/>): o operador só lista, abre, grava e exclui registros das
/// empresas liberadas para ele; registro de outra empresa se comporta como inexistente. Empresas, certificados e
/// usuários só são gravados pelo administrador.
/// </para>
/// </summary>
/// <typeparam name="T">Entidade de cadastro.</typeparam>
public class RepositorioCadastro<T> where T : EntidadeBase
{
    /// <summary>Código de erro do SQLite para violação de restrição (única ou chave estrangeira).</summary>
    private const int SqliteRestricao = 19;

    private readonly EmissorDbContext _contexto;

    /// <summary>Cria o repositório com acesso total, para uso interno do sistema (preparação do banco, testes).</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    public RepositorioCadastro(EmissorDbContext contexto)
        : this(contexto, ContextoAcesso.Sistema)
    {
    }

    /// <summary>Cria o repositório aplicando a alçada do usuário.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    /// <param name="acesso">Quem acessa e em quais empresas.</param>
    public RepositorioCadastro(EmissorDbContext contexto, IContextoAcesso acesso)
    {
        _contexto = contexto;
        Acesso = acesso;
    }

    /// <summary>Contexto do EF Core, para os repositórios derivados.</summary>
    protected EmissorDbContext Contexto => _contexto;

    /// <summary>Alçada aplicada.</summary>
    protected IContextoAcesso Acesso { get; }

    /// <summary>Lista os registros que atendem ao filtro, sem rastreamento (só leitura).</summary>
    /// <param name="filtro">Condição opcional.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public async Task<IReadOnlyList<T>> ListarAsync(Expression<Func<T, bool>>? filtro = null, CancellationToken cancelamento = default)
    {
        var consulta = Consulta().AsNoTracking();
        if (filtro is not null)
        {
            consulta = consulta.Where(filtro);
        }

        return await consulta.OrderBy(e => e.Id).ToListAsync(cancelamento);
    }

    /// <summary>Obtém uma cópia do registro para edição, ou <c>null</c> se não existir ou estiver fora da alçada.</summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public Task<T?> ObterAsync(int id, CancellationToken cancelamento = default) =>
        Consulta().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancelamento);

    /// <summary>Indica se algum registro, dentro da alçada, atende à condição.</summary>
    /// <param name="condicao">Condição.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public Task<bool> ExisteAsync(Expression<Func<T, bool>> condicao, CancellationToken cancelamento = default) =>
        Consulta().AnyAsync(condicao, cancelamento);

    /// <summary>Valida e inclui o registro; o <c>Id</c> é preenchido pelo banco.</summary>
    /// <param name="registro">Registro novo.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <exception cref="RegraNegocioException">Fora da alçada, pendências de validação ou registro repetido.</exception>
    public async Task IncluirAsync(T registro, CancellationToken cancelamento = default)
    {
        GarantirEscrita(registro);
        Validar(registro);
        _contexto.Set<T>().Add(registro);
        await GravarAsync(cancelamento, "incluir");
    }

    /// <summary>Valida e altera o registro de mesmo <c>Id</c>, preservando a data de inclusão.</summary>
    /// <param name="registro">Registro editado.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <exception cref="RegraNegocioException">Fora da alçada, pendências de validação, registro repetido ou inexistente.</exception>
    public virtual async Task AlterarAsync(T registro, CancellationToken cancelamento = default)
    {
        GarantirEscrita(registro);
        Validar(registro);
        if (!await Consulta().AnyAsync(e => e.Id == registro.Id, cancelamento))
        {
            throw new RegraNegocioException("O registro não existe mais. Atualize a lista.");
        }

        _contexto.Set<T>().Update(registro);
        await GravarAsync(cancelamento, "alterar");
    }

    /// <summary>Exclui o registro; recusa se ele estiver em uso por outros cadastros.</summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <exception cref="RegraNegocioException">Fora da alçada ou registro em uso.</exception>
    public virtual async Task ExcluirAsync(int id, CancellationToken cancelamento = default)
    {
        var registro = await Consulta().FirstOrDefaultAsync(e => e.Id == id, cancelamento);
        if (registro is null)
        {
            return;
        }

        GarantirEscrita(registro);
        _contexto.Set<T>().Remove(registro);
        await GravarAsync(cancelamento, "excluir");
    }

    /// <summary>
    /// Consulta já restrita à alçada: com acesso total, tudo; senão, só registros das empresas liberadas (empresas:
    /// pelo próprio código; cadastros e documentos: pela empresa dona); usuários, nada.
    /// </summary>
    protected IQueryable<T> Consulta()
    {
        IQueryable<T> consulta = _contexto.Set<T>();
        if (Acesso.AcessoTotal)
        {
            return consulta;
        }

        var empresas = Acesso.Empresas.ToList();
        if (typeof(T) == typeof(Empresa))
        {
            return consulta.Where(e => empresas.Contains(e.Id));
        }

        if (typeof(EntidadeDaEmpresa).IsAssignableFrom(typeof(T)))
        {
            return consulta.Where(e => empresas.Contains(EF.Property<int>(e, nameof(EntidadeDaEmpresa.EmpresaId))));
        }

        return consulta.Where(_ => false);
    }

    /// <summary>
    /// Recusa a gravação fora da alçada: empresas, certificados e usuários só pelo administrador; cadastros e documentos
    /// só de empresa liberada.
    /// </summary>
    /// <param name="registro">Registro a gravar ou excluir.</param>
    /// <exception cref="RegraNegocioException">Fora da alçada.</exception>
    protected void GarantirEscrita(T registro)
    {
        if (Acesso.AcessoTotal)
        {
            return;
        }

        if (registro is Empresa or CertificadoDigital or Usuario)
        {
            throw new RegraNegocioException("Somente o administrador pode cadastrar ou alterar empresas, certificados e usuários.");
        }

        if (registro is EntidadeDaEmpresa daEmpresa && !Acesso.PodeAcessar(daEmpresa.EmpresaId))
        {
            throw new RegraNegocioException("Você não tem acesso a esta empresa.");
        }
    }

    /// <summary>Recusa o registro com pendências de validação.</summary>
    /// <param name="registro">Registro a gravar.</param>
    /// <exception cref="RegraNegocioException">Pendências de validação.</exception>
    protected static void Validar(T registro)
    {
        var pendencias = registro.Validar();
        if (pendencias.Count > 0)
        {
            throw new RegraNegocioException(pendencias);
        }
    }

    /// <summary>Grava e, em caso de recusa do banco, desfaz as mudanças pendentes e explica o motivo.</summary>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <param name="operacao">"incluir", "alterar" ou "excluir" (para a mensagem).</param>
    protected async Task GravarAsync(CancellationToken cancelamento, string operacao)
    {
        try
        {
            await _contexto.SaveChangesAsync(cancelamento);
        }
        catch (DbUpdateException erro) when (erro.InnerException is SqliteException { SqliteErrorCode: SqliteRestricao } sqlite)
        {
            _contexto.ChangeTracker.Clear();
            var mensagem = sqlite.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
                ? operacao == "excluir"
                    ? "O registro está em uso por outros cadastros ou documentos e não pode ser excluído."
                    : "O registro aponta para um cadastro que não existe mais (empresa, município, participante, veículo, série...)."
                : "Já existe um registro com estes dados (documento, placa, série, apólice ou login repetidos).";
            throw new RegraNegocioException(mensagem);
        }
        finally
        {
            // Mantém o contexto limpo entre operações: as telas trabalham sempre com cópias.
            _contexto.ChangeTracker.Clear();
        }
    }
}
