namespace Emissor.WinForms.Formularios.Acesso;

partial class FormLogin
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
        lblTitulo = new Label();
        lblLogin = new Label();
        txtLogin = new TextBox();
        lblSenha = new Label();
        txtSenha = new TextBox();
        lblMensagem = new Label();
        btnEntrar = new Button();
        btnCancelar = new Button();
        SuspendLayout();
        //
        // lblTitulo
        //
        lblTitulo.AutoSize = true;
        lblTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTitulo.Location = new Point(12, 12);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new Size(200, 15);
        lblTitulo.TabIndex = 0;
        lblTitulo.Text = "Emissor CT-e / MDF-e";
        //
        // lblLogin
        //
        lblLogin.AutoSize = true;
        lblLogin.Location = new Point(12, 48);
        lblLogin.Name = "lblLogin";
        lblLogin.Size = new Size(40, 15);
        lblLogin.TabIndex = 1;
        lblLogin.Text = "&Login";
        //
        // txtLogin
        //
        txtLogin.Location = new Point(12, 66);
        txtLogin.MaxLength = 30;
        txtLogin.Name = "txtLogin";
        txtLogin.Size = new Size(336, 23);
        txtLogin.TabIndex = 2;
        //
        // lblSenha
        //
        lblSenha.AutoSize = true;
        lblSenha.Location = new Point(12, 98);
        lblSenha.Name = "lblSenha";
        lblSenha.Size = new Size(40, 15);
        lblSenha.TabIndex = 3;
        lblSenha.Text = "&Senha";
        //
        // txtSenha
        //
        txtSenha.Location = new Point(12, 116);
        txtSenha.MaxLength = 128;
        txtSenha.Name = "txtSenha";
        txtSenha.Size = new Size(336, 23);
        txtSenha.TabIndex = 4;
        txtSenha.UseSystemPasswordChar = true;
        //
        // lblMensagem
        //
        lblMensagem.ForeColor = Color.Firebrick;
        lblMensagem.Location = new Point(12, 146);
        lblMensagem.Name = "lblMensagem";
        lblMensagem.Size = new Size(336, 32);
        lblMensagem.TabIndex = 5;
        lblMensagem.Text = "";
        //
        // btnEntrar
        //
        btnEntrar.Location = new Point(172, 184);
        btnEntrar.Name = "btnEntrar";
        btnEntrar.Size = new Size(85, 27);
        btnEntrar.TabIndex = 6;
        btnEntrar.Text = "&Entrar";
        btnEntrar.UseVisualStyleBackColor = true;
        btnEntrar.Click += btnEntrar_Click;
        //
        // btnCancelar
        //
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(263, 184);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(85, 27);
        btnCancelar.TabIndex = 7;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormLogin
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblTitulo);
        Controls.Add(lblLogin);
        Controls.Add(txtLogin);
        Controls.Add(lblSenha);
        Controls.Add(txtSenha);
        Controls.Add(lblMensagem);
        Controls.Add(btnEntrar);
        Controls.Add(btnCancelar);
        AcceptButton = btnEntrar;
        CancelButton = btnCancelar;
        ClientSize = new Size(360, 223);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormLogin";
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Entrar - Emissor CT-e / MDF-e";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTitulo;
    private Label lblLogin;
    private TextBox txtLogin;
    private Label lblSenha;
    private TextBox txtSenha;
    private Label lblMensagem;
    private Button btnEntrar;
    private Button btnCancelar;
}
