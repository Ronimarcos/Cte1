using System.ComponentModel;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Etapa 1: identificação do CT-e (grupo <c>ide</c>): tipo, serviço, série, CFOP e data.</summary>
public partial class UcCteIdentificacao : UserControl, IEtapaCte
{
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
    void IEtapaCte.Carregar(CtePrototipo cte, EmpresaPrototipo empresa)
    {
        cboTipo.Carregar(OpcoesCadastro.TiposCte);
        cboTipoServico.Carregar(OpcoesCadastro.TiposServicoCte);

        // Só as séries de CT-e da empresa no ambiente em que ela está configurada.
        var series = DadosFicticios.Series
            .Listar(s => s.EmpresaId == empresa.Id && s.Modelo == ModeloDocumento.Cte && s.Homologacao == empresa.Homologacao)
            .Select(s => new Opcao<int>(s.Id, $"Série {s.Serie} ({(s.Homologacao ? "homologação" : "produção")})"));
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
    void IEtapaCte.AoEntrar(CtePrototipo cte)
    {
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(CtePrototipo cte)
    {
        cte.Tipo = cboTipo.ValorSelecionado<TipoCte>();
        cte.TipoServico = cboTipoServico.ValorSelecionado<TipoServicoCte>();
        cte.SerieId = cboSerie.ValorSelecionado<int>();
        cte.Cfop = txtCfop.Text.Trim();
        cte.NaturezaOperacao = txtNaturezaOperacao.Text.Trim();
        cte.DataEmissao = dtpEmissao.Value.Date;
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaCte.Validar(CtePrototipo cte) => cte.ValidarIdentificacao();

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
