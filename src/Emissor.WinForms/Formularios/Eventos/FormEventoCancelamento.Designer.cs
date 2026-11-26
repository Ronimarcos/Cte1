namespace Emissor.WinForms.Formularios.Eventos;

partial class FormEventoCancelamento
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
        lblJustificativa = new Label();
        txtJustificativa = new TextBox();
        lblContador = new Label();
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
        // lblJustificativa
        //
        lblJustificativa.AutoSize = true;
        lblJustificativa.Location = new Point(12, 40);
        lblJustificativa.Name = "lblJustificativa";
        lblJustificativa.Size = new Size(220, 15);
        lblJustificativa.TabIndex = 1;
        lblJustificativa.Text = "&Justificativa (15 a 255 caracteres)";
        //
        // txtJustificativa
        //
        txtJustificativa.Location = new Point(12, 58);
        txtJustificativa.MaxLength = 255;
        txtJustificativa.Multiline = true;
        txtJustificativa.Name = "txtJustificativa";
        txtJustificativa.ScrollBars = ScrollBars.Vertical;
        txtJustificativa.Size = new Size(456, 90);
        txtJustificativa.TabIndex = 2;
        txtJustificativa.TextChanged += txtJustificativa_TextChanged;
        //
        // lblContador
        //
        lblContador.AutoSize = true;
        lblContador.Location = new Point(12, 152);
        lblContador.Name = "lblContador";
        lblContador.Size = new Size(52, 15);
        lblContador.TabIndex = 3;
        lblContador.Text = "0 de 255";
        //
        // lblAviso
        //
        lblAviso.ForeColor = SystemColors.GrayText;
        lblAviso.Location = new Point(12, 176);
        lblAviso.Name = "lblAviso";
        lblAviso.Size = new Size(456, 45);
        lblAviso.TabIndex = 4;
        lblAviso.Text = "";
        //
        // btnGerar
        //
        btnGerar.Location = new Point(220, 228);
        btnGerar.Name = "btnGerar";
        btnGerar.Size = new Size(150, 27);
        btnGerar.TabIndex = 5;
        btnGerar.Text = "&Gerar XML de teste";
        btnGerar.UseVisualStyleBackColor = true;
        btnGerar.Click += btnGerar_Click;
        //
        // btnFechar
        //
        btnFechar.DialogResult = DialogResult.Cancel;
        btnFechar.Location = new Point(376, 228);
        btnFechar.Name = "btnFechar";
        btnFechar.Size = new Size(92, 27);
        btnFechar.TabIndex = 6;
        btnFechar.Text = "&Fechar";
        btnFechar.UseVisualStyleBackColor = true;
        //
        // FormEventoCancelamento
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblDocumento);
        Controls.Add(lblJustificativa);
        Controls.Add(txtJustificativa);
        Controls.Add(lblContador);
        Controls.Add(lblAviso);
        Controls.Add(btnGerar);
        Controls.Add(btnFechar);
        ClientSize = new Size(480, 268);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        CancelButton = btnFechar;
        Name = "FormEventoCancelamento";
        Text = "Cancelamento";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblDocumento;
    private Label lblJustificativa;
    private TextBox txtJustificativa;
    private Label lblContador;
    private Label lblAviso;
    private Button btnGerar;
    private Button btnFechar;
}
