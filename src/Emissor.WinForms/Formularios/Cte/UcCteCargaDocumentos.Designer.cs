namespace Emissor.WinForms.Formularios.Cte;

partial class UcCteCargaDocumentos
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        grpComplemento = new GroupBox();
        lblChaveComplementado = new Label();
        txtChaveComplementado = new TextBox();
        grpCarga = new GroupBox();
        lblValorCarga = new Label();
        nudValorCarga = new NumericUpDown();
        lblProdutoPredominante = new Label();
        txtProdutoPredominante = new TextBox();
        lblOutrasCaracteristicas = new Label();
        txtOutrasCaracteristicas = new TextBox();
        lblQuantidades = new Label();
        dgvQuantidades = new DataGridView();
        grpNfe = new GroupBox();
        dgvNfe = new DataGridView();
        btnImportarXml = new Button();
        btnBuscarChave = new Button();
        grpDocumentoAnterior = new GroupBox();
        dgvDocumentosAnteriores = new DataGridView();
        grpComplemento.SuspendLayout();
        grpCarga.SuspendLayout();
        grpNfe.SuspendLayout();
        grpDocumentoAnterior.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudValorCarga).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvQuantidades).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvNfe).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvDocumentosAnteriores).BeginInit();
        SuspendLayout();
        //
        // grpComplemento
        //
        grpComplemento.Controls.Add(lblChaveComplementado);
        grpComplemento.Controls.Add(txtChaveComplementado);
        grpComplemento.Location = new Point(12, 6);
        grpComplemento.Name = "grpComplemento";
        grpComplemento.Size = new Size(656, 80);
        grpComplemento.TabIndex = 0;
        grpComplemento.Text = "CT-e complementado";
        //
        // lblChaveComplementado
        //
        lblChaveComplementado.AutoSize = true;
        lblChaveComplementado.Location = new Point(12, 22);
        lblChaveComplementado.Name = "lblChaveComplementado";
        lblChaveComplementado.Size = new Size(252, 15);
        lblChaveComplementado.TabIndex = 0;
        lblChaveComplementado.Text = "Chave do CT-e complementado (44 dígitos) *";
        //
        // txtChaveComplementado
        //
        txtChaveComplementado.Location = new Point(12, 40);
        txtChaveComplementado.MaxLength = 54;
        txtChaveComplementado.Name = "txtChaveComplementado";
        txtChaveComplementado.Size = new Size(400, 23);
        txtChaveComplementado.TabIndex = 1;
        //
        // grpCarga
        //
        grpCarga.Controls.Add(lblValorCarga);
        grpCarga.Controls.Add(nudValorCarga);
        grpCarga.Controls.Add(lblProdutoPredominante);
        grpCarga.Controls.Add(txtProdutoPredominante);
        grpCarga.Controls.Add(lblOutrasCaracteristicas);
        grpCarga.Controls.Add(txtOutrasCaracteristicas);
        grpCarga.Controls.Add(lblQuantidades);
        grpCarga.Controls.Add(dgvQuantidades);
        grpCarga.Location = new Point(12, 6);
        grpCarga.Name = "grpCarga";
        grpCarga.Size = new Size(656, 200);
        grpCarga.TabIndex = 1;
        grpCarga.Text = "Carga";
        //
        // lblValorCarga
        //
        lblValorCarga.AutoSize = true;
        lblValorCarga.Location = new Point(12, 22);
        lblValorCarga.Name = "lblValorCarga";
        lblValorCarga.Size = new Size(94, 15);
        lblValorCarga.TabIndex = 0;
        lblValorCarga.Text = "Valor da carga *";
        //
        // nudValorCarga
        //
        nudValorCarga.DecimalPlaces = 2;
        nudValorCarga.Location = new Point(12, 40);
        nudValorCarga.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        nudValorCarga.Name = "nudValorCarga";
        nudValorCarga.Size = new Size(140, 23);
        nudValorCarga.TabIndex = 1;
        nudValorCarga.TextAlign = HorizontalAlignment.Right;
        nudValorCarga.ThousandsSeparator = true;
        //
        // lblProdutoPredominante
        //
        lblProdutoPredominante.AutoSize = true;
        lblProdutoPredominante.Location = new Point(164, 22);
        lblProdutoPredominante.Name = "lblProdutoPredominante";
        lblProdutoPredominante.Size = new Size(135, 15);
        lblProdutoPredominante.TabIndex = 2;
        lblProdutoPredominante.Text = "Produto predominante *";
        //
        // txtProdutoPredominante
        //
        txtProdutoPredominante.Location = new Point(164, 40);
        txtProdutoPredominante.MaxLength = 60;
        txtProdutoPredominante.Name = "txtProdutoPredominante";
        txtProdutoPredominante.Size = new Size(220, 23);
        txtProdutoPredominante.TabIndex = 3;
        //
        // lblOutrasCaracteristicas
        //
        lblOutrasCaracteristicas.AutoSize = true;
        lblOutrasCaracteristicas.Location = new Point(396, 22);
        lblOutrasCaracteristicas.Name = "lblOutrasCaracteristicas";
        lblOutrasCaracteristicas.Size = new Size(124, 15);
        lblOutrasCaracteristicas.TabIndex = 4;
        lblOutrasCaracteristicas.Text = "Outras características";
        //
        // txtOutrasCaracteristicas
        //
        txtOutrasCaracteristicas.Location = new Point(396, 40);
        txtOutrasCaracteristicas.MaxLength = 30;
        txtOutrasCaracteristicas.Name = "txtOutrasCaracteristicas";
        txtOutrasCaracteristicas.Size = new Size(248, 23);
        txtOutrasCaracteristicas.TabIndex = 5;
        //
        // lblQuantidades
        //
        lblQuantidades.AutoSize = true;
        lblQuantidades.Location = new Point(12, 70);
        lblQuantidades.Name = "lblQuantidades";
        lblQuantidades.Size = new Size(81, 15);
        lblQuantidades.TabIndex = 6;
        lblQuantidades.Text = "Quantidades *";
        //
        // dgvQuantidades
        //
        dgvQuantidades.AllowUserToAddRows = true;
        dgvQuantidades.AllowUserToDeleteRows = true;
        dgvQuantidades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvQuantidades.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvQuantidades.Location = new Point(12, 88);
        dgvQuantidades.Name = "dgvQuantidades";
        dgvQuantidades.RowHeadersWidth = 24;
        dgvQuantidades.Size = new Size(632, 102);
        dgvQuantidades.TabIndex = 7;
        dgvQuantidades.DataError += dgv_DataError;
        //
        // grpNfe
        //
        grpNfe.Controls.Add(dgvNfe);
        grpNfe.Controls.Add(btnImportarXml);
        grpNfe.Controls.Add(btnBuscarChave);
        grpNfe.Location = new Point(12, 212);
        grpNfe.Name = "grpNfe";
        grpNfe.Size = new Size(656, 150);
        grpNfe.TabIndex = 2;
        grpNfe.Text = "Documentos transportados (NF-e) *";
        //
        // dgvNfe
        //
        dgvNfe.AllowUserToAddRows = true;
        dgvNfe.AllowUserToDeleteRows = true;
        dgvNfe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvNfe.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvNfe.Location = new Point(12, 22);
        dgvNfe.Name = "dgvNfe";
        dgvNfe.RowHeadersWidth = 24;
        dgvNfe.Size = new Size(632, 88);
        dgvNfe.TabIndex = 0;
        dgvNfe.DataError += dgv_DataError;
        //
        // btnImportarXml
        //
        btnImportarXml.Location = new Point(12, 116);
        btnImportarXml.Name = "btnImportarXml";
        btnImportarXml.Size = new Size(140, 27);
        btnImportarXml.TabIndex = 1;
        btnImportarXml.Text = "&Importar XML...";
        btnImportarXml.UseVisualStyleBackColor = true;
        btnImportarXml.Click += btnImportarXml_Click;
        //
        // btnBuscarChave
        //
        btnBuscarChave.Location = new Point(158, 116);
        btnBuscarChave.Name = "btnBuscarChave";
        btnBuscarChave.Size = new Size(170, 27);
        btnBuscarChave.TabIndex = 2;
        btnBuscarChave.Text = "&Buscar pela chave...";
        btnBuscarChave.UseVisualStyleBackColor = true;
        btnBuscarChave.Click += btnBuscarChave_Click;
        //
        // grpDocumentoAnterior
        //
        grpDocumentoAnterior.Controls.Add(dgvDocumentosAnteriores);
        grpDocumentoAnterior.Location = new Point(12, 368);
        grpDocumentoAnterior.Name = "grpDocumentoAnterior";
        grpDocumentoAnterior.Size = new Size(656, 110);
        grpDocumentoAnterior.TabIndex = 3;
        grpDocumentoAnterior.Text = "Documento anterior (subcontratação e redespacho) *";
        //
        // dgvDocumentosAnteriores
        //
        dgvDocumentosAnteriores.AllowUserToAddRows = true;
        dgvDocumentosAnteriores.AllowUserToDeleteRows = true;
        dgvDocumentosAnteriores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvDocumentosAnteriores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvDocumentosAnteriores.Location = new Point(12, 22);
        dgvDocumentosAnteriores.Name = "dgvDocumentosAnteriores";
        dgvDocumentosAnteriores.RowHeadersWidth = 24;
        dgvDocumentosAnteriores.Size = new Size(632, 78);
        dgvDocumentosAnteriores.TabIndex = 0;
        dgvDocumentosAnteriores.DataError += dgv_DataError;
        //
        // UcCteCargaDocumentos
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(grpComplemento);
        Controls.Add(grpCarga);
        Controls.Add(grpNfe);
        Controls.Add(grpDocumentoAnterior);
        Name = "UcCteCargaDocumentos";
        Size = new Size(680, 484);
        ((System.ComponentModel.ISupportInitialize)nudValorCarga).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvQuantidades).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvNfe).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvDocumentosAnteriores).EndInit();
        grpComplemento.ResumeLayout(false);
        grpComplemento.PerformLayout();
        grpCarga.ResumeLayout(false);
        grpCarga.PerformLayout();
        grpNfe.ResumeLayout(false);
        grpNfe.PerformLayout();
        grpDocumentoAnterior.ResumeLayout(false);
        grpDocumentoAnterior.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private GroupBox grpComplemento;
    private Label lblChaveComplementado;
    private TextBox txtChaveComplementado;
    private GroupBox grpCarga;
    private Label lblValorCarga;
    private NumericUpDown nudValorCarga;
    private Label lblProdutoPredominante;
    private TextBox txtProdutoPredominante;
    private Label lblOutrasCaracteristicas;
    private TextBox txtOutrasCaracteristicas;
    private Label lblQuantidades;
    private DataGridView dgvQuantidades;
    private GroupBox grpNfe;
    private DataGridView dgvNfe;
    private Button btnImportarXml;
    private Button btnBuscarChave;
    private GroupBox grpDocumentoAnterior;
    private DataGridView dgvDocumentosAnteriores;
}
