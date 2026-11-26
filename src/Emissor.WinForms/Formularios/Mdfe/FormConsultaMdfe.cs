using Emissor.DFe;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Formularios.Eventos;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>Linha da consulta de MDF-e, com a placa e a série lidas dos cadastros do banco.</summary>
internal sealed class LinhaMdfe
{
    /// <summary>Monta a linha do MDF-e.</summary>
    /// <param name="mdfe">MDF-e (lido do banco).</param>
    /// <param name="cadastros">Cadastros da empresa.</param>
    public LinhaMdfe(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
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
    public SituacaoDocumento Situacao { get; }

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
    private readonly Button _btnDamdfe = new()
    {
        Anchor = AnchorStyles.Top | AnchorStyles.Right,
        Location = new Point(610, 8),
        Size = new Size(126, 27),
        Text = "DA&MDFE de teste",
        UseVisualStyleBackColor = true,
    };
    private CadastrosEmissao _cadastros;
    private bool _ocupado;
    private bool _carregado;
    private int _versao;

    /// <summary>Cria a consulta para a empresa ativa.</summary>
    /// <param name="cadastros">Cadastros da empresa, já lidos do banco.</param>
    /// <param name="carregarCadastros">Relê os cadastros do banco (antes de abrir o assistente).</param>
    internal FormConsultaMdfe(CadastrosEmissao cadastros, Func<Task<CadastrosEmissao>> carregarCadastros)
    {
        InitializeComponent();
        _cadastros = cadastros;
        _carregarCadastros = carregarCadastros;
        CriarColunas();
        _btnDamdfe.Click += async (_, _) => await GerarDamdfeTesteAsync();
        pnlRodape.Controls.Add(_btnDamdfe);
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

    /// <summary>Opções do filtro de situação; <c>null</c> = todas.</summary>
    private static IReadOnlyList<Opcao<int>> OpcoesSituacao { get; } =
    [
        new(-1, "Todas"),
        new((int)SituacaoDocumento.EmDigitacao, "Em digitação"),
        new((int)SituacaoDocumento.ProntoParaTransmitir, "Pronto para transmitir"),
    ];

    /// <summary>Recarrega a grade com os filtros atuais e posiciona no MDF-e indicado (ou no selecionado).</summary>
    /// <param name="idSelecionar">MDF-e a selecionar após recarregar.</param>
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

        IReadOnlyList<ManifestoEletronico> documentos;
        try
        {
            documentos = await _cadastros.Mdfes.ListarAsync(_cadastros.Empresa.Id, de, ate,
                situacao < 0 ? null : (SituacaoDocumento)situacao);
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler os MDF-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Filtros alterados durante a leitura: vale só a leitura mais recente.
        if (versao != _versao || IsDisposed)
        {
            return;
        }

        var lista = documentos
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

    private async void Filtro_Alterado(object? sender, EventArgs e) => await RecarregarAsync();

    private async void btnNovo_Click(object? sender, EventArgs e) => await NovoAsync();

    private async void btnAbrir_Click(object? sender, EventArgs e) => await AbrirSelecionadoAsync();

    private async void btnExcluir_Click(object? sender, EventArgs e) => await ExcluirSelecionadoAsync();

    private void btnFechar_Click(object? sender, EventArgs e) => Close();

    private async void btnXmlTeste_Click(object? sender, EventArgs e) => await GerarXmlTesteAsync();

    /// <summary>
    /// Gera o DAMDFE de teste do MDF-e selecionado (a partir do XML de teste, em homologação e sem protocolo) e
    /// oferece salvar e abrir o PDF.
    /// </summary>
    private async Task GerarDamdfeTesteAsync()
    {
        const string titulo = "DAMDFE de teste";
        if (_ocupado)
        {
            return;
        }

        if (Selecionado is not { } atual)
        {
            MessageBox.Show(this, "Selecione um MDF-e na lista.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _ocupado = true;
        UseWaitCursor = true;
        try
        {
            var mdfe = await _cadastros.Mdfes.ObterAsync(atual.Id)
                ?? throw new RegraNegocioException("O MDF-e não existe mais. Atualize a lista.");
            _cadastros = await _carregarCadastros();
            var (xml, pdf) = await _cadastros.XmlTeste.GerarDamdfeAsync(mdfe, _cadastros);
            UseWaitCursor = false;
            if (pdf is null)
            {
                AvisosCadastro.HaPendencias(this, titulo, xml.Pendencias.Count > 0 ? xml.Pendencias : ["O XML do MDF-e não pôde ser gerado."]);
                return;
            }

            using var dialogo = new SaveFileDialog
            {
                Title = titulo,
                Filter = "Arquivo PDF (*.pdf)|*.pdf",
                FileName = $"{xml.Chave}-teste-damdfe.pdf",
            };
            if (dialogo.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            await File.WriteAllBytesAsync(dialogo.FileName, pdf);
            if (MessageBox.Show(this, "DAMDFE de teste salvo (homologação, sem valor fiscal). Deseja abrir o arquivo?", titulo,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(dialogo.FileName) { UseShellExecute = true });
            }
        }
        catch (RegraNegocioException erro)
        {
            UseWaitCursor = false;
            MessageBox.Show(this, string.Join(Environment.NewLine, erro.Pendencias), titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception erro)
        {
            UseWaitCursor = false;
            MessageBox.Show(this, $"Não foi possível gerar o DAMDFE.{Environment.NewLine}{erro.GetBaseException().Message}",
                titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
            _ocupado = false;
        }
    }

    /// <summary>Encerrar, cancelar, incluir condutor e incluir DF-e são eventos enviados à SEFAZ: só no motor DF-e.</summary>
    private async void btnEvento_Click(object? sender, EventArgs e)
    {
        TipoEvento? tipo = sender == btnEncerrar ? TipoEvento.EncerramentoMdfe
            : sender == btnCancelarMdfe ? TipoEvento.CancelamentoMdfe
            : sender == btnIncluirCondutor ? TipoEvento.InclusaoCondutorMdfe
            : null;
        if (tipo is null)
        {
            MessageBox.Show(this, "A inclusão de DF-e no MDF-e (evento 110115) fica para uma fase futura.",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        await ExecutarEventoAsync(tipo.Value);
    }

    /// <summary>
    /// Lê do banco o MDF-e selecionado, relê os cadastros e abre a tela do evento, que gera o XML de teste (sem
    /// transmitir nem gravar).
    /// </summary>
    /// <param name="tipo">Tipo do evento.</param>
    private async Task ExecutarEventoAsync(TipoEvento tipo)
    {
        if (_ocupado)
        {
            return;
        }

        if (Selecionado is not { } atual)
        {
            MessageBox.Show(this, "Selecione um MDF-e na lista.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _ocupado = true;
        try
        {
            var documento = await _cadastros.Mdfes.ObterAsync(atual.Id)
                ?? throw new RegraNegocioException("O MDF-e não existe mais. Atualize a lista.");
            _cadastros = await _carregarCadastros();
            var numero = documento.Numero is { } n ? $"nº {n}" : "sem número";
            await TelaEvento.ExecutarAsync(this, _cadastros, documento, tipo,
                $"MDF-e {numero} de {documento.DataEmissao:dd/MM/yyyy} ({documento.SituacaoDescricao})");
        }
        catch (RegraNegocioException erro)
        {
            MessageBox.Show(this, string.Join(Environment.NewLine, erro.Pendencias), tipo.Descricao(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível gerar o XML do evento.{Environment.NewLine}{erro.GetBaseException().Message}",
                tipo.Descricao(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
            _ocupado = false;
        }
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
            await ExcluirSelecionadoAsync();
        }
    }

    /// <summary>Destaca em verde os MDF-e prontos para transmitir.</summary>
    private void dgvMdfes_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.CellStyle is not null
            && dgvMdfes.Rows[e.RowIndex].DataBoundItem is LinhaMdfe { Situacao: SituacaoDocumento.ProntoParaTransmitir })
        {
            e.CellStyle.BackColor = Color.Honeydew;
        }
    }

    private LinhaMdfe? Selecionado => dgvMdfes.CurrentRow?.DataBoundItem as LinhaMdfe;

    private void AtualizarBotoes()
    {
        var mdfe = Selecionado;
        btnAbrir.Enabled = mdfe is not null;
        btnExcluir.Enabled = mdfe is { Situacao: SituacaoDocumento.EmDigitacao };
        btnEncerrar.Enabled = btnCancelarMdfe.Enabled = btnIncluirCondutor.Enabled = _btnDamdfe.Enabled = mdfe is not null;
    }

    private Task NovoAsync() => AbrirAssistenteAsync(new ManifestoEletronico { EmpresaId = _cadastros.Empresa.Id });

    /// <summary>Lê do banco o MDF-e selecionado, com os itens, e abre o assistente.</summary>
    private async Task AbrirSelecionadoAsync()
    {
        if (Selecionado is not { } atual || _ocupado)
        {
            return;
        }

        ManifestoEletronico? copia;
        try
        {
            copia = await _cadastros.Mdfes.ObterAsync(atual.Id);
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler o MDF-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (copia is null)
        {
            MessageBox.Show(this, "O MDF-e não existe mais. A lista será atualizada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RecarregarAsync();
            return;
        }

        await AbrirAssistenteAsync(copia);
    }

    /// <summary>Relê os cadastros do banco e abre o assistente; ao salvar, atualiza a lista.</summary>
    private async Task AbrirAssistenteAsync(ManifestoEletronico mdfe)
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
                await RecarregarAsync(mdfe.Id);
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

    /// <summary>
    /// Gera o XML do MDF-e selecionado (montado, assinado e validado contra o XSD), sem transmitir e sem reservar
    /// número, e oferece salvar o arquivo.
    /// </summary>
    private async Task GerarXmlTesteAsync()
    {
        if (_ocupado)
        {
            return;
        }

        if (Selecionado is not { } atual)
        {
            MessageBox.Show(this, "Selecione um MDF-e na lista.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _ocupado = true;
        UseWaitCursor = true;
        btnXmlTeste.Enabled = false;
        try
        {
            var mdfe = await _cadastros.Mdfes.ObterAsync(atual.Id)
                ?? throw new RegraNegocioException("O MDF-e não existe mais. Atualize a lista.");
            _cadastros = await _carregarCadastros();
            var resultado = await _cadastros.XmlTeste.GerarMdfeAsync(mdfe, _cadastros);
            UseWaitCursor = false;
            await MostrarXmlTesteAsync(resultado);
        }
        catch (RegraNegocioException erro)
        {
            UseWaitCursor = false;
            MessageBox.Show(this, string.Join(Environment.NewLine, erro.Pendencias), "Gerar XML de teste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception erro)
        {
            UseWaitCursor = false;
            MessageBox.Show(this, $"Não foi possível gerar o XML.{Environment.NewLine}{erro.GetBaseException().Message}",
                "Gerar XML de teste", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
            btnXmlTeste.Enabled = true;
            _ocupado = false;
        }
    }

    /// <summary>Mostra as pendências ou o resultado da validação e oferece salvar o XML.</summary>
    private async Task MostrarXmlTesteAsync(XmlGerado resultado)
    {
        const string titulo = "Gerar XML de teste";
        if (resultado.Xml.Length == 0)
        {
            AvisosCadastro.HaPendencias(this, titulo, resultado.Pendencias);
            return;
        }

        var linha = Environment.NewLine;
        var mensagem = resultado.Valido
            ? $"XML assinado e válido contra o XSD do MDF-e 3.00b.{linha}{linha}Chave: {resultado.Chave}{linha}Ambiente: homologação{linha}{linha}" +
              $"É só um teste: o número usado não foi reservado e nada foi enviado à SEFAZ.{linha}{linha}Deseja salvar o arquivo?"
            : $"O XML foi gerado, mas não passou na validação do XSD:{linha}{linha}" +
              string.Join(linha, resultado.Pendencias.Take(10).Select(p => $"• {p}")) +
              (resultado.Pendencias.Count > 10 ? $"{linha}• ... e mais {resultado.Pendencias.Count - 10}" : string.Empty) +
              $"{linha}{linha}Deseja salvar o arquivo para análise?";
        if (MessageBox.Show(this, mensagem, titulo, MessageBoxButtons.YesNo,
                resultado.Valido ? MessageBoxIcon.Information : MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        using var dialogo = new SaveFileDialog
        {
            Title = titulo,
            Filter = "Arquivo XML (*.xml)|*.xml",
            FileName = $"{resultado.Chave}-teste-mdfe.xml",
        };
        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            await File.WriteAllTextAsync(dialogo.FileName, resultado.Xml, new System.Text.UTF8Encoding(false));
        }
    }

    private async Task ExcluirSelecionadoAsync()
    {
        if (Selecionado is not { } mdfe)
        {
            return;
        }

        if (mdfe.Situacao != SituacaoDocumento.EmDigitacao)
        {
            MessageBox.Show(this, "Só MDF-e em digitação podem ser excluídos.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var resposta = MessageBox.Show(this, $"Excluir o rascunho de MDF-e de {mdfe.DataEmissao:dd/MM/yyyy} com destino a {mdfe.UfFim}?",
            Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta == DialogResult.Yes)
        {
            try
            {
                await _cadastros.Mdfes.ExcluirAsync(mdfe.Id);
            }
            catch (RegraNegocioException erro)
            {
                MessageBox.Show(this, string.Join(Environment.NewLine, erro.Pendencias), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception erro)
            {
                MessageBox.Show(this, $"Não foi possível excluir o MDF-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            await RecarregarAsync();
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
