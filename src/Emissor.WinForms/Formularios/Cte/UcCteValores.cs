using System.ComponentModel;
using System.Globalization;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>
/// Etapa 4: componentes do valor da prestação (<c>vPrest/Comp</c>), total, valor a receber e impostos.
/// O ICMS do emitente do Simples Nacional vai no grupo <c>ICMSSN</c> (CST 90, indSN 1), sem alíquota digitada.
/// </summary>
public partial class UcCteValores : UserControl, IEtapaCte
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");
    private BindingList<ComponenteFretePrototipo> _componentes = [];

    /// <summary>Cria a etapa.</summary>
    public UcCteValores()
    {
        InitializeComponent();
        CriarColunas();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "4. Valores e impostos";

    /// <inheritdoc />
    void IEtapaCte.Carregar(CtePrototipo cte, CadastrosEmissao cadastros)
    {
        _componentes = new(cte.Componentes.Select(c => c.Copiar()).ToList());
        _componentes.ListChanged += (_, _) => AtualizarTotal();
        dgvComponentes.DataSource = _componentes;
        nudValorReceber.Value = cte.ValorReceber ?? 0;
        AtualizarTotal();
    }

    /// <inheritdoc />
    void IEtapaCte.AoEntrar(CtePrototipo cte)
    {
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(CtePrototipo cte)
    {
        dgvComponentes.EndEdit();
        Validate();
        cte.Componentes = _componentes
            .Where(c => c.Valor != 0 || !string.IsNullOrWhiteSpace(c.Nome))
            .Select(c => new ComponenteFretePrototipo { Nome = c.Nome.Trim(), Valor = c.Valor })
            .ToList();
        cte.ValorReceber = nudValorReceber.Value;
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaCte.Validar(CtePrototipo cte) => cte.ValidarValores();

    private void btnReceberTotal_Click(object? sender, EventArgs e) => nudValorReceber.Value = Total();

    private void dgvComponentes_CellEndEdit(object? sender, DataGridViewCellEventArgs e) => AtualizarTotal();

    private void dgvComponentes_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.ThrowException = false;
        MessageBox.Show(this, "Valor inválido. Use apenas números.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private decimal Total() => _componentes.Sum(c => c.Valor);

    private void AtualizarTotal() => lblTotal.Text = $"Total da prestação: {Total().ToString("C2", Brasil)}";

    private void CriarColunas()
    {
        dgvComponentes.AutoGenerateColumns = false;
        dgvComponentes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ComponenteFretePrototipo.Nome),
            HeaderText = "Componente (ex.: FRETE PESO, PEDAGIO)",
            MaxInputLength = 15,
            FillWeight = 65,
        });
        dgvComponentes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ComponenteFretePrototipo.Valor),
            HeaderText = "Valor (R$)",
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight },
            FillWeight = 35,
        });
    }
}
