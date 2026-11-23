using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de certificados digitais da empresa ativa, com destaque para vencidos e próximos do vencimento.</summary>
internal sealed class FonteCertificados : FonteCadastro<CertificadoPrototipo>
{
    private readonly int _empresaId;

    /// <summary>Cria o cadastro de certificados da empresa informada.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteCertificados(int empresaId)
        : base(DadosFicticios.Certificados) => _empresaId = empresaId;

    /// <inheritdoc />
    public override string Titulo => "Certificados digitais";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(CertificadoPrototipo.TipoDescricao), "Tipo", 18),
        new(nameof(CertificadoPrototipo.Titular), "Titular", 36),
        new(nameof(CertificadoPrototipo.CnpjTitular), "CNPJ do titular", 18),
        new(nameof(CertificadoPrototipo.ValidadeDescricao), "Validade", 22),
        new(nameof(CertificadoPrototipo.SituacaoDescricao), "Situação", 16),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o certificado";

    /// <inheritdoc />
    protected override bool PertenceALista(CertificadoPrototipo item) => item.EmpresaId == _empresaId;

    /// <inheritdoc />
    protected override string TextoPesquisa(CertificadoPrototipo item) =>
        $"{item.TipoDescricao} {item.Titular} {item.CnpjTitular} {item.SituacaoDescricao}";

    /// <inheritdoc />
    protected override string Descrever(CertificadoPrototipo item) => $"{item.TipoDescricao} - {item.Titular}";

    /// <inheritdoc />
    protected override CertificadoPrototipo CriarNovo() => new() { EmpresaId = _empresaId };

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, CertificadoPrototipo item, bool novo)
    {
        using var tela = new FormEdicaoCertificado(item, novo);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }

    /// <inheritdoc />
    protected override Color? CorDoRegistro(CertificadoPrototipo item) => item.Situacao switch
    {
        SituacaoCertificado.Vencido => Color.MistyRose,
        SituacaoCertificado.VencendoEmBreve => Color.LightGoldenrodYellow,
        _ => null,
    };
}
