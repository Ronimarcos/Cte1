using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>Linha da consulta de MDF-e, com a placa e a série lidas dos cadastros do banco.</summary>
internal sealed class LinhaMdfe
{
    /// <summary>Monta a linha do MDF-e.</summary>
    /// <param name="mdfe">MDF-e (ainda em memória até a fatia P4).</param>
    /// <param name="cadastros">Cadastros da empresa.</param>
    public LinhaMdfe(MdfePrototipo mdfe, CadastrosEmissao cadastros)
    {
        Id = mdfe.Id;
        Situacao = mdfe.Situacao;
        UfFim = mdfe.UfFim;
        NumeroDescricao = mdfe.NumeroDescricao;
        SerieDescricao = cadastros.NumeroSerie(mdfe.SerieId);
        DataEmissao = mdfe.DataEmissao;
        PercursoDescricao = mdfe.PercursoDescricao;
        PlacaTracao = cadastros.Placa(mdfe.VeiculoTracaoId);
        QuantidadeCte = mdfe.QuantidadeCte;
        ProdutoPredominante = mdfe.ProdutoPredominante;
        ValorCarga = mdfe.ValorCarga;
        SituacaoDescricao = mdfe.SituacaoDescricao;
        TextoPesquisa = $"{NumeroDescricao} {PlacaTracao} {PercursoDescricao} {ProdutoPredominante}";
    }

    /// <summary>Identificador.</summary>
    public int Id { get; }

    /// <summary>Situação (para a cor e o botão Excluir).</summary>
    public SituacaoMdfe Situacao { get; }

    /// <summary>UF de fim.</summary>
    public string UfFim { get; }

    /// <summary>Número.</summary>
    public string NumeroDescricao { get; }

    /// <summary>Série.</summary>
    public string SerieDescricao { get; }

    /// <summary>Data de emissão.</summary>
    public DateTime DataEmissao { get; }

    /// <summary>Percurso.</summary>
    public string PercursoDescricao { get; }

    /// <summary>Placa da tração.</summary>
    public string PlacaTracao { get; }

    /// <summary>Quantidade de CT-e.</summary>
    public int QuantidadeCte { get; }

    /// <summary>Produto predominante.</summary>
    public string ProdutoPredominante { get; }

    /// <summary>Valor da carga.</summary>
    public decimal? ValorCarga { get; }

    /// <summary>Situação para exibição.</summary>
    public string SituacaoDescricao { get; }

    /// <summary>Texto em que a pesquisa procura.</summary>
    public string TextoPesquisa { get; }
}

/// <summary>
/// Consulta dos MDF-e da empresa ativa (janela MDI): filtro por período, situação e texto;
/// Novo e Abrir usam o assistente de emissão; Excluir só para MDF-e em digitação.
/// </summary>
public partial class FormConsultaMdfe : Form
{
    private readonly Func<Task<CadastrosEmissao>> _carregarCadastros;
    private CadastrosEmissao _cadastros;
    private bool _ocupado;

    /// <summary>Cria a consulta para a empresa ativa.</summary>
    /// <param name="cadastros">Cadastros da empresa, já lidos do banco.</param>
    /// <param name="carregarCadastros">Relê os cadastros do banco (antes de abrir o assistente).</param>
    internal FormConsultaMdfe(CadastrosEmissao cadastros, Func<Task<CadastrosEmissao>> carregarCadastros)
    {
        InitializeComponent();
        _cadastros = cadastros;
        _carregarCadastros = carregarCadastros;
        CriarColunas();
        cboSituacao.Carregar(OpcoesSituacao);
        cboSituacao.SelectedIndex = 0;
        dtpDe.Value = DateTime.Today.AddDays(-30);
        dtpAte.Value = DateTime.Today;
        Recarregar();
    }

    /// <summary>Opções do filtro de situação; <c>null</c> = todas.</summary>
    private static IReadOnlyList<Opcao<int>> OpcoesSituacao { get; } =
    [
        new(-1, "Todas"),
        new((int)SituacaoMdfe.EmDigitacao, "Em digitação"),
        new((int)SituacaoMdfe.ProntoParaTransmitir, "Pronto para transmitir"),
    ];

    /// <summary>Recarrega a grade com os filtros atuais e posiciona no MDF-e indicado (ou no selecionado).</summary>
    /// <param name="idSelecionar">MDF-e a selecionar após recarregar.</param>
    internal void Recarregar(int? idSelecionar = null)
    {
        idSelecionar ??= Selecionado?.Id;
        var de = dtpDe.Value.Date;
        var ate = dtpAte.Value.Date;
        var situacao = cboSituacao.ValorSelecionado<int>() ?? -1;
        var termo = txtPesquisa.Text.Trim();

        var lista = DadosFicticios.Mdfes
            .Listar(c => c.EmpresaId == _cadastros.Empresa.Id
                && c.DataEmissao.Date >= de && c.DataEmissao.Date <= ate
                && (situacao < 0 || (int)c.Situacao == situacao))
            .Select(c => new LinhaMdfe(c, _cadastros))
            .Where(l => termo.Length == 0 || l.TextoPesquisa.Contains(termo, StringComparison.CurrentCultureIgnoreCase))
            .OrderByDescending(l => l.DataEmissao)
            .ThenByDescending(l => l.Id)
            .ToList();

        dgvMdfes.DataSource = lista;
        lblTotal.Text = lista.Count == 1 ? "1 MDF-e" : $"{lista.Count} MDF-e";

        if (idSelecionar is not null)
        {
            foreach (DataGridViewRow linha in dgvMdfes.Rows)
            {
                if (linha.DataBoundItem is LinhaMdfe mdfe && mdfe.Id == idSelecionar)
                {
                    dgvMdfes.CurrentCell = linha.Cells[0];
                    break;
                }
            }
        }

        AtualizarBotoes();
    }

