using System.ComponentModel;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios;

/// <summary>
/// Diálogo de seleção da empresa (emitente) com que o usuário vai trabalhar.
/// Exibido ao abrir o sistema e pela opção "Trocar empresa" da janela principal.
/// </summary>
public partial class FormSelecaoEmpresa : Form
{
    private readonly int? _empresaAtualId;

    /// <summary>Cria o diálogo com a lista de empresas disponíveis.</summary>
    /// <param name="empresas">Empresas exibidas na grade.</param>
    /// <param name="empresaAtualId">Empresa já ativa, que fica pré-selecionada (opcional).</param>
    public FormSelecaoEmpresa(IReadOnlyList<EmpresaResumo> empresas, int? empresaAtualId = null)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, salvar: null);
        _empresaAtualId = empresaAtualId;
        dgvEmpresas.AutoGenerateColumns = false;
        dgvEmpresas.DataSource = empresas.ToList();
        dgvEmpresas.KeyDown += dgvEmpresas_KeyDown;
    }

    /// <summary>Empresa escolhida; preenchida somente quando o diálogo retorna <see cref="DialogResult.OK"/>.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public EmpresaResumo? EmpresaSelecionada { get; private set; }

    private void FormSelecaoEmpresa_Shown(object? sender, EventArgs e) => SelecionarEmpresaAtual();

    private void btnOk_Click(object? sender, EventArgs e) => ConfirmarSelecao();

    private void dgvEmpresas_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        // Duplo clique no cabeçalho (RowIndex -1) não seleciona.
        if (e.RowIndex >= 0)
        {
            ConfirmarSelecao();
        }
    }

    /// <summary>
    /// Enter confirma a empresa da linha atual. Sem isso a grade trata o Enter como "descer uma linha"
    /// e o diálogo confirmava a empresa seguinte à escolhida.
    /// </summary>
    private void dgvEmpresas_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            ConfirmarSelecao();
        }
    }

    /// <summary>Posiciona a grade na empresa já ativa, quando houver.</summary>
    private void SelecionarEmpresaAtual()
    {
        if (_empresaAtualId is null)
        {
            return;
        }

        foreach (DataGridViewRow linha in dgvEmpresas.Rows)
        {
            if (linha.DataBoundItem is EmpresaResumo empresa && empresa.Id == _empresaAtualId)
            {
                dgvEmpresas.CurrentCell = linha.Cells[0];
                return;
            }
        }
    }

    /// <summary>Confirma a empresa da linha atual e fecha o diálogo com OK.</summary>
    private void ConfirmarSelecao()
    {
        if (dgvEmpresas.CurrentRow?.DataBoundItem is not EmpresaResumo empresa)
        {
            MessageBox.Show(this, "Selecione uma empresa na lista.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        EmpresaSelecionada = empresa;
        DialogResult = DialogResult.OK;
    }
}
