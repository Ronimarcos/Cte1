namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormEdicaoSerie
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
        lblModelo = new Label();
        cboModelo = new ComboBox();
        lblSerie = new Label();
        nudSerie = new NumericUpDown();
        lblAmbiente = new Label();
        cboAmbiente = new ComboBox();
        lblProximoNumero = new Label();
        nudProximoNumero = new NumericUpDown();
        lblAvisoNumero = new Label();
        pnlBotoes = new Panel();
        btnSalvar = new Button();
        btnCancelar = new Button();
        ((System.ComponentModel.ISupportInitialize)nudSerie).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudProximoNumero).BeginInit();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // lblModelo
        //
        lblModelo.AutoSize = true;
        lblModelo.Location = new Point(12, 12);
        lblModelo.Name = "lblModelo";
        lblModelo.Size = new Size(56, 15);
        lblModelo.TabIndex = 0;
        lblModelo.Text = "Modelo *";
        //
        // cboModelo
        //
        cboModelo.DropDownStyle = ComboBoxStyle.DropDownList;
        cboModelo.FormattingEnabled = true;
        cboModelo.Location = new Point(12, 30);
        cboModelo.Name = "cboModelo";
        cboModelo.Size = new Size(180, 23);
        cboModelo.TabIndex = 1;
        //
        // lblSerie
        //
        lblSerie.AutoSize = true;
        lblSerie.Location = new Point(204, 12);
        lblSerie.Name = "lblSerie";
        lblSerie.Size = new Size(40, 15);
        lblSerie.TabIndex = 2;
        lblSerie.Text = "Série *";
        //
        // nudSerie
        //
        nudSerie.Location = new Point(204, 30);
        nudSerie.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
        nudSerie.Name = "nudSerie";
        nudSerie.Size = new Size(80, 23);
        nudSerie.TabIndex = 3;
        nudSerie.TextAlign = HorizontalAlignment.Right;
        //
        // lblAmbiente
        //
        lblAmbiente.AutoSize = true;
        lblAmbiente.Location = new Point(296, 12);
        lblAmbiente.Name = "lblAmbiente";
        lblAmbiente.Size = new Size(66, 15);
        lblAmbiente.TabIndex = 4;
        lblAmbiente.Text = "Ambiente *";
        //
        // cboAmbiente
        //
        cboAmbiente.DropDownStyle = ComboBoxStyle.DropDownList;
        cboAmbiente.FormattingEnabled = true;
        cboAmbiente.Location = new Point(296, 30);
        cboAmbiente.Name = "cboAmbiente";
        cboAmbiente.Size = new Size(212, 23);
        cboAmbiente.TabIndex = 5;
        //
        // lblProximoNumero
        //
        lblProximoNumero.AutoSize = true;
        lblProximoNumero.Location = new Point(12, 62);
        lblProximoNumero.Name = "lblProximoNumero";
        lblProximoNumero.Size = new Size(103, 15);
        lblProximoNumero.TabIndex = 6;
        lblProximoNumero.Text = "Próximo número *";
        //
        // nudProximoNumero
        //
        nudProximoNumero.Location = new Point(12, 80);
        nudProximoNumero.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        nudProximoNumero.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudProximoNumero.Name = "nudProximoNumero";
        nudProximoNumero.Size = new Size(140, 23);
        nudProximoNumero.TabIndex = 7;
        nudProximoNumero.TextAlign = HorizontalAlignment.Right;
        nudProximoNumero.ThousandsSeparator = true;
        nudProximoNumero.Value = new decimal(new int[] { 1, 0, 0, 0 });
        //
        // lblAvisoNumero
        //
        lblAvisoNumero.ForeColor = SystemColors.GrayText;
        lblAvisoNumero.Location = new Point(164, 80);
        lblAvisoNumero.Name = "lblAvisoNumero";
        lblAvisoNumero.Size = new Size(344, 34);
        lblAvisoNumero.TabIndex = 8;
        lblAvisoNumero.Text = "Aviso sobre o próximo número.";
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnSalvar);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 126);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(520, 48);
        pnlBotoes.TabIndex = 9;
        //
        // btnSalvar
        //
        btnSalvar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSalvar.Location = new Point(326, 12);
        btnSalvar.Name = "btnSalvar";
        btnSalvar.Size = new Size(88, 27);
        btnSalvar.TabIndex = 0;
        btnSalvar.Text = "&Salvar";
        btnSalvar.UseVisualStyleBackColor = true;
        btnSalvar.Click += btnSalvar_Click;
        //
        // btnCancelar
        //
        btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(420, 12);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(88, 27);
        btnCancelar.TabIndex = 1;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormEdicaoSerie
        //
        AcceptButton = btnSalvar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(520, 174);
        Controls.Add(lblModelo);
        Controls.Add(cboModelo);
        Controls.Add(lblSerie);
        Controls.Add(nudSerie);
        Controls.Add(lblAmbiente);
        Controls.Add(cboAmbiente);
        Controls.Add(lblProximoNumero);
        Controls.Add(nudProximoNumero);
        Controls.Add(lblAvisoNumero);
        Controls.Add(pnlBotoes);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormEdicaoSerie";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Série de documento";
        ((System.ComponentModel.ISupportInitialize)nudSerie).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudProximoNumero).EndInit();
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblModelo;
    private ComboBox cboModelo;
    private Label lblSerie;
    private NumericUpDown nudSerie;
    private Label lblAmbiente;
    private ComboBox cboAmbiente;
    private Label lblProximoNumero;
    private NumericUpDown nudProximoNumero;
    private Label lblAvisoNumero;
    private Panel pnlBotoes;
    private Button btnSalvar;
    private Button btnCancelar;
}
