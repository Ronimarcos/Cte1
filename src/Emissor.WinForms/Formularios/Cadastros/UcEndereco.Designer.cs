namespace Emissor.WinForms.Formularios.Cadastros;

partial class UcEndereco
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

    #region Component Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblLogradouro = new Label();
        txtLogradouro = new TextBox();
        lblNumero = new Label();
        txtNumero = new TextBox();
        lblComplemento = new Label();
        txtComplemento = new TextBox();
        lblBairro = new Label();
        txtBairro = new TextBox();
        lblMunicipio = new Label();
        cboMunicipio = new ComboBox();
        lblCep = new Label();
        txtCep = new TextBox();
        SuspendLayout();
        //
        // lblLogradouro
        //
        lblLogradouro.AutoSize = true;
        lblLogradouro.Location = new Point(0, 0);
        lblLogradouro.Name = "lblLogradouro";
        lblLogradouro.Size = new Size(75, 15);
        lblLogradouro.TabIndex = 0;
        lblLogradouro.Text = "Logradouro *";
        //
        // txtLogradouro
        //
        txtLogradouro.Location = new Point(0, 18);
        txtLogradouro.MaxLength = 60;
        txtLogradouro.Name = "txtLogradouro";
        txtLogradouro.Size = new Size(400, 23);
        txtLogradouro.TabIndex = 1;
        //
        // lblNumero
        //
        lblNumero.AutoSize = true;
        lblNumero.Location = new Point(412, 0);
        lblNumero.Name = "lblNumero";
        lblNumero.Size = new Size(59, 15);
        lblNumero.TabIndex = 2;
        lblNumero.Text = "Número *";
        //
        // txtNumero
        //
        txtNumero.Location = new Point(412, 18);
        txtNumero.MaxLength = 60;
        txtNumero.Name = "txtNumero";
        txtNumero.Size = new Size(80, 23);
        txtNumero.TabIndex = 3;
        //
        // lblComplemento
        //
        lblComplemento.AutoSize = true;
        lblComplemento.Location = new Point(504, 0);
        lblComplemento.Name = "lblComplemento";
        lblComplemento.Size = new Size(84, 15);
        lblComplemento.TabIndex = 4;
        lblComplemento.Text = "Complemento";
        //
        // txtComplemento
        //
        txtComplemento.Location = new Point(504, 18);
        txtComplemento.MaxLength = 60;
        txtComplemento.Name = "txtComplemento";
        txtComplemento.Size = new Size(112, 23);
        txtComplemento.TabIndex = 5;
        //
        // lblBairro
        //
        lblBairro.AutoSize = true;
        lblBairro.Location = new Point(0, 50);
        lblBairro.Name = "lblBairro";
        lblBairro.Size = new Size(47, 15);
        lblBairro.TabIndex = 6;
        lblBairro.Text = "Bairro *";
        //
        // txtBairro
        //
        txtBairro.Location = new Point(0, 68);
        txtBairro.MaxLength = 60;
        txtBairro.Name = "txtBairro";
        txtBairro.Size = new Size(220, 23);
        txtBairro.TabIndex = 7;
        //
        // lblMunicipio
        //
        lblMunicipio.AutoSize = true;
        lblMunicipio.Location = new Point(232, 50);
        lblMunicipio.Name = "lblMunicipio";
        lblMunicipio.Size = new Size(70, 15);
        lblMunicipio.TabIndex = 8;
        lblMunicipio.Text = "Município *";
        //
        // cboMunicipio
        //
        cboMunicipio.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMunicipio.FormattingEnabled = true;
        cboMunicipio.Location = new Point(232, 68);
        cboMunicipio.Name = "cboMunicipio";
        cboMunicipio.Size = new Size(260, 23);
        cboMunicipio.TabIndex = 9;
        //
        // lblCep
        //
        lblCep.AutoSize = true;
        lblCep.Location = new Point(504, 50);
        lblCep.Name = "lblCep";
        lblCep.Size = new Size(28, 15);
        lblCep.TabIndex = 10;
        lblCep.Text = "CEP";
        //
        // txtCep
        //
        txtCep.Location = new Point(504, 68);
        txtCep.MaxLength = 9;
        txtCep.Name = "txtCep";
        txtCep.Size = new Size(112, 23);
        txtCep.TabIndex = 11;
        //
        // UcEndereco
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblLogradouro);
        Controls.Add(txtLogradouro);
        Controls.Add(lblNumero);
        Controls.Add(txtNumero);
        Controls.Add(lblComplemento);
        Controls.Add(txtComplemento);
        Controls.Add(lblBairro);
        Controls.Add(txtBairro);
        Controls.Add(lblMunicipio);
        Controls.Add(cboMunicipio);
        Controls.Add(lblCep);
        Controls.Add(txtCep);
        Name = "UcEndereco";
        Size = new Size(616, 94);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblLogradouro;
    private TextBox txtLogradouro;
    private Label lblNumero;
    private TextBox txtNumero;
    private Label lblComplemento;
    private TextBox txtComplemento;
    private Label lblBairro;
    private TextBox txtBairro;
    private Label lblMunicipio;
    private ComboBox cboMunicipio;
    private Label lblCep;
    private TextBox txtCep;
}
