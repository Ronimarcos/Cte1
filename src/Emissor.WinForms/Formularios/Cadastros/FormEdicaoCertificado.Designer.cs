namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoCertificado
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
        lblArquivo = new Label();
        txtArquivo = new TextBox();
        btnProcurarArquivo = new Button();
        lblSenha = new Label();
        txtSenha = new TextBox();
        lblThumbprint = new Label();
        txtThumbprint = new TextBox();
        lblTitular = new Label();
        txtTitular = new TextBox();
        lblCnpjTitular = new Label();
        txtCnpjTitular = new TextBox();
        lblValidoDe = new Label();
        dtpValidoDe = new DateTimePicker();
        lblValidoAte = new Label();
        dtpValidoAte = new DateTimePicker();
        lblAvisoPrototipo = new Label();
        pnlBotoes = new Panel();
        btnSalvar = new Button();
        btnCancelar = new Button();
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
        cboTipo.Size = new Size(200, 23);
        cboTipo.TabIndex = 1;
        cboTipo.SelectedIndexChanged += cboTipo_SelectedIndexChanged;
        //
        // lblArquivo
        //
        lblArquivo.AutoSize = true;
        lblArquivo.Location = new Point(12, 62);
        lblArquivo.Name = "lblArquivo";
        lblArquivo.Size = new Size(171, 15);
        lblArquivo.TabIndex = 2;
        lblArquivo.Text = "Arquivo do certificado (.pfx) *";
        //
        // txtArquivo
        //
        txtArquivo.Location = new Point(12, 80);
        txtArquivo.Name = "txtArquivo";
        txtArquivo.ReadOnly = true;
        txtArquivo.Size = new Size(402, 23);
        txtArquivo.TabIndex = 3;
        //
        // btnProcurarArquivo
        //
        btnProcurarArquivo.Location = new Point(420, 79);
        btnProcurarArquivo.Name = "btnProcurarArquivo";
        btnProcurarArquivo.Size = new Size(88, 25);
        btnProcurarArquivo.TabIndex = 4;
        btnProcurarArquivo.Text = "&Procurar...";
        btnProcurarArquivo.UseVisualStyleBackColor = true;
        btnProcurarArquivo.Click += btnProcurarArquivo_Click;
        //
        // lblSenha
        //
        lblSenha.AutoSize = true;
        lblSenha.Location = new Point(12, 112);
        lblSenha.Name = "lblSenha";
        lblSenha.Size = new Size(126, 15);
        lblSenha.TabIndex = 5;
        lblSenha.Text = "Senha do certificado *";
        //
        // txtSenha
        //
        txtSenha.Location = new Point(12, 130);
        txtSenha.Name = "txtSenha";
        txtSenha.Size = new Size(200, 23);
        txtSenha.TabIndex = 6;
        txtSenha.UseSystemPasswordChar = true;
        //
        // lblThumbprint
        //
        lblThumbprint.AutoSize = true;
        lblThumbprint.Location = new Point(224, 112);
        lblThumbprint.Name = "lblThumbprint";
        lblThumbprint.Size = new Size(76, 15);
        lblThumbprint.TabIndex = 7;
        lblThumbprint.Text = "Thumbprint *";
        //
        // txtThumbprint
        //
        txtThumbprint.Location = new Point(224, 130);
        txtThumbprint.MaxLength = 59;
        txtThumbprint.Name = "txtThumbprint";
        txtThumbprint.Size = new Size(284, 23);
        txtThumbprint.TabIndex = 8;
        //
        // lblTitular
        //
        lblTitular.AutoSize = true;
        lblTitular.Location = new Point(12, 162);
        lblTitular.Name = "lblTitular";
        lblTitular.Size = new Size(50, 15);
        lblTitular.TabIndex = 9;
        lblTitular.Text = "Titular *";
        //
        // txtTitular
        //
        txtTitular.Location = new Point(12, 180);
        txtTitular.MaxLength = 100;
        txtTitular.Name = "txtTitular";
        txtTitular.Size = new Size(340, 23);
        txtTitular.TabIndex = 10;
        //
        // lblCnpjTitular
        //
        lblCnpjTitular.AutoSize = true;
        lblCnpjTitular.Location = new Point(364, 162);
        lblCnpjTitular.Name = "lblCnpjTitular";
        lblCnpjTitular.Size = new Size(103, 15);
        lblCnpjTitular.TabIndex = 11;
        lblCnpjTitular.Text = "CNPJ do titular *";
        //
        // txtCnpjTitular
        //
        txtCnpjTitular.Location = new Point(364, 180);
        txtCnpjTitular.MaxLength = 18;
        txtCnpjTitular.Name = "txtCnpjTitular";
        txtCnpjTitular.Size = new Size(144, 23);
        txtCnpjTitular.TabIndex = 12;
        //
        // lblValidoDe
        //
        lblValidoDe.AutoSize = true;
        lblValidoDe.Location = new Point(12, 212);
        lblValidoDe.Name = "lblValidoDe";
        lblValidoDe.Size = new Size(65, 15);
        lblValidoDe.TabIndex = 13;
        lblValidoDe.Text = "Válido de *";
        //
        // dtpValidoDe
        //
        dtpValidoDe.Format = DateTimePickerFormat.Short;
        dtpValidoDe.Location = new Point(12, 230);
        dtpValidoDe.Name = "dtpValidoDe";
        dtpValidoDe.Size = new Size(120, 23);
        dtpValidoDe.TabIndex = 14;
        //
        // lblValidoAte
        //
        lblValidoAte.AutoSize = true;
        lblValidoAte.Location = new Point(144, 212);
        lblValidoAte.Name = "lblValidoAte";
        lblValidoAte.Size = new Size(67, 15);
        lblValidoAte.TabIndex = 15;
        lblValidoAte.Text = "Válido até *";
        //
        // dtpValidoAte
        //
        dtpValidoAte.Format = DateTimePickerFormat.Short;
        dtpValidoAte.Location = new Point(144, 230);
        dtpValidoAte.Name = "dtpValidoAte";
        dtpValidoAte.Size = new Size(120, 23);
        dtpValidoAte.TabIndex = 16;
        //
        // lblAvisoPrototipo
        //
        lblAvisoPrototipo.ForeColor = SystemColors.GrayText;
        lblAvisoPrototipo.Location = new Point(12, 264);
        lblAvisoPrototipo.Name = "lblAvisoPrototipo";
        lblAvisoPrototipo.Size = new Size(496, 34);
        lblAvisoPrototipo.TabIndex = 17;
        lblAvisoPrototipo.Text = "Protótipo: o arquivo e a senha não são lidos nem guardados. Na versão final, titular, CNPJ e validade serão lidos do próprio certificado.";
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 306);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(520, 48);
        pnlBotoes.TabIndex = 18;
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
        // FormEdicaoCertificado
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(520, 354);
        Controls.Add(lblTipo);
        Controls.Add(cboTipo);
        Controls.Add(lblArquivo);
        Controls.Add(txtArquivo);
        Controls.Add(btnProcurarArquivo);
        Controls.Add(lblSenha);
        Controls.Add(txtSenha);
        Controls.Add(lblThumbprint);
        Controls.Add(txtThumbprint);
        Controls.Add(lblTitular);
        Controls.Add(txtTitular);
        Controls.Add(lblCnpjTitular);
        Controls.Add(txtCnpjTitular);
        Controls.Add(lblValidoDe);
        Controls.Add(dtpValidoDe);
        Controls.Add(lblValidoAte);
        Controls.Add(dtpValidoAte);
        Controls.Add(lblAvisoPrototipo);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoCertificado";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Certificado digital";
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTipo;
    private ComboBox cboTipo;
    private Label lblArquivo;
    private TextBox txtArquivo;
    private Button btnProcurarArquivo;
    private Label lblSenha;
    private TextBox txtSenha;
    private Label lblThumbprint;
    private TextBox txtThumbprint;
    private Label lblTitular;
    private TextBox txtTitular;
    private Label lblCnpjTitular;
    private TextBox txtCnpjTitular;
    private Label lblValidoDe;
    private DateTimePicker dtpValidoDe;
    private Label lblValidoAte;
    private DateTimePicker dtpValidoAte;
    private Label lblAvisoPrototipo;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
