using System.ComponentModel;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>Etapa 5: resumo para conferência e informações complementares (<c>infAdic/infCpl</c>).</summary>
public partial class UcMdfeConferencia : UserControl, IEtapaMdfe
{
    private CadastrosEmissao? _cadastros;

    /// <summary>Cria a etapa.</summary>
    public UcMdfeConferencia()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "5. Conferência";

    /// <inheritdoc />
    void IEtapaMdfe.Carregar(MdfePrototipo mdfe, CadastrosEmissao cadastros)
    {
        _cadastros = cadastros;
        txtInformacoes.Text = mdfe.InformacoesComplementares;
    }

    /// <summary>Monta o resumo com o que foi preenchido nas etapas anteriores.</summary>
    void IEtapaMdfe.AoEntrar(MdfePrototipo mdfe)
    {
        txtResumo.Text = _cadastros is null ? string.Empty : mdfe.MontarResumo(_cadastros);
        txtResumo.Select(0, 0);
    }

    /// <inheritdoc />
    void IEtapaMdfe.Ler(MdfePrototipo mdfe) => mdfe.InformacoesComplementares = txtInformacoes.Text.Trim();

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaMdfe.Validar(MdfePrototipo mdfe) => [];
}
