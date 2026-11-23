namespace Emissor.WinForms.Formularios.Mdfe;

partial class UcMdfeCarregamentoDescarga
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
        grpCarregamento = new GroupBox();
        cboMunicipioCarregamento = new ComboBox();
        btnAdicionarCarregamento = new Button();
        lstCarregamento = new ListBox();
        btnRemoverCarregamento = new Button();
        grpDescarga = new GroupBox();
        cboMunicipioDescarga = new ComboBox();
        btnAdicionarDescarga = new Button();
        btnRemoverDescarga = new Button();
        lblDescargas = new Label();
        lstDescargas = new ListBox();
        lblDocumentos = new Label();
        dgvDocumentos = new DataGridView();
        btnAdicionarCtes = new Button();
        grpCarregamento.SuspendLayout();
        grpDescarga.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvDocumentos).BeginInit();
        SuspendLayout();
        //
        // grpCarregamento
        //
        grpCarregamento.Controls.Add(cboMunicipioCarregamento);
        grpCarregamento.Controls.Add(btnAdicionarCarregamento);
        grpCarregamento.Controls.Add(lstCarregamento);
        grpCarregamento.Controls.Add(btnRemoverCarregamento);
        grpCarregamento.Location = new Point(12, 6);
        grpCarregamento.Name = "grpCarregamento";
        grpCarregamento.Size = new Size(656, 150);
        grpCarregamento.TabIndex = 0;
        grpCarregamento.Text = "Municípios de carregamento *";
        //
        // cboMunicipioCarregamento
        //
        cboMunicipioCarregamento.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMunicipioCarregamento.FormattingEnabled = true;
        cboMunicipioCarregamento.Location = new Point(12, 22);
        cboMunicipioCarregamento.Name = "cboMunicipioCarregamento";
        cboMunicipioCarregamento.Size = new Size(300, 23);
        cboMunicipioCarregamento.TabIndex = 0;
        //
        // btnAdicionarCarregamento
        //
        btnAdicionarCarregamento.Location = new Point(318, 20);
        btnAdicionarCarregamento.Name = "btnAdicionarCarregamento";
        btnAdicionarCarregamento.Size = new Size(90, 27);
        btnAdicionarCarregamento.TabIndex = 1;
        btnAdicionarCarregamento.Text = "Adicionar";
        btnAdicionarCarregamento.UseVisualStyleBackColor = true;
        btnAdicionarCarregamento.Click += btnAdicionarCarregamento_Click;
        //
        // lstCarregamento
        //
        lstCarregamento.FormattingEnabled = true;
        lstCarregamento.IntegralHeight = false;
        lstCarregamento.Location = new Point(12, 52);
        lstCarregamento.Name = "lstCarregamento";
        lstCarregamento.Size = new Size(396, 88);
        lstCarregamento.TabIndex = 2;
        //
        // btnRemoverCarregamento
        //
        btnRemoverCarregamento.Location = new Point(414, 52);
        btnRemoverCarregamento.Name = "btnRemoverCarregamento";
        btnRemoverCarregamento.Size = new Size(90, 27);
        btnRemoverCarregamento.TabIndex = 3;
        btnRemoverCarregamento.Text = "Remover";
        btnRemoverCarregamento.UseVisualStyleBackColor = true;
        btnRemoverCarregamento.Click += btnRemoverCarregamento_Click;
        //
        // grpDescarga
        //
        grpDescarga.Controls.Add(cboMunicipioDescarga);
        grpDescarga.Controls.Add(btnAdicionarDescarga);
        grpDescarga.Controls.Add(btnRemoverDescarga);
        grpDescarga.Controls.Add(lblDescargas);
        grpDescarga.Controls.Add(lstDescargas);
        grpDescarga.Controls.Add(lblDocumentos);
        grpDescarga.Controls.Add(dgvDocumentos);
        grpDescarga.Controls.Add(btnAdicionarCtes);
        grpDescarga.Location = new Point(12, 162);
        grpDescarga.Name = "grpDescarga";
        grpDescarga.Size = new Size(656, 318);
        grpDescarga.TabIndex = 1;
        grpDescarga.Text = "Municípios de descarga e documentos *";
        //
        // cboMunicipioDescarga
        //
        cboMunicipioDescarga.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMunicipioDescarga.FormattingEnabled = true;
        cboMunicipioDescarga.Location = new Point(12, 22);
        cboMunicipioDescarga.Name = "cboMunicipioDescarga";
        cboMunicipioDescarga.Size = new Size(300, 23);
        cboMunicipioDescarga.TabIndex = 0;
        //
        // btnAdicionarDescarga
        //
        btnAdicionarDescarga.Location = new Point(318, 20);
        btnAdicionarDescarga.Name = "btnAdicionarDescarga";
        btnAdicionarDescarga.Size = new Size(90, 27);
        btnAdicionarDescarga.TabIndex = 1;
        btnAdicionarDescarga.Text = "Adicionar";
        btnAdicionarDescarga.UseVisualStyleBackColor = true;
        btnAdicionarDescarga.Click += btnAdicionarDescarga_Click;
        //
        // btnRemoverDescarga
        //
        btnRemoverDescarga.Location = new Point(414, 20);
        btnRemoverDescarga.Name = "btnRemoverDescarga";
        btnRemoverDescarga.Size = new Size(90, 27);
        btnRemoverDescarga.TabIndex = 2;
        btnRemoverDescarga.Text = "Remover";
        btnRemoverDescarga.UseVisualStyleBackColor = true;
        btnRemoverDescarga.Click += btnRemoverDescarga_Click;
        //
        // lblDescargas
        //
        lblDescargas.AutoSize = true;
        lblDescargas.Location = new Point(12, 54);
        lblDescargas.Name = "lblDescargas";
        lblDescargas.Size = new Size(63, 15);
        lblDescargas.TabIndex = 3;
        lblDescargas.Text = "Municípios";
        //
        // lstDescargas
        //
        lstDescargas.FormattingEnabled = true;
        lstDescargas.IntegralHeight = false;
        lstDescargas.Location = new Point(12, 72);
        lstDescargas.Name = "lstDescargas";
        lstDescargas.Size = new Size(240, 236);
        lstDescargas.TabIndex = 4;
        lstDescargas.SelectedIndexChanged += lstDescargas_SelectedIndexChanged;
        //
        // lblDocumentos
        //
        lblDocumentos.AutoSize = true;
        lblDocumentos.Location = new Point(264, 54);
        lblDocumentos.Name = "lblDocumentos";
        lblDocumentos.Size = new Size(210, 15);
        lblDocumentos.TabIndex = 5;
        lblDocumentos.Text = "Documentos do município selecionado";
        //
        // dgvDocumentos
        //
        dgvDocumentos.AllowUserToAddRows = true;
        dgvDocumentos.AllowUserToDeleteRows = true;
        dgvDocumentos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvDocumentos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvDocumentos.Location = new Point(264, 72);
        dgvDocumentos.Name = "dgvDocumentos";
        dgvDocumentos.RowHeadersWidth = 24;
        dgvDocumentos.Size = new Size(380, 200);
        dgvDocumentos.TabIndex = 6;
        dgvDocumentos.CellBeginEdit += dgvDocumentos_CellBeginEdit;
        dgvDocumentos.CellFormatting += dgvDocumentos_CellFormatting;
        dgvDocumentos.DataError += dgvDocumentos_DataError;
        dgvDocumentos.RowsAdded += dgvDocumentos_RowsChanged;
        dgvDocumentos.RowsRemoved += dgvDocumentos_RowsChanged;
        //
        // btnAdicionarCtes
        //
        btnAdicionarCtes.Location = new Point(264, 280);
        btnAdicionarCtes.Name = "btnAdicionarCtes";
        btnAdicionarCtes.Size = new Size(210, 27);
        btnAdicionarCtes.TabIndex = 7;
        btnAdicionarCtes.Text = "Adicionar &CT-e emitidos...";
        btnAdicionarCtes.UseVisualStyleBackColor = true;
        btnAdicionarCtes.Click += btnAdicionarCtes_Click;
        //
        // UcMdfeCarregamentoDescarga
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(grpCarregamento);
        Controls.Add(grpDescarga);
        Name = "UcMdfeCarregamentoDescarga";
        Size = new Size(680, 484);
        ((System.ComponentModel.ISupportInitialize)dgvDocumentos).EndInit();
        grpCarregamento.ResumeLayout(false);
        grpCarregamento.PerformLayout();
        grpDescarga.ResumeLayout(false);
        grpDescarga.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private GroupBox grpCarregamento;
    private ComboBox cboMunicipioCarregamento;
    private Button btnAdicionarCarregamento;
    private ListBox lstCarregamento;
    private Button btnRemoverCarregamento;
    private GroupBox grpDescarga;
    private ComboBox cboMunicipioDescarga;
    private Button btnAdicionarDescarga;
    private Button btnRemoverDescarga;
    private Label lblDescargas;
    private ListBox lstDescargas;
    private Label lblDocumentos;
    private DataGridView dgvDocumentos;
    private Button btnAdicionarCtes;
}
