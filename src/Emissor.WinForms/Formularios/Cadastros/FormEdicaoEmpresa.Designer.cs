namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoEmpresa
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
        lblRazaoSocial = new Label();
        txtRazaoSocial = new TextBox();
        lblNomeFantasia = new Label();
        txtNomeFantasia = new TextBox();
        rdoCnpj = new RadioButton();
        rdoCpf = new RadioButton();
        txtCnpj = new TextBox();
        lblIe = new Label();
        txtIe = new TextBox();
        lblIeSt = new Label();
        txtIeSt = new TextBox();
        lblRntrc = new Label();
        txtRntrc = new TextBox();
        lblCrt = new Label();
        cboCrt = new ComboBox();
        lblRegimeIbsCbs = new Label();
        cboRegimeIbsCbs = new ComboBox();
        lblCstIbsCbs = new Label();
        txtCstIbsCbs = new TextBox();
        lblClassTrib = new Label();
        txtClassTrib = new TextBox();
        lblAliquotaIbs = new Label();
        txtAliquotaIbs = new TextBox();
        lblAliquotaCbs = new Label();
        txtAliquotaCbs = new TextBox();
        ucEndereco = new UcEndereco();
        lblFone = new Label();
        txtFone = new TextBox();
        lblEmail = new Label();
        txtEmail = new TextBox();
        lblAmbiente = new Label();
        cboAmbiente = new ComboBox();
        pnlBotoes = new Panel();
        btnSalvar = new Button();
        btnCancelar = new Button();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // lblRazaoSocial
        //
        lblRazaoSocial.AutoSize = true;
        lblRazaoSocial.Location = new Point(12, 12);
        lblRazaoSocial.Name = "lblRazaoSocial";
        lblRazaoSocial.Size = new Size(82, 15);
        lblRazaoSocial.TabIndex = 0;
        lblRazaoSocial.Text = "Razão social *";
        //
        // txtRazaoSocial
        //
        txtRazaoSocial.Location = new Point(12, 30);
        txtRazaoSocial.MaxLength = 60;
        txtRazaoSocial.Name = "txtRazaoSocial";
        txtRazaoSocial.Size = new Size(616, 23);
        txtRazaoSocial.TabIndex = 1;
        //
        // lblNomeFantasia
        //
        lblNomeFantasia.AutoSize = true;
        lblNomeFantasia.Location = new Point(12, 62);
        lblNomeFantasia.Name = "lblNomeFantasia";
        lblNomeFantasia.Size = new Size(86, 15);
        lblNomeFantasia.TabIndex = 2;
        lblNomeFantasia.Text = "Nome fantasia";
        //
        // txtNomeFantasia
        //
        txtNomeFantasia.Location = new Point(12, 80);
        txtNomeFantasia.MaxLength = 60;
        txtNomeFantasia.Name = "txtNomeFantasia";
        txtNomeFantasia.Size = new Size(616, 23);
        txtNomeFantasia.TabIndex = 3;
        //
        // rdoCnpj
        //
        rdoCnpj.AutoSize = true;
        rdoCnpj.Checked = true;
        rdoCnpj.Location = new Point(12, 110);
        rdoCnpj.Name = "rdoCnpj";
        rdoCnpj.Size = new Size(62, 19);
        rdoCnpj.TabIndex = 4;
        rdoCnpj.TabStop = true;
        rdoCnpj.Text = "CNPJ *";
        rdoCnpj.UseVisualStyleBackColor = true;
        rdoCnpj.CheckedChanged += rdoDocumento_CheckedChanged;
        //
        // rdoCpf
        //
        rdoCpf.AutoSize = true;
        rdoCpf.Location = new Point(84, 110);
        rdoCpf.Name = "rdoCpf";
        rdoCpf.Size = new Size(54, 19);
        rdoCpf.TabIndex = 4;
        rdoCpf.Text = "CPF *";
        rdoCpf.UseVisualStyleBackColor = true;
        rdoCpf.CheckedChanged += rdoDocumento_CheckedChanged;
        //
        // txtCnpj
        //
        txtCnpj.Location = new Point(12, 130);
        txtCnpj.MaxLength = 18;
        txtCnpj.Name = "txtCnpj";
        txtCnpj.Size = new Size(160, 23);
        txtCnpj.TabIndex = 5;
        //
        // lblIe
        //
        lblIe.AutoSize = true;
        lblIe.Location = new Point(184, 112);
        lblIe.Name = "lblIe";
        lblIe.Size = new Size(108, 15);
        lblIe.TabIndex = 6;
        lblIe.Text = "Inscrição Estadual";
        //
        // txtIe
        //
        txtIe.Location = new Point(184, 130);
        txtIe.MaxLength = 14;
        txtIe.Name = "txtIe";
        txtIe.Size = new Size(140, 23);
        txtIe.TabIndex = 7;
        //
        // lblIeSt
        //
        lblIeSt.AutoSize = true;
        lblIeSt.Location = new Point(336, 112);
        lblIeSt.Name = "lblIeSt";
        lblIeSt.Size = new Size(36, 15);
        lblIeSt.TabIndex = 8;
        lblIeSt.Text = "IE ST";
        //
        // txtIeSt
        //
        txtIeSt.Location = new Point(336, 130);
        txtIeSt.MaxLength = 14;
        txtIeSt.Name = "txtIeSt";
        txtIeSt.Size = new Size(140, 23);
        txtIeSt.TabIndex = 9;
        //
        // lblRntrc
        //
        lblRntrc.AutoSize = true;
        lblRntrc.Location = new Point(488, 112);
        lblRntrc.Name = "lblRntrc";
        lblRntrc.Size = new Size(50, 15);
        lblRntrc.TabIndex = 10;
        lblRntrc.Text = "RNTRC *";
        //
        // txtRntrc
        //
        txtRntrc.Location = new Point(488, 130);
        txtRntrc.MaxLength = 8;
        txtRntrc.Name = "txtRntrc";
        txtRntrc.Size = new Size(140, 23);
        txtRntrc.TabIndex = 11;
        //
        // lblCrt
        //
        lblCrt.AutoSize = true;
        lblCrt.Location = new Point(12, 162);
        lblCrt.Name = "lblCrt";
        lblCrt.Size = new Size(163, 15);
        lblCrt.TabIndex = 12;
        lblCrt.Text = "Regime tributário (CRT) *";
        //
        // cboCrt
        //
        cboCrt.DropDownStyle = ComboBoxStyle.DropDownList;
        cboCrt.FormattingEnabled = true;
        cboCrt.Location = new Point(12, 180);
        cboCrt.Name = "cboCrt";
        cboCrt.Size = new Size(300, 23);
        cboCrt.TabIndex = 13;
        //
        // lblRegimeIbsCbs
        //
        lblRegimeIbsCbs.AutoSize = true;
        lblRegimeIbsCbs.Location = new Point(324, 162);
        lblRegimeIbsCbs.Name = "lblRegimeIbsCbs";
        lblRegimeIbsCbs.Size = new Size(247, 15);
        lblRegimeIbsCbs.TabIndex = 14;
        lblRegimeIbsCbs.Text = "Regime do IBS/CBS (opcional; Delete limpa)";
        //
        // cboRegimeIbsCbs
        //
        cboRegimeIbsCbs.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRegimeIbsCbs.DropDownWidth = 480;
        cboRegimeIbsCbs.FormattingEnabled = true;
        cboRegimeIbsCbs.Location = new Point(324, 180);
        cboRegimeIbsCbs.Name = "cboRegimeIbsCbs";
        cboRegimeIbsCbs.Size = new Size(304, 23);
        cboRegimeIbsCbs.TabIndex = 15;
        cboRegimeIbsCbs.KeyDown += cboRegimeIbsCbs_KeyDown;
        //
        // lblCstIbsCbs
        //
        lblCstIbsCbs.AutoSize = true;
        lblCstIbsCbs.Location = new Point(12, 212);
        lblCstIbsCbs.Name = "lblCstIbsCbs";
        lblCstIbsCbs.Size = new Size(70, 15);
        lblCstIbsCbs.TabIndex = 16;
        lblCstIbsCbs.Text = "CST IBS/CBS";
        //
        // txtCstIbsCbs
        //
        txtCstIbsCbs.Location = new Point(12, 230);
        txtCstIbsCbs.MaxLength = 3;
        txtCstIbsCbs.Name = "txtCstIbsCbs";
        txtCstIbsCbs.Size = new Size(70, 23);
        txtCstIbsCbs.TabIndex = 17;
        //
        // lblClassTrib
        //
        lblClassTrib.AutoSize = true;
        lblClassTrib.Location = new Point(94, 212);
        lblClassTrib.Name = "lblClassTrib";
        lblClassTrib.Size = new Size(64, 15);
        lblClassTrib.TabIndex = 18;
        lblClassTrib.Text = "cClassTrib";
        //
        // txtClassTrib
        //
        txtClassTrib.Location = new Point(94, 230);
        txtClassTrib.MaxLength = 6;
        txtClassTrib.Name = "txtClassTrib";
        txtClassTrib.Size = new Size(90, 23);
        txtClassTrib.TabIndex = 19;
        //
        // lblAliquotaIbs
        //
        lblAliquotaIbs.AutoSize = true;
        lblAliquotaIbs.Location = new Point(196, 212);
        lblAliquotaIbs.Name = "lblAliquotaIbs";
        lblAliquotaIbs.Size = new Size(166, 15);
        lblAliquotaIbs.TabIndex = 20;
        lblAliquotaIbs.Text = "Alíq. efetiva IBS no SN (%)";
        //
        // txtAliquotaIbs
        //
        txtAliquotaIbs.Location = new Point(196, 230);
        txtAliquotaIbs.MaxLength = 9;
        txtAliquotaIbs.Name = "txtAliquotaIbs";
        txtAliquotaIbs.Size = new Size(150, 23);
        txtAliquotaIbs.TabIndex = 21;
        //
        // lblAliquotaCbs
        //
        lblAliquotaCbs.AutoSize = true;
        lblAliquotaCbs.Location = new Point(358, 212);
        lblAliquotaCbs.Name = "lblAliquotaCbs";
        lblAliquotaCbs.Size = new Size(166, 15);
        lblAliquotaCbs.TabIndex = 22;
        lblAliquotaCbs.Text = "Alíq. efetiva CBS no SN (%)";
        //
        // txtAliquotaCbs
        //
        txtAliquotaCbs.Location = new Point(358, 230);
        txtAliquotaCbs.MaxLength = 9;
        txtAliquotaCbs.Name = "txtAliquotaCbs";
        txtAliquotaCbs.Size = new Size(150, 23);
        txtAliquotaCbs.TabIndex = 23;
        //
        // ucEndereco
        //
        ucEndereco.Location = new Point(12, 262);
        ucEndereco.Name = "ucEndereco";
        ucEndereco.Size = new Size(616, 94);
        ucEndereco.TabIndex = 24;
        //
        // lblFone
        //
        lblFone.AutoSize = true;
        lblFone.Location = new Point(12, 362);
        lblFone.Name = "lblFone";
        lblFone.Size = new Size(51, 15);
        lblFone.TabIndex = 25;
        lblFone.Text = "Telefone";
        //
        // txtFone
        //
        txtFone.Location = new Point(12, 380);
        txtFone.MaxLength = 14;
        txtFone.Name = "txtFone";
        txtFone.Size = new Size(160, 23);
        txtFone.TabIndex = 26;
        //
        // lblEmail
        //
        lblEmail.AutoSize = true;
        lblEmail.Location = new Point(184, 362);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(41, 15);
        lblEmail.TabIndex = 27;
        lblEmail.Text = "E-mail";
        //
        // txtEmail
        //
        txtEmail.Location = new Point(184, 380);
        txtEmail.MaxLength = 60;
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(260, 23);
        txtEmail.TabIndex = 28;
        //
        // lblAmbiente
        //
        lblAmbiente.AutoSize = true;
        lblAmbiente.Location = new Point(456, 362);
        lblAmbiente.Name = "lblAmbiente";
        lblAmbiente.Size = new Size(108, 15);
        lblAmbiente.TabIndex = 29;
        lblAmbiente.Text = "Ambiente padrão *";
        //
        // cboAmbiente
        //
        cboAmbiente.DropDownStyle = ComboBoxStyle.DropDownList;
        cboAmbiente.FormattingEnabled = true;
        cboAmbiente.Location = new Point(456, 380);
        cboAmbiente.Name = "cboAmbiente";
        cboAmbiente.Size = new Size(172, 23);
        cboAmbiente.TabIndex = 30;
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 418);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(640, 48);
        pnlBotoes.TabIndex = 31;
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
        // FormEdicaoEmpresa
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(640, 466);
        Controls.Add(lblRazaoSocial);
        Controls.Add(txtRazaoSocial);
        Controls.Add(lblNomeFantasia);
        Controls.Add(txtNomeFantasia);
        Controls.Add(rdoCnpj);
        Controls.Add(rdoCpf);
        Controls.Add(txtCnpj);
        Controls.Add(lblIe);
        Controls.Add(txtIe);
        Controls.Add(lblIeSt);
        Controls.Add(txtIeSt);
        Controls.Add(lblRntrc);
        Controls.Add(txtRntrc);
        Controls.Add(lblCrt);
        Controls.Add(cboCrt);
        Controls.Add(lblRegimeIbsCbs);
        Controls.Add(cboRegimeIbsCbs);
        Controls.Add(lblCstIbsCbs);
        Controls.Add(txtCstIbsCbs);
        Controls.Add(lblClassTrib);
        Controls.Add(txtClassTrib);
        Controls.Add(lblAliquotaIbs);
        Controls.Add(txtAliquotaIbs);
        Controls.Add(lblAliquotaCbs);
        Controls.Add(txtAliquotaCbs);
        Controls.Add(ucEndereco);
        Controls.Add(lblFone);
        Controls.Add(txtFone);
        Controls.Add(lblEmail);
        Controls.Add(txtEmail);
        Controls.Add(lblAmbiente);
        Controls.Add(cboAmbiente);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoEmpresa";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Empresa";
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblRazaoSocial;
    private TextBox txtRazaoSocial;
    private Label lblNomeFantasia;
    private TextBox txtNomeFantasia;
    private RadioButton rdoCnpj;
    private RadioButton rdoCpf;
    private TextBox txtCnpj;
    private Label lblIe;
    private TextBox txtIe;
    private Label lblIeSt;
    private TextBox txtIeSt;
    private Label lblRntrc;
    private TextBox txtRntrc;
    private Label lblCrt;
    private ComboBox cboCrt;
    private Label lblRegimeIbsCbs;
    private ComboBox cboRegimeIbsCbs;
    private Label lblCstIbsCbs;
    private TextBox txtCstIbsCbs;
    private Label lblClassTrib;
    private TextBox txtClassTrib;
    private Label lblAliquotaIbs;
    private TextBox txtAliquotaIbs;
    private Label lblAliquotaCbs;
    private TextBox txtAliquotaCbs;
    private UcEndereco ucEndereco;
    private Label lblFone;
    private TextBox txtFone;
    private Label lblEmail;
    private TextBox txtEmail;
    private Label lblAmbiente;
    private ComboBox cboAmbiente;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
