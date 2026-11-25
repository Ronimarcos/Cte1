namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Lista genérica dos cadastros (janela MDI): pesquisa, grade e botões Novo, Editar, Excluir e Fechar.
/// O que muda de um cadastro para outro vem de <see cref="IFonteCadastro"/>. As operações são assíncronas,
/// para não travar a tela enquanto o banco responde.
/// </summary>
public partial class FormListaCadastro : Form
{
    private readonly IFonteCadastro _fonte;

    /// <summary>Número da última leitura pedida; leituras mais antigas que terminarem depois são descartadas.</summary>
    private int _leitura;

    /// <summary>Evita abrir duas operações ao mesmo tempo (ex.: duplo clique rápido).</summary>
    private bool _ocupado;

    /// <summary>Cria a lista para o cadastro informado.</summary>
    /// <param name="fonte">Cadastro exibido.</param>
    internal FormListaCadastro(IFonteCadastro fonte)
    {
        InitializeComponent();
        _fonte = fonte;
        Text = fonte.Titulo;
        dgvRegistros.CellFormatting += dgvRegistros_CellFormatting;
        Load += async (_, _) => await RecarregarAsync();
        CriarColunas();
        AtualizarBotoes();
    }

    /// <summary>Título do cadastro; usado pela janela principal para não abrir a mesma lista duas vezes.</summary>
    public string Titulo => _fonte.Titulo;

    /// <summary>
    /// Atalhos da lista: Insert/F2 = Novo, Ctrl+F/F3 = pesquisa, Esc = fechar.
    /// Enter e Delete são tratados na própria grade.
    /// </summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Insert:
            case Keys.F2:
                _ = IncluirRegistroAsync();
                return true;
            case Keys.Control | Keys.F:
            case Keys.F3:
                txtPesquisa.Focus();
                txtPesquisa.SelectAll();
                return true;
            case Keys.Escape:
                Close();
                return true;
            default:
                return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    private async void txtPesquisa_TextChanged(object? sender, EventArgs e) => await RecarregarAsync();

    private async void btnNovo_Click(object? sender, EventArgs e) => await IncluirRegistroAsync();

    private async void btnEditar_Click(object? sender, EventArgs e) => await EditarSelecionadoAsync();

    private async void btnExcluir_Click(object? sender, EventArgs e) => await ExcluirSelecionadoAsync();

    private void btnFechar_Click(object? sender, EventArgs e) => Close();

    private void dgvRegistros_SelectionChanged(object? sender, EventArgs e) => AtualizarBotoes();

    private async void dgvRegistros_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        // Duplo clique no cabeçalho (RowIndex -1) não abre a edição.
        if (e.RowIndex >= 0)
        {
            await EditarSelecionadoAsync();
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

    private async void dgvRegistros_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            await EditarSelecionadoAsync();
        }
        else if (e.KeyCode == Keys.Delete)
        {
            e.Handled = true;
            await ExcluirSelecionadoAsync();
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
    private async Task RecarregarAsync(int? idSelecionar = null)
    {
        idSelecionar ??= IdSelecionado;
        var leitura = ++_leitura;

        System.Collections.IList registros;
        try
        {
            registros = await _fonte.ListarAsync(txtPesquisa.Text);
        }
        catch (Exception erro)
        {
            MostrarErro("ler os registros", erro);
            return;
        }

        // A pesquisa pode ter mudado enquanto o banco respondia: só a leitura mais recente é exibida.
        if (leitura != _leitura || IsDisposed)
        {
            return;
        }

        dgvRegistros.DataSource = registros;
        lblTotal.Text = registros.Count == 1 ? "1 registro" : $"{registros.Count} registros";

        if (idSelecionar is not null)
        {
            foreach (DataGridViewRow linha in dgvRegistros.Rows)
            {
                if (linha.DataBoundItem is { } registro && _fonte.IdDe(registro) == idSelecionar)
                {
                    dgvRegistros.CurrentCell = linha.Cells[0];
                    break;
                }
            }
        }

        AtualizarBotoes();
    }

    /// <summary>Registro da linha atual, se houver.</summary>
    private object? RegistroSelecionado => dgvRegistros.CurrentRow?.DataBoundItem;

    private int? IdSelecionado => RegistroSelecionado is { } registro ? _fonte.IdDe(registro) : null;

    /// <summary>Editar e Excluir só ficam disponíveis com uma linha selecionada.</summary>
    private void AtualizarBotoes()
    {
        var temSelecao = RegistroSelecionado is not null;
        btnEditar.Enabled = temSelecao;
        btnExcluir.Enabled = temSelecao;
    }

    private Task IncluirRegistroAsync() => ExecutarAsync("incluir", async () =>
    {
        if (await _fonte.NovoAsync(this) is { } id)
        {
            await RecarregarAsync(id);
        }
    });

    private Task EditarSelecionadoAsync() => ExecutarAsync("alterar", async () =>
    {
        if (RegistroSelecionado is { } registro && await _fonte.EditarAsync(this, registro))
        {
            await RecarregarAsync(_fonte.IdDe(registro));
        }
    });

    private Task ExcluirSelecionadoAsync() => ExecutarAsync("excluir", async () =>
    {
        if (RegistroSelecionado is { } registro && await _fonte.ExcluirAsync(this, registro))
        {
            await RecarregarAsync();
        }
    });

    /// <summary>Executa uma operação por vez e mostra erros inesperados (ex.: banco inacessível) sem fechar a tela.</summary>
    private async Task ExecutarAsync(string operacao, Func<Task> acao)
    {
        if (_ocupado)
        {
            return;
        }

        _ocupado = true;
        try
        {
            await acao();
        }
        catch (Exception erro)
        {
            MostrarErro(operacao, erro);
        }
        finally
        {
            _ocupado = false;
        }
    }

    private void MostrarErro(string operacao, Exception erro) =>
        MessageBox.Show(this, $"Não foi possível {operacao}.{Environment.NewLine}{Environment.NewLine}{erro.GetBaseException().Message}",
            Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
}
