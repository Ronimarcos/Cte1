using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Linha da consulta de CT-e, com os nomes lidos dos cadastros do banco.</summary>
internal sealed class LinhaCte
{
    /// <summary>Monta a linha do CT-e.</summary>
    /// <param name="cte">CT-e (ainda em memória até a fatia P4).</param>
    /// <param name="cadastros">Cadastros da empresa.</param>
    public LinhaCte(CtePrototipo cte, CadastrosEmissao cadastros)
    {
        Id = cte.Id;
        Situacao = cte.Situacao;
        NumeroDescricao = cte.NumeroDescricao;
        SerieDescricao = cadastros.NumeroSerie(cte.SerieId);
        DataEmissao = cte.DataEmissao;
        TipoDescricao = cte.TipoDescricao;
        RemetenteNome = cadastros.NomeParticipante(cte.RemetenteId);
        DestinatarioNome = cadastros.NomeParticipante(cte.DestinatarioId);
        TomadorNome = cadastros.NomeParticipante(cte.TomadorId);
        ValorTotalPrestacao = cte.ValorTotalPrestacao;
        SituacaoDescricao = cte.SituacaoDescricao;
        TextoPesquisa = $"{NumeroDescricao} {RemetenteNome} {DestinatarioNome} {TomadorNome} {cte.NaturezaOperacao} {TipoDescricao}";
    }

    /// <summary>Identificador.</summary>
    public int Id { get; }

    /// <summary>Situação (para a cor e o botão Excluir).</summary>
    public SituacaoCte Situacao { get; }

    /// <summary>Número.</summary>
    public string NumeroDescricao { get; }

    /// <summary>Série.</summary>
    public string SerieDescricao { get; }

    /// <summary>Data de emissão.</summary>
    public DateTime DataEmissao { get; }

    /// <summary>Tipo do CT-e.</summary>
    public string TipoDescricao { get; }

    /// <summary>Remetente.</summary>
    public string RemetenteNome { get; }

    /// <summary>Destinatário.</summary>
    public string DestinatarioNome { get; }

    /// <summary>Tomador.</summary>
    public string TomadorNome { get; }

    /// <summary>Valor total da prestação.</summary>
    public decimal ValorTotalPrestacao { get; }

    /// <summary>Situação para exibição.</summary>
    public string SituacaoDescricao { get; }

    /// <summary>Texto em que a pesquisa procura.</summary>
    public string TextoPesquisa { get; }
}

/// <summary>
/// Consulta dos CT-e da empresa ativa (janela MDI): filtro por período, situação e texto;
/// Novo e Abrir usam o assistente de emissão, com os cadastros relidos do banco a cada abertura;
/// Excluir só para CT-e em digitação.
/// </summary>
public partial class FormConsultaCte : Form
{
    private readonly Func<Task<CadastrosEmissao>> _carregarCadastros;
    private CadastrosEmissao _cadastros;
    private bool _ocupado;

    /// <summary>Cria a consulta para a empresa ativa.</summary>
    /// <param name="cadastros">Cadastros da empresa, já lidos do banco.</param>
    /// <param name="carregarCadastros">Relê os cadastros do banco (antes de abrir o assistente).</param>
    internal FormConsultaCte(CadastrosEmissao cadastros, Func<Task<CadastrosEmissao>> carregarCadastros)
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

    /// <summary>Opções do filtro de situação; -1 = todas.</summary>
    private static IReadOnlyList<Opcao<int>> OpcoesSituacao { get; } =
    [
        new(-1, "Todas"),
        new((int)SituacaoCte.EmDigitacao, "Em digitação"),
        new((int)SituacaoCte.ProntoParaTransmitir, "Pronto para transmitir"),
    ];

