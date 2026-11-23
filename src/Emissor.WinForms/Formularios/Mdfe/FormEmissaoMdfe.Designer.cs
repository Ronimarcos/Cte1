namespace Emissor.WinForms.Formularios.Mdfe;

partial class FormEmissaoMdfe
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
        pnlConteudo = new Panel();
        ucConferencia = new UcMdfeConferencia();
        ucCargaSeguro = new UcMdfeCargaSeguro();
        ucVeiculos = new UcMdfeVeiculos();
        ucCarregamentoDescarga = new UcMdfeCarregamentoDescarga();
        ucIdentificacao = new UcMdfeIdentificacao();
        lblTituloEtapa = new Label();
        lstEtapas = new ListBox();
        pnlBotoes = new Panel();
        btnCancelar = new Button();
        btnSalvarRascunho = new Button();
        btnVoltar = new Button();
        btnAvancar = new Button();
        btnConcluir = new Button();
        pnlConteudo.SuspendLayout();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // pnlConteudo
        //
        pnlConteudo.Controls.Add(ucConferencia);
        pnlConteudo.Controls.Add(ucCargaSeguro);
        pnlConteudo.Controls.Add(ucVeiculos);
        pnlConteudo.Controls.Add(ucCarregamentoDescarga);
        pnlConteudo.Controls.Add(ucIdentificacao);
        pnlConteudo.Controls.Add(lblTituloEtapa);
        pnlConteudo.Dock = DockStyle.Fill;
        pnlConteudo.Location = new Point(200, 0);
        pnlConteudo.Name = "pnlConteudo";
        pnlConteudo.Padding = new Padding(8, 0, 8, 0);
        pnlConteudo.Size = new Size(700, 540);
        pnlConteudo.TabIndex = 0;
        //
        // ucConferencia
        //
        ucConferencia.AutoScroll = true;
        ucConferencia.Dock = DockStyle.Fill;
        ucConferencia.Location = new Point(8, 36);
        ucConferencia.Name = "ucConferencia";
        ucConferencia.Size = new Size(684, 504);
        ucConferencia.TabIndex = 0;
        //
        // ucCargaSeguro
        //
        ucCargaSeguro.AutoScroll = true;
        ucCargaSeguro.Dock = DockStyle.Fill;
        ucCargaSeguro.Location = new Point(8, 36);
        ucCargaSeguro.Name = "ucCargaSeguro";
        ucCargaSeguro.Size = new Size(684, 504);
        ucCargaSeguro.TabIndex = 1;
        //
        // ucVeiculos
        //
        ucVeiculos.AutoScroll = true;
        ucVeiculos.Dock = DockStyle.Fill;
        ucVeiculos.Location = new Point(8, 36);
        ucVeiculos.Name = "ucVeiculos";
        ucVeiculos.Size = new Size(684, 504);
        ucVeiculos.TabIndex = 2;
        //
        // ucCarregamentoDescarga
        //
        ucCarregamentoDescarga.AutoScroll = true;
        ucCarregamentoDescarga.Dock = DockStyle.Fill;
        ucCarregamentoDescarga.Location = new Point(8, 36);
        ucCarregamentoDescarga.Name = "ucCarregamentoDescarga";
        ucCarregamentoDescarga.Size = new Size(684, 504);
        ucCarregamentoDescarga.TabIndex = 3;
        //
        // ucIdentificacao
        //
        ucIdentificacao.AutoScroll = true;
        ucIdentificacao.Dock = DockStyle.Fill;
        ucIdentificacao.Location = new Point(8, 36);
        ucIdentificacao.Name = "ucIdentificacao";
        ucIdentificacao.Size = new Size(684, 504);
        ucIdentificacao.TabIndex = 4;
        //
        // lblTituloEtapa
        //
        lblTituloEtapa.Dock = DockStyle.Top;
        lblTituloEtapa.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTituloEtapa.Location = new Point(8, 0);
        lblTituloEtapa.Name = "lblTituloEtapa";
        lblTituloEtapa.Size = new Size(684, 36);
        lblTituloEtapa.TabIndex = 5;
        lblTituloEtapa.Text = "Etapa";
        lblTituloEtapa.TextAlign = ContentAlignment.MiddleLeft;
        //
        // lstEtapas
        //
        lstEtapas.Dock = DockStyle.Left;
        lstEtapas.Font = new Font("Segoe UI", 10F);
        lstEtapas.FormattingEnabled = true;
        lstEtapas.IntegralHeight = false;
        lstEtapas.ItemHeight = 17;
        lstEtapas.Location = new Point(0, 0);
        lstEtapas.Name = "lstEtapas";
        lstEtapas.Size = new Size(200, 540);
        lstEtapas.TabIndex = 1;
        lstEtapas.SelectedIndexChanged += lstEtapas_SelectedIndexChanged;
        //
        // pnlBotoes
        //
        pnlBotoes.BorderStyle = BorderStyle.FixedSingle;
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Controls.Add(btnSalvarRascunho);
        pnlBotoes.Controls.Add(btnVoltar);
        pnlBotoes.Controls.Add(btnAvancar);
        pnlBotoes.Controls.Add(btnConcluir);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 540);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(900, 52);
        pnlBotoes.TabIndex = 2;
        //
        // btnCancelar
        //
        btnCancelar.Location = new Point(12, 12);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 27);
        btnCancelar.TabIndex = 0;
        btnCancelar.Text = "Ca&ncelar";
        btnCancelar.UseVisualStyleBackColor = true;
        btnCancelar.Click += btnCancelar_Click;
        //
        // btnSalvarRascunho
        //
        btnSalvarRascunho.Location = new Point(118, 12);
        btnSalvarRascunho.Name = "btnSalvarRascunho";
        btnSalvarRascunho.Size = new Size(140, 27);
        btnSalvarRascunho.TabIndex = 1;
        btnSalvarRascunho.Text = "Salvar &rascunho";
        btnSalvarRascunho.UseVisualStyleBackColor = true;
        btnSalvarRascunho.Click += btnSalvarRascunho_Click;
        //
        // btnVoltar
        //
        btnVoltar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnVoltar.Location = new Point(556, 12);
        btnVoltar.Name = "btnVoltar";
        btnVoltar.Size = new Size(100, 27);
        btnVoltar.TabIndex = 2;
        btnVoltar.Text = "< &Voltar";
        btnVoltar.UseVisualStyleBackColor = true;
        btnVoltar.Click += btnVoltar_Click;
        //
        // btnAvancar
        //
        btnAvancar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAvancar.Location = new Point(662, 12);
        btnAvancar.Name = "btnAvancar";
        btnAvancar.Size = new Size(100, 27);
        btnAvancar.TabIndex = 3;
        btnAvancar.Text = "&Avançar >";
        btnAvancar.UseVisualStyleBackColor = true;
        btnAvancar.Click += btnAvancar_Click;
        //
        // btnConcluir
        //
        btnConcluir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnConcluir.Location = new Point(768, 12);
        btnConcluir.Name = "btnConcluir";
        btnConcluir.Size = new Size(118, 27);
        btnConcluir.TabIndex = 4;
        btnConcluir.Text = "&Concluir";
        btnConcluir.UseVisualStyleBackColor = true;
        btnConcluir.Click += btnConcluir_Click;
        //
        // FormEmissaoMdfe
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(pnlConteudo);
        Controls.Add(lstEtapas);
        Controls.Add(pnlBotoes);
        ClientSize = new Size(900, 592);
        MinimumSize = new Size(916, 631);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = false;
        Name = "FormEmissaoMdfe";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Emitir MDF-e";
        FormClosing += FormEmissaoMdfe_FormClosing;
        pnlConteudo.ResumeLayout(false);
        pnlConteudo.PerformLayout();
        pnlBotoes.ResumeLayout(false);
        pnlBotoes.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel pnlConteudo;
    private UcMdfeConferencia ucConferencia;
    private UcMdfeCargaSeguro ucCargaSeguro;
    private UcMdfeVeiculos ucVeiculos;
    private UcMdfeCarregamentoDescarga ucCarregamentoDescarga;
    private UcMdfeIdentificacao ucIdentificacao;
    private Label lblTituloEtapa;
    private ListBox lstEtapas;
    private Panel pnlBotoes;
    private Button btnCancelar;
    private Button btnSalvarRascunho;
    private Button btnVoltar;
    private Button btnAvancar;
    private Button btnConcluir;
}
