namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoVeiculo
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

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblTipo = new Label();
        cboTipo = new ComboBox();
        lblPlaca = new Label();
        txtPlaca = new TextBox();
        lblRenavam = new Label();
        txtRenavam = new TextBox();
        lblCodigoInterno = new Label();
        txtCodigoInterno = new TextBox();
        lblUf = new Label();
        cboUf = new ComboBox();
        lblTara = new Label();
        nudTara = new NumericUpDown();
        lblCapacidadeKg = new Label();
        nudCapacidadeKg = new NumericUpDown();
        lblCapacidadeM3 = new Label();
        nudCapacidadeM3 = new NumericUpDown();
        lblTipoRodado = new Label();
        cboTipoRodado = new ComboBox();
        lblTipoCarroceria = new Label();
        cboTipoCarroceria = new ComboBox();
        chkProprio = new CheckBox();
        lblProprietario = new Label();
        cboProprietario = new ComboBox();
        pnlBotoes = new Panel();
        btnSalvar = new Button();
        btnCancelar = new Button();
        ((System.ComponentModel.ISupportInitialize)nudTara).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudCapacidadeKg).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudCapacidadeM3).BeginInit();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // lblTipo
        //
        lblTipo.AutoSize = true;
        lblTipo.Location = new Point(12, 12);
        lblTipo.Name = "lblTipo";
        lblTipo.Size = new Size(39, 15);
        lblTipo.TabIndex = 0;
        lblTipo.Text = "Tipo *";
        //
        // cboTipo
        //
        cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipo.FormattingEnabled = true;
        cboTipo.Location = new Point(12, 30);
        cboTipo.Name = "cboTipo";
        cboTipo.Size = new Size(140, 23);
        cboTipo.TabIndex = 1;
        cboTipo.SelectedIndexChanged += cboTipo_SelectedIndexChanged;
        //
        // lblPlaca
        //
        lblPlaca.AutoSize = true;
        lblPlaca.Location = new Point(164, 12);
        lblPlaca.Name = "lblPlaca";
        lblPlaca.Size = new Size(43, 15);
        lblPlaca.TabIndex = 2;
        lblPlaca.Text = "Placa *";
        //
        // txtPlaca
        //
        txtPlaca.CharacterCasing = CharacterCasing.Upper;
        txtPlaca.Location = new Point(164, 30);
        txtPlaca.MaxLength = 7;
        txtPlaca.Name = "txtPlaca";
        txtPlaca.Size = new Size(100, 23);
        txtPlaca.TabIndex = 3;
        //
        // lblRenavam
        //
        lblRenavam.AutoSize = true;
        lblRenavam.Location = new Point(276, 12);
        lblRenavam.Name = "lblRenavam";
        lblRenavam.Size = new Size(59, 15);
        lblRenavam.TabIndex = 4;
        lblRenavam.Text = "RENAVAM";
        //
        // txtRenavam
        //
        txtRenavam.Location = new Point(276, 30);
        txtRenavam.MaxLength = 11;
        txtRenavam.Name = "txtRenavam";
        txtRenavam.Size = new Size(130, 23);
        txtRenavam.TabIndex = 5;
        //
        // lblCodigoInterno
        //
        lblCodigoInterno.AutoSize = true;
        lblCodigoInterno.Location = new Point(418, 12);
        lblCodigoInterno.Name = "lblCodigoInterno";
        lblCodigoInterno.Size = new Size(88, 15);
        lblCodigoInterno.TabIndex = 6;
        lblCodigoInterno.Text = "Código interno";
        //
        // txtCodigoInterno
        //
        txtCodigoInterno.Location = new Point(418, 30);
        txtCodigoInterno.MaxLength = 10;
        txtCodigoInterno.Name = "txtCodigoInterno";
        txtCodigoInterno.Size = new Size(100, 23);
        txtCodigoInterno.TabIndex = 7;
        //
        // lblUf
        //
        lblUf.AutoSize = true;
        lblUf.Location = new Point(530, 12);
        lblUf.Name = "lblUf";
        lblUf.Size = new Size(21, 15);
        lblUf.TabIndex = 8;
        lblUf.Text = "UF";
        //
        // cboUf
        //
        cboUf.DropDownStyle = ComboBoxStyle.DropDownList;
        cboUf.FormattingEnabled = true;
        cboUf.Location = new Point(530, 30);
        cboUf.Name = "cboUf";
        cboUf.Size = new Size(98, 23);
        cboUf.TabIndex = 9;
        //
        // lblTara
        //
        lblTara.AutoSize = true;
        lblTara.Location = new Point(12, 62);
        lblTara.Name = "lblTara";
        lblTara.Size = new Size(64, 15);
        lblTara.TabIndex = 10;
        lblTara.Text = "Tara (kg) *";
        //
        // nudTara
        //
        nudTara.Location = new Point(12, 80);
        nudTara.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
        nudTara.Name = "nudTara";
        nudTara.Size = new Size(120, 23);
        nudTara.TabIndex = 11;
        nudTara.ThousandsSeparator = true;
        //
        // lblCapacidadeKg
        //
        lblCapacidadeKg.AutoSize = true;
        lblCapacidadeKg.Location = new Point(144, 62);
        lblCapacidadeKg.Name = "lblCapacidadeKg";
        lblCapacidadeKg.Size = new Size(94, 15);
        lblCapacidadeKg.TabIndex = 12;
        lblCapacidadeKg.Text = "Capacidade (kg)";
        //
        // nudCapacidadeKg
        //
        nudCapacidadeKg.Location = new Point(144, 80);
        nudCapacidadeKg.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
        nudCapacidadeKg.Name = "nudCapacidadeKg";
        nudCapacidadeKg.Size = new Size(120, 23);
        nudCapacidadeKg.TabIndex = 13;
        nudCapacidadeKg.ThousandsSeparator = true;
        //
        // lblCapacidadeM3
        //
        lblCapacidadeM3.AutoSize = true;
        lblCapacidadeM3.Location = new Point(276, 62);
        lblCapacidadeM3.Name = "lblCapacidadeM3";
        lblCapacidadeM3.Size = new Size(97, 15);
        lblCapacidadeM3.TabIndex = 14;
        lblCapacidadeM3.Text = "Capacidade (m³)";
        //
        // nudCapacidadeM3
        //
        nudCapacidadeM3.Location = new Point(276, 80);
        nudCapacidadeM3.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
        nudCapacidadeM3.Name = "nudCapacidadeM3";
        nudCapacidadeM3.Size = new Size(120, 23);
        nudCapacidadeM3.TabIndex = 15;
        //
        // lblTipoRodado
        //
        lblTipoRodado.AutoSize = true;
        lblTipoRodado.Location = new Point(12, 112);
        lblTipoRodado.Name = "lblTipoRodado";
        lblTipoRodado.Size = new Size(152, 15);
        lblTipoRodado.TabIndex = 16;
        lblTipoRodado.Text = "Tipo de rodado (tração) *";
        //
        // cboTipoRodado
        //
        cboTipoRodado.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipoRodado.FormattingEnabled = true;
        cboTipoRodado.Location = new Point(12, 130);
        cboTipoRodado.Name = "cboTipoRodado";
        cboTipoRodado.Size = new Size(300, 23);
        cboTipoRodado.TabIndex = 17;
        //
        // lblTipoCarroceria
        //
        lblTipoCarroceria.AutoSize = true;
        lblTipoCarroceria.Location = new Point(324, 112);
        lblTipoCarroceria.Name = "lblTipoCarroceria";
        lblTipoCarroceria.Size = new Size(118, 15);
        lblTipoCarroceria.TabIndex = 18;
        lblTipoCarroceria.Text = "Tipo de carroceria *";
        //
        // cboTipoCarroceria
        //
        cboTipoCarroceria.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipoCarroceria.FormattingEnabled = true;
        cboTipoCarroceria.Location = new Point(324, 130);
        cboTipoCarroceria.Name = "cboTipoCarroceria";
        cboTipoCarroceria.Size = new Size(304, 23);
        cboTipoCarroceria.TabIndex = 19;
        //
        // chkProprio
        //
        chkProprio.AutoSize = true;
        chkProprio.Location = new Point(12, 182);
        chkProprio.Name = "chkProprio";
        chkProprio.Size = new Size(108, 19);
        chkProprio.TabIndex = 20;
        chkProprio.Text = "Veículo &próprio";
        chkProprio.UseVisualStyleBackColor = true;
        chkProprio.CheckedChanged += chkProprio_CheckedChanged;
        //
        // lblProprietario
        //
        lblProprietario.AutoSize = true;
        lblProprietario.Location = new Point(164, 162);
        lblProprietario.Name = "lblProprietario";
        lblProprietario.Size = new Size(186, 15);
        lblProprietario.TabIndex = 21;
        lblProprietario.Text = "Proprietário (veículo de terceiro) *";
        //
        // cboProprietario
        //
        cboProprietario.DropDownStyle = ComboBoxStyle.DropDownList;
        cboProprietario.FormattingEnabled = true;
        cboProprietario.Location = new Point(164, 180);
        cboProprietario.Name = "cboProprietario";
        cboProprietario.Size = new Size(464, 23);
        cboProprietario.TabIndex = 22;
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 222);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(640, 48);
        pnlBotoes.TabIndex = 23;
        //
        // btnSalvar
        //
        btnSalvar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSalvar.Location = new Point(446, 12);
        btnSalvar.Name = "btnSalvar";
        btnSalvar.Size = new Size(88, 27);
        btnSalvar.TabIndex = 0;
        btnSalvar.Text = "&Salvar";
        btnSalvar.UseVisualStyleBackColor = true;
        btnSalvar.Click += btnSalvar_Click;
        //
        // btnCancelar
        //
        btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(540, 12);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(88, 27);
        btnCancelar.TabIndex = 1;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormEdicaoVeiculo
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(640, 270);
        Controls.Add(lblTipo);
        Controls.Add(cboTipo);
        Controls.Add(lblPlaca);
        Controls.Add(txtPlaca);
        Controls.Add(lblRenavam);
        Controls.Add(txtRenavam);
        Controls.Add(lblCodigoInterno);
        Controls.Add(txtCodigoInterno);
        Controls.Add(lblUf);
        Controls.Add(cboUf);
        Controls.Add(lblTara);
        Controls.Add(nudTara);
        Controls.Add(lblCapacidadeKg);
        Controls.Add(nudCapacidadeKg);
        Controls.Add(lblCapacidadeM3);
        Controls.Add(nudCapacidadeM3);
        Controls.Add(lblTipoRodado);
        Controls.Add(cboTipoRodado);
        Controls.Add(lblTipoCarroceria);
        Controls.Add(cboTipoCarroceria);
        Controls.Add(chkProprio);
        Controls.Add(lblProprietario);
        Controls.Add(cboProprietario);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoVeiculo";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Veículo";
        ((System.ComponentModel.ISupportInitialize)nudTara).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudCapacidadeKg).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudCapacidadeM3).EndInit();
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTipo;
    private ComboBox cboTipo;
    private Label lblPlaca;
    private TextBox txtPlaca;
    private Label lblRenavam;
    private TextBox txtRenavam;
    private Label lblCodigoInterno;
    private TextBox txtCodigoInterno;
    private Label lblUf;
    private ComboBox cboUf;
    private Label lblTara;
    private NumericUpDown nudTara;
    private Label lblCapacidadeKg;
    private NumericUpDown nudCapacidadeKg;
    private Label lblCapacidadeM3;
    private NumericUpDown nudCapacidadeM3;
    private Label lblTipoRodado;
    private ComboBox cboTipoRodado;
    private Label lblTipoCarroceria;
    private ComboBox cboTipoCarroceria;
    private CheckBox chkProprio;
    private Label lblProprietario;
    private ComboBox cboProprietario;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
