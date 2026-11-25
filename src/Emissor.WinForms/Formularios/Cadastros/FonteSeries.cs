using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de séries.</summary>
internal sealed class LinhaSerie : LinhaCadastro
{
    /// <summary>Modelo.</summary>
    public string ModeloDescricao { get; init; } = string.Empty;

    /// <summary>Série.</summary>
    public int Serie { get; init; }

    /// <summary>Ambiente.</summary>
    public string AmbienteDescricao { get; init; } = string.Empty;

    /// <summary>Próximo número.</summary>
    public int ProximoNumero { get; init; }

    /// <inheritdoc />
    public override string TextoPesquisa => $"{ModeloDescricao} {Serie} {AmbienteDescricao}";

    /// <inheritdoc />
    public override string Descricao => $"{ModeloDescricao}, série {Serie}, {AmbienteDescricao}";
}

/// <summary>
/// Cadastro de séries de numeração (CT-e e MDF-e) da empresa ativa, gravado no banco.
/// A série repetida (mesmo modelo, número e ambiente) é recusada pelo banco.
/// </summary>
internal sealed class FonteSeries : FonteCadastroBanco<SerieDocumento, LinhaSerie>
{
    /// <summary>Cria o cadastro de séries da empresa informada.</summary>
    /// <param name="servico">Operações no banco.</param>
    /// <param name="empresaId">Empresa ativa.</param>
    public FonteSeries(ServicoCadastro<SerieDocumento> servico, int empresaId)
        : base(servico, empresaId)
    {
    }

    /// <inheritdoc />
    public override string Titulo => "Séries de documentos";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(LinhaSerie.ModeloDescricao), "Modelo", 25),
        new(nameof(LinhaSerie.Serie), "Série", 15),
        new(nameof(LinhaSerie.AmbienteDescricao), "Ambiente", 25),
        new(nameof(LinhaSerie.ProximoNumero), "Próximo número", 20),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "a série";

    /// <inheritdoc />
    protected override Task<IReadOnlyList<LinhaSerie>> CriarLinhasAsync(IReadOnlyList<SerieDocumento> registros) =>
        Task.FromResult<IReadOnlyList<LinhaSerie>>(registros
            .OrderBy(s => s.Modelo).ThenBy(s => s.Ambiente).ThenBy(s => s.Serie)
            .Select(s => new LinhaSerie
            {
                Id = s.Id,
                ModeloDescricao = OpcoesCadastro.Texto(OpcoesCadastro.ModelosDocumento, s.Modelo),
                Serie = s.Serie,
                AmbienteDescricao = OpcoesCadastro.Texto(OpcoesCadastro.Ambientes, s.Ambiente == TipoAmbiente.Homologacao),
                ProximoNumero = s.ProximoNumero,
            })
            .ToList());

    /// <inheritdoc />
    protected override SerieDocumento CriarNovo() => new();

    /// <inheritdoc />
    protected override Task<bool> AbrirEdicaoAsync(IWin32Window dono, SerieDocumento registro, bool novo, Func<SerieDocumento, Task> gravar)
    {
        using var tela = new FormEdicaoSerie(registro, novo, gravar);
        return Task.FromResult(tela.ShowDialog(dono) == DialogResult.OK);
    }
}
