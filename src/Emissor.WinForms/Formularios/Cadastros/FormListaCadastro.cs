using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Lista genérica dos cadastros (janela MDI): pesquisa, grade e botões Novo, Editar, Excluir e Fechar.
/// O que muda de um cadastro para outro vem de <see cref="IFonteCadastro"/>.
/// </summary>
public partial class FormListaCadastro : Form
{
    private readonly IFonteCadastro _fonte;

    /// <summary>Cria a lista para o cadastro informado.</summary>
    /// <param name="fonte">Cadastro exibido.</param>
    internal FormListaCadastro(IFonteCadastro fonte)
    {
        InitializeComponent();
        _fonte = fonte;
        Text = fonte.Titulo;
        dgvRegistros.CellFormatting += dgvRegistros_CellFormatting;
        CriarColunas();
        Recarregar();
    }

    /// <summary>Título do cadastro; usado pela janela principal para não abrir a mesma lista duas vezes.</summary>
    public string Titulo => _fonte.Titulo;

    private void txtPesquisa_TextChanged(object? sender, EventArgs e) => Recarregar();

    private void btnNovo_Click(object? sender, EventArgs e) => IncluirRegistro();

    private void btnEditar_Click(object? sender, EventArgs e) => EditarSelecionado();

    private void btnExcluir_Click(object? sender, EventArgs e) => ExcluirSelecionado();

    private void btnFechar_Click(object? sender, EventArgs e) => Close();

    private void dgvRegistros_SelectionChanged(object? sender, EventArgs e) => AtualizarBotoes();

    private void dgvRegistros_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        // Duplo clique no cabeçalho (RowIndex -1) não abre a edição.
        if (e.RowIndex >= 0)
        {
            EditarSelecionado();
        }
    }

    /// <summary>Aplica a cor de destaque que o cadastro definir para a linha (ex.: certificado vencido).</summary>
    private void dgvRegistros_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.CellStyle is null || dgvRegistros.Rows[e.RowIndex].DataBoundItem is not { } registro)
        {
            return;
        }

        if (_fonte.CorDaLinha(registro) is { } cor)
        {
            e.CellStyle.BackColor = cor;
        }
    }

    private void dgvRegistros_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            EditarSelecionado();
        }
        else if (e.KeyCode == Keys.Delete)
        {
            e.Handled = true;
            ExcluirSelecionado();
        }
    }

    /// <summary>Cria as colunas da grade a partir da definição do cadastro.</summary>
    private void CriarColunas()
    {
        dgvRegistros.AutoGenerateColumns = false;
        foreach (var coluna in _fonte.Colunas)
        {
            dgvRegistros.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = $"col{coluna.Propriedade}",
                DataPropertyName = coluna.Propriedade,
                HeaderText = coluna.Titulo,
                FillWeight = coluna.Peso,
                ReadOnly = true,
            });
        }
    }

    /// <summary>Recarrega a grade com a pesquisa atual e posiciona no registro indicado (ou no que estava selecionado).</summary>
    /// <param name="idSelecionar">Registro a selecionar após recarregar.</param>
    private void Recarregar(int? idSelecionar = null)
    {
        idSelecionar ??= RegistroSelecionado?.Id;

        var registros = _fonte.Listar(txtPesquisa.Text);
        dgvRegistros.DataSource = registros;
        lblTotal.Text = registros.Count == 1 ? "1 registro" : $"{registros.Count} registros";

        if (idSelecionar is not null)
        {
            foreach (DataGridViewRow linha in dgvRegistros.Rows)
            {
                if (linha.DataBoundItem is RegistroPrototipo registro && registro.Id == idSelecionar)
                {
                    dgvRegistros.CurrentCell = linha.Cells[0];
                    break;
                }
            }
        }

        AtualizarBotoes();
    }

    /// <summary>Registro da linha atual, se houver.</summary>
    private RegistroPrototipo? RegistroSelecionado => dgvRegistros.CurrentRow?.DataBoundItem as RegistroPrototipo;

    /// <summary>Editar e Excluir só ficam disponíveis com uma linha selecionada.</summary>
    private void AtualizarBotoes()
    {
        var temSelecao = RegistroSelecionado is not null;
        btnEditar.Enabled = temSelecao;
        btnExcluir.Enabled = temSelecao;
    }

    private void IncluirRegistro()
    {
        var id = _fonte.Novo(this);
        if (id is not null)
        {
            Recarregar(id);
        }
    }

    private void EditarSelecionado()
    {
        if (RegistroSelecionado is { } registro && _fonte.Editar(this, registro))
        {
            Recarregar(registro.Id);
        }
    }

    private void ExcluirSelecionado()
    {
        if (RegistroSelecionado is { } registro && _fonte.Excluir(this, registro))
        {
            Recarregar();
        }
    }
}
