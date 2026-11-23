namespace Emissor.WinForms.Formularios.Mdfe;

partial class UcMdfeVeiculos
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
        lblTracao = new Label();
        cboTracao = new ComboBox();
        lblReboques = new Label();
        cboReboque1 = new ComboBox();
        cboReboque2 = new ComboBox();
        cboReboque3 = new ComboBox();
        lblCondutores = new Label();
        clbCondutores = new CheckedListBox();
        lblRntrc = new Label();
        grpCiot = new GroupBox();
        lblAvisoCiot = new Label();
        dgvCiot = new DataGridView();
        grpCiot.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCiot).BeginInit();
        SuspendLayout();
        //
        // lblTracao
        //
        lblTracao.AutoSize = true;
        lblTracao.Location = new Point(12, 12);
        lblTracao.Name = "lblTracao";
        lblTracao.Size = new Size(108, 15);
        lblTracao.TabIndex = 0;
        lblTracao.Text = "Veículo de tração *";
        //
        // cboTracao
        //
        cboTracao.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTracao.FormattingEnabled = true;
        cboTracao.Location = new Point(12, 30);
        cboTracao.Name = "cboTracao";
        cboTracao.Size = new Size(420, 23);
        cboTracao.TabIndex = 1;
        //
        // lblReboques
        //
        lblReboques.AutoSize = true;
        lblReboques.Location = new Point(12, 62);
        lblReboques.Name = "lblReboques";
        lblReboques.Size = new Size(97, 15);
        lblReboques.TabIndex = 2;
        lblReboques.Text = "Reboques (até 3)";
        //
        // cboReboque1
        //
        cboReboque1.DropDownStyle = ComboBoxStyle.DropDownList;
        cboReboque1.FormattingEnabled = true;
        cboReboque1.Location = new Point(12, 80);
        cboReboque1.Name = "cboReboque1";
        cboReboque1.Size = new Size(210, 23);
        cboReboque1.TabIndex = 3;
        //
        // cboReboque2
        //
        cboReboque2.DropDownStyle = ComboBoxStyle.DropDownList;
        cboReboque2.FormattingEnabled = true;
        cboReboque2.Location = new Point(228, 80);
        cboReboque2.Name = "cboReboque2";
        cboReboque2.Size = new Size(210, 23);
        cboReboque2.TabIndex = 4;
        //
        // cboReboque3
        //
        cboReboque3.DropDownStyle = ComboBoxStyle.DropDownList;
        cboReboque3.FormattingEnabled = true;
        cboReboque3.Location = new Point(444, 80);
        cboReboque3.Name = "cboReboque3";
        cboReboque3.Size = new Size(210, 23);
        cboReboque3.TabIndex = 5;
        //
        // lblCondutores
        //
        lblCondutores.AutoSize = true;
        lblCondutores.Location = new Point(12, 112);
        lblCondutores.Name = "lblCondutores";
        lblCondutores.Size = new Size(185, 15);
        lblCondutores.TabIndex = 6;
        lblCondutores.Text = "Condutores * (marque de 1 a 10)";
        //
        // clbCondutores
        //
        clbCondutores.CheckOnClick = true;
        clbCondutores.FormattingEnabled = true;
        clbCondutores.IntegralHeight = false;
        clbCondutores.Location = new Point(12, 130);
        clbCondutores.Name = "clbCondutores";
        clbCondutores.Size = new Size(420, 110);
        clbCondutores.TabIndex = 7;
        //
        // lblRntrc
        //
        lblRntrc.Location = new Point(444, 130);
        lblRntrc.Name = "lblRntrc";
        lblRntrc.Size = new Size(210, 40);
        lblRntrc.TabIndex = 8;
        lblRntrc.Text = "RNTRC da empresa:";
        //
        // grpCiot
        //
        grpCiot.Controls.Add(lblAvisoCiot);
        grpCiot.Controls.Add(dgvCiot);
        grpCiot.Location = new Point(12, 252);
        grpCiot.Name = "grpCiot";
        grpCiot.Size = new Size(656, 190);
        grpCiot.TabIndex = 9;
        grpCiot.Text = "CIOT";
        //
        // lblAvisoCiot
        //
        lblAvisoCiot.Location = new Point(12, 20);
        lblAvisoCiot.Name = "lblAvisoCiot";
        lblAvisoCiot.Size = new Size(632, 34);
        lblAvisoCiot.TabIndex = 0;
        lblAvisoCiot.Text = "Aviso do CIOT";
        //
        // dgvCiot
        //
        dgvCiot.AllowUserToAddRows = true;
        dgvCiot.AllowUserToDeleteRows = true;
        dgvCiot.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCiot.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvCiot.Location = new Point(12, 58);
        dgvCiot.Name = "dgvCiot";
        dgvCiot.RowHeadersWidth = 24;
        dgvCiot.Size = new Size(632, 120);
        dgvCiot.TabIndex = 1;
        dgvCiot.DataError += dgvCiot_DataError;
        //
        // UcMdfeVeiculos
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblTracao);
        Controls.Add(cboTracao);
        Controls.Add(lblReboques);
        Controls.Add(cboReboque1);
        Controls.Add(cboReboque2);
        Controls.Add(cboReboque3);
        Controls.Add(lblCondutores);
        Controls.Add(clbCondutores);
        Controls.Add(lblRntrc);
        Controls.Add(grpCiot);
        Name = "UcMdfeVeiculos";
        Size = new Size(680, 484);
        ((System.ComponentModel.ISupportInitialize)dgvCiot).EndInit();
        grpCiot.ResumeLayout(false);
        grpCiot.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTracao;
    private ComboBox cboTracao;
    private Label lblReboques;
    private ComboBox cboReboque1;
    private ComboBox cboReboque2;
    private ComboBox cboReboque3;
    private Label lblCondutores;
    private CheckedListBox clbCondutores;
    private Label lblRntrc;
    private GroupBox grpCiot;
    private Label lblAvisoCiot;
    private DataGridView dgvCiot;
}
