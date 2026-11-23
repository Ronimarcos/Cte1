namespace Emissor.WinForms.Formularios.Mdfe;

partial class UcMdfeCargaSeguro
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
        grpProduto = new GroupBox();
        lblTipoCarga = new Label();
        cboTipoCarga = new ComboBox();
        lblProduto = new Label();
        txtProduto = new TextBox();
        grpSeguro = new GroupBox();
        lblApolice = new Label();
        cboApolice = new ComboBox();
        lblDadosApolice = new Label();
        lblAverbacoes = new Label();
        txtAverbacoes = new TextBox();
        grpTotais = new GroupBox();
        lblQuantidades = new Label();
        lblValorCarga = new Label();
        nudValorCarga = new NumericUpDown();
        lblUnidadePeso = new Label();
        cboUnidadePeso = new ComboBox();
        lblPeso = new Label();
        nudPeso = new NumericUpDown();
        btnRecalcular = new Button();
        lblDicaRecalcular = new Label();
        grpProduto.SuspendLayout();
        grpSeguro.SuspendLayout();
        grpTotais.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudValorCarga).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudPeso).BeginInit();
        SuspendLayout();
        //
        // grpProduto
        //
        grpProduto.Controls.Add(lblTipoCarga);
        grpProduto.Controls.Add(cboTipoCarga);
        grpProduto.Controls.Add(lblProduto);
        grpProduto.Controls.Add(txtProduto);
        grpProduto.Location = new Point(12, 6);
        grpProduto.Name = "grpProduto";
        grpProduto.Size = new Size(656, 80);
        grpProduto.TabIndex = 0;
        grpProduto.Text = "Produto predominante *";
        //
        // lblTipoCarga
        //
        lblTipoCarga.AutoSize = true;
        lblTipoCarga.Location = new Point(12, 22);
        lblTipoCarga.Name = "lblTipoCarga";
        lblTipoCarga.Size = new Size(86, 15);
        lblTipoCarga.TabIndex = 0;
        lblTipoCarga.Text = "Tipo de carga *";
        //
        // cboTipoCarga
        //
        cboTipoCarga.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipoCarga.FormattingEnabled = true;
        cboTipoCarga.Location = new Point(12, 40);
        cboTipoCarga.Name = "cboTipoCarga";
        cboTipoCarga.Size = new Size(260, 23);
        cboTipoCarga.TabIndex = 1;
        //
        // lblProduto
        //
        lblProduto.AutoSize = true;
        lblProduto.Location = new Point(284, 22);
        lblProduto.Name = "lblProduto";
        lblProduto.Size = new Size(66, 15);
        lblProduto.TabIndex = 2;
        lblProduto.Text = "Descrição *";
        //
        // txtProduto
        //
        txtProduto.Location = new Point(284, 40);
        txtProduto.MaxLength = 120;
        txtProduto.Name = "txtProduto";
        txtProduto.Size = new Size(360, 23);
        txtProduto.TabIndex = 3;
        //
        // grpSeguro
        //
        grpSeguro.Controls.Add(lblApolice);
        grpSeguro.Controls.Add(cboApolice);
        grpSeguro.Controls.Add(lblDadosApolice);
        grpSeguro.Controls.Add(lblAverbacoes);
        grpSeguro.Controls.Add(txtAverbacoes);
        grpSeguro.Location = new Point(12, 92);
        grpSeguro.Name = "grpSeguro";
        grpSeguro.Size = new Size(656, 172);
        grpSeguro.TabIndex = 1;
        grpSeguro.Text = "Seguro da carga";
        //
        // lblApolice
        //
        lblApolice.AutoSize = true;
        lblApolice.Location = new Point(12, 22);
        lblApolice.Name = "lblApolice";
        lblApolice.Size = new Size(45, 15);
        lblApolice.TabIndex = 0;
        lblApolice.Text = "Apólice";
        //
        // cboApolice
        //
        cboApolice.DropDownStyle = ComboBoxStyle.DropDownList;
        cboApolice.FormattingEnabled = true;
        cboApolice.Location = new Point(12, 40);
        cboApolice.Name = "cboApolice";
        cboApolice.Size = new Size(500, 23);
        cboApolice.TabIndex = 1;
        cboApolice.SelectedIndexChanged += cboApolice_SelectedIndexChanged;
        //
        // lblDadosApolice
        //
        lblDadosApolice.AutoSize = true;
        lblDadosApolice.ForeColor = SystemColors.GrayText;
        lblDadosApolice.Location = new Point(12, 68);
        lblDadosApolice.Name = "lblDadosApolice";
        lblDadosApolice.Size = new Size(0, 15);
        lblDadosApolice.TabIndex = 2;
        //
        // lblAverbacoes
        //
        lblAverbacoes.AutoSize = true;
        lblAverbacoes.Location = new Point(12, 90);
        lblAverbacoes.Name = "lblAverbacoes";
        lblAverbacoes.Size = new Size(155, 15);
        lblAverbacoes.TabIndex = 3;
        lblAverbacoes.Text = "Averbações (uma por linha)";
        //
        // txtAverbacoes
        //
        txtAverbacoes.Location = new Point(12, 108);
        txtAverbacoes.Multiline = true;
        txtAverbacoes.Name = "txtAverbacoes";
        txtAverbacoes.ScrollBars = ScrollBars.Vertical;
        txtAverbacoes.Size = new Size(500, 54);
        txtAverbacoes.TabIndex = 4;
        //
        // grpTotais
        //
        grpTotais.Controls.Add(lblQuantidades);
        grpTotais.Controls.Add(lblValorCarga);
        grpTotais.Controls.Add(nudValorCarga);
        grpTotais.Controls.Add(lblUnidadePeso);
        grpTotais.Controls.Add(cboUnidadePeso);
        grpTotais.Controls.Add(lblPeso);
        grpTotais.Controls.Add(nudPeso);
        grpTotais.Controls.Add(btnRecalcular);
        grpTotais.Controls.Add(lblDicaRecalcular);
        grpTotais.Location = new Point(12, 270);
        grpTotais.Name = "grpTotais";
        grpTotais.Size = new Size(656, 130);
        grpTotais.TabIndex = 2;
        grpTotais.Text = "Totais";
        //
        // lblQuantidades
        //
        lblQuantidades.AutoSize = true;
        lblQuantidades.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblQuantidades.Location = new Point(12, 22);
        lblQuantidades.Name = "lblQuantidades";
        lblQuantidades.Size = new Size(90, 15);
        lblQuantidades.TabIndex = 0;
        lblQuantidades.Text = "CT-e: 0    NF-e: 0";
        //
        // lblValorCarga
        //
        lblValorCarga.AutoSize = true;
        lblValorCarga.Location = new Point(12, 46);
        lblValorCarga.Name = "lblValorCarga";
        lblValorCarga.Size = new Size(93, 15);
        lblValorCarga.TabIndex = 1;
        lblValorCarga.Text = "Valor da carga *";
        //
        // nudValorCarga
        //
        nudValorCarga.DecimalPlaces = 2;
        nudValorCarga.Location = new Point(12, 64);
        nudValorCarga.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        nudValorCarga.Name = "nudValorCarga";
        nudValorCarga.Size = new Size(150, 23);
        nudValorCarga.TabIndex = 2;
        nudValorCarga.TextAlign = HorizontalAlignment.Right;
        nudValorCarga.ThousandsSeparator = true;
        //
        // lblUnidadePeso
        //
        lblUnidadePeso.AutoSize = true;
        lblUnidadePeso.Location = new Point(174, 46);
        lblUnidadePeso.Name = "lblUnidadePeso";
        lblUnidadePeso.Size = new Size(58, 15);
        lblUnidadePeso.TabIndex = 3;
        lblUnidadePeso.Text = "Unidade *";
        //
        // cboUnidadePeso
        //
        cboUnidadePeso.DropDownStyle = ComboBoxStyle.DropDownList;
        cboUnidadePeso.FormattingEnabled = true;
        cboUnidadePeso.Location = new Point(174, 64);
        cboUnidadePeso.Name = "cboUnidadePeso";
        cboUnidadePeso.Size = new Size(110, 23);
        cboUnidadePeso.TabIndex = 4;
        //
        // lblPeso
        //
        lblPeso.AutoSize = true;
        lblPeso.Location = new Point(296, 46);
        lblPeso.Name = "lblPeso";
        lblPeso.Size = new Size(72, 15);
        lblPeso.TabIndex = 5;
        lblPeso.Text = "Peso bruto *";
        //
        // nudPeso
        //
        nudPeso.DecimalPlaces = 4;
        nudPeso.Location = new Point(296, 64);
        nudPeso.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        nudPeso.Name = "nudPeso";
        nudPeso.Size = new Size(150, 23);
        nudPeso.TabIndex = 6;
        nudPeso.TextAlign = HorizontalAlignment.Right;
        nudPeso.ThousandsSeparator = true;
        //
        // btnRecalcular
        //
        btnRecalcular.Location = new Point(458, 62);
        btnRecalcular.Name = "btnRecalcular";
        btnRecalcular.Size = new Size(186, 27);
        btnRecalcular.TabIndex = 7;
        btnRecalcular.Text = "&Recalcular pelos CT-e";
        btnRecalcular.UseVisualStyleBackColor = true;
        btnRecalcular.Click += btnRecalcular_Click;
        //
        // lblDicaRecalcular
        //
        lblDicaRecalcular.AutoSize = true;
        lblDicaRecalcular.ForeColor = SystemColors.GrayText;
        lblDicaRecalcular.Location = new Point(12, 98);
        lblDicaRecalcular.Name = "lblDicaRecalcular";
        lblDicaRecalcular.Size = new Size(470, 15);
        lblDicaRecalcular.TabIndex = 8;
        lblDicaRecalcular.Text = "Soma o valor da carga e o peso (KG/TON) dos CT-e do sistema vinculados na etapa 2.";
        //
        // UcMdfeCargaSeguro
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(grpProduto);
        Controls.Add(grpSeguro);
        Controls.Add(grpTotais);
        Name = "UcMdfeCargaSeguro";
        Size = new Size(680, 484);
        ((System.ComponentModel.ISupportInitialize)nudValorCarga).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudPeso).EndInit();
        grpProduto.ResumeLayout(false);
        grpProduto.PerformLayout();
        grpSeguro.ResumeLayout(false);
        grpSeguro.PerformLayout();
        grpTotais.ResumeLayout(false);
        grpTotais.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private GroupBox grpProduto;
    private Label lblTipoCarga;
    private ComboBox cboTipoCarga;
    private Label lblProduto;
    private TextBox txtProduto;
    private GroupBox grpSeguro;
    private Label lblApolice;
    private ComboBox cboApolice;
    private Label lblDadosApolice;
    private Label lblAverbacoes;
    private TextBox txtAverbacoes;
    private GroupBox grpTotais;
    private Label lblQuantidades;
    private Label lblValorCarga;
    private NumericUpDown nudValorCarga;
    private Label lblUnidadePeso;
    private ComboBox cboUnidadePeso;
    private Label lblPeso;
    private NumericUpDown nudPeso;
    private Button btnRecalcular;
    private Label lblDicaRecalcular;
}
