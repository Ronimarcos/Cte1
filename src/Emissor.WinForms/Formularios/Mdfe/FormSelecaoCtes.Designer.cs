namespace Emissor.WinForms.Formularios.Mdfe;

partial class FormSelecaoCtes
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
        clbCtes = new CheckedListBox();
        lblVazio = new Label();
        btnOk = new Button();
        btnCancelar = new Button();
        SuspendLayout();
        //
        // lblInstrucao
        //
        lblInstrucao.AutoSize = true;
        lblInstrucao.Location = new Point(12, 12);
        lblInstrucao.Name = "lblInstrucao";
        lblInstrucao.Size = new Size(440, 15);
        lblInstrucao.TabIndex = 0;
        lblInstrucao.Text = "Marque os CT-e prontos para transmitir que serão descarregados neste município:";
        //
        // clbCtes
        //
        clbCtes.CheckOnClick = true;
        clbCtes.FormattingEnabled = true;
        clbCtes.IntegralHeight = false;
        clbCtes.Location = new Point(12, 32);
        clbCtes.Name = "clbCtes";
        clbCtes.Size = new Size(676, 260);
        clbCtes.TabIndex = 1;
        //
        // lblVazio
        //
        lblVazio.AutoSize = true;
        lblVazio.ForeColor = Color.Firebrick;
        lblVazio.Location = new Point(12, 300);
        lblVazio.Name = "lblVazio";
        lblVazio.Size = new Size(470, 15);
        lblVazio.TabIndex = 2;
        lblVazio.Text = "Nenhum CT-e disponível: só entram CT-e prontos e ainda não vinculados a este MDF-e.";
        lblVazio.Visible = false;
        //
        // btnOk
        //
        btnOk.Location = new Point(506, 322);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(88, 27);
        btnOk.TabIndex = 3;
        btnOk.Text = "&OK";
        btnOk.UseVisualStyleBackColor = true;
        btnOk.Click += btnOk_Click;
        //
        // btnCancelar
        //
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(600, 322);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(88, 27);
        btnCancelar.TabIndex = 4;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormSelecaoCtes
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblInstrucao);
        Controls.Add(clbCtes);
        Controls.Add(lblVazio);
        Controls.Add(btnOk);
        Controls.Add(btnCancelar);
        AcceptButton = btnOk;
        CancelButton = btnCancelar;
        ClientSize = new Size(700, 361);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormSelecaoCtes";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Adicionar CT-e emitidos";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblInstrucao;
    private CheckedListBox clbCtes;
    private Label lblVazio;
    private Button btnOk;
    private Button btnCancelar;
}
