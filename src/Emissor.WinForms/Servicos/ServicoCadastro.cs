using System.Linq.Expressions;
using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Operações de um cadastro da empresa no banco, para as telas. Cada operação abre um contexto novo
/// (um por operação, padrão para aplicativos desktop) e usa o <see cref="RepositorioCadastro{T}"/>, que valida
/// e traduz as recusas do banco em <c>RegraNegocioException</c>.
/// </summary>
/// <typeparam name="T">Entidade do cadastro.</typeparam>
public sealed class ServicoCadastro<T> where T : EntidadeDaEmpresa
{
    private readonly IDbContextFactory<EmissorDbContext> _fabrica;
    private readonly Sessao _sessao;

    /// <summary>Cria o serviço sobre a fábrica de contextos.</summary>
    /// <param name="fabrica">Fábrica de contextos do banco.</param>
    /// <param name="sessao">Usuário logado: o repositório só deixa ver e gravar as empresas da alçada dele.</param>
    public ServicoCadastro(IDbContextFactory<EmissorDbContext> fabrica, Sessao sessao)
    {
        _fabrica = fabrica;
        _sessao = sessao;
    }

    /// <summary>Registros da empresa.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public Task<IReadOnlyList<T>> ListarAsync(int empresaId) => ExecutarAsync(r => r.ListarAsync(e => e.EmpresaId == empresaId));

    /// <summary>Cópia do registro para edição, ou <c>null</c>.</summary>
    /// <param name="id">Identificador.</param>
    public Task<T?> ObterAsync(int id) => ExecutarAsync(r => r.ObterAsync(id));

    /// <summary>Indica se algum registro atende à condição.</summary>
    /// <param name="condicao">Condição.</param>
    public Task<bool> ExisteAsync(Expression<Func<T, bool>> condicao) => ExecutarAsync(r => r.ExisteAsync(condicao));

    /// <summary>Inclui o registro.</summary>
    /// <param name="registro">Registro novo.</param>
    public Task IncluirAsync(T registro) => ExecutarAsync(async r => { await r.IncluirAsync(registro); return true; });

    /// <summary>Altera o registro.</summary>
    /// <param name="registro">Registro editado.</param>
    public Task AlterarAsync(T registro) => ExecutarAsync(async r => { await r.AlterarAsync(registro); return true; });

    /// <summary>Exclui o registro; o banco recusa se ele estiver em uso.</summary>
    /// <param name="id">Identificador.</param>
    public Task ExcluirAsync(int id) => ExecutarAsync(async r => { await r.ExcluirAsync(id); return true; });

    private async Task<TResultado> ExecutarAsync<TResultado>(Func<RepositorioCadastro<T>, Task<TResultado>> operacao)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await operacao(new RepositorioCadastro<T>(contexto, _sessao));
    }
}

/// <summary>Os serviços dos cadastros e documentos da empresa, reunidos para a janela principal.</summary>
/// <param name="Participantes">Participantes.</param>
/// <param name="Veiculos">Veículos.</param>
/// <param name="Proprietarios">Proprietários de veículos.</param>
/// <param name="Motoristas">Motoristas.</param>
/// <param name="Apolices">Apólices de seguro.</param>
/// <param name="Series">Séries de numeração.</param>
/// <param name="Certificados">Certificados digitais.</param>
/// <param name="Protetor">Criptografia do arquivo e da senha do certificado A1.</param>
/// <param name="Ctes">CT-e emitidos.</param>
/// <param name="Mdfes">MDF-e emitidos.</param>
/// <param name="Auditoria">Eventos e consulta da auditoria.</param>
public sealed record ServicosCadastro(
    ServicoCadastro<Participante> Participantes,
    ServicoCadastro<Veiculo> Veiculos,
    ServicoCadastro<ProprietarioVeiculo> Proprietarios,
    ServicoCadastro<Motorista> Motoristas,
    ServicoCadastro<ApoliceSeguro> Apolices,
    ServicoCadastro<SerieDocumento> Series,
    ServicoCadastro<CertificadoDigital> Certificados,
    IProtetorSegredos Protetor,
    ServicoDocumentos<ConhecimentoTransporte> Ctes,
    ServicoDocumentos<ManifestoEletronico> Mdfes,
    ServicoAuditoria Auditoria);
