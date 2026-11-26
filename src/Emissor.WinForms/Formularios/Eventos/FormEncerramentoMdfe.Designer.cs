namespace Emissor.WinForms.Formularios.Eventos;

partial class FormEncerramentoMdfe
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
        lblData = new Label();
        dtpData = new DateTimePicker();
        lblUf = new Label();
        cboUf = new ComboBox();
        lblMunicipio = new Label();
        cboMunicipio = new ComboBox();
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
        // lblData
        //
        lblData.AutoSize = true;
        lblData.Location = new Point(12, 40);
        lblData.Name = "lblData";
        lblData.Size = new Size(130, 15);
        lblData.TabIndex = 1;
        lblData.Text = "&Data do encerramento";
        //
        // dtpData
        //
        dtpData.Format = DateTimePickerFormat.Short;
        dtpData.Location = new Point(12, 58);
        dtpData.Name = "dtpData";
        dtpData.Size = new Size(120, 23);
        dtpData.TabIndex = 2;
        //
        // lblUf
        //
        lblUf.AutoSize = true;
        lblUf.Location = new Point(144, 40);
        lblUf.Name = "lblUf";
        lblUf.Size = new Size(22, 15);
        lblUf.TabIndex = 3;
        lblUf.Text = "&UF";
        //
        // cboUf
        //
        cboUf.DropDownStyle = ComboBoxStyle.DropDownList;
        cboUf.FormattingEnabled = true;
        cboUf.Location = new Point(144, 58);
        cboUf.Name = "cboUf";
        cboUf.Size = new Size(70, 23);
        cboUf.TabIndex = 4;
        cboUf.SelectedIndexChanged += cboUf_SelectedIndexChanged;
        //
        // lblMunicipio
        //
        lblMunicipio.AutoSize = true;
        lblMunicipio.Location = new Point(226, 40);
        lblMunicipio.Name = "lblMunicipio";
        lblMunicipio.Size = new Size(64, 15);
        lblMunicipio.TabIndex = 5;
        lblMunicipio.Text = "&Município";
        //
        // cboMunicipio
        //
        cboMunicipio.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMunicipio.FormattingEnabled = true;
        cboMunicipio.Location = new Point(226, 58);
        cboMunicipio.Name = "cboMunicipio";
        cboMunicipio.Size = new Size(242, 23);
        cboMunicipio.TabIndex = 6;
        //
        // lblAviso
        //
        lblAviso.ForeColor = SystemColors.GrayText;
        lblAviso.Location = new Point(12, 92);
        lblAviso.Name = "lblAviso";
        lblAviso.Size = new Size(456, 45);
        lblAviso.TabIndex = 7;
        lblAviso.Text = "";
        //
        // btnGerar
        //
        btnGerar.Location = new Point(220, 144);
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
        btnFechar.Location = new Point(376, 144);
        btnFechar.Name = "btnFechar";
        btnFechar.Size = new Size(92, 27);
        btnFechar.TabIndex = 9;
        btnFechar.Text = "&Fechar";
        btnFechar.UseVisualStyleBackColor = true;
        //
        // FormEncerramentoMdfe
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblDocumento);
        Controls.Add(lblData);
        Controls.Add(dtpData);
        Controls.Add(lblUf);
        Controls.Add(cboUf);
        Controls.Add(lblMunicipio);
        Controls.Add(cboMunicipio);
        Controls.Add(lblAviso);
        Controls.Add(btnGerar);
        Controls.Add(btnFechar);
        AcceptButton = btnGerar;
        ClientSize = new Size(480, 184);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        CancelButton = btnFechar;
        Name = "FormEncerramentoMdfe";
        Text = "Encerramento do MDF-e";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblDocumento;
    private Label lblData;
    private DateTimePicker dtpData;
    private Label lblUf;
    private ComboBox cboUf;
    private Label lblMunicipio;
    private ComboBox cboMunicipio;
    private Label lblAviso;
    private Button btnGerar;
    private Button btnFechar;
}
