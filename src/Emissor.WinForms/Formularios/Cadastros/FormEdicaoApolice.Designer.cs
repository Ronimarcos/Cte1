namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoApolice
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
        lblResponsavel = new Label();
        cboResponsavel = new ComboBox();
        lblCnpjCpfResponsavel = new Label();
        txtCnpjCpfResponsavel = new TextBox();
        lblNomeSeguradora = new Label();
        txtNomeSeguradora = new TextBox();
        lblCnpjSeguradora = new Label();
        txtCnpjSeguradora = new TextBox();
        lblNumeroApolice = new Label();
        txtNumeroApolice = new TextBox();
        pnlBotoes = new Panel();
        btnSalvar = new Button();
        btnCancelar = new Button();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // lblResponsavel
        //
        lblResponsavel.AutoSize = true;
        lblResponsavel.Location = new Point(12, 12);
        lblResponsavel.Name = "lblResponsavel";
        lblResponsavel.Size = new Size(147, 15);
        lblResponsavel.TabIndex = 0;
        lblResponsavel.Text = "Responsável pelo seguro *";
        //
        // cboResponsavel
        //
        cboResponsavel.DropDownStyle = ComboBoxStyle.DropDownList;
        cboResponsavel.FormattingEnabled = true;
        cboResponsavel.Location = new Point(12, 30);
        cboResponsavel.Name = "cboResponsavel";
        cboResponsavel.Size = new Size(300, 23);
        cboResponsavel.TabIndex = 1;
        cboResponsavel.SelectedIndexChanged += cboResponsavel_SelectedIndexChanged;
        //
        // lblCnpjCpfResponsavel
        //
        lblCnpjCpfResponsavel.AutoSize = true;
        lblCnpjCpfResponsavel.Location = new Point(324, 12);
        lblCnpjCpfResponsavel.Name = "lblCnpjCpfResponsavel";
        lblCnpjCpfResponsavel.Size = new Size(154, 15);
        lblCnpjCpfResponsavel.TabIndex = 2;
        lblCnpjCpfResponsavel.Text = "CNPJ/CPF do contratante *";
        //
        // txtCnpjCpfResponsavel
        //
        txtCnpjCpfResponsavel.Location = new Point(324, 30);
        txtCnpjCpfResponsavel.MaxLength = 18;
        txtCnpjCpfResponsavel.Name = "txtCnpjCpfResponsavel";
        txtCnpjCpfResponsavel.Size = new Size(184, 23);
        txtCnpjCpfResponsavel.TabIndex = 3;
        //
        // lblNomeSeguradora
        //
        lblNomeSeguradora.AutoSize = true;
        lblNomeSeguradora.Location = new Point(12, 62);
        lblNomeSeguradora.Name = "lblNomeSeguradora";
        lblNomeSeguradora.Size = new Size(74, 15);
        lblNomeSeguradora.TabIndex = 4;
        lblNomeSeguradora.Text = "Seguradora *";
        //
        // txtNomeSeguradora
        //
        txtNomeSeguradora.Location = new Point(12, 80);
        txtNomeSeguradora.MaxLength = 30;
        txtNomeSeguradora.Name = "txtNomeSeguradora";
        txtNomeSeguradora.Size = new Size(300, 23);
        txtNomeSeguradora.TabIndex = 5;
        //
        // lblCnpjSeguradora
        //
        lblCnpjSeguradora.AutoSize = true;
        lblCnpjSeguradora.Location = new Point(324, 62);
        lblCnpjSeguradora.Name = "lblCnpjSeguradora";
        lblCnpjSeguradora.Size = new Size(124, 15);
        lblCnpjSeguradora.TabIndex = 6;
        lblCnpjSeguradora.Text = "CNPJ da seguradora *";
        //
        // txtCnpjSeguradora
        //
        txtCnpjSeguradora.Location = new Point(324, 80);
        txtCnpjSeguradora.MaxLength = 18;
        txtCnpjSeguradora.Name = "txtCnpjSeguradora";
        txtCnpjSeguradora.Size = new Size(184, 23);
        txtCnpjSeguradora.TabIndex = 7;
        //
        // lblNumeroApolice
        //
        lblNumeroApolice.AutoSize = true;
        lblNumeroApolice.Location = new Point(12, 112);
        lblNumeroApolice.Name = "lblNumeroApolice";
        lblNumeroApolice.Size = new Size(118, 15);
        lblNumeroApolice.TabIndex = 8;
        lblNumeroApolice.Text = "Número da apólice *";
        //
        // txtNumeroApolice
        //
        txtNumeroApolice.Location = new Point(12, 130);
        txtNumeroApolice.MaxLength = 20;
        txtNumeroApolice.Name = "txtNumeroApolice";
        txtNumeroApolice.Size = new Size(200, 23);
        txtNumeroApolice.TabIndex = 9;
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 172);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(520, 48);
        pnlBotoes.TabIndex = 10;
        //
        // btnSalvar
        //
        btnSalvar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSalvar.Location = new Point(326, 12);
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
        btnCancelar.Location = new Point(420, 12);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(88, 27);
        btnCancelar.TabIndex = 1;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormEdicaoApolice
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(520, 220);
        Controls.Add(lblResponsavel);
        Controls.Add(cboResponsavel);
        Controls.Add(lblCnpjCpfResponsavel);
        Controls.Add(txtCnpjCpfResponsavel);
        Controls.Add(lblNomeSeguradora);
        Controls.Add(txtNomeSeguradora);
        Controls.Add(lblCnpjSeguradora);
        Controls.Add(txtCnpjSeguradora);
        Controls.Add(lblNumeroApolice);
        Controls.Add(txtNumeroApolice);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoApolice";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Apólice de seguro";
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblResponsavel;
    private ComboBox cboResponsavel;
    private Label lblCnpjCpfResponsavel;
    private TextBox txtCnpjCpfResponsavel;
    private Label lblNomeSeguradora;
    private TextBox txtNomeSeguradora;
    private Label lblCnpjSeguradora;
    private TextBox txtCnpjSeguradora;
    private Label lblNumeroApolice;
    private TextBox txtNumeroApolice;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
