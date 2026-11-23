namespace Emissor.WinForms.Formularios.Mdfe;

partial class FormConsultaMdfe
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
        dgvMdfes = new DataGridView();
        pnlFiltros = new Panel();
        lblPeriodo = new Label();
        dtpDe = new DateTimePicker();
        lblAte = new Label();
        dtpAte = new DateTimePicker();
        lblSituacao = new Label();
        cboSituacao = new ComboBox();
        lblPesquisa = new Label();
        txtPesquisa = new TextBox();
        btnNovo = new Button();
        btnAbrir = new Button();
        btnExcluir = new Button();
        pnlRodape = new Panel();
        lblTotal = new Label();
        lblEventos = new Label();
        btnEncerrar = new Button();
        btnCancelarMdfe = new Button();
        btnIncluirCondutor = new Button();
        btnIncluirDfe = new Button();
        btnFechar = new Button();
        pnlFiltros.SuspendLayout();
        pnlRodape.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvMdfes).BeginInit();
        SuspendLayout();
        //
        // dgvMdfes
        //
        dgvMdfes.AllowUserToAddRows = false;
        dgvMdfes.AllowUserToDeleteRows = false;
        dgvMdfes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvMdfes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvMdfes.Dock = DockStyle.Fill;
        dgvMdfes.Location = new Point(0, 64);
        dgvMdfes.MultiSelect = false;
        dgvMdfes.Name = "dgvMdfes";
        dgvMdfes.ReadOnly = true;
        dgvMdfes.RowHeadersVisible = false;
        dgvMdfes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvMdfes.Size = new Size(1000, 412);
        dgvMdfes.TabIndex = 0;
        dgvMdfes.CellDoubleClick += dgvMdfes_CellDoubleClick;
        dgvMdfes.CellFormatting += dgvMdfes_CellFormatting;
        dgvMdfes.KeyDown += dgvMdfes_KeyDown;
        dgvMdfes.SelectionChanged += dgvMdfes_SelectionChanged;
        //
        // pnlFiltros
        //
        pnlFiltros.Controls.Add(lblPeriodo);
        pnlFiltros.Controls.Add(dtpDe);
        pnlFiltros.Controls.Add(lblAte);
        pnlFiltros.Controls.Add(dtpAte);
        pnlFiltros.Controls.Add(lblSituacao);
        pnlFiltros.Controls.Add(cboSituacao);
        pnlFiltros.Controls.Add(lblPesquisa);
        pnlFiltros.Controls.Add(txtPesquisa);
        pnlFiltros.Controls.Add(btnNovo);
        pnlFiltros.Controls.Add(btnAbrir);
        pnlFiltros.Controls.Add(btnExcluir);
        pnlFiltros.Dock = DockStyle.Top;
        pnlFiltros.Location = new Point(0, 0);
        pnlFiltros.Name = "pnlFiltros";
        pnlFiltros.Size = new Size(1000, 64);
        pnlFiltros.TabIndex = 1;
        //
        // lblPeriodo
        //
        lblPeriodo.AutoSize = true;
        lblPeriodo.Location = new Point(12, 10);
        lblPeriodo.Name = "lblPeriodo";
        lblPeriodo.Size = new Size(63, 15);
        lblPeriodo.TabIndex = 0;
        lblPeriodo.Text = "Emissão de";
        //
        // dtpDe
        //
        dtpDe.Format = DateTimePickerFormat.Short;
        dtpDe.Location = new Point(12, 28);
        dtpDe.Name = "dtpDe";
        dtpDe.Size = new Size(110, 23);
        dtpDe.TabIndex = 1;
        dtpDe.ValueChanged += Filtro_Alterado;
        //
        // lblAte
        //
        lblAte.AutoSize = true;
        lblAte.Location = new Point(128, 32);
        lblAte.Name = "lblAte";
        lblAte.Size = new Size(12, 15);
        lblAte.TabIndex = 2;
        lblAte.Text = "a";
        //
        // dtpAte
        //
        dtpAte.Format = DateTimePickerFormat.Short;
        dtpAte.Location = new Point(146, 28);
        dtpAte.Name = "dtpAte";
        dtpAte.Size = new Size(110, 23);
        dtpAte.TabIndex = 3;
        dtpAte.ValueChanged += Filtro_Alterado;
        //
        // lblSituacao
        //
        lblSituacao.AutoSize = true;
        lblSituacao.Location = new Point(268, 10);
        lblSituacao.Name = "lblSituacao";
        lblSituacao.Size = new Size(51, 15);
        lblSituacao.TabIndex = 4;
        lblSituacao.Text = "Situação";
        //
        // cboSituacao
        //
        cboSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
        cboSituacao.FormattingEnabled = true;
        cboSituacao.Location = new Point(268, 28);
        cboSituacao.Name = "cboSituacao";
        cboSituacao.Size = new Size(170, 23);
        cboSituacao.TabIndex = 5;
        cboSituacao.SelectedIndexChanged += Filtro_Alterado;
        //
        // lblPesquisa
        //
        lblPesquisa.AutoSize = true;
        lblPesquisa.Location = new Point(450, 10);
        lblPesquisa.Name = "lblPesquisa";
        lblPesquisa.Size = new Size(215, 15);
        lblPesquisa.TabIndex = 6;
        lblPesquisa.Text = "Pesquisar (número, placa, UF, produto)";
        //
        // txtPesquisa
        //
        txtPesquisa.Location = new Point(450, 28);
        txtPesquisa.MaxLength = 60;
        txtPesquisa.Name = "txtPesquisa";
        txtPesquisa.PlaceholderText = "Digite para filtrar";
        txtPesquisa.Size = new Size(230, 23);
        txtPesquisa.TabIndex = 7;
        txtPesquisa.TextChanged += Filtro_Alterado;
        //
        // btnNovo
        //
        btnNovo.Location = new Point(692, 27);
        btnNovo.Name = "btnNovo";
        btnNovo.Size = new Size(90, 27);
        btnNovo.TabIndex = 8;
        btnNovo.Text = "&Novo";
        btnNovo.UseVisualStyleBackColor = true;
        btnNovo.Click += btnNovo_Click;
        //
        // btnAbrir
        //
        btnAbrir.Location = new Point(788, 27);
        btnAbrir.Name = "btnAbrir";
        btnAbrir.Size = new Size(90, 27);
        btnAbrir.TabIndex = 9;
        btnAbrir.Text = "&Abrir";
        btnAbrir.UseVisualStyleBackColor = true;
        btnAbrir.Click += btnAbrir_Click;
        //
        // btnExcluir
        //
        btnExcluir.Location = new Point(884, 27);
        btnExcluir.Name = "btnExcluir";
        btnExcluir.Size = new Size(90, 27);
        btnExcluir.TabIndex = 10;
        btnExcluir.Text = "E&xcluir";
        btnExcluir.UseVisualStyleBackColor = true;
        btnExcluir.Click += btnExcluir_Click;
        //
        // pnlRodape
        //
        pnlRodape.Controls.Add(lblTotal);
        pnlRodape.Controls.Add(lblEventos);
        pnlRodape.Controls.Add(btnEncerrar);
        pnlRodape.Controls.Add(btnCancelarMdfe);
        pnlRodape.Controls.Add(btnIncluirCondutor);
        pnlRodape.Controls.Add(btnIncluirDfe);
        pnlRodape.Controls.Add(btnFechar);
        pnlRodape.Dock = DockStyle.Bottom;
        pnlRodape.Location = new Point(0, 476);
        pnlRodape.Name = "pnlRodape";
        pnlRodape.Size = new Size(1000, 44);
        pnlRodape.TabIndex = 2;
        //
        // lblTotal
        //
        lblTotal.AutoSize = true;
        lblTotal.Location = new Point(12, 14);
        lblTotal.Name = "lblTotal";
        lblTotal.Size = new Size(48, 15);
        lblTotal.TabIndex = 0;
        lblTotal.Text = "0 MDF-e";
        //
        // lblEventos
        //
        lblEventos.AutoSize = true;
        lblEventos.Location = new Point(96, 14);
        lblEventos.Name = "lblEventos";
        lblEventos.Size = new Size(49, 15);
        lblEventos.TabIndex = 1;
        lblEventos.Text = "Eventos:";
        //
        // btnEncerrar
        //
        btnEncerrar.Location = new Point(150, 8);
        btnEncerrar.Name = "btnEncerrar";
        btnEncerrar.Size = new Size(90, 27);
        btnEncerrar.TabIndex = 2;
        btnEncerrar.Text = "Encerrar";
        btnEncerrar.UseVisualStyleBackColor = true;
        btnEncerrar.Click += btnEvento_Click;
        //
        // btnCancelarMdfe
        //
        btnCancelarMdfe.Location = new Point(246, 8);
        btnCancelarMdfe.Name = "btnCancelarMdfe";
        btnCancelarMdfe.Size = new Size(120, 27);
        btnCancelarMdfe.TabIndex = 3;
        btnCancelarMdfe.Text = "Cancelar MDF-e";
        btnCancelarMdfe.UseVisualStyleBackColor = true;
        btnCancelarMdfe.Click += btnEvento_Click;
        //
        // btnIncluirCondutor
        //
        btnIncluirCondutor.Location = new Point(372, 8);
        btnIncluirCondutor.Name = "btnIncluirCondutor";
        btnIncluirCondutor.Size = new Size(120, 27);
        btnIncluirCondutor.TabIndex = 4;
        btnIncluirCondutor.Text = "Incluir condutor";
        btnIncluirCondutor.UseVisualStyleBackColor = true;
        btnIncluirCondutor.Click += btnEvento_Click;
        //
        // btnIncluirDfe
        //
        btnIncluirDfe.Location = new Point(498, 8);
        btnIncluirDfe.Name = "btnIncluirDfe";
        btnIncluirDfe.Size = new Size(100, 27);
        btnIncluirDfe.TabIndex = 5;
        btnIncluirDfe.Text = "Incluir DF-e";
        btnIncluirDfe.UseVisualStyleBackColor = true;
        btnIncluirDfe.Click += btnEvento_Click;
        //
        // btnFechar
        //
        btnFechar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnFechar.Location = new Point(898, 8);
        btnFechar.Name = "btnFechar";
        btnFechar.Size = new Size(90, 27);
        btnFechar.TabIndex = 6;
        btnFechar.Text = "&Fechar";
        btnFechar.UseVisualStyleBackColor = true;
        btnFechar.Click += btnFechar_Click;
        //
        // FormConsultaMdfe
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(dgvMdfes);
        Controls.Add(pnlFiltros);
        Controls.Add(pnlRodape);
        ClientSize = new Size(1000, 520);
        Name = "FormConsultaMdfe";
        Text = "Consulta de MDF-e";
        ((System.ComponentModel.ISupportInitialize)dgvMdfes).EndInit();
        pnlFiltros.ResumeLayout(false);
        pnlFiltros.PerformLayout();
        pnlRodape.ResumeLayout(false);
        pnlRodape.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private DataGridView dgvMdfes;
    private Panel pnlFiltros;
    private Label lblPeriodo;
    private DateTimePicker dtpDe;
    private Label lblAte;
    private DateTimePicker dtpAte;
    private Label lblSituacao;
    private ComboBox cboSituacao;
    private Label lblPesquisa;
    private TextBox txtPesquisa;
    private Button btnNovo;
    private Button btnAbrir;
    private Button btnExcluir;
    private Panel pnlRodape;
    private Label lblTotal;
    private Label lblEventos;
    private Button btnEncerrar;
    private Button btnCancelarMdfe;
    private Button btnIncluirCondutor;
    private Button btnIncluirDfe;
    private Button btnFechar;
}
