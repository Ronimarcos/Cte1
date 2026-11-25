namespace Emissor.WinForms.Formularios.Cadastros;

partial class FormSelecaoCertificadoWindows
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
        dgvCertificados = new DataGridView();
        lblVazio = new Label();
        btnOk = new Button();
        btnCancelar = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvCertificados).BeginInit();
        SuspendLayout();
        //
        // lblInstrucao
        //
        lblInstrucao.AutoSize = true;
        lblInstrucao.Location = new Point(12, 12);
        lblInstrucao.Name = "lblInstrucao";
        lblInstrucao.Size = new Size(480, 15);
        lblInstrucao.TabIndex = 0;
        lblInstrucao.Text = "Certificados com chave privada instalados no Windows (token, cartão ou A1 instalado):";
        //
        // dgvCertificados
        //
        dgvCertificados.AllowUserToAddRows = false;
        dgvCertificados.AllowUserToDeleteRows = false;
        dgvCertificados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCertificados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvCertificados.Location = new Point(12, 32);
        dgvCertificados.MultiSelect = false;
        dgvCertificados.Name = "dgvCertificados";
        dgvCertificados.ReadOnly = true;
        dgvCertificados.RowHeadersVisible = false;
        dgvCertificados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCertificados.Size = new Size(776, 300);
        dgvCertificados.TabIndex = 1;
        dgvCertificados.CellDoubleClick += dgvCertificados_CellDoubleClick;
        //
        // lblVazio
        //
        lblVazio.AutoSize = true;
        lblVazio.ForeColor = Color.Firebrick;
        lblVazio.Location = new Point(12, 342);
        lblVazio.Name = "lblVazio";
        lblVazio.Size = new Size(540, 15);
        lblVazio.TabIndex = 2;
        lblVazio.Text = "Nenhum certificado com chave privada foi encontrado. Conecte o token ou cartão e confira o driver.";
        lblVazio.Visible = false;
        //
        // btnOk
        //
        btnOk.Location = new Point(606, 368);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(88, 27);
        btnOk.TabIndex = 3;
        btnOk.Text = "&OK";
        btnOk.UseVisualStyleBackColor = true;
        btnOk.Click += btnOk_Click;
        //
        // btnCancelar
        //
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(700, 368);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(88, 27);
        btnCancelar.TabIndex = 4;
        btnCancelar.Text = "&Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        //
        // FormSelecaoCertificadoWindows
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblInstrucao);
        Controls.Add(dgvCertificados);
        Controls.Add(lblVazio);
        Controls.Add(btnOk);
        Controls.Add(btnCancelar);
        AcceptButton = btnOk;
        CancelButton = btnCancelar;
        ClientSize = new Size(800, 407);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormSelecaoCertificadoWindows";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Escolher certificado instalado no Windows";
        ((System.ComponentModel.ISupportInitialize)dgvCertificados).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblInstrucao;
    private DataGridView dgvCertificados;
    private Label lblVazio;
    private Button btnOk;
    private Button btnCancelar;
}
