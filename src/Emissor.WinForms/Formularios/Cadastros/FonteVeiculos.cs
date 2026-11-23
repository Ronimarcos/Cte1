using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de veículos (tração e reboque) da empresa ativa.</summary>
internal sealed class FonteVeiculos : FonteCadastro<VeiculoPrototipo>
{
    private readonly int _empresaId;

    /// <summary>Cria o cadastro de veículos da empresa informada.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteVeiculos(int empresaId)
        : base(DadosFicticios.Veiculos) => _empresaId = empresaId;

    /// <inheritdoc />
    public override string Titulo => "Veículos";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(VeiculoPrototipo.Placa), "Placa", 10),
        new(nameof(VeiculoPrototipo.TipoDescricao), "Tipo", 10),
        new(nameof(VeiculoPrototipo.TipoRodadoDescricao), "Rodado", 18),
        new(nameof(VeiculoPrototipo.TipoCarroceriaDescricao), "Carroceria", 18),
        new(nameof(VeiculoPrototipo.TaraKg), "Tara (kg)", 10),
        new(nameof(VeiculoPrototipo.CapacidadeKg), "Capacidade (kg)", 12),
        new(nameof(VeiculoPrototipo.ProprietarioDescricao), "Proprietário", 22),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o veículo";

    /// <inheritdoc />
    protected override bool PertenceALista(VeiculoPrototipo item) => item.EmpresaId == _empresaId;

    /// <inheritdoc />
    protected override string TextoPesquisa(VeiculoPrototipo item) =>
        $"{item.Placa} {item.CodigoInterno} {item.Renavam} {item.ProprietarioDescricao}";

    /// <inheritdoc />
    protected override string Descrever(VeiculoPrototipo item) => item.Placa;

    /// <inheritdoc />
    protected override VeiculoPrototipo CriarNovo() => new() { EmpresaId = _empresaId, Uf = "MT" };

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, VeiculoPrototipo item, bool novo)
    {
        var proprietarios = DadosFicticios.Proprietarios
            .Listar(p => p.EmpresaId == _empresaId)
            .Select(p => new Opcao<int>(p.Id, $"{p.Nome} - {p.CnpjCpf}"));

        using var tela = new FormEdicaoVeiculo(item, proprietarios, novo);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }
}
