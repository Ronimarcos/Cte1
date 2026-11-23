using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>
/// Consulta dos CT-e da empresa ativa (janela MDI): filtro por período, situação e texto;
/// Novo e Abrir usam o assistente de emissão; Excluir só para CT-e em digitação.
/// </summary>
public partial class FormConsultaCte : Form
{
    private readonly EmpresaPrototipo _empresa;

    /// <summary>Cria a consulta para a empresa ativa.</summary>
    /// <param name="empresa">Empresa emitente.</param>
    internal FormConsultaCte(EmpresaPrototipo empresa)
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
            .Listar(c => c.EmpresaId == _empresa.Id
                && c.DataEmissao.Date >= de && c.DataEmissao.Date <= ate
                && (situacao < 0 || (int)c.Situacao == situacao)
                && (termo.Length == 0 || TextoPesquisa(c).Contains(termo, StringComparison.CurrentCultureIgnoreCase)))
            .OrderByDescending(c => c.DataEmissao)
            .ThenByDescending(c => c.Id)
            .ToList();

        dgvCtes.DataSource = lista;
        lblTotal.Text = lista.Count == 1 ? "1 CT-e" : $"{lista.Count} CT-e";

        if (idSelecionar is not null)
        {
            foreach (DataGridViewRow linha in dgvCtes.Rows)
            {
                if (linha.DataBoundItem is CtePrototipo cte && cte.Id == idSelecionar)
                {
                    dgvCtes.CurrentCell = linha.Cells[0];
                    break;
                }
            }
        }

        AtualizarBotoes();
    }

    private void Filtro_Alterado(object? sender, EventArgs e) => Recarregar();

    private void btnNovo_Click(object? sender, EventArgs e) => Novo();

    private void btnAbrir_Click(object? sender, EventArgs e) => AbrirSelecionado();

    private void btnExcluir_Click(object? sender, EventArgs e) => ExcluirSelecionado();

    private void btnFechar_Click(object? sender, EventArgs e) => Close();

    private void dgvCtes_SelectionChanged(object? sender, EventArgs e) => AtualizarBotoes();

    private void dgvCtes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            AbrirSelecionado();
        }
    }

    private void dgvCtes_KeyDown(object? sender, KeyEventArgs e)
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

    /// <summary>Destaca em verde os CT-e prontos para transmitir.</summary>
    private void dgvCtes_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.CellStyle is not null
            && dgvCtes.Rows[e.RowIndex].DataBoundItem is CtePrototipo { Situacao: SituacaoCte.ProntoParaTransmitir })
        {
            e.CellStyle.BackColor = Color.Honeydew;
        }
    }

    private CtePrototipo? Selecionado => dgvCtes.CurrentRow?.DataBoundItem as CtePrototipo;

    private void AtualizarBotoes()
    {
        var cte = Selecionado;
        btnAbrir.Enabled = cte is not null;
        btnExcluir.Enabled = cte is { Situacao: SituacaoCte.EmDigitacao };
    }

    private void Novo()
    {
        var cte = new CtePrototipo { EmpresaId = _empresa.Id };
        using var tela = new FormEmissaoCte(cte, _empresa);
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            Recarregar(cte.Id);
        }
    }

    private void AbrirSelecionado()
    {
        if (Selecionado is not { } atual || DadosFicticios.Ctes.ObterPorId(atual.Id) is not { } copia)
        {
            return;
        }

        using var tela = new FormEmissaoCte(copia, _empresa);
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            Recarregar(copia.Id);
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

    private static string TextoPesquisa(CtePrototipo cte) =>
        $"{cte.NumeroDescricao} {cte.RemetenteNome} {cte.DestinatarioNome} {cte.TomadorNome} {cte.NaturezaOperacao} {cte.TipoDescricao}";

    private void CriarColunas()
    {
        dgvCtes.AutoGenerateColumns = false;
        Coluna(nameof(CtePrototipo.NumeroDescricao), "Número", 10);
        Coluna(nameof(CtePrototipo.SerieDescricao), "Série", 5);
        Coluna(nameof(CtePrototipo.DataEmissao), "Emissão", 8, "dd/MM/yyyy");
        Coluna(nameof(CtePrototipo.TipoDescricao), "Tipo", 12);
        Coluna(nameof(CtePrototipo.RemetenteNome), "Remetente", 18);
        Coluna(nameof(CtePrototipo.DestinatarioNome), "Destinatário", 18);
        Coluna(nameof(CtePrototipo.TomadorNome), "Tomador", 14);
        Coluna(nameof(CtePrototipo.ValorTotalPrestacao), "Valor (R$)", 9, "N2", DataGridViewContentAlignment.MiddleRight);
        Coluna(nameof(CtePrototipo.SituacaoDescricao), "Situação", 12);
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
