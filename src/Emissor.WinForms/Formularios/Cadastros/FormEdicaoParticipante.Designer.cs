namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoParticipante
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
        lblCnpjCpf = new Label();
        txtCnpjCpf = new TextBox();
        lblIndicadorIe = new Label();
        cboIndicadorIe = new ComboBox();
        lblIe = new Label();
        txtIe = new TextBox();
        lblRazaoSocial = new Label();
        txtRazaoSocial = new TextBox();
        lblNomeFantasia = new Label();
        txtNomeFantasia = new TextBox();
        ucEndereco = new UcEndereco();
        lblFone = new Label();
        txtFone = new TextBox();
        lblEmail = new Label();
        txtEmail = new TextBox();
        pnlBotoes = new Panel();
        btnSalvar = new Button();
        btnCancelar = new Button();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // lblCnpjCpf
        //
        lblCnpjCpf.AutoSize = true;
        lblCnpjCpf.Location = new Point(12, 12);
        lblCnpjCpf.Name = "lblCnpjCpf";
        lblCnpjCpf.Size = new Size(68, 15);
        lblCnpjCpf.TabIndex = 0;
        lblCnpjCpf.Text = "CNPJ/CPF *";
        //
        // txtCnpjCpf
        //
        txtCnpjCpf.Location = new Point(12, 30);
        txtCnpjCpf.MaxLength = 18;
        txtCnpjCpf.Name = "txtCnpjCpf";
        txtCnpjCpf.Size = new Size(160, 23);
        txtCnpjCpf.TabIndex = 1;
        //
        // lblIndicadorIe
        //
        lblIndicadorIe.AutoSize = true;
        lblIndicadorIe.Location = new Point(184, 12);
        lblIndicadorIe.Name = "lblIndicadorIe";
        lblIndicadorIe.Size = new Size(98, 15);
        lblIndicadorIe.TabIndex = 2;
        lblIndicadorIe.Text = "Indicador da IE *";
        //
        // cboIndicadorIe
        //
        cboIndicadorIe.DropDownStyle = ComboBoxStyle.DropDownList;
        cboIndicadorIe.FormattingEnabled = true;
        cboIndicadorIe.Location = new Point(184, 30);
        cboIndicadorIe.Name = "cboIndicadorIe";
        cboIndicadorIe.Size = new Size(240, 23);
        cboIndicadorIe.TabIndex = 3;
        //
        // lblIe
        //
        lblIe.AutoSize = true;
        lblIe.Location = new Point(436, 12);
        lblIe.Name = "lblIe";
        lblIe.Size = new Size(108, 15);
        lblIe.TabIndex = 4;
        lblIe.Text = "Inscrição Estadual";
        //
        // txtIe
        //
        txtIe.Location = new Point(436, 30);
        txtIe.MaxLength = 14;
        txtIe.Name = "txtIe";
        txtIe.Size = new Size(192, 23);
        txtIe.TabIndex = 5;
        //
        // lblRazaoSocial
        //
        lblRazaoSocial.AutoSize = true;
        lblRazaoSocial.Location = new Point(12, 62);
        lblRazaoSocial.Name = "lblRazaoSocial";
        lblRazaoSocial.Size = new Size(129, 15);
        lblRazaoSocial.TabIndex = 6;
        lblRazaoSocial.Text = "Razão social ou nome *";
        //
        // txtRazaoSocial
        //
        txtRazaoSocial.Location = new Point(12, 80);
        txtRazaoSocial.MaxLength = 60;
        txtRazaoSocial.Name = "txtRazaoSocial";
        txtRazaoSocial.Size = new Size(616, 23);
        txtRazaoSocial.TabIndex = 7;
        //
        // lblNomeFantasia
        //
        lblNomeFantasia.AutoSize = true;
        lblNomeFantasia.Location = new Point(12, 112);
        lblNomeFantasia.Name = "lblNomeFantasia";
        lblNomeFantasia.Size = new Size(86, 15);
        lblNomeFantasia.TabIndex = 8;
        lblNomeFantasia.Text = "Nome fantasia";
        //
        // txtNomeFantasia
        //
        txtNomeFantasia.Location = new Point(12, 130);
        txtNomeFantasia.MaxLength = 60;
        txtNomeFantasia.Name = "txtNomeFantasia";
        txtNomeFantasia.Size = new Size(616, 23);
        txtNomeFantasia.TabIndex = 9;
        //
        // ucEndereco
        //
        ucEndereco.Location = new Point(12, 162);
        ucEndereco.Name = "ucEndereco";
        ucEndereco.Size = new Size(616, 94);
        ucEndereco.TabIndex = 10;
        //
        // lblFone
        //
        lblFone.AutoSize = true;
        lblFone.Location = new Point(12, 262);
        lblFone.Name = "lblFone";
        lblFone.Size = new Size(51, 15);
        lblFone.TabIndex = 11;
        lblFone.Text = "Telefone";
        //
        // txtFone
        //
        txtFone.Location = new Point(12, 280);
        txtFone.MaxLength = 14;
        txtFone.Name = "txtFone";
        txtFone.Size = new Size(160, 23);
        txtFone.TabIndex = 12;
        //
        // lblEmail
        //
        lblEmail.AutoSize = true;
        lblEmail.Location = new Point(184, 262);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(41, 15);
        lblEmail.TabIndex = 13;
        lblEmail.Text = "E-mail";
        //
        // txtEmail
        //
        txtEmail.Location = new Point(184, 280);
        txtEmail.MaxLength = 60;
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(444, 23);
        txtEmail.TabIndex = 14;
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 320);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(640, 48);
        pnlBotoes.TabIndex = 15;
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
        // FormEdicaoParticipante
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(640, 368);
        Controls.Add(lblCnpjCpf);
        Controls.Add(txtCnpjCpf);
        Controls.Add(lblIndicadorIe);
        Controls.Add(cboIndicadorIe);
        Controls.Add(lblIe);
        Controls.Add(txtIe);
        Controls.Add(lblRazaoSocial);
        Controls.Add(txtRazaoSocial);
        Controls.Add(lblNomeFantasia);
        Controls.Add(txtNomeFantasia);
        Controls.Add(ucEndereco);
        Controls.Add(lblFone);
        Controls.Add(txtFone);
        Controls.Add(lblEmail);
        Controls.Add(txtEmail);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoParticipante";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Participante";
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblCnpjCpf;
    private TextBox txtCnpjCpf;
    private Label lblIndicadorIe;
    private ComboBox cboIndicadorIe;
    private Label lblIe;
    private TextBox txtIe;
    private Label lblRazaoSocial;
    private TextBox txtRazaoSocial;
    private Label lblNomeFantasia;
    private TextBox txtNomeFantasia;
    private UcEndereco ucEndereco;
    private Label lblFone;
    private TextBox txtFone;
    private Label lblEmail;
    private TextBox txtEmail;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
