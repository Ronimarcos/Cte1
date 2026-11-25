using Emissor.Dominio.Enums;
using Emissor.Dominio.Entidades;
using System.ComponentModel;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Etapa 5: resumo para conferência e observações gerais (<c>compl/xObs</c>).</summary>
public partial class UcCteConferencia : UserControl, IEtapaCte
{
    private CadastrosEmissao? _cadastros;

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
    void IEtapaCte.Carregar(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
    {
        _cadastros = cadastros;
        txtObservacoes.Text = cte.Observacoes;
    }

    /// <summary>Monta o resumo com o que foi preenchido nas etapas anteriores.</summary>
    void IEtapaCte.AoEntrar(ConhecimentoTransporte cte)
    {
        txtResumo.Text = _cadastros is null ? string.Empty : cte.MontarResumo(_cadastros);

        // Sem isso o resumo aparece todo selecionado ao receber o foco.
        txtResumo.Select(0, 0);
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(ConhecimentoTransporte cte) => cte.Observacoes = txtObservacoes.Text.Trim();

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaCte.Validar(ConhecimentoTransporte cte) => [];
}
