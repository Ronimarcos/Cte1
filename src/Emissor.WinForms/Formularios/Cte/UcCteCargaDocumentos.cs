using Emissor.Dominio.Enums;
using Emissor.Dominio.Entidades;
using System.ComponentModel;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>
/// Etapa 3: carga (<c>infCarga</c>), NF-e transportadas (<c>infDoc/infNFe</c>), documento anterior (<c>docAnt</c>,
/// só em subcontratação e redespacho) e, no CT-e de complemento, a chave do CT-e complementado (<c>infCteComp</c>).
/// </summary>
public partial class UcCteCargaDocumentos : UserControl, IEtapaCte
{
    private BindingList<QuantidadeCarga> _quantidades = [];
    private BindingList<DocumentoNfeCte> _documentosNfe = [];
    private BindingList<DocumentoAnteriorCte> _documentosAnteriores = [];

    /// <summary>Cria a etapa.</summary>
    public UcCteCargaDocumentos()
    {
        InitializeComponent();
        CriarColunas();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "3. Carga e documentos";

    /// <inheritdoc />
    void IEtapaCte.Carregar(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
    {
        nudValorCarga.Value = cte.ValorCarga ?? 0;
        txtProdutoPredominante.Text = cte.ProdutoPredominante;
        txtOutrasCaracteristicas.Text = cte.OutrasCaracteristicas;
        txtChaveComplementado.Text = cte.ChaveCteComplementado;

        // As grades editam cópias; o CT-e só recebe as linhas em Ler.
        _quantidades = new(cte.Quantidades.Select(q => q.Copiar()).ToList());
        _documentosNfe = new(cte.DocumentosNfe.Select(d => d.Copiar()).ToList());
        _documentosAnteriores = new(cte.DocumentosAnteriores.Select(d => d.Copiar()).ToList());
        dgvQuantidades.DataSource = _quantidades;
        dgvNfe.DataSource = _documentosNfe;
        dgvDocumentosAnteriores.DataSource = _documentosAnteriores;
    }

    /// <summary>Mostra só os grupos que valem para o tipo do CT-e e do serviço escolhidos na etapa 1.</summary>
    void IEtapaCte.AoEntrar(ConhecimentoTransporte cte)
    {
        grpComplemento.Visible = cte.EhComplemento;
        grpCarga.Visible = !cte.EhComplemento;
        grpNfe.Visible = !cte.EhComplemento;
        grpDocumentoAnterior.Visible = cte.ExigeDocumentoAnterior;
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(ConhecimentoTransporte cte)
    {
        FinalizarEdicao();
        cte.ValorCarga = nudValorCarga.Value;
        cte.ProdutoPredominante = txtProdutoPredominante.Text.Trim();
        cte.OutrasCaracteristicas = txtOutrasCaracteristicas.Text.Trim();
        cte.ChaveCteComplementado = SoDigitos(txtChaveComplementado.Text);

        // Linhas totalmente em branco são ignoradas.
        cte.Quantidades = _quantidades
            .Where(q => q.Quantidade != 0 || !string.IsNullOrWhiteSpace(q.TipoMedida))
            .Select(q => { var copia = q.Copiar(); copia.TipoMedida = copia.TipoMedida.Trim(); return copia; })
            .ToList();
        cte.DocumentosNfe = _documentosNfe
            .Where(d => !string.IsNullOrWhiteSpace(d.Chave))
            .Select(d => new DocumentoNfeCte { Chave = SoDigitos(d.Chave) })
            .ToList();
        cte.DocumentosAnteriores = _documentosAnteriores
            .Where(d => !string.IsNullOrWhiteSpace(d.CnpjCpfEmitente) || !string.IsNullOrWhiteSpace(d.NomeEmitente) || !string.IsNullOrWhiteSpace(d.ChaveCte))
            .Select(d => new DocumentoAnteriorCte
            {
                CnpjCpfEmitente = d.CnpjCpfEmitente.Trim(),
                NomeEmitente = d.NomeEmitente.Trim(),
                ChaveCte = SoDigitos(d.ChaveCte),
            })
            .ToList();

        // Documento anterior só existe em subcontratação e redespacho.
        if (!cte.ExigeDocumentoAnterior)
        {
            cte.DocumentosAnteriores = [];
        }
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaCte.Validar(ConhecimentoTransporte cte) => cte.ValidarCargaDocumentos();

    private void btnImportarXml_Click(object? sender, EventArgs e) => AvisarIndisponivel("A importação do XML da NF-e");

    private void btnBuscarChave_Click(object? sender, EventArgs e) => AvisarIndisponivel("A busca da NF-e pela chave (Distribuição DF-e)");

    /// <summary>Evita a exceção padrão da grade quando o valor digitado não é número.</summary>
    private void dgv_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.ThrowException = false;
        MessageBox.Show(this, "Valor inválido nesta coluna. Use apenas números.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void AvisarIndisponivel(string recurso) =>
        MessageBox.Show(this, $"{recurso} fica disponível na implementação do motor DF-e. No protótipo, digite as chaves na grade.",
            Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);

    /// <summary>Confirma a célula em edição para que o valor entre na lista antes da leitura.</summary>
    private void FinalizarEdicao()
    {
        foreach (var grade in new[] { dgvQuantidades, dgvNfe, dgvDocumentosAnteriores })
        {
            grade.EndEdit();
        }

        Validate();
    }

    /// <summary>Colunas das grades, criadas no código porque dependem das listas de opções.</summary>
    private void CriarColunas()
    {
        dgvQuantidades.AutoGenerateColumns = false;
        dgvQuantidades.Columns.Add(new DataGridViewComboBoxColumn
        {
            DataPropertyName = nameof(QuantidadeCarga.Unidade),
            HeaderText = "Unidade",
            DataSource = OpcoesCadastro.UnidadesMedidaCarga.ToList(),
            ValueMember = nameof(Opcao<int>.Valor),
            DisplayMember = nameof(Opcao<int>.Texto),
            FillWeight = 25,
        });
        dgvQuantidades.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(QuantidadeCarga.TipoMedida),
            HeaderText = "Tipo da medida (ex.: PESO BRUTO)",
            MaxInputLength = 20,
            FillWeight = 45,
        });
        dgvQuantidades.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(QuantidadeCarga.Quantidade),
            HeaderText = "Quantidade",
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N4", Alignment = DataGridViewContentAlignment.MiddleRight },
            FillWeight = 30,
        });

        dgvNfe.AutoGenerateColumns = false;
        dgvNfe.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DocumentoNfeCte.Chave),
            HeaderText = "Chave de acesso da NF-e (44 dígitos)",
            MaxInputLength = 54,
        });

        dgvDocumentosAnteriores.AutoGenerateColumns = false;
        dgvDocumentosAnteriores.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DocumentoAnteriorCte.CnpjCpfEmitente),
            HeaderText = "CNPJ/CPF do emitente",
            MaxInputLength = 18,
            FillWeight = 22,
        });
        dgvDocumentosAnteriores.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DocumentoAnteriorCte.NomeEmitente),
            HeaderText = "Emitente",
            MaxInputLength = 60,
            FillWeight = 33,
        });
        dgvDocumentosAnteriores.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DocumentoAnteriorCte.ChaveCte),
            HeaderText = "Chave do CT-e anterior",
            MaxInputLength = 54,
            FillWeight = 45,
        });
    }

    private static string SoDigitos(string texto) => new(texto.Where(char.IsDigit).ToArray());
}
