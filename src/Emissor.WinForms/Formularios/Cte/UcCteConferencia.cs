using System.ComponentModel;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Etapa 5: resumo para conferência e observações gerais (<c>compl/xObs</c>).</summary>
public partial class UcCteConferencia : UserControl, IEtapaCte
{
    /// <summary>Cria a etapa.</summary>
    public UcCteConferencia()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "5. Conferência";

    /// <inheritdoc />
    void IEtapaCte.Carregar(CtePrototipo cte, EmpresaPrototipo empresa) => txtObservacoes.Text = cte.Observacoes;

    /// <summary>Monta o resumo com o que foi preenchido nas etapas anteriores.</summary>
    void IEtapaCte.AoEntrar(CtePrototipo cte)
    {
        txtResumo.Text = cte.MontarResumo();

        // Sem isso o resumo aparece todo selecionado ao receber o foco.
        txtResumo.Select(0, 0);
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(CtePrototipo cte) => cte.Observacoes = txtObservacoes.Text.Trim();

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaCte.Validar(CtePrototipo cte) => [];
}
