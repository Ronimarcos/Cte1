namespace Emissor.WinForms.Formularios.Acesso;

partial class FormTrocarSenha
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
        lblAviso = new Label();
        lblAtual = new Label();
        txtAtual = new TextBox();
        lblNova = new Label();
        txtNova = new TextBox();
        lblConfirmar = new Label();
        txtConfirmar = new TextBox();
        lblRegra = new Label();
        btnSalvar = new Button();
        btnCancelar = new Button();
        SuspendLayout();
        //
        // lblAviso
        //
        lblAviso.Location = new Point(12, 12);
        lblAviso.Name = "lblAviso";
        lblAviso.Size = new Size(336, 32);
        lblAviso.TabIndex = 0;
        lblAviso.Text = "Informe a senha atual e a nova senha.";
        //
        // lblAtual
        //
        lblAtual.AutoSize = true;
        lblAtual.Location = new Point(12, 52);
        lblAtual.Name = "lblAtual";
        lblAtual.Size = new Size(80, 15);
        lblAtual.TabIndex = 1;
        lblAtual.Text = "Senha &atual *";
        //
        // txtAtual
        //
        txtAtual.Location = new Point(12, 70);
        txtAtual.MaxLength = 128;
        txtAtual.Name = "txtAtual";
        txtAtual.Size = new Size(336, 23);
        txtAtual.TabIndex = 2;
        txtAtual.UseSystemPasswordChar = true;
        //
        // lblNova
        //
        lblNova.AutoSize = true;
        lblNova.Location = new Point(12, 102);
        lblNova.Name = "lblNova";
        lblNova.Size = new Size(80, 15);
        lblNova.TabIndex = 3;
        lblNova.Text = "&Nova senha *";
        //
        // txtNova
        //
        txtNova.Location = new Point(12, 120);
        txtNova.MaxLength = 128;
        txtNova.Name = "txtNova";
        txtNova.Size = new Size(160, 23);
        txtNova.TabIndex = 4;
        txtNova.UseSystemPasswordChar = true;
        //
        // lblConfirmar
        //
        lblConfirmar.AutoSize = true;
        lblConfirmar.Location = new Point(188, 102);
        lblConfirmar.Name = "lblConfirmar";
        lblConfirmar.Size = new Size(80, 15);
        lblConfirmar.TabIndex = 5;
        lblConfirmar.Text = "&Confirmar *";
        //
        // txtConfirmar
        //
        txtConfirmar.Location = new Point(188, 120);
        txtConfirmar.MaxLength = 128;
        txtConfirmar.Name = "txtConfirmar";
        txtConfirmar.Size = new Size(160, 23);
        txtConfirmar.TabIndex = 6;
        txtConfirmar.UseSystemPasswordChar = true;
        //
        // lblRegra
        //
        lblRegra.ForeColor = SystemColors.GrayText;
        lblRegra.Location = new Point(12, 152);
        lblRegra.Name = "lblRegra";
        lblRegra.Size = new Size(336, 32);
        lblRegra.TabIndex = 7;
        lblRegra.Text = "A senha precisa ter pelo menos 8 caracteres, com letras e números.";
        //
        // btnSalvar
        //
        btnSalvar.Location = new Point(172, 194);
        btnSalvar.Name = "btnSalvar";
        btnSalvar.Size = new Size(85, 27);
        btnSalvar.TabIndex = 8;
        btnSalvar.Text = "&Salvar";
        btnSalvar.UseVisualStyleBackColor = true;
        btnSalvar.Click += btnSalvar_Click;
        //
        // btnCancelar
        //
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(263, 194);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(85, 27);
        btnCancelar.TabIndex = 9;
        btnCancelar.Text = "Ca&ncelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormTrocarSenha
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblAviso);
        Controls.Add(lblAtual);
        Controls.Add(txtAtual);
        Controls.Add(lblNova);
        Controls.Add(txtNova);
        Controls.Add(lblConfirmar);
        Controls.Add(txtConfirmar);
        Controls.Add(lblRegra);
        Controls.Add(btnSalvar);
        Controls.Add(btnCancelar);
        AcceptButton = btnSalvar;
        CancelButton = btnCancelar;
        ClientSize = new Size(360, 233);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormTrocarSenha";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Trocar senha";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblAviso;
    private Label lblAtual;
    private TextBox txtAtual;
    private Label lblNova;
    private TextBox txtNova;
    private Label lblConfirmar;
    private TextBox txtConfirmar;
    private Label lblRegra;
    private Button btnSalvar;
    private Button btnCancelar;
}
