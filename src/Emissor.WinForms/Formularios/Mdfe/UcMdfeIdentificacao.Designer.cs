namespace Emissor.WinForms.Formularios.Mdfe;

partial class UcMdfeIdentificacao
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
        lblTipoEmitente = new Label();
        cboTipoEmitente = new ComboBox();
        lblTipoTransportador = new Label();
        cboTipoTransportador = new ComboBox();
        lblSerie = new Label();
        cboSerie = new ComboBox();
        lblProximoNumero = new Label();
        lblSemSerie = new Label();
        lblUfInicio = new Label();
        cboUfInicio = new ComboBox();
        lblUfFim = new Label();
        cboUfFim = new ComboBox();
        lblPercurso = new Label();
        txtPercurso = new TextBox();
        lblDicaPercurso = new Label();
        lblEmissao = new Label();
        dtpEmissao = new DateTimePicker();
        lblInicioViagem = new Label();
        dtpInicioViagem = new DateTimePicker();
        lblDicaCiot = new Label();
        SuspendLayout();
        //
        // lblTipoEmitente
        //
        lblTipoEmitente.AutoSize = true;
        lblTipoEmitente.Location = new Point(12, 12);
        lblTipoEmitente.Name = "lblTipoEmitente";
        lblTipoEmitente.Size = new Size(104, 15);
        lblTipoEmitente.TabIndex = 0;
        lblTipoEmitente.Text = "Tipo do emitente *";
        //
        // cboTipoEmitente
        //
        cboTipoEmitente.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipoEmitente.FormattingEnabled = true;
        cboTipoEmitente.Location = new Point(12, 30);
        cboTipoEmitente.Name = "cboTipoEmitente";
        cboTipoEmitente.Size = new Size(330, 23);
        cboTipoEmitente.TabIndex = 1;
        //
        // lblTipoTransportador
        //
        lblTipoTransportador.AutoSize = true;
        lblTipoTransportador.Location = new Point(354, 12);
        lblTipoTransportador.Name = "lblTipoTransportador";
        lblTipoTransportador.Size = new Size(125, 15);
        lblTipoTransportador.TabIndex = 2;
        lblTipoTransportador.Text = "Tipo do transportador";
        //
        // cboTipoTransportador
        //
        cboTipoTransportador.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTipoTransportador.FormattingEnabled = true;
        cboTipoTransportador.Location = new Point(354, 30);
        cboTipoTransportador.Name = "cboTipoTransportador";
        cboTipoTransportador.Size = new Size(220, 23);
        cboTipoTransportador.TabIndex = 3;
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
        lblSemSerie.Size = new Size(480, 15);
        lblSemSerie.TabIndex = 7;
        lblSemSerie.Text = "Nenhuma série de MDF-e neste ambiente. Cadastre em Cadastros > Séries de documentos.";
        lblSemSerie.Visible = false;
        //
        // lblUfInicio
        //
        lblUfInicio.AutoSize = true;
        lblUfInicio.Location = new Point(12, 128);
        lblUfInicio.Name = "lblUfInicio";
        lblUfInicio.Size = new Size(78, 15);
        lblUfInicio.TabIndex = 8;
        lblUfInicio.Text = "UF de início *";
        //
        // cboUfInicio
        //
        cboUfInicio.DropDownStyle = ComboBoxStyle.DropDownList;
        cboUfInicio.FormattingEnabled = true;
        cboUfInicio.Location = new Point(12, 146);
        cboUfInicio.Name = "cboUfInicio";
        cboUfInicio.Size = new Size(80, 23);
        cboUfInicio.TabIndex = 9;
        //
        // lblUfFim
        //
        lblUfFim.AutoSize = true;
        lblUfFim.Location = new Point(104, 128);
        lblUfFim.Name = "lblUfFim";
        lblUfFim.Size = new Size(64, 15);
        lblUfFim.TabIndex = 10;
        lblUfFim.Text = "UF de fim *";
        //
        // cboUfFim
        //
        cboUfFim.DropDownStyle = ComboBoxStyle.DropDownList;
        cboUfFim.FormattingEnabled = true;
        cboUfFim.Location = new Point(104, 146);
        cboUfFim.Name = "cboUfFim";
        cboUfFim.Size = new Size(80, 23);
        cboUfFim.TabIndex = 11;
        //
        // lblPercurso
        //
        lblPercurso.AutoSize = true;
        lblPercurso.Location = new Point(196, 128);
        lblPercurso.Name = "lblPercurso";
        lblPercurso.Size = new Size(90, 15);
        lblPercurso.TabIndex = 12;
        lblPercurso.Text = "UFs do percurso";
        //
        // txtPercurso
        //
        txtPercurso.CharacterCasing = CharacterCasing.Upper;
        txtPercurso.Location = new Point(196, 146);
        txtPercurso.MaxLength = 100;
        txtPercurso.Name = "txtPercurso";
        txtPercurso.Size = new Size(300, 23);
        txtPercurso.TabIndex = 13;
        //
        // lblDicaPercurso
        //
        lblDicaPercurso.AutoSize = true;
        lblDicaPercurso.ForeColor = SystemColors.GrayText;
        lblDicaPercurso.Location = new Point(196, 172);
        lblDicaPercurso.Name = "lblDicaPercurso";
        lblDicaPercurso.Size = new Size(420, 15);
        lblDicaPercurso.TabIndex = 14;
        lblDicaPercurso.Text = "UFs entre o início e o fim, em ordem, separadas por vírgula (ex.: MS, SP).";
        //
        // lblEmissao
        //
        lblEmissao.AutoSize = true;
        lblEmissao.Location = new Point(12, 198);
        lblEmissao.Name = "lblEmissao";
        lblEmissao.Size = new Size(101, 15);
        lblEmissao.TabIndex = 15;
        lblEmissao.Text = "Data de emissão *";
        //
        // dtpEmissao
        //
        dtpEmissao.Format = DateTimePickerFormat.Short;
        dtpEmissao.Location = new Point(12, 216);
        dtpEmissao.Name = "dtpEmissao";
        dtpEmissao.Size = new Size(120, 23);
        dtpEmissao.TabIndex = 16;
        //
        // lblInicioViagem
        //
        lblInicioViagem.AutoSize = true;
        lblInicioViagem.Location = new Point(144, 198);
        lblInicioViagem.Name = "lblInicioViagem";
        lblInicioViagem.Size = new Size(270, 15);
        lblInicioViagem.TabIndex = 17;
        lblInicioViagem.Text = "Início previsto da viagem (marque para informar)";
        //
        // dtpInicioViagem
        //
        dtpInicioViagem.CustomFormat = "dd/MM/yyyy HH:mm";
        dtpInicioViagem.Format = DateTimePickerFormat.Custom;
        dtpInicioViagem.Location = new Point(144, 216);
        dtpInicioViagem.Name = "dtpInicioViagem";
        dtpInicioViagem.ShowCheckBox = true;
        dtpInicioViagem.Size = new Size(170, 23);
        dtpInicioViagem.TabIndex = 18;
        //
        // lblDicaCiot
        //
        lblDicaCiot.AutoSize = true;
        lblDicaCiot.ForeColor = SystemColors.GrayText;
        lblDicaCiot.Location = new Point(12, 256);
        lblDicaCiot.Name = "lblDicaCiot";
        lblDicaCiot.Size = new Size(480, 15);
        lblDicaCiot.TabIndex = 19;
        lblDicaCiot.Text = "Tipos de emitente 1 e 3 exigem o CIOT na etapa 3 (NT MDF-e 2026.001, rejeição 684).";
        //
        // UcMdfeIdentificacao
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblTipoEmitente);
        Controls.Add(cboTipoEmitente);
        Controls.Add(lblTipoTransportador);
        Controls.Add(cboTipoTransportador);
        Controls.Add(lblSerie);
        Controls.Add(cboSerie);
        Controls.Add(lblProximoNumero);
        Controls.Add(lblSemSerie);
        Controls.Add(lblUfInicio);
        Controls.Add(cboUfInicio);
        Controls.Add(lblUfFim);
        Controls.Add(cboUfFim);
        Controls.Add(lblPercurso);
        Controls.Add(txtPercurso);
        Controls.Add(lblDicaPercurso);
        Controls.Add(lblEmissao);
        Controls.Add(dtpEmissao);
        Controls.Add(lblInicioViagem);
        Controls.Add(dtpInicioViagem);
        Controls.Add(lblDicaCiot);
        Name = "UcMdfeIdentificacao";
        Size = new Size(680, 484);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTipoEmitente;
    private ComboBox cboTipoEmitente;
    private Label lblTipoTransportador;
    private ComboBox cboTipoTransportador;
    private Label lblSerie;
    private ComboBox cboSerie;
    private Label lblProximoNumero;
    private Label lblSemSerie;
    private Label lblUfInicio;
    private ComboBox cboUfInicio;
    private Label lblUfFim;
    private ComboBox cboUfFim;
    private Label lblPercurso;
    private TextBox txtPercurso;
    private Label lblDicaPercurso;
    private Label lblEmissao;
    private DateTimePicker dtpEmissao;
    private Label lblInicioViagem;
    private DateTimePicker dtpInicioViagem;
    private Label lblDicaCiot;
}