    /// <summary>
    /// Atalhos da consulta: Insert/F2 = Novo, Ctrl+F/F3 = pesquisa, Esc = fechar.
    /// Enter e Delete são tratados na própria grade.
    /// </summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Insert:
            case Keys.F2:
                _ = NovoAsync();
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

    private void Filtro_Alterado(object? sender, EventArgs e) => Recarregar();

    private async void btnNovo_Click(object? sender, EventArgs e) => await NovoAsync();

    private async void btnAbrir_Click(object? sender, EventArgs e) => await AbrirSelecionadoAsync();

    private void btnExcluir_Click(object? sender, EventArgs e) => ExcluirSelecionado();

    private void btnFechar_Click(object? sender, EventArgs e) => Close();

    /// <summary>Encerrar, cancelar, incluir condutor e incluir DF-e são eventos enviados à SEFAZ: só no motor DF-e.</summary>
    private void btnEvento_Click(object? sender, EventArgs e)
    {
        var evento = (sender as Button)?.Text.Replace("&", string.Empty) ?? "Este evento";
        MessageBox.Show(this,
            $"\"{evento}\" é um evento do MDF-e autorizado e fica disponível na implementação do motor DF-e (homologação).",
            Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void dgvMdfes_SelectionChanged(object? sender, EventArgs e) => AtualizarBotoes();

    private async void dgvMdfes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            await AbrirSelecionadoAsync();
        }
    }

    private async void dgvMdfes_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            await AbrirSelecionadoAsync();
        }
        else if (e.KeyCode == Keys.Delete)
        {
            e.Handled = true;
            ExcluirSelecionado();
        }
    }

    /// <summary>Destaca em verde os MDF-e prontos para transmitir.</summary>
    private void dgvMdfes_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.CellStyle is not null
            && dgvMdfes.Rows[e.RowIndex].DataBoundItem is LinhaMdfe { Situacao: SituacaoMdfe.ProntoParaTransmitir })
        {
            e.CellStyle.BackColor = Color.Honeydew;
        }
    }

    private LinhaMdfe? Selecionado => dgvMdfes.CurrentRow?.DataBoundItem as LinhaMdfe;

    private void AtualizarBotoes()
    {
        var mdfe = Selecionado;
        btnAbrir.Enabled = mdfe is not null;
        btnExcluir.Enabled = mdfe is { Situacao: SituacaoMdfe.EmDigitacao };
    }

    private Task NovoAsync() => AbrirAssistenteAsync(new MdfePrototipo { EmpresaId = _cadastros.Empresa.Id });

    private Task AbrirSelecionadoAsync() =>
        Selecionado is { } atual && DadosFicticios.Mdfes.ObterPorId(atual.Id) is { } copia
            ? AbrirAssistenteAsync(copia)
            : Task.CompletedTask;

    /// <summary>Relê os cadastros do banco e abre o assistente; ao salvar, atualiza a lista.</summary>
    private async Task AbrirAssistenteAsync(MdfePrototipo mdfe)
    {
        if (_ocupado)
        {
            return;
        }

        _ocupado = true;
        try
        {
            _cadastros = await _carregarCadastros();
            using var tela = new FormEmissaoMdfe(mdfe, _cadastros);
            if (tela.ShowDialog(this) == DialogResult.OK)
            {
                Recarregar(mdfe.Id);
            }
            else
            {
                Recarregar();
            }
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler os cadastros.{Environment.NewLine}{erro.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _ocupado = false;
        }
    }

    private void ExcluirSelecionado()
    {
        if (Selecionado is not { } mdfe)
        {
            return;
        }

        if (mdfe.Situacao != SituacaoMdfe.EmDigitacao)
        {
            MessageBox.Show(this, "Só MDF-e em digitação podem ser excluídos.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var resposta = MessageBox.Show(this, $"Excluir o rascunho de MDF-e de {mdfe.DataEmissao:dd/MM/yyyy} com destino a {mdfe.UfFim}?",
            Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta == DialogResult.Yes)
        {
            DadosFicticios.Mdfes.Excluir(mdfe.Id);
            Recarregar();
        }
    }

    private void CriarColunas()
    {
        dgvMdfes.AutoGenerateColumns = false;
        Coluna(nameof(LinhaMdfe.NumeroDescricao), "Número", 9);
        Coluna(nameof(LinhaMdfe.SerieDescricao), "Série", 5);
        Coluna(nameof(LinhaMdfe.DataEmissao), "Emissão", 9, "dd/MM/yyyy");
        Coluna(nameof(LinhaMdfe.PercursoDescricao), "Percurso", 16);
        Coluna(nameof(LinhaMdfe.PlacaTracao), "Tração", 9);
        Coluna(nameof(LinhaMdfe.QuantidadeCte), "CT-e", 6, null, DataGridViewContentAlignment.MiddleRight);
        Coluna(nameof(LinhaMdfe.ProdutoPredominante), "Produto", 18);
        Coluna(nameof(LinhaMdfe.ValorCarga), "Valor da carga (R$)", 12, "N2", DataGridViewContentAlignment.MiddleRight);
        Coluna(nameof(LinhaMdfe.SituacaoDescricao), "Situação", 14);
    }

    private void Coluna(string propriedade, string titulo, float peso, string? formato = null,
        DataGridViewContentAlignment alinhamento = DataGridViewContentAlignment.MiddleLeft)
    {
        dgvMdfes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propriedade,
            HeaderText = titulo,
            FillWeight = peso,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle { Format = formato ?? string.Empty, Alignment = alinhamento },
        });
    }
}
