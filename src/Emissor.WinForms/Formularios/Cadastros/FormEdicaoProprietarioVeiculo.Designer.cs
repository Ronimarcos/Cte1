namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoProprietarioVeiculo
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
        lblRntrc = new Label();
        txtRntrc = new TextBox();
        lblTipoProprietario = new Label();
        cboTipoProprietario = new ComboBox();
        lblNome = new Label();
        txtNome = new TextBox();
        lblIe = new Label();
        txtIe = new TextBox();
        lblUf = new Label();
        cboUf = new ComboBox();
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
        txtCnpjCpf.Size = new Size(180, 23);
        txtCnpjCpf.TabIndex = 1;
        //
        // lblRntrc
        //
        lblRntrc.AutoSize = true;
        lblRntrc.Location = new Point(204, 12);
        lblRntrc.Name = "lblRntrc";
        lblRntrc.Size = new Size(50, 15);
        lblRntrc.TabIndex = 2;
        lblRntrc.Text = "RNTRC *";
        //
        // txtRntrc
        //
        txtRntrc.Location = new Point(204, 30);
        txtRntrc.MaxLength = 8;
        txtRntrc.Name = "txtRntrc";
        txtRntrc.Size = new Size(100, 23);
        txtRntrc.TabIndex = 3;
        //
        // lblTipoProprietario
        //
        lblTipoProprietario.AutoSize = true;
        lblTipoProprietario.Location = new Point(316, 12);
        lblTipoProprietario.Name = "lblTipoProprietario";
        lblTipoProprietario.Size = new Size(39, 15);
        lblTipoProprietario.TabIndex = 4;
        lblTipoProprietario.Text = "Tipo *";
        //
        // cboTipoProprietario
        //
        cboTipoProprietario.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipoProprietario.FormattingEnabled = true;
        cboTipoProprietario.Location = new Point(316, 30);
        cboTipoProprietario.Name = "cboTipoProprietario";
        cboTipoProprietario.Size = new Size(192, 23);
        cboTipoProprietario.TabIndex = 5;
        //
        // lblNome
        //
        lblNome.AutoSize = true;
        lblNome.Location = new Point(12, 62);
        lblNome.Name = "lblNome";
        lblNome.Size = new Size(129, 15);
        lblNome.TabIndex = 6;
        lblNome.Text = "Nome ou razão social *";
        //
        // txtNome
        //
        txtNome.Location = new Point(12, 80);
        txtNome.MaxLength = 60;
        txtNome.Name = "txtNome";
        txtNome.Size = new Size(300, 23);
        txtNome.TabIndex = 7;
        //
        // lblIe
        //
        lblIe.AutoSize = true;
        lblIe.Location = new Point(324, 62);
        lblIe.Name = "lblIe";
        lblIe.Size = new Size(17, 15);
        lblIe.TabIndex = 8;
        lblIe.Text = "IE";
        //
        // txtIe
        //
        txtIe.CharacterCasing = CharacterCasing.Upper;
        txtIe.Location = new Point(324, 80);
        txtIe.MaxLength = 14;
        txtIe.Name = "txtIe";
        txtIe.PlaceholderText = "ou ISENTO";
        txtIe.Size = new Size(110, 23);
        txtIe.TabIndex = 9;
        //
        // lblUf
        //
        lblUf.AutoSize = true;
        lblUf.Location = new Point(446, 62);
        lblUf.Name = "lblUf";
        lblUf.Size = new Size(30, 15);
        lblUf.TabIndex = 10;
        lblUf.Text = "UF *";
        //
        // cboUf
        //
        cboUf.DropDownStyle = ComboBoxStyle.DropDownList;
        cboUf.FormattingEnabled = true;
        cboUf.Location = new Point(446, 80);
        cboUf.Name = "cboUf";
        cboUf.Size = new Size(62, 23);
        cboUf.TabIndex = 11;
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 122);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(520, 48);
        pnlBotoes.TabIndex = 12;
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
        // FormEdicaoProprietarioVeiculo
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(520, 170);
        Controls.Add(lblCnpjCpf);
        Controls.Add(txtCnpjCpf);
        Controls.Add(lblRntrc);
        Controls.Add(txtRntrc);
        Controls.Add(lblTipoProprietario);
        Controls.Add(cboTipoProprietario);
        Controls.Add(lblNome);
        Controls.Add(txtNome);
        Controls.Add(lblIe);
        Controls.Add(txtIe);
        Controls.Add(lblUf);
        Controls.Add(cboUf);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoProprietarioVeiculo";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Proprietário de veículo";
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblCnpjCpf;
    private TextBox txtCnpjCpf;
    private Label lblRntrc;
    private TextBox txtRntrc;
    private Label lblTipoProprietario;
    private ComboBox cboTipoProprietario;
    private Label lblNome;
    private TextBox txtNome;
    private Label lblIe;
    private TextBox txtIe;
    private Label lblUf;
    private ComboBox cboUf;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
