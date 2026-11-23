namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoMotorista
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
        lblNome = new Label();
        txtNome = new TextBox();
        lblCpf = new Label();
        txtCpf = new TextBox();
        lblTelefone = new Label();
        txtTelefone = new TextBox();
        pnlBotoes = new Panel();
        btnSalvar = new Button();
        btnCancelar = new Button();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // lblNome
        //
        lblNome.AutoSize = true;
        lblNome.Location = new Point(12, 12);
        lblNome.Name = "lblNome";
        lblNome.Size = new Size(50, 15);
        lblNome.TabIndex = 0;
        lblNome.Text = "Nome *";
        //
        // txtNome
        //
        txtNome.Location = new Point(12, 30);
        txtNome.MaxLength = 60;
        txtNome.Name = "txtNome";
        txtNome.Size = new Size(496, 23);
        txtNome.TabIndex = 1;
        //
        // lblCpf
        //
        lblCpf.AutoSize = true;
        lblCpf.Location = new Point(12, 62);
        lblCpf.Name = "lblCpf";
        lblCpf.Size = new Size(38, 15);
        lblCpf.TabIndex = 2;
        lblCpf.Text = "CPF *";
        //
        // txtCpf
        //
        txtCpf.Location = new Point(12, 80);
        txtCpf.MaxLength = 14;
        txtCpf.Name = "txtCpf";
        txtCpf.Size = new Size(140, 23);
        txtCpf.TabIndex = 3;
        //
        // lblTelefone
        //
        lblTelefone.AutoSize = true;
        lblTelefone.Location = new Point(164, 62);
        lblTelefone.Name = "lblTelefone";
        lblTelefone.Size = new Size(51, 15);
        lblTelefone.TabIndex = 4;
        lblTelefone.Text = "Telefone";
        //
        // txtTelefone
        //
        txtTelefone.Location = new Point(164, 80);
        txtTelefone.MaxLength = 15;
        txtTelefone.Name = "txtTelefone";
        txtTelefone.Size = new Size(140, 23);
        txtTelefone.TabIndex = 5;
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 122);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(520, 48);
        pnlBotoes.TabIndex = 6;
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
        // FormEdicaoMotorista
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(520, 170);
        Controls.Add(lblNome);
        Controls.Add(txtNome);
        Controls.Add(lblCpf);
        Controls.Add(txtCpf);
        Controls.Add(lblTelefone);
        Controls.Add(txtTelefone);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoMotorista";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Motorista";
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblNome;
    private TextBox txtNome;
    private Label lblCpf;
    private TextBox txtCpf;
    private Label lblTelefone;
    private TextBox txtTelefone;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
