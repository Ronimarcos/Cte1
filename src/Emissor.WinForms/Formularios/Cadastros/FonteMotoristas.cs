using Emissor.Dominio.Entidades;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de motoristas.</summary>
internal sealed class LinhaMotorista : LinhaCadastro
{
    /// <summary>Nome.</summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>CPF formatado.</summary>
    public string Cpf { get; init; } = string.Empty;

    /// <summary>Telefone.</summary>
    public string Telefone { get; init; } = string.Empty;

    /// <inheritdoc />
    public override string TextoPesquisa => $"{Nome} {Cpf} {Telefone}";

    /// <inheritdoc />
    public override string Descricao => Nome;
}

/// <summary>Cadastro de motoristas (condutores do MDF-e) da empresa ativa, gravado no banco.</summary>
internal sealed class FonteMotoristas : FonteCadastroBanco<Motorista, LinhaMotorista>
{
    /// <summary>Cria o cadastro de motoristas da empresa informada.</summary>
    /// <param name="servico">Operações no banco.</param>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteMotoristas(ServicoCadastro<Motorista> servico, int empresaId)
        : base(servico, empresaId)
    {
    }

    /// <inheritdoc />
    public override string Titulo => "Motoristas";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaMotorista.Nome), "Nome", 50),
        new(nameof(LinhaMotorista.Cpf), "CPF", 20),
        new(nameof(LinhaMotorista.Telefone), "Telefone", 20),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o motorista";

    /// <inheritdoc />
    protected override Task<IReadOnlyList<LinhaMotorista>> CriarLinhasAsync(IReadOnlyList<Motorista> registros) =>
        Task.FromResult<IReadOnlyList<LinhaMotorista>>(registros
            .OrderBy(m => m.Nome)
            .Select(m => new LinhaMotorista { Id = m.Id, Nome = m.Nome, Cpf = Formatador.Cpf(m.Cpf), Telefone = m.Telefone })
            .ToList());

    /// <inheritdoc />
    protected override Motorista CriarNovo() => new();

    /// <inheritdoc />
    protected override Task<bool> AbrirEdicaoAsync(IWin32Window dono, Motorista registro, bool novo, Func<Motorista, Task> gravar)
    {
        using var tela = new FormEdicaoMotorista(registro, novo, gravar);
        return Task.FromResult(tela.ShowDialog(dono) == DialogResult.OK);
    }
}
