namespace Emissor.WinForms.Formularios;

partial class FormSelecaoEmpresa
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
        dgvEmpresas = new DataGridView();
        colRazaoSocial = new DataGridViewTextBoxColumn();
        colCnpj = new DataGridViewTextBoxColumn();
        colUf = new DataGridViewTextBoxColumn();
        colAmbiente = new DataGridViewTextBoxColumn();
        pnlBotoes = new Panel();
        btnOk = new Button();
        btnCancelar = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvEmpresas).BeginInit();
        pnlBotoes.SuspendLayout();
        SuspendLayout();
        //
        // lblInstrucao
        //
        lblInstrucao.Dock = DockStyle.Top;
        lblInstrucao.Location = new Point(0, 0);
        lblInstrucao.Name = "lblInstrucao";
        lblInstrucao.Padding = new Padding(8, 0, 0, 0);
        lblInstrucao.Size = new Size(640, 36);
        lblInstrucao.TabIndex = 0;
        lblInstrucao.Text = "Selecione a empresa com que deseja trabalhar:";
        lblInstrucao.TextAlign = ContentAlignment.MiddleLeft;
        //
        // dgvEmpresas
        //
        dgvEmpresas.AllowUserToAddRows = false;
        dgvEmpresas.AllowUserToDeleteRows = false;
        dgvEmpresas.AllowUserToResizeRows = false;
        dgvEmpresas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvEmpresas.BackgroundColor = SystemColors.Window;
        dgvEmpresas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvEmpresas.Columns.AddRange(new DataGridViewColumn[] { colRazaoSocial, colCnpj, colUf, colAmbiente });
        dgvEmpresas.Dock = DockStyle.Fill;
        dgvEmpresas.Location = new Point(0, 36);
        dgvEmpresas.MultiSelect = false;
        dgvEmpresas.Name = "dgvEmpresas";
        dgvEmpresas.ReadOnly = true;
        dgvEmpresas.RowHeadersVisible = false;
        dgvEmpresas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvEmpresas.Size = new Size(640, 276);
        dgvEmpresas.TabIndex = 1;
        dgvEmpresas.CellDoubleClick += dgvEmpresas_CellDoubleClick;
        //
        // colRazaoSocial
        //
        colRazaoSocial.DataPropertyName = "RazaoSocial";
        colRazaoSocial.FillWeight = 50F;
        colRazaoSocial.HeaderText = "Razão social";
        colRazaoSocial.Name = "colRazaoSocial";
        colRazaoSocial.ReadOnly = true;
        //
        // colCnpj
        //
        colCnpj.DataPropertyName = "Cnpj";
        colCnpj.FillWeight = 25F;
        colCnpj.HeaderText = "CNPJ";
        colCnpj.Name = "colCnpj";
        colCnpj.ReadOnly = true;
        //
        // colUf
        //
        colUf.DataPropertyName = "Uf";
        colUf.FillWeight = 8F;
        colUf.HeaderText = "UF";
        colUf.Name = "colUf";
        colUf.ReadOnly = true;
        //
        // colAmbiente
        //
        colAmbiente.DataPropertyName = "AmbienteDescricao";
        colAmbiente.FillWeight = 17F;
        colAmbiente.HeaderText = "Ambiente";
        colAmbiente.Name = "colAmbiente";
        colAmbiente.ReadOnly = true;
        //
        // pnlBotoes
        //
        pnlBotoes.Controls.Add(btnOk);
        pnlBotoes.Controls.Add(btnCancelar);
        pnlBotoes.Dock = DockStyle.Bottom;
        pnlBotoes.Location = new Point(0, 312);
        pnlBotoes.Name = "pnlBotoes";
        pnlBotoes.Size = new Size(640, 48);
        pnlBotoes.TabIndex = 2;
        //
        // btnOk
        //
        btnOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnOk.Location = new Point(446, 12);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(88, 27);
        btnOk.TabIndex = 0;
        btnOk.Text = "&OK";
        btnOk.UseVisualStyleBackColor = true;
        btnOk.Click += btnOk_Click;
        //
        // btnCancelar
        //
        btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(540, 12);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(88, 27);
        btnCancelar.TabIndex = 1;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormSelecaoEmpresa
        //
        AcceptButton = btnOk;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(640, 360);
        Controls.Add(dgvEmpresas);
        Controls.Add(pnlBotoes);
        Controls.Add(lblInstrucao);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormSelecaoEmpresa";
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Emissor CT-e / MDF-e — Seleção de empresa";
        Shown += FormSelecaoEmpresa_Shown;
        ((System.ComponentModel.ISupportInitialize)dgvEmpresas).EndInit();
        pnlBotoes.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Label lblInstrucao;
    private DataGridView dgvEmpresas;
    private DataGridViewTextBoxColumn colRazaoSocial;
    private DataGridViewTextBoxColumn colCnpj;
    private DataGridViewTextBoxColumn colUf;
    private DataGridViewTextBoxColumn colAmbiente;
    private Panel pnlBotoes;
    private Button btnOk;
    private Button btnCancelar;
}
