namespace Emissor.WinForms.Formularios.Cte;

partial class UcCteIdentificacao
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
        lblTipo = new Label();
        cboTipo = new ComboBox();
        lblTipoServico = new Label();
        cboTipoServico = new ComboBox();
        lblSerie = new Label();
        cboSerie = new ComboBox();
        lblProximoNumero = new Label();
        lblSemSerie = new Label();
        lblCfop = new Label();
        txtCfop = new TextBox();
        lblNaturezaOperacao = new Label();
        txtNaturezaOperacao = new TextBox();
        lblEmissao = new Label();
        dtpEmissao = new DateTimePicker();
        SuspendLayout();
        //
        // lblTipo
        //
        lblTipo.AutoSize = true;
        lblTipo.Location = new Point(12, 12);
        lblTipo.Name = "lblTipo";
        lblTipo.Size = new Size(87, 15);
        lblTipo.TabIndex = 0;
        lblTipo.Text = "Tipo do CT-e *";
        //
        // cboTipo
        //
        cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipo.FormattingEnabled = true;
        cboTipo.Location = new Point(12, 30);
        cboTipo.Name = "cboTipo";
        cboTipo.Size = new Size(240, 23);
        cboTipo.TabIndex = 1;
        //
        // lblTipoServico
        //
        lblTipoServico.AutoSize = true;
        lblTipoServico.Location = new Point(264, 12);
        lblTipoServico.Name = "lblTipoServico";
        lblTipoServico.Size = new Size(98, 15);
        lblTipoServico.TabIndex = 2;
        lblTipoServico.Text = "Tipo do serviço *";
        //
        // cboTipoServico
        //
        cboTipoServico.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipoServico.FormattingEnabled = true;
        cboTipoServico.Location = new Point(264, 30);
        cboTipoServico.Name = "cboTipoServico";
        cboTipoServico.Size = new Size(240, 23);
        cboTipoServico.TabIndex = 3;
        //
        // lblSerie
        //
        lblSerie.AutoSize = true;
        lblSerie.Location = new Point(12, 62);
        lblSerie.Name = "lblSerie";
        lblSerie.Size = new Size(40, 15);
        lblSerie.TabIndex = 4;
        lblSerie.Text = "Série *";
        //
        // cboSerie
        //
        cboSerie.DropDownStyle = ComboBoxStyle.DropDownList;
        cboSerie.FormattingEnabled = true;
        cboSerie.Location = new Point(12, 80);
        cboSerie.Name = "cboSerie";
        cboSerie.Size = new Size(240, 23);
        cboSerie.TabIndex = 5;
        cboSerie.SelectedIndexChanged += cboSerie_SelectedIndexChanged;
        //
        // lblProximoNumero
        //
        lblProximoNumero.AutoSize = true;
        lblProximoNumero.ForeColor = SystemColors.GrayText;
        lblProximoNumero.Location = new Point(264, 84);
        lblProximoNumero.Name = "lblProximoNumero";
        lblProximoNumero.Size = new Size(0, 15);
        lblProximoNumero.TabIndex = 6;
        //
        // lblSemSerie
        //
        lblSemSerie.AutoSize = true;
        lblSemSerie.ForeColor = Color.Firebrick;
        lblSemSerie.Location = new Point(12, 106);
        lblSemSerie.Name = "lblSemSerie";
        lblSemSerie.Size = new Size(411, 15);
        lblSemSerie.TabIndex = 7;
        lblSemSerie.Text = "Nenhuma série de CT-e neste ambiente. Cadastre em Cadastros > Séries de documentos.";
        lblSemSerie.Visible = false;
        //
        // lblCfop
        //
        lblCfop.AutoSize = true;
        lblCfop.Location = new Point(12, 128);
        lblCfop.Name = "lblCfop";
        lblCfop.Size = new Size(43, 15);
        lblCfop.TabIndex = 8;
        lblCfop.Text = "CFOP *";
        //
        // txtCfop
        //
        txtCfop.Location = new Point(12, 146);
        txtCfop.MaxLength = 4;
        txtCfop.Name = "txtCfop";
        txtCfop.Size = new Size(80, 23);
        txtCfop.TabIndex = 9;
        //
        // lblNaturezaOperacao
        //
        lblNaturezaOperacao.AutoSize = true;
        lblNaturezaOperacao.Location = new Point(104, 128);
        lblNaturezaOperacao.Name = "lblNaturezaOperacao";
        lblNaturezaOperacao.Size = new Size(133, 15);
        lblNaturezaOperacao.TabIndex = 10;
        lblNaturezaOperacao.Text = "Natureza da operação *";
        //
        // txtNaturezaOperacao
        //
        txtNaturezaOperacao.Location = new Point(104, 146);
        txtNaturezaOperacao.MaxLength = 60;
        txtNaturezaOperacao.Name = "txtNaturezaOperacao";
        txtNaturezaOperacao.Size = new Size(400, 23);
        txtNaturezaOperacao.TabIndex = 11;
        //
        // lblEmissao
        //
        lblEmissao.AutoSize = true;
        lblEmissao.Location = new Point(12, 178);
        lblEmissao.Name = "lblEmissao";
        lblEmissao.Size = new Size(101, 15);
        lblEmissao.TabIndex = 12;
        lblEmissao.Text = "Data de emissão *";
        //
        // dtpEmissao
        //
        dtpEmissao.Format = DateTimePickerFormat.Short;
        dtpEmissao.Location = new Point(12, 196);
        dtpEmissao.Name = "dtpEmissao";
        dtpEmissao.Size = new Size(120, 23);
        dtpEmissao.TabIndex = 13;
        //
        // UcCteIdentificacao
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblTipo);
        Controls.Add(cboTipo);
        Controls.Add(lblTipoServico);
        Controls.Add(cboTipoServico);
        Controls.Add(lblSerie);
        Controls.Add(cboSerie);
        Controls.Add(lblProximoNumero);
        Controls.Add(lblSemSerie);
        Controls.Add(lblCfop);
        Controls.Add(txtCfop);
        Controls.Add(lblNaturezaOperacao);
        Controls.Add(txtNaturezaOperacao);
        Controls.Add(lblEmissao);
        Controls.Add(dtpEmissao);
        Name = "UcCteIdentificacao";
        Size = new Size(680, 480);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTipo;
    private ComboBox cboTipo;
    private Label lblTipoServico;
    private ComboBox cboTipoServico;
    private Label lblSerie;
    private ComboBox cboSerie;
    private Label lblProximoNumero;
    private Label lblSemSerie;
    private Label lblCfop;
    private TextBox txtCfop;
    private Label lblNaturezaOperacao;
    private TextBox txtNaturezaOperacao;
    private Label lblEmissao;
    private DateTimePicker dtpEmissao;
}
