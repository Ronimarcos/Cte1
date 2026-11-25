namespace Emissor.WinForms.Formularios.Acesso;

partial class FormConsultaAuditoria
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
        dgvAuditoria = new DataGridView();
        pnlFiltros = new Panel();
        lblPeriodo = new Label();
        dtpDe = new DateTimePicker();
        lblAte = new Label();
        dtpAte = new DateTimePicker();
        lblUsuario = new Label();
        cboUsuario = new ComboBox();
        lblEmpresa = new Label();
        cboEmpresa = new ComboBox();
        lblAcao = new Label();
        cboAcao = new ComboBox();
        lblPesquisa = new Label();
        txtPesquisa = new TextBox();
        pnlRodape = new Panel();
        lblTotal = new Label();
        btnFechar = new Button();
        pnlFiltros.SuspendLayout();
        pnlRodape.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAuditoria).BeginInit();
        SuspendLayout();
        //
        // dgvAuditoria
        //
        dgvAuditoria.AllowUserToAddRows = false;
        dgvAuditoria.AllowUserToDeleteRows = false;
        dgvAuditoria.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAuditoria.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        dgvAuditoria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvAuditoria.DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True };
        dgvAuditoria.Dock = DockStyle.Fill;
        dgvAuditoria.Location = new Point(0, 64);
        dgvAuditoria.MultiSelect = false;
        dgvAuditoria.Name = "dgvAuditoria";
        dgvAuditoria.ReadOnly = true;
        dgvAuditoria.RowHeadersVisible = false;
        dgvAuditoria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAuditoria.Size = new Size(1000, 412);
        dgvAuditoria.TabIndex = 0;
        //
        // pnlFiltros
        //
        pnlFiltros.Controls.Add(lblPeriodo);
        pnlFiltros.Controls.Add(dtpDe);
        pnlFiltros.Controls.Add(lblAte);
        pnlFiltros.Controls.Add(dtpAte);
        pnlFiltros.Controls.Add(lblUsuario);
        pnlFiltros.Controls.Add(cboUsuario);
        pnlFiltros.Controls.Add(lblEmpresa);
        pnlFiltros.Controls.Add(cboEmpresa);
        pnlFiltros.Controls.Add(lblAcao);
        pnlFiltros.Controls.Add(cboAcao);
        pnlFiltros.Controls.Add(lblPesquisa);
        pnlFiltros.Controls.Add(txtPesquisa);
        pnlFiltros.Dock = DockStyle.Top;
        pnlFiltros.Location = new Point(0, 0);
        pnlFiltros.Name = "pnlFiltros";
        pnlFiltros.Size = new Size(1000, 64);
        pnlFiltros.TabIndex = 1;
        //
        // lblPeriodo
        //
        lblPeriodo.AutoSize = true;
        lblPeriodo.Location = new Point(12, 8);
        lblPeriodo.Name = "lblPeriodo";
        lblPeriodo.Size = new Size(50, 15);
        lblPeriodo.TabIndex = 0;
        lblPeriodo.Text = "Período";
        //
        // dtpDe
        //
        dtpDe.Format = DateTimePickerFormat.Short;
        dtpDe.Location = new Point(12, 26);
        dtpDe.Name = "dtpDe";
        dtpDe.Size = new Size(110, 23);
        dtpDe.TabIndex = 1;
        dtpDe.ValueChanged += Filtro_Alterado;
        //
        // lblAte
        //
        lblAte.AutoSize = true;
        lblAte.Location = new Point(126, 30);
        lblAte.Name = "lblAte";
        lblAte.Size = new Size(12, 15);
        lblAte.TabIndex = 2;
        lblAte.Text = "a";
        //
        // dtpAte
        //
        dtpAte.Format = DateTimePickerFormat.Short;
        dtpAte.Location = new Point(142, 26);
        dtpAte.Name = "dtpAte";
        dtpAte.Size = new Size(110, 23);
        dtpAte.TabIndex = 3;
        dtpAte.ValueChanged += Filtro_Alterado;
        //
        // lblUsuario
        //
        lblUsuario.AutoSize = true;
        lblUsuario.Location = new Point(264, 8);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Size = new Size(50, 15);
        lblUsuario.TabIndex = 4;
        lblUsuario.Text = "&Usuário";
        //
        // cboUsuario
        //
        cboUsuario.DropDownStyle = ComboBoxStyle.DropDownList;
        cboUsuario.FormattingEnabled = true;
        cboUsuario.Location = new Point(264, 26);
        cboUsuario.Name = "cboUsuario";
        cboUsuario.Size = new Size(150, 23);
        cboUsuario.TabIndex = 5;
        cboUsuario.SelectedIndexChanged += Filtro_Alterado;
        //
        // lblEmpresa
        //
        lblEmpresa.AutoSize = true;
        lblEmpresa.Location = new Point(426, 8);
        lblEmpresa.Name = "lblEmpresa";
        lblEmpresa.Size = new Size(55, 15);
        lblEmpresa.TabIndex = 6;
        lblEmpresa.Text = "&Empresa";
        //
        // cboEmpresa
        //
        cboEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboEmpresa.FormattingEnabled = true;
        cboEmpresa.Location = new Point(426, 26);
        cboEmpresa.Name = "cboEmpresa";
        cboEmpresa.Size = new Size(200, 23);
        cboEmpresa.TabIndex = 7;
        cboEmpresa.SelectedIndexChanged += Filtro_Alterado;
        //
        // lblAcao
        //
        lblAcao.AutoSize = true;
        lblAcao.Location = new Point(638, 8);
        lblAcao.Name = "lblAcao";
        lblAcao.Size = new Size(35, 15);
        lblAcao.TabIndex = 8;
        lblAcao.Text = "&Ação";
        //
        // cboAcao
        //
        cboAcao.DropDownStyle = ComboBoxStyle.DropDownList;
        cboAcao.FormattingEnabled = true;
        cboAcao.Location = new Point(638, 26);
        cboAcao.Name = "cboAcao";
        cboAcao.Size = new Size(160, 23);
        cboAcao.TabIndex = 9;
        cboAcao.SelectedIndexChanged += Filtro_Alterado;
        //
        // lblPesquisa
        //
        lblPesquisa.AutoSize = true;
        lblPesquisa.Location = new Point(810, 8);
        lblPesquisa.Name = "lblPesquisa";
        lblPesquisa.Size = new Size(60, 15);
        lblPesquisa.TabIndex = 10;
        lblPesquisa.Text = "&Pesquisar";
        //
        // txtPesquisa
        //
        txtPesquisa.Location = new Point(810, 26);
        txtPesquisa.MaxLength = 100;
        txtPesquisa.Name = "txtPesquisa";
        txtPesquisa.PlaceholderText = "Login, registro ou detalhe";
        txtPesquisa.Size = new Size(178, 23);
        txtPesquisa.TabIndex = 11;
        txtPesquisa.TextChanged += Filtro_Alterado;
        //
        // pnlRodape
        //
        pnlRodape.Controls.Add(lblTotal);
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
        lblTotal.Size = new Size(100, 15);
        lblTotal.TabIndex = 0;
        lblTotal.Text = "0 registros";
        //
        // btnFechar
        //
        btnFechar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnFechar.Location = new Point(898, 8);
        btnFechar.Name = "btnFechar";
        btnFechar.Size = new Size(90, 27);
        btnFechar.TabIndex = 1;
        btnFechar.Text = "&Fechar";
        btnFechar.UseVisualStyleBackColor = true;
        btnFechar.Click += btnFechar_Click;
        //
        // FormConsultaAuditoria
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(dgvAuditoria);
        Controls.Add(pnlFiltros);
        Controls.Add(pnlRodape);
        ClientSize = new Size(1000, 520);
        Name = "FormConsultaAuditoria";
        Text = "Auditoria";
        ((System.ComponentModel.ISupportInitialize)dgvAuditoria).EndInit();
        pnlFiltros.ResumeLayout(false);
        pnlFiltros.PerformLayout();
        pnlRodape.ResumeLayout(false);
        pnlRodape.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private DataGridView dgvAuditoria;
    private Panel pnlFiltros;
    private Label lblPeriodo;
    private DateTimePicker dtpDe;
    private Label lblAte;
    private DateTimePicker dtpAte;
    private Label lblUsuario;
    private ComboBox cboUsuario;
    private Label lblEmpresa;
    private ComboBox cboEmpresa;
    private Label lblAcao;
    private ComboBox cboAcao;
    private Label lblPesquisa;
    private TextBox txtPesquisa;
    private Panel pnlRodape;
    private Label lblTotal;
    private Button btnFechar;
}
