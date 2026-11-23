using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de apólices de seguro da carga da empresa ativa.</summary>
internal sealed class FonteApolices : FonteCadastro<ApolicePrototipo>
{
    private readonly int _empresaId;

    /// <summary>Cria o cadastro de apólices da empresa informada.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteApolices(int empresaId)
        : base(DadosFicticios.Apolices) => _empresaId = empresaId;

    /// <inheritdoc />
    public override string Titulo => "Apólices de seguro";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(ApolicePrototipo.NumeroApolice), "Apólice", 16),
        new(nameof(ApolicePrototipo.NomeSeguradora), "Seguradora", 30),
        new(nameof(ApolicePrototipo.CnpjSeguradora), "CNPJ da seguradora", 18),
        new(nameof(ApolicePrototipo.ResponsavelDescricao), "Responsável", 24),
        new(nameof(ApolicePrototipo.CnpjCpfResponsavel), "CNPJ/CPF do responsável", 18),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "a apólice";

    /// <inheritdoc />
    protected override bool PertenceALista(ApolicePrototipo item) => item.EmpresaId == _empresaId;

    /// <inheritdoc />
    protected override string TextoPesquisa(ApolicePrototipo item) =>
        $"{item.NumeroApolice} {item.NomeSeguradora} {item.CnpjSeguradora} {item.CnpjCpfResponsavel}";

    /// <inheritdoc />
    protected override string Descrever(ApolicePrototipo item) => $"{item.NumeroApolice} - {item.NomeSeguradora}";

    /// <inheritdoc />
    protected override ApolicePrototipo CriarNovo() => new() { EmpresaId = _empresaId };

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, ApolicePrototipo item, bool novo)
    {
        using var tela = new FormEdicaoApolice(item, novo);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }
}
