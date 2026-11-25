using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de certificados.</summary>
internal sealed class LinhaCertificado : LinhaCadastro
{
    /// <summary>A1 ou A3.</summary>
    public string TipoDescricao { get; init; } = string.Empty;

    /// <summary>Titular.</summary>
    public string Titular { get; init; } = string.Empty;

    /// <summary>CNPJ/CPF do titular formatado.</summary>
    public string CnpjTitular { get; init; } = string.Empty;

    /// <summary>Validade "de a até".</summary>
    public string ValidadeDescricao { get; init; } = string.Empty;

    /// <summary>Situação para exibição.</summary>
    public string SituacaoDescricao { get; init; } = string.Empty;

    /// <summary>Situação (para a cor da linha).</summary>
    public SituacaoCertificado Situacao { get; init; }

    /// <inheritdoc />
    public override string TextoPesquisa => $"{TipoDescricao} {Titular} {CnpjTitular} {SituacaoDescricao}";

    /// <inheritdoc />
    public override string Descricao => $"{TipoDescricao} - {Titular}";
}

/// <summary>
/// Cadastro de certificados digitais da empresa ativa, gravado no banco, com destaque para os vencidos e os que
/// vencem em até 30 dias.
/// </summary>
internal sealed class FonteCertificados : FonteCadastroBanco<CertificadoDigital, LinhaCertificado>
{
    private readonly IProtetorSegredos _protetor;
    private readonly Empresa _empresa;

    /// <summary>Cria o cadastro de certificados da empresa informada.</summary>
    /// <param name="servico">Operações no banco.</param>
    /// <param name="protetor">Criptografia do arquivo e da senha do A1.</param>
    /// <param name="empresa">Empresa ativa (o certificado precisa ser dela).</param>
    public FonteCertificados(ServicoCadastro<CertificadoDigital> servico, IProtetorSegredos protetor, Empresa empresa)
        : base(servico, empresa.Id)
    {
        _protetor = protetor;
        _empresa = empresa;
    }

    /// <inheritdoc />
    public override string Titulo => "Certificados digitais";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaCertificado.TipoDescricao), "Tipo", 18),
        new(nameof(LinhaCertificado.Titular), "Titular", 36),
        new(nameof(LinhaCertificado.CnpjTitular), "CNPJ do titular", 18),
        new(nameof(LinhaCertificado.ValidadeDescricao), "Validade", 22),
        new(nameof(LinhaCertificado.SituacaoDescricao), "Situação", 16),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o certificado";

    /// <inheritdoc />
    protected override Task<IReadOnlyList<LinhaCertificado>> CriarLinhasAsync(IReadOnlyList<CertificadoDigital> registros) =>
        Task.FromResult<IReadOnlyList<LinhaCertificado>>(registros
            .OrderByDescending(c => c.ValidoAte)
            .Select(c =>
            {
                var situacao = c.SituacaoEm(DateTime.Today);
                return new LinhaCertificado
                {
                    Id = c.Id,
                    TipoDescricao = OpcoesCadastro.Texto(OpcoesCadastro.TiposCertificado, c.Tipo),
                    Titular = c.Titular,
                    CnpjTitular = Formatador.CnpjCpf(c.CnpjTitular),
                    ValidadeDescricao = $"{c.ValidoDe:dd/MM/yyyy} a {c.ValidoAte:dd/MM/yyyy}",
                    Situacao = situacao,
                    SituacaoDescricao = DescreverSituacao(c, situacao),
                };
            })
            .ToList());

    /// <inheritdoc />
    protected override CertificadoDigital CriarNovo() => new();

    /// <inheritdoc />
    protected override Task<bool> AbrirEdicaoAsync(IWin32Window dono, CertificadoDigital registro, bool novo, Func<CertificadoDigital, Task> gravar)
    {
        using var tela = new FormEdicaoCertificado(registro, novo, _empresa, _protetor, gravar);
        return Task.FromResult(tela.ShowDialog(dono) == DialogResult.OK);
    }

    /// <inheritdoc />
    protected override Color? CorDoRegistro(LinhaCertificado linha) => linha.Situacao switch
    {
        SituacaoCertificado.Vencido => Color.MistyRose,
        SituacaoCertificado.VencendoEmBreve => Color.LightGoldenrodYellow,
        _ => null,
    };

    private static string DescreverSituacao(CertificadoDigital certificado, SituacaoCertificado situacao)
    {
        var dias = (certificado.ValidoAte.Date - DateTime.Today).Days;
        return situacao switch
        {
            SituacaoCertificado.Vencido => "Vencido",
            SituacaoCertificado.AindaNaoValido => "Ainda não vale",
            SituacaoCertificado.VencendoEmBreve => dias switch { 0 => "Vence hoje", 1 => "Vence amanhã", _ => $"Vence em {dias} dias" },
            _ => "Válido",
        };
    }
}
