using Emissor.Dominio.Enums;
using Emissor.Dominio.Entidades;
using System.ComponentModel;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>
/// Etapa 2: municípios de carregamento (<c>ide/infMunCarrega</c>) e de descarga (<c>infDoc/infMunDescarga</c>),
/// cada descarga com seus CT-e e NF-e. Os CT-e podem ser escolhidos entre os emitidos por este sistema.
/// </summary>
public partial class UcMdfeCarregamentoDescarga : UserControl, IEtapaMdfe
{
    private List<int> _carregamento = [];
    private List<DescargaMdfe> _descargas = [];
    private CadastrosEmissao? _cadastros;
    private bool _atualizandoTexto;

    /// <summary>Cria a etapa.</summary>
    public UcMdfeCarregamentoDescarga()
    {
        InitializeComponent();
        CriarColunas();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "2. Carregamento e descarga";

    /// <inheritdoc />
    void IEtapaMdfe.Carregar(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
    {
        _cadastros = cadastros;
        _carregamento = [.. mdfe.MunicipiosCarregamento];
        _descargas = mdfe.Descargas.Select(d => d.Copiar()).ToList();
        AtualizarCarregamento();
        AtualizarDescargas(selecionar: 0);
    }

    /// <summary>As listas de municípios seguem as UFs de início e de fim escolhidas na etapa 1.</summary>
    void IEtapaMdfe.AoEntrar(ManifestoEletronico mdfe)
    {
        cboMunicipioCarregamento.Carregar(MunicipiosDaUf(mdfe.UfInicio));
        cboMunicipioDescarga.Carregar(MunicipiosDaUf(mdfe.UfFim));
        grpCarregamento.Text = $"Municípios de carregamento ({mdfe.UfInicio}) *";
        grpDescarga.Text = $"Municípios de descarga ({mdfe.UfFim}) e documentos *";
    }

    /// <inheritdoc />
    void IEtapaMdfe.Ler(ManifestoEletronico mdfe)
    {
        dgvDocumentos.EndEdit();
        Validate();
        mdfe.MunicipiosCarregamento = [.. _carregamento];
        mdfe.Descargas = _descargas
            .Select(d => new DescargaMdfe
            {
                MunicipioCodigo = d.MunicipioCodigo,
                Documentos = d.Documentos
                    .Where(x => x.CteId is not null || !string.IsNullOrWhiteSpace(x.Chave))
                    .Select(x => { var copia = x.Copiar(); copia.Chave = SoDigitos(copia.Chave); return copia; })
                    .ToList(),
            })
            .ToList();
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaMdfe.Validar(ManifestoEletronico mdfe) =>
        mdfe.ValidarCarregamentoDescarga(codigo => _cadastros?.UfDoMunicipio(codigo) ?? string.Empty);

    private void btnAdicionarCarregamento_Click(object? sender, EventArgs e)
    {
        if (cboMunicipioCarregamento.ValorSelecionado<int>() is { } codigo && !_carregamento.Contains(codigo))
        {
            _carregamento.Add(codigo);
            AtualizarCarregamento();
        }
    }

    private void btnRemoverCarregamento_Click(object? sender, EventArgs e)
    {
        if (lstCarregamento.SelectedIndex >= 0)
        {
            _carregamento.RemoveAt(lstCarregamento.SelectedIndex);
            AtualizarCarregamento();
        }
    }

    private void btnAdicionarDescarga_Click(object? sender, EventArgs e)
    {
        if (cboMunicipioDescarga.ValorSelecionado<int>() is not { } codigo)
        {
            return;
        }

        var existente = _descargas.FindIndex(d => d.MunicipioCodigo == codigo);
        if (existente < 0)
        {
            _descargas.Add(new DescargaMdfe { MunicipioCodigo = codigo });
            existente = _descargas.Count - 1;
        }

        AtualizarDescargas(existente);
    }

    private void btnRemoverDescarga_Click(object? sender, EventArgs e)
    {
        if (lstDescargas.SelectedIndex < 0)
        {
            return;
        }

        var resposta = MessageBox.Show(this, "Remover o município de descarga e os documentos dele?", Titulo,
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta == DialogResult.Yes)
        {
            _descargas.RemoveAt(lstDescargas.SelectedIndex);
            AtualizarDescargas(0);
        }
    }

    /// <summary>Escolhe CT-e emitidos por este sistema e os inclui no município de descarga selecionado.</summary>
    private async void btnAdicionarCtes_Click(object? sender, EventArgs e)
    {
        if (DescargaSelecionada is not { } descarga)
        {
            MessageBox.Show(this, "Adicione e selecione um município de descarga antes.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var jaVinculados = _descargas.SelectMany(d => d.Documentos).Where(d => d.CteId is not null).Select(d => d.CteId!.Value).ToHashSet();
        List<ConhecimentoTransporte> disponiveis;
        btnAdicionarCtes.Enabled = false;
        try
        {
            disponiveis = (await _cadastros!.Ctes.ListarPorSituacaoAsync(_cadastros.Empresa.Id, SituacaoDocumento.ProntoParaTransmitir))
                .Where(c => !jaVinculados.Contains(c.Id))
                .OrderByDescending(c => c.DataEmissao)
                .ToList();
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler os CT-e.{Environment.NewLine}{erro.GetBaseException().Message}",
                Titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        finally
        {
            btnAdicionarCtes.Enabled = true;
        }

        using var tela = new FormSelecaoCtes(disponiveis, _cadastros!);
        if (tela.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        dgvDocumentos.EndEdit();
        descarga.Documentos.AddRange(tela.Selecionados.Select(id => new DocumentoMdfe { Tipo = TipoDocumentoMdfe.Cte, CteId = id }));
        AtualizarDescargas(lstDescargas.SelectedIndex);
    }

    private void lstDescargas_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_atualizandoTexto)
        {
            MostrarDocumentos();
        }
    }

    /// <summary>CT-e escolhidos da lista mostram a descrição em vez da chave e não podem ser editados.</summary>
    private void dgvDocumentos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex == colChave.Index
            && dgvDocumentos.Rows[e.RowIndex].DataBoundItem is DocumentoMdfe { CteId: not null } documento)
        {
            e.Value = documento.ChaveDescricao;
            e.FormattingApplied = true;
        }
    }

    private void dgvDocumentos_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
    {
        if (dgvDocumentos.Rows[e.RowIndex].DataBoundItem is DocumentoMdfe { CteId: not null })
        {
            e.Cancel = true;
        }
    }

    /// <summary>Atualiza a contagem de documentos do município na lista.</summary>
    private void dgvDocumentos_RowsChanged(object? sender, EventArgs e) => AtualizarTextoDescargas();

    private void dgvDocumentos_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    private DescargaMdfe? DescargaSelecionada =>
        lstDescargas.SelectedIndex >= 0 && lstDescargas.SelectedIndex < _descargas.Count ? _descargas[lstDescargas.SelectedIndex] : null;

    private void AtualizarCarregamento()
    {
        lstCarregamento.Items.Clear();
        foreach (var codigo in _carregamento)
        {
            lstCarregamento.Items.Add(_cadastros?.Municipio(codigo) ?? codigo.ToString());
        }
    }

    private void AtualizarDescargas(int selecionar)
    {
        lstDescargas.Items.Clear();
        foreach (var descarga in _descargas)
        {
            lstDescargas.Items.Add(TextoDescarga(descarga));
        }

        if (_descargas.Count > 0)
        {
            lstDescargas.SelectedIndex = Math.Clamp(selecionar, 0, _descargas.Count - 1);
        }

        MostrarDocumentos();
    }

    private void AtualizarTextoDescargas()
    {
        if (_atualizandoTexto)
        {
            return;
        }

        _atualizandoTexto = true;
        var selecionado = lstDescargas.SelectedIndex;
        for (var i = 0; i < _descargas.Count && i < lstDescargas.Items.Count; i++)
        {
            var texto = TextoDescarga(_descargas[i]);
            if (!Equals(lstDescargas.Items[i], texto))
            {
                lstDescargas.Items[i] = texto;
            }
        }

        lstDescargas.SelectedIndex = selecionado;
        _atualizandoTexto = false;
    }

    /// <summary>A grade edita diretamente a lista de documentos do município selecionado.</summary>
    private void MostrarDocumentos()
    {
        dgvDocumentos.EndEdit();
        var descarga = DescargaSelecionada;
        dgvDocumentos.DataSource = descarga is null ? null : new BindingList<DocumentoMdfe>(descarga.Documentos);
        dgvDocumentos.Enabled = descarga is not null;
        btnAdicionarCtes.Enabled = descarga is not null;
    }

    private IEnumerable<Opcao<int>> MunicipiosDaUf(string uf) => _cadastros?.Municipios.OpcoesDaUf(uf) ?? [];

    /// <summary>Município e quantidade de documentos, para a lista de descargas.</summary>
    private string TextoDescarga(DescargaMdfe descarga) =>
        $"{_cadastros?.Municipio(descarga.MunicipioCodigo) ?? descarga.MunicipioCodigo.ToString()} ({descarga.Documentos.Count} doc.)";

    private void CriarColunas()
    {
        dgvDocumentos.AutoGenerateColumns = false;

        // As dicas da grade mostravam o valor interno do tipo ("Cte") e ficavam soltas na tela.
        dgvDocumentos.ShowCellToolTips = false;
        dgvDocumentos.Columns.Add(new DataGridViewComboBoxColumn
        {
            DataPropertyName = nameof(DocumentoMdfe.Tipo),
            HeaderText = "Tipo",
            DataSource = new List<Opcao<TipoDocumentoMdfe>> { new(TipoDocumentoMdfe.Cte, "CT-e"), new(TipoDocumentoMdfe.Nfe, "NF-e") },
            ValueMember = nameof(Opcao<int>.Valor),
            DisplayMember = nameof(Opcao<int>.Texto),
            FillWeight = 18,
        });
        colChave = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DocumentoMdfe.Chave),
            HeaderText = "Chave de acesso (44 dígitos)",
            MaxInputLength = 54,
            FillWeight = 82,
        };
        dgvDocumentos.Columns.Add(colChave);
    }

    private DataGridViewTextBoxColumn colChave = null!;

    private static string SoDigitos(string texto) => new(texto.Where(char.IsDigit).ToArray());
}
