using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de motoristas (condutores do MDF-e) da empresa ativa.</summary>
internal sealed class FonteMotoristas : FonteCadastro<MotoristaPrototipo>
{
    private readonly int _empresaId;

    /// <summary>Cria o cadastro de motoristas da empresa informada.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteMotoristas(int empresaId)
        : base(DadosFicticios.Motoristas) => _empresaId = empresaId;

    /// <inheritdoc />
    public override string Titulo => "Motoristas";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(MotoristaPrototipo.Nome), "Nome", 50),
        new(nameof(MotoristaPrototipo.Cpf), "CPF", 20),
        new(nameof(MotoristaPrototipo.Telefone), "Telefone", 20),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o motorista";

    /// <inheritdoc />
    protected override bool PertenceALista(MotoristaPrototipo item) => item.EmpresaId == _empresaId;

    /// <inheritdoc />
    protected override string TextoPesquisa(MotoristaPrototipo item) => $"{item.Nome} {item.Cpf} {item.Telefone}";

    /// <inheritdoc />
    protected override string Descrever(MotoristaPrototipo item) => item.Nome;

    /// <inheritdoc />
    protected override MotoristaPrototipo CriarNovo() => new() { EmpresaId = _empresaId };

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, MotoristaPrototipo item, bool novo)
    {
        using var tela = new FormEdicaoMotorista(item, novo);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }
}
