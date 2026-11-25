using Emissor.Dominio.Entidades;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de proprietários de veículos.</summary>
internal sealed class LinhaProprietario : LinhaCadastro
{
    /// <summary>Nome ou razão social.</summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>CNPJ/CPF formatado.</summary>
    public string CnpjCpf { get; init; } = string.Empty;

    /// <summary>RNTRC.</summary>
    public string Rntrc { get; init; } = string.Empty;

    /// <summary>Tipo do proprietário.</summary>
    public string TipoProprietarioDescricao { get; init; } = string.Empty;

    /// <inheritdoc />
    public override string TextoPesquisa => $"{Nome} {CnpjCpf} {Rntrc}";

    /// <inheritdoc />
    public override string Descricao => Nome;
}

/// <summary>
/// Cadastro de proprietários de veículos de terceiros da empresa ativa, gravado no banco.
/// O banco recusa excluir proprietário vinculado a veículo.
/// </summary>
internal sealed class FonteProprietarios : FonteCadastroBanco<ProprietarioVeiculo, LinhaProprietario>
{
    /// <summary>Cria o cadastro de proprietários da empresa informada.</summary>
    /// <param name="servico">Operações no banco.</param>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteProprietarios(ServicoCadastro<ProprietarioVeiculo> servico, int empresaId)
        : base(servico, empresaId)
    {
    }

    /// <inheritdoc />
    public override string Titulo => "Proprietários de veículos";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaProprietario.Nome), "Nome ou razão social", 40),
        new(nameof(LinhaProprietario.CnpjCpf), "CNPJ/CPF", 20),
        new(nameof(LinhaProprietario.Rntrc), "RNTRC", 14),
        new(nameof(LinhaProprietario.TipoProprietarioDescricao), "Tipo", 26),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o proprietário";

    /// <inheritdoc />
    protected override Task<IReadOnlyList<LinhaProprietario>> CriarLinhasAsync(IReadOnlyList<ProprietarioVeiculo> registros) =>
        Task.FromResult<IReadOnlyList<LinhaProprietario>>(registros
            .OrderBy(p => p.Nome)
            .Select(p => new LinhaProprietario
            {
                Id = p.Id,
                Nome = p.Nome,
                CnpjCpf = Formatador.CnpjCpf(p.CnpjCpf),
                Rntrc = p.Rntrc,
                TipoProprietarioDescricao = OpcoesCadastro.Texto(OpcoesCadastro.TiposProprietario, p.TipoProprietario),
            })
            .ToList());

    /// <inheritdoc />
    protected override ProprietarioVeiculo CriarNovo() => new();

    /// <inheritdoc />
    protected override Task<bool> AbrirEdicaoAsync(IWin32Window dono, ProprietarioVeiculo registro, bool novo, Func<ProprietarioVeiculo, Task> gravar)
    {
        using var tela = new FormEdicaoProprietarioVeiculo(registro, novo, gravar);
        return Task.FromResult(tela.ShowDialog(dono) == DialogResult.OK);
    }
}
