using Emissor.Dominio.Entidades;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de apólices de seguro.</summary>
internal sealed class LinhaApolice : LinhaCadastro
{
    /// <summary>Número da apólice.</summary>
    public string NumeroApolice { get; init; } = string.Empty;

    /// <summary>Nome da seguradora.</summary>
    public string NomeSeguradora { get; init; } = string.Empty;

    /// <summary>CNPJ da seguradora formatado.</summary>
    public string CnpjSeguradora { get; init; } = string.Empty;

    /// <summary>Responsável pelo seguro.</summary>
    public string ResponsavelDescricao { get; init; } = string.Empty;

    /// <summary>CNPJ/CPF do contratante formatado.</summary>
    public string CnpjCpfResponsavel { get; init; } = string.Empty;

    /// <inheritdoc />
    public override string TextoPesquisa => $"{NumeroApolice} {NomeSeguradora} {CnpjSeguradora} {CnpjCpfResponsavel}";

    /// <inheritdoc />
    public override string Descricao => $"{NumeroApolice} - {NomeSeguradora}";
}

/// <summary>Cadastro de apólices de seguro da carga da empresa ativa, gravado no banco.</summary>
internal sealed class FonteApolices : FonteCadastroBanco<ApoliceSeguro, LinhaApolice>
{
    /// <summary>Cria o cadastro de apólices da empresa informada.</summary>
    /// <param name="servico">Operações no banco.</param>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteApolices(ServicoCadastro<ApoliceSeguro> servico, int empresaId)
        : base(servico, empresaId)
    {
    }

    /// <inheritdoc />
    public override string Titulo => "Apólices de seguro";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaApolice.NumeroApolice), "Apólice", 16),
        new(nameof(LinhaApolice.NomeSeguradora), "Seguradora", 30),
        new(nameof(LinhaApolice.CnpjSeguradora), "CNPJ da seguradora", 18),
        new(nameof(LinhaApolice.ResponsavelDescricao), "Responsável", 24),
        new(nameof(LinhaApolice.CnpjCpfResponsavel), "CNPJ/CPF do responsável", 18),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "a apólice";

    /// <inheritdoc />
    protected override Task<IReadOnlyList<LinhaApolice>> CriarLinhasAsync(IReadOnlyList<ApoliceSeguro> registros) =>
        Task.FromResult<IReadOnlyList<LinhaApolice>>(registros
            .OrderBy(a => a.NumeroApolice)
            .Select(a => new LinhaApolice
            {
                Id = a.Id,
                NumeroApolice = a.NumeroApolice,
                NomeSeguradora = a.NomeSeguradora,
                CnpjSeguradora = Formatador.Cnpj(a.CnpjSeguradora),
                ResponsavelDescricao = OpcoesCadastro.Texto(OpcoesCadastro.ResponsaveisSeguro, a.Responsavel),
                CnpjCpfResponsavel = Formatador.CnpjCpf(a.CnpjCpfResponsavel),
            })
            .ToList());

    /// <inheritdoc />
    protected override ApoliceSeguro CriarNovo() => new();

    /// <inheritdoc />
    protected override Task<bool> AbrirEdicaoAsync(IWin32Window dono, ApoliceSeguro registro, bool novo, Func<ApoliceSeguro, Task> gravar)
    {
        using var tela = new FormEdicaoApolice(registro, novo, gravar);
        return Task.FromResult(tela.ShowDialog(dono) == DialogResult.OK);
    }
}
