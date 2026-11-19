namespace Emissor.WinForms.Formularios;

partial class FormEmConstrucao
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
        lblMensagem = new Label();
        SuspendLayout();
        //
        // lblTitulo
        //
        lblTitulo.Dock = DockStyle.Top;
        lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitulo.Location = new Point(0, 0);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Padding = new Padding(12, 0, 0, 0);
        lblTitulo.Size = new Size(640, 56);
        lblTitulo.TabIndex = 0;
        lblTitulo.Text = "Título";
        lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
        //
        // lblMensagem
        //
        lblMensagem.Dock = DockStyle.Fill;
        lblMensagem.ForeColor = SystemColors.GrayText;
        lblMensagem.Location = new Point(0, 56);
        lblMensagem.Name = "lblMensagem";
        lblMensagem.Size = new Size(640, 344);
        lblMensagem.TabIndex = 1;
        lblMensagem.Text = "Tela em construção — será desenhada nas próximas etapas do protótipo.";
        lblMensagem.TextAlign = ContentAlignment.MiddleCenter;
        //
        // FormEmConstrucao
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(640, 400);
        Controls.Add(lblMensagem);
        Controls.Add(lblTitulo);
        Name = "FormEmConstrucao";
        Text = "Em construção";
        ResumeLayout(false);
    }

    #endregion

    private Label lblTitulo;
    private Label lblMensagem;
}
