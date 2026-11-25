using Emissor.Dominio.Entidades;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de participantes.</summary>
internal sealed class LinhaParticipante : LinhaCadastro
{
    /// <summary>Razão social ou nome.</summary>
    public string RazaoSocial { get; init; } = string.Empty;

    /// <summary>Nome fantasia.</summary>
    public string NomeFantasia { get; init; } = string.Empty;

    /// <summary>CNPJ/CPF formatado.</summary>
    public string CnpjCpf { get; init; } = string.Empty;

    /// <summary>Indicador da IE.</summary>
    public string IndicadorIeDescricao { get; init; } = string.Empty;

    /// <summary>Município "Nome/UF".</summary>
    public string MunicipioDescricao { get; init; } = string.Empty;

    /// <summary>Telefone.</summary>
    public string Fone { get; init; } = string.Empty;

    /// <inheritdoc />
    public override string TextoPesquisa => $"{RazaoSocial} {NomeFantasia} {CnpjCpf} {MunicipioDescricao}";

    /// <inheritdoc />
    public override string Descricao => RazaoSocial;
}

/// <summary>Cadastro de participantes da empresa ativa, gravado no banco.</summary>
internal sealed class FonteParticipantes : FonteCadastroBanco<Participante, LinhaParticipante>
{
    private readonly CatalogoMunicipios _municipios;

    /// <summary>Cria o cadastro de participantes da empresa informada.</summary>
    /// <param name="servico">Operações no banco.</param>
    /// <param name="municipios">Municípios do IBGE.</param>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteParticipantes(ServicoCadastro<Participante> servico, CatalogoMunicipios municipios, int empresaId)
        : base(servico, empresaId) => _municipios = municipios;

    /// <inheritdoc />
    public override string Titulo => "Participantes";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaParticipante.RazaoSocial), "Razão social ou nome", 36),
        new(nameof(LinhaParticipante.CnpjCpf), "CNPJ/CPF", 16),
        new(nameof(LinhaParticipante.IndicadorIeDescricao), "Indicador da IE", 18),
        new(nameof(LinhaParticipante.MunicipioDescricao), "Município", 18),
        new(nameof(LinhaParticipante.Fone), "Telefone", 12),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o participante";

    /// <inheritdoc />
    protected override Task<IReadOnlyList<LinhaParticipante>> CriarLinhasAsync(IReadOnlyList<Participante> registros) =>
        Task.FromResult<IReadOnlyList<LinhaParticipante>>(registros
            .OrderBy(p => p.RazaoSocial)
            .Select(p => new LinhaParticipante
            {
                Id = p.Id,
                RazaoSocial = p.RazaoSocial,
                NomeFantasia = p.NomeFantasia,
                CnpjCpf = Formatador.CnpjCpf(p.CnpjCpf),
                IndicadorIeDescricao = OpcoesCadastro.Texto(OpcoesCadastro.IndicadoresIe, p.IndicadorIe),
                MunicipioDescricao = _municipios.Descricao(p.Endereco.MunicipioCodigo),
                Fone = p.Fone,
            })
            .ToList());

    /// <inheritdoc />
    protected override Participante CriarNovo() => new();

    /// <inheritdoc />
    protected override Task<bool> AbrirEdicaoAsync(IWin32Window dono, Participante registro, bool novo, Func<Participante, Task> gravar)
    {
        using var tela = new FormEdicaoParticipante(registro, novo, _municipios.Opcoes, gravar);
        return Task.FromResult(tela.ShowDialog(dono) == DialogResult.OK);
    }
}
