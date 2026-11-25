namespace Emissor.WinForms.Formularios;

partial class FormAguarde
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
        lblMensagem = new Label();
        pbrAndamento = new ProgressBar();
        SuspendLayout();
        //
        // lblMensagem
        //
        lblMensagem.Location = new Point(20, 20);
        lblMensagem.Name = "lblMensagem";
        lblMensagem.Size = new Size(380, 40);
        lblMensagem.TabIndex = 0;
        lblMensagem.Text = "Aguarde...";
        lblMensagem.TextAlign = ContentAlignment.MiddleLeft;
        //
        // pbrAndamento
        //
        pbrAndamento.Location = new Point(20, 68);
        pbrAndamento.MarqueeAnimationSpeed = 30;
        pbrAndamento.Name = "pbrAndamento";
        pbrAndamento.Size = new Size(380, 18);
        pbrAndamento.Style = ProgressBarStyle.Marquee;
        pbrAndamento.TabIndex = 1;
        //
        // FormAguarde
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblMensagem);
        Controls.Add(pbrAndamento);
        ClientSize = new Size(420, 110);
        ControlBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormAguarde";
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Emissor CT-e / MDF-e";
        Shown += FormAguarde_Shown;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblMensagem;
    private ProgressBar pbrAndamento;
}
