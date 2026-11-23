namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormListaCadastro
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
        pnlTopo = new Panel();
        lblPesquisa = new Label();
        txtPesquisa = new TextBox();
        btnNovo = new Button();
        btnEditar = new Button();
        btnExcluir = new Button();
        dgvRegistros = new DataGridView();
        pnlRodape = new Panel();
        lblTotal = new Label();
        btnFechar = new Button();
        pnlTopo.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvRegistros).BeginInit();
        pnlRodape.SuspendLayout();
        SuspendLayout();
        //
        // pnlTopo
        //
        pnlTopo.Controls.Add(lblPesquisa);
        pnlTopo.Controls.Add(txtPesquisa);
        pnlTopo.Controls.Add(btnNovo);
        pnlTopo.Controls.Add(btnEditar);
        pnlTopo.Controls.Add(btnExcluir);
        pnlTopo.Dock = DockStyle.Top;
        pnlTopo.Location = new Point(0, 0);
        pnlTopo.Name = "pnlTopo";
        pnlTopo.Size = new Size(900, 48);
        pnlTopo.TabIndex = 0;
        //
        // lblPesquisa
        //
        lblPesquisa.AutoSize = true;
        lblPesquisa.Location = new Point(12, 16);
        lblPesquisa.Name = "lblPesquisa";
        lblPesquisa.Size = new Size(60, 15);
        lblPesquisa.TabIndex = 0;
        lblPesquisa.Text = "&Pesquisar:";
        //
        // txtPesquisa
        //
        txtPesquisa.Location = new Point(80, 12);
        txtPesquisa.Name = "txtPesquisa";
        txtPesquisa.PlaceholderText = "Nome, documento, placa...";
        txtPesquisa.Size = new Size(320, 23);
        txtPesquisa.TabIndex = 1;
        txtPesquisa.TextChanged += txtPesquisa_TextChanged;
        //
        // btnNovo
        //
        btnNovo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnNovo.Location = new Point(612, 11);
        btnNovo.Name = "btnNovo";
        btnNovo.Size = new Size(88, 27);
        btnNovo.TabIndex = 2;
        btnNovo.Text = "&Novo";
        btnNovo.UseVisualStyleBackColor = true;
        btnNovo.Click += btnNovo_Click;
        //
        // btnEditar
        //
        btnEditar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnEditar.Location = new Point(706, 11);
        btnEditar.Name = "btnEditar";
        btnEditar.Size = new Size(88, 27);
        btnEditar.TabIndex = 3;
        btnEditar.Text = "&Editar";
        btnEditar.UseVisualStyleBackColor = true;
        btnEditar.Click += btnEditar_Click;
        //
        // btnExcluir
        //
        btnExcluir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExcluir.Location = new Point(800, 11);
        btnExcluir.Name = "btnExcluir";
        btnExcluir.Size = new Size(88, 27);
        btnExcluir.TabIndex = 4;
        btnExcluir.Text = "E&xcluir";
        btnExcluir.UseVisualStyleBackColor = true;
        btnExcluir.Click += btnExcluir_Click;
        //
        // dgvRegistros
        //
        dgvRegistros.AllowUserToAddRows = false;
        dgvRegistros.AllowUserToDeleteRows = false;
        dgvRegistros.AllowUserToResizeRows = false;
        dgvRegistros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvRegistros.BackgroundColor = SystemColors.Window;
        dgvRegistros.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvRegistros.Dock = DockStyle.Fill;
        dgvRegistros.Location = new Point(0, 48);
        dgvRegistros.MultiSelect = false;
        dgvRegistros.Name = "dgvRegistros";
        dgvRegistros.ReadOnly = true;
        dgvRegistros.RowHeadersVisible = false;
        dgvRegistros.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRegistros.Size = new Size(900, 412);
        dgvRegistros.TabIndex = 1;
        dgvRegistros.CellDoubleClick += dgvRegistros_CellDoubleClick;
        dgvRegistros.SelectionChanged += dgvRegistros_SelectionChanged;
        dgvRegistros.KeyDown += dgvRegistros_KeyDown;
        //
        // pnlRodape
        //
        pnlRodape.Controls.Add(lblTotal);
        pnlRodape.Controls.Add(btnFechar);
        pnlRodape.Dock = DockStyle.Bottom;
        pnlRodape.Location = new Point(0, 460);
        pnlRodape.Name = "pnlRodape";
        pnlRodape.Size = new Size(900, 40);
        pnlRodape.TabIndex = 2;
        //
        // lblTotal
        //
        lblTotal.AutoSize = true;
        lblTotal.Location = new Point(12, 12);
        lblTotal.Name = "lblTotal";
        lblTotal.Size = new Size(64, 15);
        lblTotal.TabIndex = 0;
        lblTotal.Text = "0 registros";
        //
        // btnFechar
        //
        btnFechar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnFechar.Location = new Point(800, 7);
        btnFechar.Name = "btnFechar";
        btnFechar.Size = new Size(88, 27);
        btnFechar.TabIndex = 1;
        btnFechar.Text = "&Fechar";
        btnFechar.UseVisualStyleBackColor = true;
        btnFechar.Click += btnFechar_Click;
        //
        // FormListaCadastro
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 500);
        Controls.Add(dgvRegistros);
        Controls.Add(pnlRodape);
        Controls.Add(pnlTopo);
        MinimumSize = new Size(640, 320);
        Name = "FormListaCadastro";
        Text = "Cadastro";
        pnlTopo.ResumeLayout(false);
        pnlTopo.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvRegistros).EndInit();
        pnlRodape.ResumeLayout(false);
        pnlRodape.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTopo;
    private Label lblPesquisa;
    private TextBox txtPesquisa;
    private Button btnNovo;
    private Button btnEditar;
    private Button btnExcluir;
    private DataGridView dgvRegistros;
    private Panel pnlRodape;
    private Label lblTotal;
    private Button btnFechar;
}
