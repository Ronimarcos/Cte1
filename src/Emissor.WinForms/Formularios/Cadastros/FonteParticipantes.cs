using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de participantes da empresa ativa.</summary>
internal sealed class FonteParticipantes : FonteCadastro<ParticipantePrototipo>
{
    private readonly int _empresaId;

    /// <summary>Cria o cadastro de participantes da empresa informada.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteParticipantes(int empresaId)
        : base(DadosFicticios.Participantes) => _empresaId = empresaId;

    /// <inheritdoc />
    public override string Titulo => "Participantes";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(ParticipantePrototipo.RazaoSocial), "Razão social ou nome", 36),
        new(nameof(ParticipantePrototipo.CnpjCpf), "CNPJ/CPF", 16),
        new(nameof(ParticipantePrototipo.IndicadorIeDescricao), "Indicador da IE", 18),
        new(nameof(ParticipantePrototipo.MunicipioDescricao), "Município", 18),
        new(nameof(ParticipantePrototipo.Fone), "Telefone", 12),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "o participante";

    /// <inheritdoc />
    protected override bool PertenceALista(ParticipantePrototipo item) => item.EmpresaId == _empresaId;

    /// <inheritdoc />
    protected override string TextoPesquisa(ParticipantePrototipo item) =>
        $"{item.RazaoSocial} {item.NomeFantasia} {item.CnpjCpf} {item.MunicipioDescricao}";

    /// <inheritdoc />
    protected override string Descrever(ParticipantePrototipo item) => item.RazaoSocial;

    /// <inheritdoc />
    protected override ParticipantePrototipo CriarNovo() => new() { EmpresaId = _empresaId };

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, ParticipantePrototipo item, bool novo)
    {
        using var tela = new FormEdicaoParticipante(item, novo);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }
}
