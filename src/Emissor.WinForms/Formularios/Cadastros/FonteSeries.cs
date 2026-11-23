using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de séries de numeração (CT-e e MDF-e) da empresa ativa.</summary>
internal sealed class FonteSeries : FonteCadastro<SeriePrototipo>
{
    private readonly int _empresaId;

    /// <summary>Cria o cadastro de séries da empresa informada.</summary>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteSeries(int empresaId)
        : base(DadosFicticios.Series) => _empresaId = empresaId;

    /// <inheritdoc />
    public override string Titulo => "Séries de documentos";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(SeriePrototipo.ModeloDescricao), "Modelo", 25),
        new(nameof(SeriePrototipo.Serie), "Série", 15),
        new(nameof(SeriePrototipo.AmbienteDescricao), "Ambiente", 25),
        new(nameof(SeriePrototipo.ProximoNumero), "Próximo número", 20),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "a série";

    /// <inheritdoc />
    protected override bool PertenceALista(SeriePrototipo item) => item.EmpresaId == _empresaId;

    /// <inheritdoc />
    protected override string TextoPesquisa(SeriePrototipo item) =>
        $"{item.ModeloDescricao} {item.Serie} {item.AmbienteDescricao}";

    /// <inheritdoc />
    protected override string Descrever(SeriePrototipo item) => $"{item.ModeloDescricao}, série {item.Serie}, {item.AmbienteDescricao}";

    /// <inheritdoc />
    protected override SeriePrototipo CriarNovo() => new() { EmpresaId = _empresaId };

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, SeriePrototipo item, bool novo)
    {
        // Outra série da empresa com o mesmo modelo, número e ambiente impede salvar.
        bool Duplicada(SeriePrototipo serie) => Repositorio.Existe(s => s.Id != serie.Id && s.MesmaChave(serie));

        using var tela = new FormEdicaoSerie(item, novo, Duplicada);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }
}
