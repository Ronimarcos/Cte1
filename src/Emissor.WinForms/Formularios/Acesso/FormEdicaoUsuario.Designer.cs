namespace Emissor.WinForms.Formularios.Acesso;

partial class FormEdicaoUsuario
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
        lblLogin = new Label();
        txtLogin = new TextBox();
        lblNome = new Label();
        txtNome = new TextBox();
        lblPerfil = new Label();
        cboPerfil = new ComboBox();
        chkAtivo = new CheckBox();
        lblSenha = new Label();
        txtSenha = new TextBox();
        lblConfirmar = new Label();
        txtConfirmar = new TextBox();
        lblRegraSenha = new Label();
        lblEmpresas = new Label();
        clbEmpresas = new CheckedListBox();
        lblSituacao = new Label();
        btnSalvar = new Button();
        btnCancelar = new Button();
        SuspendLayout();
        //
        // lblLogin
        //
        lblLogin.AutoSize = true;
        lblLogin.Location = new Point(12, 12);
        lblLogin.Name = "lblLogin";
        lblLogin.Size = new Size(50, 15);
        lblLogin.TabIndex = 0;
        lblLogin.Text = "&Login *";
        //
        // txtLogin
        //
        txtLogin.Location = new Point(12, 30);
        txtLogin.MaxLength = 30;
        txtLogin.Name = "txtLogin";
        txtLogin.Size = new Size(150, 23);
        txtLogin.TabIndex = 1;
        //
        // lblNome
        //
        lblNome.AutoSize = true;
        lblNome.Location = new Point(174, 12);
        lblNome.Name = "lblNome";
        lblNome.Size = new Size(50, 15);
        lblNome.TabIndex = 2;
        lblNome.Text = "&Nome *";
        //
        // txtNome
        //
        txtNome.Location = new Point(174, 30);
        txtNome.MaxLength = 60;
        txtNome.Name = "txtNome";
        txtNome.Size = new Size(334, 23);
        txtNome.TabIndex = 3;
        //
        // lblPerfil
        //
        lblPerfil.AutoSize = true;
        lblPerfil.Location = new Point(12, 62);
        lblPerfil.Name = "lblPerfil";
        lblPerfil.Size = new Size(50, 15);
        lblPerfil.TabIndex = 4;
        lblPerfil.Text = "&Perfil *";
        //
        // cboPerfil
        //
        cboPerfil.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPerfil.FormattingEnabled = true;
        cboPerfil.Location = new Point(12, 80);
        cboPerfil.Name = "cboPerfil";
        cboPerfil.Size = new Size(150, 23);
        cboPerfil.TabIndex = 5;
        cboPerfil.SelectedIndexChanged += cboPerfil_SelectedIndexChanged;
        //
        // chkAtivo
        //
        chkAtivo.AutoSize = true;
        chkAtivo.Location = new Point(174, 82);
        chkAtivo.Name = "chkAtivo";
        chkAtivo.Size = new Size(54, 19);
        chkAtivo.TabIndex = 6;
        chkAtivo.Text = "Ati&vo";
        chkAtivo.UseVisualStyleBackColor = true;
        //
        // lblSenha
        //
        lblSenha.AutoSize = true;
        lblSenha.Location = new Point(12, 112);
        lblSenha.Name = "lblSenha";
        lblSenha.Size = new Size(110, 15);
        lblSenha.TabIndex = 7;
        lblSenha.Text = "&Senha";
        //
        // txtSenha
        //
        txtSenha.Location = new Point(12, 130);
        txtSenha.MaxLength = 128;
        txtSenha.Name = "txtSenha";
        txtSenha.Size = new Size(240, 23);
        txtSenha.TabIndex = 8;
        txtSenha.UseSystemPasswordChar = true;
        //
        // lblConfirmar
        //
        lblConfirmar.AutoSize = true;
        lblConfirmar.Location = new Point(268, 112);
        lblConfirmar.Name = "lblConfirmar";
        lblConfirmar.Size = new Size(110, 15);
        lblConfirmar.TabIndex = 9;
        lblConfirmar.Text = "C&onfirmar senha";
        //
        // txtConfirmar
        //
        txtConfirmar.Location = new Point(268, 130);
        txtConfirmar.MaxLength = 128;
        txtConfirmar.Name = "txtConfirmar";
        txtConfirmar.Size = new Size(240, 23);
        txtConfirmar.TabIndex = 10;
        txtConfirmar.UseSystemPasswordChar = true;
        //
        // lblRegraSenha
        //
        lblRegraSenha.ForeColor = SystemColors.GrayText;
        lblRegraSenha.Location = new Point(12, 158);
        lblRegraSenha.Name = "lblRegraSenha";
        lblRegraSenha.Size = new Size(496, 18);
        lblRegraSenha.TabIndex = 11;
        lblRegraSenha.Text = "";
        //
        // lblEmpresas
        //
        lblEmpresas.AutoSize = true;
        lblEmpresas.Location = new Point(12, 184);
        lblEmpresas.Name = "lblEmpresas";
        lblEmpresas.Size = new Size(190, 15);
        lblEmpresas.TabIndex = 12;
        lblEmpresas.Text = "&Empresas liberadas (operador) *";
        //
        // clbEmpresas
        //
        clbEmpresas.CheckOnClick = true;
        clbEmpresas.FormattingEnabled = true;
        clbEmpresas.Location = new Point(12, 202);
        clbEmpresas.Name = "clbEmpresas";
        clbEmpresas.Size = new Size(496, 148);
        clbEmpresas.TabIndex = 13;
        //
        // lblSituacao
        //
        lblSituacao.ForeColor = SystemColors.GrayText;
        lblSituacao.Location = new Point(12, 356);
        lblSituacao.Name = "lblSituacao";
        lblSituacao.Size = new Size(496, 18);
        lblSituacao.TabIndex = 14;
        lblSituacao.Text = "";
        //
        // btnSalvar
        //
        btnSalvar.Location = new Point(326, 384);
        btnSalvar.Name = "btnSalvar";
        btnSalvar.Size = new Size(88, 27);
        btnSalvar.TabIndex = 15;
        btnSalvar.Text = "&Salvar";
        btnSalvar.UseVisualStyleBackColor = true;
        btnSalvar.Click += btnSalvar_Click;
        //
        // btnCancelar
        //
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(420, 384);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(88, 27);
        btnCancelar.TabIndex = 16;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormEdicaoUsuario
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblLogin);
        Controls.Add(txtLogin);
        Controls.Add(lblNome);
        Controls.Add(txtNome);
        Controls.Add(lblPerfil);
        Controls.Add(cboPerfil);
        Controls.Add(chkAtivo);
        Controls.Add(lblSenha);
        Controls.Add(txtSenha);
        Controls.Add(lblConfirmar);
        Controls.Add(txtConfirmar);
        Controls.Add(lblRegraSenha);
        Controls.Add(lblEmpresas);
        Controls.Add(clbEmpresas);
        Controls.Add(lblSituacao);
        Controls.Add(btnSalvar);
        Controls.Add(btnCancelar);
        AcceptButton = btnSalvar;
        CancelButton = btnCancelar;
        ClientSize = new Size(520, 423);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoUsuario";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Usuário";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblLogin;
    private TextBox txtLogin;
    private Label lblNome;
    private TextBox txtNome;
    private Label lblPerfil;
    private ComboBox cboPerfil;
    private CheckBox chkAtivo;
    private Label lblSenha;
    private TextBox txtSenha;
    private Label lblConfirmar;
    private TextBox txtConfirmar;
    private Label lblRegraSenha;
    private Label lblEmpresas;
    private CheckedListBox clbEmpresas;
    private Label lblSituacao;
    private Button btnSalvar;
    private Button btnCancelar;
}
