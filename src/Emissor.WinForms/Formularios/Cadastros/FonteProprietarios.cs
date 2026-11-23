using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de proprietários de veículos de terceiros da empresa ativa.</summary>
internal sealed class FonteProprietarios : FonteCadastro<ProprietarioVeiculoPrototipo>
{
    private readonly int _empresaId;

    /// <summary>Cria o cadastro de proprietários da empresa informada.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteProprietarios(int empresaId)
        : base(DadosFicticios.Proprietarios) => _empresaId = empresaId;

    /// <inheritdoc />
    public override string Titulo => "Proprietários de veículos";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(ProprietarioVeiculoPrototipo.Nome), "Nome ou razão social", 40),
        new(nameof(ProprietarioVeiculoPrototipo.CnpjCpf), "CNPJ/CPF", 20),
        new(nameof(ProprietarioVeiculoPrototipo.Rntrc), "RNTRC", 14),
        new(nameof(ProprietarioVeiculoPrototipo.TipoProprietarioDescricao), "Tipo", 26),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o proprietário";

    /// <inheritdoc />
    protected override bool PertenceALista(ProprietarioVeiculoPrototipo item) => item.EmpresaId == _empresaId;

    /// <inheritdoc />
    protected override string TextoPesquisa(ProprietarioVeiculoPrototipo item) => $"{item.Nome} {item.CnpjCpf} {item.Rntrc}";

    /// <inheritdoc />
    protected override string Descrever(ProprietarioVeiculoPrototipo item) => item.Nome;

    /// <inheritdoc />
    protected override ProprietarioVeiculoPrototipo CriarNovo() => new() { EmpresaId = _empresaId };

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, ProprietarioVeiculoPrototipo item, bool novo)
    {
        using var tela = new FormEdicaoProprietarioVeiculo(item, novo);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }

    /// <inheritdoc />
    protected override string? MotivoParaNaoExcluir(ProprietarioVeiculoPrototipo item) =>
        DadosFicticios.Veiculos.Existe(v => !v.Proprio && v.ProprietarioId == item.Id)
            ? "O proprietário está vinculado a veículos e não pode ser excluído."
            : null;
}
