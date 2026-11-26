namespace Emissor.WinForms.Formularios.Eventos;

partial class FormInclusaoCondutor
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
        lblDocumento = new Label();
        lblMotorista = new Label();
        cboMotorista = new ComboBox();
        lblNome = new Label();
        txtNome = new TextBox();
        lblCpf = new Label();
        txtCpf = new TextBox();
        lblAviso = new Label();
        btnGerar = new Button();
        btnFechar = new Button();
        SuspendLayout();
        //
        // lblDocumento
        //
        lblDocumento.AutoSize = true;
        lblDocumento.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDocumento.Location = new Point(12, 12);
        lblDocumento.Name = "lblDocumento";
        lblDocumento.Size = new Size(70, 15);
        lblDocumento.TabIndex = 0;
        lblDocumento.Text = "Documento";
        //
        // lblMotorista
        //
        lblMotorista.AutoSize = true;
        lblMotorista.Location = new Point(12, 40);
        lblMotorista.Name = "lblMotorista";
        lblMotorista.Size = new Size(130, 15);
        lblMotorista.TabIndex = 1;
        lblMotorista.Text = "&Motorista cadastrado";
        //
        // cboMotorista
        //
        cboMotorista.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMotorista.FormattingEnabled = true;
        cboMotorista.Location = new Point(12, 58);
        cboMotorista.Name = "cboMotorista";
        cboMotorista.Size = new Size(456, 23);
        cboMotorista.TabIndex = 2;
        cboMotorista.SelectedIndexChanged += cboMotorista_SelectedIndexChanged;
        //
        // lblNome
        //
        lblNome.AutoSize = true;
        lblNome.Location = new Point(12, 90);
        lblNome.Name = "lblNome";
        lblNome.Size = new Size(34, 15);
        lblNome.TabIndex = 3;
        lblNome.Text = "&Nome";
        //
        // txtNome
        //
        txtNome.Location = new Point(12, 108);
        txtNome.MaxLength = 60;
        txtNome.Name = "txtNome";
        txtNome.Size = new Size(330, 23);
        txtNome.TabIndex = 4;
        //
        // lblCpf
        //
        lblCpf.AutoSize = true;
        lblCpf.Location = new Point(354, 90);
        lblCpf.Name = "lblCpf";
        lblCpf.Size = new Size(28, 15);
        lblCpf.TabIndex = 5;
        lblCpf.Text = "&CPF";
        //
        // txtCpf
        //
        txtCpf.Location = new Point(354, 108);
        txtCpf.MaxLength = 14;
        txtCpf.Name = "txtCpf";
        txtCpf.Size = new Size(114, 23);
        txtCpf.TabIndex = 6;
        //
        // lblAviso
        //
        lblAviso.ForeColor = SystemColors.GrayText;
        lblAviso.Location = new Point(12, 142);
        lblAviso.Name = "lblAviso";
        lblAviso.Size = new Size(456, 45);
        lblAviso.TabIndex = 7;
        lblAviso.Text = "";
        //
        // btnGerar
        //
        btnGerar.Location = new Point(220, 194);
        btnGerar.Name = "btnGerar";
        btnGerar.Size = new Size(150, 27);
        btnGerar.TabIndex = 8;
        btnGerar.Text = "&Gerar XML de teste";
        btnGerar.UseVisualStyleBackColor = true;
        btnGerar.Click += btnGerar_Click;
        //
        // btnFechar
        //
        btnFechar.DialogResult = DialogResult.Cancel;
        btnFechar.Location = new Point(376, 194);
        btnFechar.Name = "btnFechar";
        btnFechar.Size = new Size(92, 27);
        btnFechar.TabIndex = 9;
        btnFechar.Text = "&Fechar";
        btnFechar.UseVisualStyleBackColor = true;
        //
        // FormInclusaoCondutor
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblDocumento);
        Controls.Add(lblMotorista);
        Controls.Add(cboMotorista);
        Controls.Add(lblNome);
        Controls.Add(txtNome);
        Controls.Add(lblCpf);
        Controls.Add(txtCpf);
        Controls.Add(lblAviso);
        Controls.Add(btnGerar);
        Controls.Add(btnFechar);
        AcceptButton = btnGerar;
        ClientSize = new Size(480, 234);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        CancelButton = btnFechar;
        Name = "FormInclusaoCondutor";
        Text = "Inclusão de condutor";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblDocumento;
    private Label lblMotorista;
    private ComboBox cboMotorista;
    private Label lblNome;
    private TextBox txtNome;
    private Label lblCpf;
    private TextBox txtCpf;
    private Label lblAviso;
    private Button btnGerar;
    private Button btnFechar;
}
