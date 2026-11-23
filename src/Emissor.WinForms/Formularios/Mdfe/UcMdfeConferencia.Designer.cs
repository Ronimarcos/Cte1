namespace Emissor.WinForms.Formularios.Mdfe;

partial class UcMdfeConferencia
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
        lblInformacoes = new Label();
        txtInformacoes = new TextBox();
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
        // lblInformacoes
        //
        lblInformacoes.AutoSize = true;
        lblInformacoes.Location = new Point(12, 360);
        lblInformacoes.Name = "lblInformacoes";
        lblInformacoes.Size = new Size(290, 15);
        lblInformacoes.TabIndex = 2;
        lblInformacoes.Text = "Informações complementares (aparecem no DAMDFE)";
        //
        // txtInformacoes
        //
        txtInformacoes.Location = new Point(12, 378);
        txtInformacoes.MaxLength = 5000;
        txtInformacoes.Multiline = true;
        txtInformacoes.Name = "txtInformacoes";
        txtInformacoes.ScrollBars = ScrollBars.Vertical;
        txtInformacoes.Size = new Size(656, 90);
        txtInformacoes.TabIndex = 3;
        //
        // UcMdfeConferencia
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblResumo);
        Controls.Add(txtResumo);
        Controls.Add(lblInformacoes);
        Controls.Add(txtInformacoes);
        Name = "UcMdfeConferencia";
        Size = new Size(680, 480);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblResumo;
    private TextBox txtResumo;
    private Label lblInformacoes;
    private TextBox txtInformacoes;
}
