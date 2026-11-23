namespace Emissor.WinForms.Formularios.Cte;

partial class UcCteParticipantes
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
        lblRemetente = new Label();
        cboRemetente = new ComboBox();
        btnIncluirRemetente = new Button();
        btnEditarRemetente = new Button();
        lblDestinatario = new Label();
        cboDestinatario = new ComboBox();
        btnIncluirDestinatario = new Button();
        btnEditarDestinatario = new Button();
        lblExpedidor = new Label();
        cboExpedidor = new ComboBox();
        btnIncluirExpedidor = new Button();
        btnEditarExpedidor = new Button();
        lblRecebedor = new Label();
        cboRecebedor = new ComboBox();
        btnIncluirRecebedor = new Button();
        btnEditarRecebedor = new Button();
        lblTomador = new Label();
        cboTomador = new ComboBox();
        lblTomadorOutros = new Label();
        cboTomadorOutros = new ComboBox();
        btnIncluirTomadorOutros = new Button();
        btnEditarTomadorOutros = new Button();
        lblMunicipioInicio = new Label();
        cboMunicipioInicio = new ComboBox();
        lblMunicipioFim = new Label();
        cboMunicipioFim = new ComboBox();
        lblDica = new Label();
        SuspendLayout();
        //
        // lblRemetente
        //
        lblRemetente.AutoSize = true;
        lblRemetente.Location = new Point(12, 12);
        lblRemetente.Name = "lblRemetente";
        lblRemetente.Size = new Size(68, 15);
        lblRemetente.TabIndex = 0;
        lblRemetente.Text = "Remetente *";
        //
        // cboRemetente
        //
        cboRemetente.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRemetente.FormattingEnabled = true;
        cboRemetente.Location = new Point(12, 30);
        cboRemetente.Name = "cboRemetente";
        cboRemetente.Size = new Size(440, 23);
        cboRemetente.TabIndex = 1;
        cboRemetente.SelectedIndexChanged += cboRemetente_SelectedIndexChanged;
        //
        // btnIncluirRemetente
        //
        btnIncluirRemetente.Location = new Point(460, 29);
        btnIncluirRemetente.Name = "btnIncluirRemetente";
        btnIncluirRemetente.Size = new Size(80, 25);
        btnIncluirRemetente.TabIndex = 2;
        btnIncluirRemetente.Text = "Incluir";
        btnIncluirRemetente.UseVisualStyleBackColor = true;
        btnIncluirRemetente.Click += btnIncluir_Click;
        //
        // btnEditarRemetente
        //
        btnEditarRemetente.Location = new Point(546, 29);
        btnEditarRemetente.Name = "btnEditarRemetente";
        btnEditarRemetente.Size = new Size(80, 25);
        btnEditarRemetente.TabIndex = 3;
        btnEditarRemetente.Text = "Editar";
        btnEditarRemetente.UseVisualStyleBackColor = true;
        btnEditarRemetente.Click += btnEditar_Click;
        //
        // lblDestinatario
        //
        lblDestinatario.AutoSize = true;
        lblDestinatario.Location = new Point(12, 62);
        lblDestinatario.Name = "lblDestinatario";
        lblDestinatario.Size = new Size(83, 15);
        lblDestinatario.TabIndex = 4;
        lblDestinatario.Text = "Destinatário *";
        //
        // cboDestinatario
        //
        cboDestinatario.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDestinatario.FormattingEnabled = true;
        cboDestinatario.Location = new Point(12, 80);
        cboDestinatario.Name = "cboDestinatario";
        cboDestinatario.Size = new Size(440, 23);
        cboDestinatario.TabIndex = 5;
        cboDestinatario.SelectedIndexChanged += cboDestinatario_SelectedIndexChanged;
        //
        // btnIncluirDestinatario
        //
        btnIncluirDestinatario.Location = new Point(460, 79);
        btnIncluirDestinatario.Name = "btnIncluirDestinatario";
        btnIncluirDestinatario.Size = new Size(80, 25);
        btnIncluirDestinatario.TabIndex = 6;
        btnIncluirDestinatario.Text = "Incluir";
        btnIncluirDestinatario.UseVisualStyleBackColor = true;
        btnIncluirDestinatario.Click += btnIncluir_Click;
        //
        // btnEditarDestinatario
        //
        btnEditarDestinatario.Location = new Point(546, 79);
        btnEditarDestinatario.Name = "btnEditarDestinatario";
        btnEditarDestinatario.Size = new Size(80, 25);
        btnEditarDestinatario.TabIndex = 7;
        btnEditarDestinatario.Text = "Editar";
        btnEditarDestinatario.UseVisualStyleBackColor = true;
        btnEditarDestinatario.Click += btnEditar_Click;
        //
        // lblExpedidor
        //
        lblExpedidor.AutoSize = true;
        lblExpedidor.Location = new Point(12, 112);
        lblExpedidor.Name = "lblExpedidor";
        lblExpedidor.Size = new Size(57, 15);
        lblExpedidor.TabIndex = 8;
        lblExpedidor.Text = "Expedidor";
        //
        // cboExpedidor
        //
        cboExpedidor.DropDownStyle = ComboBoxStyle.DropDownList;
        cboExpedidor.FormattingEnabled = true;
        cboExpedidor.Location = new Point(12, 130);
        cboExpedidor.Name = "cboExpedidor";
        cboExpedidor.Size = new Size(440, 23);
        cboExpedidor.TabIndex = 9;
        //
        // btnIncluirExpedidor
        //
        btnIncluirExpedidor.Location = new Point(460, 129);
        btnIncluirExpedidor.Name = "btnIncluirExpedidor";
        btnIncluirExpedidor.Size = new Size(80, 25);
        btnIncluirExpedidor.TabIndex = 10;
        btnIncluirExpedidor.Text = "Incluir";
        btnIncluirExpedidor.UseVisualStyleBackColor = true;
        btnIncluirExpedidor.Click += btnIncluir_Click;
        //
        // btnEditarExpedidor
        //
        btnEditarExpedidor.Location = new Point(546, 129);
        btnEditarExpedidor.Name = "btnEditarExpedidor";
        btnEditarExpedidor.Size = new Size(80, 25);
        btnEditarExpedidor.TabIndex = 11;
        btnEditarExpedidor.Text = "Editar";
        btnEditarExpedidor.UseVisualStyleBackColor = true;
        btnEditarExpedidor.Click += btnEditar_Click;
        //
        // lblRecebedor
        //
        lblRecebedor.AutoSize = true;
        lblRecebedor.Location = new Point(12, 162);
        lblRecebedor.Name = "lblRecebedor";
        lblRecebedor.Size = new Size(58, 15);
        lblRecebedor.TabIndex = 12;
        lblRecebedor.Text = "Recebedor";
        //
        // cboRecebedor
        //
        cboRecebedor.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRecebedor.FormattingEnabled = true;
        cboRecebedor.Location = new Point(12, 180);
        cboRecebedor.Name = "cboRecebedor";
        cboRecebedor.Size = new Size(440, 23);
        cboRecebedor.TabIndex = 13;
        //
        // btnIncluirRecebedor
        //
        btnIncluirRecebedor.Location = new Point(460, 179);
        btnIncluirRecebedor.Name = "btnIncluirRecebedor";
        btnIncluirRecebedor.Size = new Size(80, 25);
        btnIncluirRecebedor.TabIndex = 14;
        btnIncluirRecebedor.Text = "Incluir";
        btnIncluirRecebedor.UseVisualStyleBackColor = true;
        btnIncluirRecebedor.Click += btnIncluir_Click;
        //
        // btnEditarRecebedor
        //
        btnEditarRecebedor.Location = new Point(546, 179);
        btnEditarRecebedor.Name = "btnEditarRecebedor";
        btnEditarRecebedor.Size = new Size(80, 25);
        btnEditarRecebedor.TabIndex = 15;
        btnEditarRecebedor.Text = "Editar";
        btnEditarRecebedor.UseVisualStyleBackColor = true;
        btnEditarRecebedor.Click += btnEditar_Click;
        //
        // lblTomador
        //
        lblTomador.AutoSize = true;
        lblTomador.Location = new Point(12, 212);
        lblTomador.Name = "lblTomador";
        lblTomador.Size = new Size(117, 15);
        lblTomador.TabIndex = 16;
        lblTomador.Text = "Tomador do serviço *";
        //
        // cboTomador
        //
        cboTomador.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTomador.FormattingEnabled = true;
        cboTomador.Location = new Point(12, 230);
        cboTomador.Name = "cboTomador";
        cboTomador.Size = new Size(200, 23);
        cboTomador.TabIndex = 17;
        cboTomador.SelectedIndexChanged += cboTomador_SelectedIndexChanged;
        //
        // lblTomadorOutros
        //
        lblTomadorOutros.AutoSize = true;
        lblTomadorOutros.Location = new Point(224, 212);
        lblTomadorOutros.Name = "lblTomadorOutros";
        lblTomadorOutros.Size = new Size(108, 15);
        lblTomadorOutros.TabIndex = 18;
        lblTomadorOutros.Text = "Tomador (outros) *";
        //
        // cboTomadorOutros
        //
        cboTomadorOutros.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTomadorOutros.DropDownWidth = 440;
        cboTomadorOutros.FormattingEnabled = true;
        cboTomadorOutros.Location = new Point(224, 230);
        cboTomadorOutros.Name = "cboTomadorOutros";
        cboTomadorOutros.Size = new Size(228, 23);
        cboTomadorOutros.TabIndex = 19;
        //
        // btnIncluirTomadorOutros
        //
        btnIncluirTomadorOutros.Location = new Point(460, 229);
        btnIncluirTomadorOutros.Name = "btnIncluirTomadorOutros";
        btnIncluirTomadorOutros.Size = new Size(80, 25);
        btnIncluirTomadorOutros.TabIndex = 20;
        btnIncluirTomadorOutros.Text = "Incluir";
        btnIncluirTomadorOutros.UseVisualStyleBackColor = true;
        btnIncluirTomadorOutros.Click += btnIncluir_Click;
        //
        // btnEditarTomadorOutros
        //
        btnEditarTomadorOutros.Location = new Point(546, 229);
        btnEditarTomadorOutros.Name = "btnEditarTomadorOutros";
        btnEditarTomadorOutros.Size = new Size(80, 25);
        btnEditarTomadorOutros.TabIndex = 21;
        btnEditarTomadorOutros.Text = "Editar";
        btnEditarTomadorOutros.UseVisualStyleBackColor = true;
        btnEditarTomadorOutros.Click += btnEditar_Click;
        //
        // lblMunicipioInicio
        //
        lblMunicipioInicio.AutoSize = true;
        lblMunicipioInicio.Location = new Point(12, 262);
        lblMunicipioInicio.Name = "lblMunicipioInicio";
        lblMunicipioInicio.Size = new Size(117, 15);
        lblMunicipioInicio.TabIndex = 22;
        lblMunicipioInicio.Text = "Início da prestação *";
        //
        // cboMunicipioInicio
        //
        cboMunicipioInicio.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMunicipioInicio.FormattingEnabled = true;
        cboMunicipioInicio.Location = new Point(12, 280);
        cboMunicipioInicio.Name = "cboMunicipioInicio";
        cboMunicipioInicio.Size = new Size(220, 23);
        cboMunicipioInicio.TabIndex = 23;
        //
        // lblMunicipioFim
        //
        lblMunicipioFim.AutoSize = true;
        lblMunicipioFim.Location = new Point(244, 262);
        lblMunicipioFim.Name = "lblMunicipioFim";
        lblMunicipioFim.Size = new Size(130, 15);
        lblMunicipioFim.TabIndex = 24;
        lblMunicipioFim.Text = "Término da prestação *";
        //
        // cboMunicipioFim
        //
        cboMunicipioFim.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMunicipioFim.FormattingEnabled = true;
        cboMunicipioFim.Location = new Point(244, 280);
        cboMunicipioFim.Name = "cboMunicipioFim";
        cboMunicipioFim.Size = new Size(220, 23);
        cboMunicipioFim.TabIndex = 25;
        //
        // lblDica
        //
        lblDica.AutoSize = true;
        lblDica.ForeColor = SystemColors.GrayText;
        lblDica.Location = new Point(12, 316);
        lblDica.Name = "lblDica";
        lblDica.Size = new Size(380, 15);
        lblDica.TabIndex = 26;
        lblDica.Text = "Incluir e Editar abrem o cadastro de participante sem sair da emissão.";
        //
        // UcCteParticipantes
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblRemetente);
        Controls.Add(cboRemetente);
        Controls.Add(btnIncluirRemetente);
        Controls.Add(btnEditarRemetente);
        Controls.Add(lblDestinatario);
        Controls.Add(cboDestinatario);
        Controls.Add(btnIncluirDestinatario);
        Controls.Add(btnEditarDestinatario);
        Controls.Add(lblExpedidor);
        Controls.Add(cboExpedidor);
        Controls.Add(btnIncluirExpedidor);
        Controls.Add(btnEditarExpedidor);
        Controls.Add(lblRecebedor);
        Controls.Add(cboRecebedor);
        Controls.Add(btnIncluirRecebedor);
        Controls.Add(btnEditarRecebedor);
        Controls.Add(lblTomador);
        Controls.Add(cboTomador);
        Controls.Add(lblTomadorOutros);
        Controls.Add(cboTomadorOutros);
        Controls.Add(btnIncluirTomadorOutros);
        Controls.Add(btnEditarTomadorOutros);
        Controls.Add(lblMunicipioInicio);
        Controls.Add(cboMunicipioInicio);
        Controls.Add(lblMunicipioFim);
        Controls.Add(cboMunicipioFim);
        Controls.Add(lblDica);
        Name = "UcCteParticipantes";
        Size = new Size(680, 480);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblRemetente;
    private ComboBox cboRemetente;
    private Button btnIncluirRemetente;
    private Button btnEditarRemetente;
    private Label lblDestinatario;
    private ComboBox cboDestinatario;
    private Button btnIncluirDestinatario;
    private Button btnEditarDestinatario;
    private Label lblExpedidor;
    private ComboBox cboExpedidor;
    private Button btnIncluirExpedidor;
    private Button btnEditarExpedidor;
    private Label lblRecebedor;
    private ComboBox cboRecebedor;
    private Button btnIncluirRecebedor;
    private Button btnEditarRecebedor;
    private Label lblTomador;
    private ComboBox cboTomador;
    private Label lblTomadorOutros;
    private ComboBox cboTomadorOutros;
    private Button btnIncluirTomadorOutros;
    private Button btnEditarTomadorOutros;
    private Label lblMunicipioInicio;
    private ComboBox cboMunicipioInicio;
    private Label lblMunicipioFim;
    private ComboBox cboMunicipioFim;
    private Label lblDica;
}
