namespace Emissor.WinForms.Formularios.Cte;

partial class UcCteConferencia
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
        lblResumo = new Label();
        txtResumo = new TextBox();
        lblObservacoes = new Label();
        txtObservacoes = new TextBox();
        SuspendLayout();
        //
        // lblResumo
        //
        lblResumo.AutoSize = true;
        lblResumo.Location = new Point(12, 12);
        lblResumo.Name = "lblResumo";
        lblResumo.Size = new Size(196, 15);
        lblResumo.TabIndex = 0;
        lblResumo.Text = "Confira os dados antes de concluir:";
        //
        // txtResumo
        //
        txtResumo.BackColor = SystemColors.Window;
        txtResumo.Font = new Font("Consolas", 9F);
        txtResumo.Location = new Point(12, 30);
        txtResumo.Multiline = true;
        txtResumo.Name = "txtResumo";
        txtResumo.ReadOnly = true;
        txtResumo.ScrollBars = ScrollBars.Vertical;
        txtResumo.Size = new Size(656, 320);
        txtResumo.TabIndex = 1;
        txtResumo.TabStop = false;
        //
        // lblObservacoes
        //
        lblObservacoes.AutoSize = true;
        lblObservacoes.Location = new Point(12, 360);
        lblObservacoes.Name = "lblObservacoes";
        lblObservacoes.Size = new Size(189, 15);
        lblObservacoes.TabIndex = 2;
        lblObservacoes.Text = "Observações (aparecem no DACTE)";
        //
        // txtObservacoes
        //
        txtObservacoes.Location = new Point(12, 378);
        txtObservacoes.MaxLength = 2000;
        txtObservacoes.Multiline = true;
        txtObservacoes.Name = "txtObservacoes";
        txtObservacoes.ScrollBars = ScrollBars.Vertical;
        txtObservacoes.Size = new Size(656, 90);
        txtObservacoes.TabIndex = 3;
        //
        // UcCteConferencia
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblResumo);
        Controls.Add(txtResumo);
        Controls.Add(lblObservacoes);
        Controls.Add(txtObservacoes);
        Name = "UcCteConferencia";
        Size = new Size(680, 480);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblResumo;
    private TextBox txtResumo;
    private Label lblObservacoes;
    private TextBox txtObservacoes;
}
