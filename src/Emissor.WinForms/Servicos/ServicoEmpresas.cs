using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;
using Microsoft.EntityFrameworkCore;

namespace Emissor.WinForms.Servicos;

/// <summary>Linha das listas de empresas (seleção e cadastro), já com os textos de exibição.</summary>
public sealed class EmpresaResumo
{
    /// <summary>Identificador.</summary>
    public int Id { get; init; }

    /// <summary>Razão social.</summary>
    public string RazaoSocial { get; init; } = string.Empty;

    /// <summary>Nome fantasia.</summary>
    public string NomeFantasia { get; init; } = string.Empty;

    /// <summary>CNPJ ou CPF formatado (o nome da propriedade segue a coluna já existente na seleção de empresa).</summary>
    public string Cnpj { get; init; } = string.Empty;

    /// <summary>Município "Nome/UF".</summary>
    public string MunicipioDescricao { get; init; } = string.Empty;

    /// <summary>UF do município.</summary>
    public string Uf { get; init; } = string.Empty;

    /// <summary>CRT para exibição.</summary>
    public string CrtDescricao { get; init; } = string.Empty;

    /// <summary>Ambiente padrão para exibição.</summary>
    public string AmbienteDescricao { get; init; } = string.Empty;
}

/// <summary>
/// Operações de empresa usadas pelas telas. Cada operação abre um contexto novo do banco
/// (padrão recomendado para aplicativos desktop) e usa o <see cref="RepositorioCadastro{T}"/>.
/// </summary>
public sealed class ServicoEmpresas
{
    private readonly IDbContextFactory<EmissorDbContext> _fabrica;
    private readonly CatalogoMunicipios _municipios;

    /// <summary>Cria o serviço.</summary>
    /// <param name="fabrica">Fábrica de contextos do banco.</param>
    /// <param name="municipios">Catálogo de municípios para os textos de exibição.</param>
    public ServicoEmpresas(IDbContextFactory<EmissorDbContext> fabrica, CatalogoMunicipios municipios)
    {
        _fabrica = fabrica;
        _municipios = municipios;
    }

    /// <summary>Empresas em ordem de razão social, prontas para as listas.</summary>
    public async Task<IReadOnlyList<EmpresaResumo>> ListarResumosAsync()
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        var empresas = await new RepositorioCadastro<Empresa>(contexto).ListarAsync();
        return empresas.OrderBy(e => e.RazaoSocial).Select(Resumir).ToList();
    }

    /// <summary>Cópia da empresa para edição, ou <c>null</c>.</summary>
    /// <param name="id">Identificador.</param>
    public async Task<Empresa?> ObterAsync(int id)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await new RepositorioCadastro<Empresa>(contexto).ObterAsync(id);
    }

    /// <summary>Inclui a empresa (valida e traduz recusas do banco em <c>RegraNegocioException</c>).</summary>
    /// <param name="empresa">Empresa nova.</param>
    public async Task IncluirAsync(Empresa empresa)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        await new RepositorioCadastro<Empresa>(contexto).IncluirAsync(empresa);
    }

    /// <summary>Altera a empresa.</summary>
    /// <param name="empresa">Empresa editada.</param>
    public async Task AlterarAsync(Empresa empresa)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        await new RepositorioCadastro<Empresa>(contexto).AlterarAsync(empresa);
    }

    /// <summary>Exclui a empresa; o banco recusa se houver cadastros vinculados.</summary>
    /// <param name="id">Identificador.</param>
    public async Task ExcluirAsync(int id)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        await new RepositorioCadastro<Empresa>(contexto).ExcluirAsync(id);
    }

    /// <summary>Indica se ainda não há empresas no banco.</summary>
    public async Task<bool> SemEmpresasAsync()
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        return await DadosDemonstracao.BancoSemEmpresasAsync(contexto);
    }

    /// <summary>Grava as empresas de demonstração.</summary>
    public async Task CarregarDemonstracaoAsync()
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();
        await DadosDemonstracao.CarregarAsync(contexto);
    }

    /// <summary>Textos de exibição da empresa.</summary>
    /// <param name="empresa">Empresa.</param>
    public EmpresaResumo Resumir(Empresa empresa) => new()
    {
        Id = empresa.Id,
        RazaoSocial = empresa.RazaoSocial,
        NomeFantasia = empresa.NomeFantasia,
        Cnpj = Formatador.CnpjCpf(empresa.Documento),
        MunicipioDescricao = _municipios.Descricao(empresa.Endereco.MunicipioCodigo),
        Uf = _municipios.Obter(empresa.Endereco.MunicipioCodigo)?.Uf ?? string.Empty,
        CrtDescricao = OpcoesCadastro.Texto(OpcoesCadastro.Crts, empresa.Crt),
        AmbienteDescricao = OpcoesCadastro.Texto(OpcoesCadastro.Ambientes, empresa.AmbientePadrao == TipoAmbiente.Homologacao),
    };
}