    /// <summary>Recarrega a grade com os filtros atuais e posiciona no CT-e indicado (ou no selecionado).</summary>
    /// <param name="idSelecionar">CT-e a selecionar após recarregar.</param>
    internal void Recarregar(int? idSelecionar = null)
    {
        idSelecionar ??= Selecionado?.Id;
        var de = dtpDe.Value.Date;
        var ate = dtpAte.Value.Date;
        var situacao = cboSituacao.ValorSelecionado<int>() ?? -1;
        var termo = txtPesquisa.Text.Trim();

        var lista = DadosFicticios.Ctes
            .Listar(c => c.EmpresaId == _cadastros.Empresa.Id
                && c.DataEmissao.Date >= de && c.DataEmissao.Date <= ate
                && (situacao < 0 || (int)c.Situacao == situacao))
            .Select(c => new LinhaCte(c, _cadastros))
            .Where(l => termo.Length == 0 || l.TextoPesquisa.Contains(termo, StringComparison.CurrentCultureIgnoreCase))
            .OrderByDescending(l => l.DataEmissao)
            .ThenByDescending(l => l.Id)
            .ToList();

        dgvCtes.DataSource = lista;
        lblTotal.Text = lista.Count == 1 ? "1 CT-e" : $"{lista.Count} CT-e";

        if (idSelecionar is not null)
        {
            foreach (DataGridViewRow linha in dgvCtes.Rows)
            {
                if (linha.DataBoundItem is LinhaCte cte && cte.Id == idSelecionar)
                {
                    dgvCtes.CurrentCell = linha.Cells[0];
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

    private void dgvCtes_SelectionChanged(object? sender, EventArgs e) => AtualizarBotoes();

    private async void dgvCtes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            await AbrirSelecionadoAsync();
        }
    }

    private async void dgvCtes_KeyDown(object? sender, KeyEventArgs e)
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

    /// <summary>Destaca em verde os CT-e prontos para transmitir.</summary>
    private void dgvCtes_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.CellStyle is not null
            && dgvCtes.Rows[e.RowIndex].DataBoundItem is LinhaCte { Situacao: SituacaoCte.ProntoParaTransmitir })
        {
            e.CellStyle.BackColor = Color.Honeydew;
        }
    }

    private LinhaCte? Selecionado => dgvCtes.CurrentRow?.DataBoundItem as LinhaCte;

    private void AtualizarBotoes()
    {
        var cte = Selecionado;
        btnAbrir.Enabled = cte is not null;
        btnExcluir.Enabled = cte is { Situacao: SituacaoCte.EmDigitacao };
    }

    private Task NovoAsync() => AbrirAssistenteAsync(new CtePrototipo { EmpresaId = _cadastros.Empresa.Id });

    private Task AbrirSelecionadoAsync() =>
        Selecionado is { } atual && DadosFicticios.Ctes.ObterPorId(atual.Id) is { } copia
            ? AbrirAssistenteAsync(copia)
            : Task.CompletedTask;

    /// <summary>Relê os cadastros do banco e abre o assistente; ao salvar, atualiza a lista.</summary>
    private async Task AbrirAssistenteAsync(CtePrototipo cte)
    {
        if (_ocupado)
        {
            return;
        }

        _ocupado = true;
        try
        {
            _cadastros = await _carregarCadastros();
            using var tela = new FormEmissaoCte(cte, _cadastros);
            if (tela.ShowDialog(this) == DialogResult.OK)
            {
                Recarregar(cte.Id);
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
        if (Selecionado is not { } cte)
        {
            return;
        }

        if (cte.Situacao != SituacaoCte.EmDigitacao)
        {
            MessageBox.Show(this, "Só CT-e em digitação podem ser excluídos.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var resposta = MessageBox.Show(this, $"Excluir o rascunho de CT-e de {cte.DataEmissao:dd/MM/yyyy} para {cte.DestinatarioNome}?",
            Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta == DialogResult.Yes)
        {
            DadosFicticios.Ctes.Excluir(cte.Id);
            Recarregar();
        }
    }

    private void CriarColunas()
    {
        dgvCtes.AutoGenerateColumns = false;
        Coluna(nameof(LinhaCte.NumeroDescricao), "Número", 10);
        Coluna(nameof(LinhaCte.SerieDescricao), "Série", 5);
        Coluna(nameof(LinhaCte.DataEmissao), "Emissão", 8, "dd/MM/yyyy");
        Coluna(nameof(LinhaCte.TipoDescricao), "Tipo", 12);
        Coluna(nameof(LinhaCte.RemetenteNome), "Remetente", 18);
        Coluna(nameof(LinhaCte.DestinatarioNome), "Destinatário", 18);
        Coluna(nameof(LinhaCte.TomadorNome), "Tomador", 14);
        Coluna(nameof(LinhaCte.ValorTotalPrestacao), "Valor (R$)", 9, "N2", DataGridViewContentAlignment.MiddleRight);
        Coluna(nameof(LinhaCte.SituacaoDescricao), "Situação", 12);
    }

    private void Coluna(string propriedade, string titulo, float peso, string? formato = null,
        DataGridViewContentAlignment alinhamento = DataGridViewContentAlignment.MiddleLeft)
    {
        dgvCtes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propriedade,
            HeaderText = titulo,
            FillWeight = peso,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle { Format = formato ?? string.Empty, Alignment = alinhamento },
        });
    }
}
