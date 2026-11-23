using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>
/// Consulta dos MDF-e da empresa ativa (janela MDI): filtro por período, situação e texto;
/// Novo e Abrir usam o assistente de emissão; Excluir só para MDF-e em digitação.
/// </summary>
public partial class FormConsultaMdfe : Form
{
    private readonly EmpresaPrototipo _empresa;

    /// <summary>Cria a consulta para a empresa ativa.</summary>
    /// <param name="empresa">Empresa emitente.</param>
    internal FormConsultaMdfe(EmpresaPrototipo empresa)
    {
        InitializeComponent();
        _empresa = empresa;
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
            .Listar(c => c.EmpresaId == _empresa.Id
                && c.DataEmissao.Date >= de && c.DataEmissao.Date <= ate
                && (situacao < 0 || (int)c.Situacao == situacao)
                && (termo.Length == 0 || TextoPesquisa(c).Contains(termo, StringComparison.CurrentCultureIgnoreCase)))
            .OrderByDescending(c => c.DataEmissao)
            .ThenByDescending(c => c.Id)
            .ToList();

        dgvMdfes.DataSource = lista;
        lblTotal.Text = lista.Count == 1 ? "1 MDF-e" : $"{lista.Count} MDF-e";

        if (idSelecionar is not null)
        {
            foreach (DataGridViewRow linha in dgvMdfes.Rows)
            {
                if (linha.DataBoundItem is MdfePrototipo mdfe && mdfe.Id == idSelecionar)
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
                Novo();
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

    private void btnNovo_Click(object? sender, EventArgs e) => Novo();

    private void btnAbrir_Click(object? sender, EventArgs e) => AbrirSelecionado();

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

    private void dgvMdfes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            AbrirSelecionado();
        }
    }

    private void dgvMdfes_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            AbrirSelecionado();
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
            && dgvMdfes.Rows[e.RowIndex].DataBoundItem is MdfePrototipo { Situacao: SituacaoMdfe.ProntoParaTransmitir })
        {
            e.CellStyle.BackColor = Color.Honeydew;
        }
    }

    private MdfePrototipo? Selecionado => dgvMdfes.CurrentRow?.DataBoundItem as MdfePrototipo;

    private void AtualizarBotoes()
    {
        var mdfe = Selecionado;
        btnAbrir.Enabled = mdfe is not null;
        btnExcluir.Enabled = mdfe is { Situacao: SituacaoMdfe.EmDigitacao };
    }

    private void Novo()
    {
        var mdfe = new MdfePrototipo { EmpresaId = _empresa.Id };
        using var tela = new FormEmissaoMdfe(mdfe, _empresa);
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            Recarregar(mdfe.Id);
        }
    }

    private void AbrirSelecionado()
    {
        if (Selecionado is not { } atual || DadosFicticios.Mdfes.ObterPorId(atual.Id) is not { } copia)
        {
            return;
        }

        using var tela = new FormEmissaoMdfe(copia, _empresa);
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            Recarregar(copia.Id);
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

    private static string TextoPesquisa(MdfePrototipo mdfe) =>
        $"{mdfe.NumeroDescricao} {mdfe.PlacaTracao} {mdfe.PercursoDescricao} {mdfe.ProdutoPredominante}";

    private void CriarColunas()
    {
        dgvMdfes.AutoGenerateColumns = false;
        Coluna(nameof(MdfePrototipo.NumeroDescricao), "Número", 9);
        Coluna(nameof(MdfePrototipo.SerieDescricao), "Série", 5);
        Coluna(nameof(MdfePrototipo.DataEmissao), "Emissão", 9, "dd/MM/yyyy");
        Coluna(nameof(MdfePrototipo.PercursoDescricao), "Percurso", 16);
        Coluna(nameof(MdfePrototipo.PlacaTracao), "Tração", 9);
        Coluna(nameof(MdfePrototipo.QuantidadeCte), "CT-e", 6, null, DataGridViewContentAlignment.MiddleRight);
        Coluna(nameof(MdfePrototipo.ProdutoPredominante), "Produto", 18);
        Coluna(nameof(MdfePrototipo.ValorCarga), "Valor da carga (R$)", 12, "N2", DataGridViewContentAlignment.MiddleRight);
        Coluna(nameof(MdfePrototipo.SituacaoDescricao), "Situação", 14);
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
