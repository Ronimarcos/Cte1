namespace Emissor.WinForms.Formularios.Acesso;

partial class FormPrimeiroAcesso
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
        lblInstrucao = new Label();
        lblLogin = new Label();
        txtLogin = new TextBox();
        lblNome = new Label();
        txtNome = new TextBox();
        lblSenha = new Label();
        txtSenha = new TextBox();
        lblConfirmar = new Label();
        txtConfirmar = new TextBox();
        lblRegra = new Label();
        btnCriar = new Button();
        btnCancelar = new Button();
        SuspendLayout();
        //
        // lblInstrucao
        //
        lblInstrucao.Location = new Point(12, 12);
        lblInstrucao.Name = "lblInstrucao";
        lblInstrucao.Size = new Size(356, 36);
        lblInstrucao.TabIndex = 0;
        lblInstrucao.Text = "Primeiro acesso: crie o usuário administrador. Ele vê todas as empresas e cadastra os demais usuários.";
        //
        // lblLogin
        //
        lblLogin.AutoSize = true;
        lblLogin.Location = new Point(12, 56);
        lblLogin.Name = "lblLogin";
        lblLogin.Size = new Size(50, 15);
        lblLogin.TabIndex = 1;
        lblLogin.Text = "&Login *";
        //
        // txtLogin
        //
        txtLogin.Location = new Point(12, 74);
        txtLogin.MaxLength = 30;
        txtLogin.Name = "txtLogin";
        txtLogin.Size = new Size(170, 23);
        txtLogin.TabIndex = 2;
        //
        // lblNome
        //
        lblNome.AutoSize = true;
        lblNome.Location = new Point(194, 56);
        lblNome.Name = "lblNome";
        lblNome.Size = new Size(50, 15);
        lblNome.TabIndex = 3;
        lblNome.Text = "&Nome *";
        //
        // txtNome
        //
        txtNome.Location = new Point(194, 74);
        txtNome.MaxLength = 60;
        txtNome.Name = "txtNome";
        txtNome.Size = new Size(174, 23);
        txtNome.TabIndex = 4;
        //
        // lblSenha
        //
        lblSenha.AutoSize = true;
        lblSenha.Location = new Point(12, 106);
        lblSenha.Name = "lblSenha";
        lblSenha.Size = new Size(50, 15);
        lblSenha.TabIndex = 5;
        lblSenha.Text = "&Senha *";
        //
        // txtSenha
        //
        txtSenha.Location = new Point(12, 124);
        txtSenha.MaxLength = 128;
        txtSenha.Name = "txtSenha";
        txtSenha.Size = new Size(170, 23);
        txtSenha.TabIndex = 6;
        txtSenha.UseSystemPasswordChar = true;
        //
        // lblConfirmar
        //
        lblConfirmar.AutoSize = true;
        lblConfirmar.Location = new Point(194, 106);
        lblConfirmar.Name = "lblConfirmar";
        lblConfirmar.Size = new Size(110, 15);
        lblConfirmar.TabIndex = 7;
        lblConfirmar.Text = "C&onfirmar senha *";
        //
        // txtConfirmar
        //
        txtConfirmar.Location = new Point(194, 124);
        txtConfirmar.MaxLength = 128;
        txtConfirmar.Name = "txtConfirmar";
        txtConfirmar.Size = new Size(174, 23);
        txtConfirmar.TabIndex = 8;
        txtConfirmar.UseSystemPasswordChar = true;
        //
        // lblRegra
        //
        lblRegra.ForeColor = SystemColors.GrayText;
        lblRegra.Location = new Point(12, 156);
        lblRegra.Name = "lblRegra";
        lblRegra.Size = new Size(356, 32);
        lblRegra.TabIndex = 9;
        lblRegra.Text = "A senha precisa ter pelo menos 8 caracteres, com letras e números. O login aceita letras minúsculas, números, ponto, hífen e sublinhado.";
        //
        // btnCriar
        //
        btnCriar.Location = new Point(192, 200);
        btnCriar.Name = "btnCriar";
        btnCriar.Size = new Size(85, 27);
        btnCriar.TabIndex = 10;
        btnCriar.Text = "&Criar";
        btnCriar.UseVisualStyleBackColor = true;
        btnCriar.Click += btnCriar_Click;
        //
        // btnCancelar
        //
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(283, 200);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(85, 27);
        btnCancelar.TabIndex = 11;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormPrimeiroAcesso
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblInstrucao);
        Controls.Add(lblLogin);
        Controls.Add(txtLogin);
        Controls.Add(lblNome);
        Controls.Add(txtNome);
        Controls.Add(lblSenha);
        Controls.Add(txtSenha);
        Controls.Add(lblConfirmar);
        Controls.Add(txtConfirmar);
        Controls.Add(lblRegra);
        Controls.Add(btnCriar);
        Controls.Add(btnCancelar);
        AcceptButton = btnCriar;
        CancelButton = btnCancelar;
        ClientSize = new Size(380, 239);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormPrimeiroAcesso";
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Primeiro acesso - criar administrador";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblInstrucao;
    private Label lblLogin;
    private TextBox txtLogin;
    private Label lblNome;
    private TextBox txtNome;
    private Label lblSenha;
    private TextBox txtSenha;
    private Label lblConfirmar;
    private TextBox txtConfirmar;
    private Label lblRegra;
    private Button btnCriar;
    private Button btnCancelar;
}
