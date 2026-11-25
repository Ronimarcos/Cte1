using Emissor.Dominio.Entidades;
using System.ComponentModel;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Etapa 1: identificação do CT-e (grupo <c>ide</c>): tipo, serviço, série, CFOP e data.</summary>
public partial class UcCteIdentificacao : UserControl, IEtapaCte
{
    private CadastrosEmissao? _cadastros;

    /// <summary>Cria a etapa.</summary>
    public UcCteIdentificacao()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "1. Identificação";

    /// <inheritdoc />
    void IEtapaCte.Carregar(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
    {
        _cadastros = cadastros;
        cboTipo.Carregar(OpcoesCadastro.TiposCte);
        cboTipoServico.Carregar(OpcoesCadastro.TiposServicoCte);

        // Só as séries de CT-e da empresa no ambiente em que ela está configurada.
        var series = cadastros.SeriesDoAmbiente(ModeloDocumento.Cte)
            .Select(s => new Opcao<int>(s.Id, $"Série {s.Serie} ({(s.Ambiente == TipoAmbiente.Homologacao ? "homologação" : "produção")})"));
        cboSerie.Carregar(series);

        cboTipo.Selecionar(cte.Tipo);
        cboTipoServico.Selecionar(cte.TipoServico);
        cboSerie.Selecionar(cte.SerieId);
        if (cboSerie.SelectedIndex < 0 && cboSerie.Items.Count == 1)
        {
            cboSerie.SelectedIndex = 0;
        }

        txtCfop.Text = cte.Cfop;
        txtNaturezaOperacao.Text = cte.NaturezaOperacao;
        dtpEmissao.Value = cte.DataEmissao;
        lblSemSerie.Visible = cboSerie.Items.Count == 0;
        AtualizarProximoNumero();
    }

    /// <inheritdoc />
    void IEtapaCte.AoEntrar(ConhecimentoTransporte cte)
    {
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(ConhecimentoTransporte cte)
    {
        cte.Tipo = cboTipo.ValorSelecionado<TipoCte>();
        cte.TipoServico = cboTipoServico.ValorSelecionado<TipoServicoCte>();
        cte.SerieId = cboSerie.ValorSelecionado<int>();
        cte.Cfop = txtCfop.Text.Trim();
        cte.NaturezaOperacao = txtNaturezaOperacao.Text.Trim();
        cte.DataEmissao = dtpEmissao.Value.Date;
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaCte.Validar(ConhecimentoTransporte cte) => cte.ValidarIdentificacao();

    private void cboSerie_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarProximoNumero();

    /// <summary>Mostra o próximo número da série só para conferência: ele é reservado na transmissão.</summary>
    private void AtualizarProximoNumero()
    {
        var serieId = cboSerie.ValorSelecionado<int>();
        var serie = _cadastros?.Serie(serieId);
        lblProximoNumero.Text = serie is null
            ? string.Empty
            : $"Próximo número da série: {serie.ProximoNumero:N0} (reservado só na transmissão)";
    }
}
