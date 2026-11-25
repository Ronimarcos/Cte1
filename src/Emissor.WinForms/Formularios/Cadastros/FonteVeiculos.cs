using Emissor.Dominio.Entidades;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de veículos.</summary>
internal sealed class LinhaVeiculo : LinhaCadastro
{
    /// <summary>Placa.</summary>
    public string Placa { get; init; } = string.Empty;

    /// <summary>Código interno.</summary>
    public string CodigoInterno { get; init; } = string.Empty;

    /// <summary>Tração ou reboque.</summary>
    public string TipoDescricao { get; init; } = string.Empty;

    /// <summary>Tipo de rodado.</summary>
    public string TipoRodadoDescricao { get; init; } = string.Empty;

    /// <summary>Tipo de carroceria.</summary>
    public string TipoCarroceriaDescricao { get; init; } = string.Empty;

    /// <summary>Tara em kg.</summary>
    public int TaraKg { get; init; }

    /// <summary>Capacidade em kg.</summary>
    public int? CapacidadeKg { get; init; }

    /// <summary>"Próprio" ou o nome do proprietário.</summary>
    public string ProprietarioDescricao { get; init; } = string.Empty;

    /// <inheritdoc />
    public override string TextoPesquisa => $"{Placa} {CodigoInterno} {ProprietarioDescricao}";

    /// <inheritdoc />
    public override string Descricao => Placa;
}

/// <summary>Cadastro de veículos (tração e reboque) da empresa ativa, gravado no banco.</summary>
internal sealed class FonteVeiculos : FonteCadastroBanco<Veiculo, LinhaVeiculo>
{
    private readonly ServicoCadastro<ProprietarioVeiculo> _proprietarios;

    /// <summary>Cria o cadastro de veículos da empresa informada.</summary>
    /// <param name="servico">Operações no banco.</param>
    /// <param name="proprietarios">Proprietários de veículos de terceiros.</param>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteVeiculos(ServicoCadastro<Veiculo> servico, ServicoCadastro<ProprietarioVeiculo> proprietarios, int empresaId)
        : base(servico, empresaId) => _proprietarios = proprietarios;

    /// <inheritdoc />
    public override string Titulo => "Veículos";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaVeiculo.Placa), "Placa", 10),
        new(nameof(LinhaVeiculo.TipoDescricao), "Tipo", 10),
        new(nameof(LinhaVeiculo.TipoRodadoDescricao), "Rodado", 18),
        new(nameof(LinhaVeiculo.TipoCarroceriaDescricao), "Carroceria", 18),
        new(nameof(LinhaVeiculo.TaraKg), "Tara (kg)", 10),
        new(nameof(LinhaVeiculo.CapacidadeKg), "Capacidade (kg)", 12),
        new(nameof(LinhaVeiculo.ProprietarioDescricao), "Proprietário", 22),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o veículo";

    /// <inheritdoc />
    protected override async Task<IReadOnlyList<LinhaVeiculo>> CriarLinhasAsync(IReadOnlyList<Veiculo> registros)
    {
        var nomes = (await _proprietarios.ListarAsync(EmpresaId)).ToDictionary(p => p.Id, p => p.Nome);
        return registros
            .OrderBy(v => v.Placa)
            .Select(v => new LinhaVeiculo
            {
                Id = v.Id,
                Placa = v.Placa,
                CodigoInterno = v.CodigoInterno,
                TipoDescricao = OpcoesCadastro.Texto(OpcoesCadastro.TiposVeiculo, v.Tipo),
                TipoRodadoDescricao = OpcoesCadastro.Texto(OpcoesCadastro.TiposRodado, v.TipoRodado),
                TipoCarroceriaDescricao = OpcoesCadastro.Texto(OpcoesCadastro.TiposCarroceria, v.TipoCarroceria),
                TaraKg = v.TaraKg,
                CapacidadeKg = v.CapacidadeKg,
                ProprietarioDescricao = v.Proprio ? "Próprio"
                    : v.ProprietarioId is { } id && nomes.TryGetValue(id, out var nome) ? nome : string.Empty,
            })
            .ToList();
    }

    /// <inheritdoc />
    protected override Veiculo CriarNovo() => new() { Uf = "MT" };

    /// <inheritdoc />
    protected override async Task<bool> AbrirEdicaoAsync(IWin32Window dono, Veiculo registro, bool novo, Func<Veiculo, Task> gravar)
    {
        var proprietarios = (await _proprietarios.ListarAsync(EmpresaId))
            .OrderBy(p => p.Nome)
            .Select(p => new Opcao<int>(p.Id, $"{p.Nome} - {Formatador.CnpjCpf(p.CnpjCpf)}"))
            .ToList();

        using var tela = new FormEdicaoVeiculo(registro, proprietarios, novo, gravar);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }
}
