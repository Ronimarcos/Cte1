namespace Emissor.WinForms.Formularios.Eventos;

partial class FormCartaCorrecao
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
        lblCorrecoes = new Label();
        dgvCorrecoes = new DataGridView();
        lblAviso = new Label();
        btnGerar = new Button();
        btnFechar = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvCorrecoes).BeginInit();
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
        // lblCorrecoes
        //
        lblCorrecoes.AutoSize = true;
        lblCorrecoes.Location = new Point(12, 40);
        lblCorrecoes.Name = "lblCorrecoes";
        lblCorrecoes.Size = new Size(610, 15);
        lblCorrecoes.TabIndex = 1;
        lblCorrecoes.Text = "&Correções: grupo e campo como no leiaute do CT-e (ex.: compl / xObs); item só em grupo que se repete";
        //
        // dgvCorrecoes
        //
        dgvCorrecoes.AllowUserToAddRows = true;
        dgvCorrecoes.AllowUserToDeleteRows = true;
        dgvCorrecoes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCorrecoes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvCorrecoes.Location = new Point(12, 58);
        dgvCorrecoes.Name = "dgvCorrecoes";
        dgvCorrecoes.RowHeadersWidth = 24;
        dgvCorrecoes.Size = new Size(616, 200);
        dgvCorrecoes.TabIndex = 2;
        //
        // lblAviso
        //
        lblAviso.ForeColor = SystemColors.GrayText;
        lblAviso.Location = new Point(12, 264);
        lblAviso.Name = "lblAviso";
        lblAviso.Size = new Size(616, 45);
        lblAviso.TabIndex = 3;
        lblAviso.Text = "";
        //
        // btnGerar
        //
        btnGerar.Location = new Point(380, 316);
        btnGerar.Name = "btnGerar";
        btnGerar.Size = new Size(150, 27);
        btnGerar.TabIndex = 4;
        btnGerar.Text = "&Gerar XML de teste";
        btnGerar.UseVisualStyleBackColor = true;
        btnGerar.Click += btnGerar_Click;
        //
        // btnFechar
        //
        btnFechar.DialogResult = DialogResult.Cancel;
        btnFechar.Location = new Point(536, 316);
        btnFechar.Name = "btnFechar";
        btnFechar.Size = new Size(92, 27);
        btnFechar.TabIndex = 5;
        btnFechar.Text = "&Fechar";
        btnFechar.UseVisualStyleBackColor = true;
        //
        // FormCartaCorrecao
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblDocumento);
        Controls.Add(lblCorrecoes);
        Controls.Add(dgvCorrecoes);
        Controls.Add(lblAviso);
        Controls.Add(btnGerar);
        Controls.Add(btnFechar);
        ClientSize = new Size(640, 356);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        CancelButton = btnFechar;
        Name = "FormCartaCorrecao";
        Text = "Carta de correção";
        ((System.ComponentModel.ISupportInitialize)dgvCorrecoes).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblDocumento;
    private Label lblCorrecoes;
    private DataGridView dgvCorrecoes;
    private Label lblAviso;
    private Button btnGerar;
    private Button btnFechar;
}
