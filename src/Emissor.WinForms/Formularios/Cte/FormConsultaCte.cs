using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Linha da consulta de CT-e, com os nomes lidos dos cadastros do banco.</summary>
internal sealed class LinhaCte
{
    /// <summary>Monta a linha do CT-e.</summary>
    /// <param name="cte">CT-e (lido do banco).</param>
    /// <param name="cadastros">Cadastros da empresa.</param>
    public LinhaCte(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
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
    public SituacaoDocumento Situacao { get; }

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
    private bool _carregado;
    private int _versao;

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
        Load += async (_, _) =>
        {
            _carregado = true;
            await RecarregarAsync();
        };
    }

    /// <summary>Opções do filtro de situação; -1 = todas.</summary>
    private static IReadOnlyList<Opcao<int>> OpcoesSituacao { get; } =
    [
        new(-1, "Todas"),
        new((int)SituacaoDocumento.EmDigitacao, "Em digitação"),
        new((int)SituacaoDocumento.ProntoParaTransmitir, "Pronto para transmitir"),
    ];

    /// <summary>Recarrega a grade com os filtros atuais e posiciona no CT-e indicado (ou no selecionado).</summary>
    /// <param name="idSelecionar">CT-e a selecionar após recarregar.</param>
    internal async Task RecarregarAsync(int? idSelecionar = null)
    {
        if (!_carregado)
        {
            return;
        }

        var versao = ++_versao;
        idSelecionar ??= Selecionado?.Id;
        var de = dtpDe.Value.Date;
        var ate = dtpAte.Value.Date;
        var situacao = cboSituacao.ValorSelecionado<int>() ?? -1;
        var termo = txtPesquisa.Text.Trim();

        IReadOnlyList<ConhecimentoTransporte> documentos;
        try
        {
            documentos = await _cadastros.Ctes.ListarAsync(_cadastros.Empresa.Id, de, ate,
                situacao < 0 ? null : (SituacaoDocumento)situacao);
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler os CT-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Filtros alterados durante a leitura: vale só a leitura mais recente.
        if (versao != _versao || IsDisposed)
        {
            return;
        }

        var lista = documentos
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

    private async void Filtro_Alterado(object? sender, EventArgs e) => await RecarregarAsync();

    private async void btnNovo_Click(object? sender, EventArgs e) => await NovoAsync();

    private async void btnAbrir_Click(object? sender, EventArgs e) => await AbrirSelecionadoAsync();

    private async void btnExcluir_Click(object? sender, EventArgs e) => await ExcluirSelecionadoAsync();

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
            await ExcluirSelecionadoAsync();
        }
    }

    /// <summary>Destaca em verde os CT-e prontos para transmitir.</summary>
    private void dgvCtes_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.CellStyle is not null
            && dgvCtes.Rows[e.RowIndex].DataBoundItem is LinhaCte { Situacao: SituacaoDocumento.ProntoParaTransmitir })
        {
            e.CellStyle.BackColor = Color.Honeydew;
        }
    }

    private LinhaCte? Selecionado => dgvCtes.CurrentRow?.DataBoundItem as LinhaCte;

    private void AtualizarBotoes()
    {
        var cte = Selecionado;
        btnAbrir.Enabled = cte is not null;
        btnExcluir.Enabled = cte is { Situacao: SituacaoDocumento.EmDigitacao };
    }

    private Task NovoAsync() => AbrirAssistenteAsync(new ConhecimentoTransporte { EmpresaId = _cadastros.Empresa.Id });

    /// <summary>Lê do banco o CT-e selecionado, com os itens, e abre o assistente.</summary>
    private async Task AbrirSelecionadoAsync()
    {
        if (Selecionado is not { } atual || _ocupado)
        {
            return;
        }

        ConhecimentoTransporte? copia;
        try
        {
            copia = await _cadastros.Ctes.ObterAsync(atual.Id);
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler o CT-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (copia is null)
        {
            MessageBox.Show(this, "O CT-e não existe mais. A lista será atualizada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RecarregarAsync();
            return;
        }

        await AbrirAssistenteAsync(copia);
    }

    /// <summary>Relê os cadastros do banco e abre o assistente; ao salvar, atualiza a lista.</summary>
    private async Task AbrirAssistenteAsync(ConhecimentoTransporte cte)
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
                await RecarregarAsync(cte.Id);
            }
            else
            {
                await RecarregarAsync();
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

    private async Task ExcluirSelecionadoAsync()
    {
        if (Selecionado is not { } cte)
        {
            return;
        }

        if (cte.Situacao != SituacaoDocumento.EmDigitacao)
        {
            MessageBox.Show(this, "Só CT-e em digitação podem ser excluídos.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var resposta = MessageBox.Show(this, $"Excluir o rascunho de CT-e de {cte.DataEmissao:dd/MM/yyyy} para {cte.DestinatarioNome}?",
            Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta == DialogResult.Yes)
        {
            try
            {
                await _cadastros.Ctes.ExcluirAsync(cte.Id);
            }
            catch (RegraNegocioException erro)
            {
                MessageBox.Show(this, string.Join(Environment.NewLine, erro.Pendencias), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception erro)
            {
                MessageBox.Show(this, $"Não foi possível excluir o CT-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            await RecarregarAsync();
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
