using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Cadastros da empresa ativa lidos do banco ao abrir um assistente ou uma consulta de CT-e/MDF-e: participantes,
/// veículos, proprietários, motoristas, apólices e séries, além dos municípios do IBGE. Também resolve os nomes
/// exibidos a partir dos códigos guardados nos documentos e dá acesso aos CT-e e MDF-e gravados.
/// </summary>
public sealed class CadastrosEmissao
{
    /// <summary>Texto exibido quando o documento aponta para um cadastro que não existe no banco.</summary>
    public const string NaoEncontrado = "(não encontrado)";

    private readonly ServicosCadastro _servicos;

    private CadastrosEmissao(Empresa empresa, CatalogoMunicipios municipios, ServicosCadastro servicos)
    {
        Empresa = empresa;
        Municipios = municipios;
        _servicos = servicos;
    }

    /// <summary>Empresa emitente.</summary>
    public Empresa Empresa { get; }

    /// <summary>Municípios do IBGE.</summary>
    public CatalogoMunicipios Municipios { get; }

    /// <summary>Participantes da empresa.</summary>
    public IReadOnlyList<Participante> Participantes { get; private set; } = [];

    /// <summary>Veículos da empresa.</summary>
    public IReadOnlyList<Veiculo> Veiculos { get; private set; } = [];

    /// <summary>Proprietários de veículos de terceiros da empresa.</summary>
    public IReadOnlyList<ProprietarioVeiculo> Proprietarios { get; private set; } = [];

    /// <summary>Motoristas da empresa.</summary>
    public IReadOnlyList<Motorista> Motoristas { get; private set; } = [];

    /// <summary>Apólices de seguro da empresa.</summary>
    public IReadOnlyList<ApoliceSeguro> Apolices { get; private set; } = [];

    /// <summary>Séries de numeração da empresa.</summary>
    public IReadOnlyList<SerieDocumento> Series { get; private set; } = [];

    /// <summary>Operações do cadastro de participantes (usado para incluir sem sair da emissão).</summary>
    public ServicoCadastro<Participante> ServicoParticipantes => _servicos.Participantes;

    /// <summary>CT-e gravados no banco.</summary>
    public ServicoDocumentos<ConhecimentoTransporte> Ctes => _servicos.Ctes;

    /// <summary>MDF-e gravados no banco.</summary>
    public ServicoDocumentos<ManifestoEletronico> Mdfes => _servicos.Mdfes;

    /// <summary>Geração do XML de teste (assinado e validado, sem transmitir).</summary>
    public ServicoXmlTeste XmlTeste => new(_servicos);

    /// <summary>Eventos gravados dos documentos.</summary>
    public ServicoEventos Eventos => _servicos.Eventos;

    /// <summary>Lê todos os cadastros da empresa do banco.</summary>
    /// <param name="empresa">Empresa ativa.</param>
    /// <param name="municipios">Municípios do IBGE.</param>
    /// <param name="servicos">Operações dos cadastros no banco.</param>
    public static async Task<CadastrosEmissao> CarregarAsync(Empresa empresa, CatalogoMunicipios municipios, ServicosCadastro servicos)
    {
        var cadastros = new CadastrosEmissao(empresa, municipios, servicos);
        cadastros.Participantes = await servicos.Participantes.ListarAsync(empresa.Id);
        cadastros.Veiculos = await servicos.Veiculos.ListarAsync(empresa.Id);
        cadastros.Proprietarios = await servicos.Proprietarios.ListarAsync(empresa.Id);
        cadastros.Motoristas = await servicos.Motoristas.ListarAsync(empresa.Id);
        cadastros.Apolices = await servicos.Apolices.ListarAsync(empresa.Id);
        cadastros.Series = await servicos.Series.ListarAsync(empresa.Id);
        return cadastros;
    }

    /// <summary>Relê os participantes (depois de incluir ou editar um dentro do assistente de CT-e).</summary>
    public async Task RecarregarParticipantesAsync() => Participantes = await _servicos.Participantes.ListarAsync(Empresa.Id);

    /// <summary>Séries do modelo no ambiente padrão da empresa.</summary>
    /// <param name="modelo">CT-e ou MDF-e.</param>
    public IEnumerable<SerieDocumento> SeriesDoAmbiente(ModeloDocumento modelo) =>
        Series.Where(s => s.Modelo == modelo && s.Ambiente == Empresa.AmbientePadrao).OrderBy(s => s.Serie);

    /// <summary>Participante do código, ou <c>null</c>.</summary>
    /// <param name="id">Código.</param>
    public Participante? Participante(int? id) => id is null ? null : Participantes.FirstOrDefault(p => p.Id == id);

    /// <summary>Razão social do participante; vazio sem código; <see cref="NaoEncontrado"/> se o código não existir.</summary>
    /// <param name="id">Código.</param>
    public string NomeParticipante(int? id) => id is null ? string.Empty : Participante(id)?.RazaoSocial ?? NaoEncontrado;

    /// <summary>Veículo do código, ou <c>null</c>.</summary>
    /// <param name="id">Código.</param>
    public Veiculo? Veiculo(int? id) => id is null ? null : Veiculos.FirstOrDefault(v => v.Id == id);

    /// <summary>Placa do veículo; vazio sem código; <see cref="NaoEncontrado"/> se o código não existir.</summary>
    /// <param name="id">Código.</param>
    public string Placa(int? id) => id is null ? string.Empty : Veiculo(id)?.Placa ?? NaoEncontrado;

    /// <summary>Motorista do código, ou <c>null</c>.</summary>
    /// <param name="id">Código.</param>
    public Motorista? Motorista(int? id) => id is null ? null : Motoristas.FirstOrDefault(m => m.Id == id);

    /// <summary>Apólice do código, ou <c>null</c>.</summary>
    /// <param name="id">Código.</param>
    public ApoliceSeguro? Apolice(int? id) => id is null ? null : Apolices.FirstOrDefault(a => a.Id == id);

    /// <summary>Série do código, ou <c>null</c>.</summary>
    /// <param name="id">Código.</param>
    public SerieDocumento? Serie(int? id) => id is null ? null : Series.FirstOrDefault(s => s.Id == id);

    /// <summary>Número da série; vazio sem código; <see cref="NaoEncontrado"/> se o código não existir.</summary>
    /// <param name="id">Código.</param>
    public string NumeroSerie(int? id) => id is null ? string.Empty : Serie(id)?.Serie.ToString() ?? NaoEncontrado;

    /// <summary>Município "Nome/UF", ou "?" se não houver.</summary>
    /// <param name="codigo">Código IBGE.</param>
    public string Municipio(int? codigo) => Municipios.Obter(codigo)?.Descricao ?? "?";

    /// <summary>UF do município, ou vazio.</summary>
    /// <param name="codigo">Código IBGE.</param>
    public string UfDoMunicipio(int? codigo) => Municipios.Obter(codigo)?.Uf ?? string.Empty;
}
