using System.ComponentModel;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>Etapa 1: identificação do MDF-e (grupo <c>ide</c>): emitente, série, UFs, percurso e datas.</summary>
public partial class UcMdfeIdentificacao : UserControl, IEtapaMdfe
{
    /// <summary>Valor da opção "(não informado)" do tipo do transportador.</summary>
    private const int NaoInformado = 0;

    /// <summary>Cria a etapa.</summary>
    public UcMdfeIdentificacao()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "1. Identificação";

    /// <inheritdoc />
    void IEtapaMdfe.Carregar(MdfePrototipo mdfe, EmpresaPrototipo empresa)
    {
        cboTipoEmitente.Carregar(OpcoesCadastro.TiposEmitenteMdfe);
        var transportadores = new List<Opcao<int>> { new(NaoInformado, "(não informado)") };
        transportadores.AddRange(OpcoesCadastro.TiposTransportador.Select(o => new Opcao<int>((int)o.Valor, o.Texto)));
        cboTipoTransportador.Carregar(transportadores);

        var series = DadosFicticios.Series
            .Listar(s => s.EmpresaId == empresa.Id && s.Modelo == ModeloDocumento.Mdfe && s.Homologacao == empresa.Homologacao)
            .Select(s => new Opcao<int>(s.Id, $"Série {s.Serie} ({(s.Homologacao ? "homologação" : "produção")})"));
        cboSerie.Carregar(series);

        cboUfInicio.Items.AddRange([.. OpcoesCadastro.Ufs]);
        cboUfFim.Items.AddRange([.. OpcoesCadastro.Ufs]);

        cboTipoEmitente.Selecionar(mdfe.TipoEmitente);
        cboTipoTransportador.Selecionar<int>(mdfe.TipoTransportador is { } tipo ? (int)tipo : NaoInformado);
        cboSerie.Selecionar(mdfe.SerieId);
        if (cboSerie.SelectedIndex < 0 && cboSerie.Items.Count == 1)
        {
            cboSerie.SelectedIndex = 0;
        }

        cboUfInicio.SelectedItem = mdfe.UfInicio;
        cboUfFim.SelectedItem = mdfe.UfFim;
        txtPercurso.Text = string.Join(", ", mdfe.Percurso);
        dtpEmissao.Value = mdfe.DataEmissao;
        dtpInicioViagem.Value = mdfe.InicioViagem ?? DateTime.Today.AddHours(DateTime.Now.Hour + 1);
        dtpInicioViagem.Checked = mdfe.InicioViagem is not null;
        lblSemSerie.Visible = cboSerie.Items.Count == 0;
        AtualizarProximoNumero();
    }

    /// <inheritdoc />
    void IEtapaMdfe.AoEntrar(MdfePrototipo mdfe)
    {
    }

    /// <inheritdoc />
    void IEtapaMdfe.Ler(MdfePrototipo mdfe)
    {
        mdfe.TipoEmitente = cboTipoEmitente.ValorSelecionado<TipoEmitenteMdfe>();
        var transportador = cboTipoTransportador.ValorSelecionado<int>() ?? NaoInformado;
        mdfe.TipoTransportador = transportador == NaoInformado ? null : (TipoTransportador)transportador;
        mdfe.SerieId = cboSerie.ValorSelecionado<int>();
        mdfe.UfInicio = cboUfInicio.SelectedItem as string ?? string.Empty;
        mdfe.UfFim = cboUfFim.SelectedItem as string ?? string.Empty;
        mdfe.Percurso = txtPercurso.Text
            .Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(u => u.ToUpperInvariant())
            .ToList();
        mdfe.DataEmissao = dtpEmissao.Value.Date;
        mdfe.InicioViagem = dtpInicioViagem.Checked ? dtpInicioViagem.Value : null;
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaMdfe.Validar(MdfePrototipo mdfe) => mdfe.ValidarIdentificacao();

    private void cboSerie_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarProximoNumero();

    /// <summary>Mostra o próximo número da série só para conferência: ele é reservado na transmissão.</summary>
    private void AtualizarProximoNumero()
    {
        var serieId = cboSerie.ValorSelecionado<int>();
        var serie = serieId is null ? null : DadosFicticios.Series.ObterPorId(serieId.Value);
        lblProximoNumero.Text = serie is null
            ? string.Empty
            : $"Próximo número da série: {serie.ProximoNumero:N0} (reservado só na transmissão)";
    }
}
