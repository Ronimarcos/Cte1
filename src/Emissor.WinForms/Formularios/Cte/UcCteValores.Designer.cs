namespace Emissor.WinForms.Formularios.Cte;

partial class UcCteValores
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
        lblComponentes = new Label();
        dgvComponentes = new DataGridView();
        lblTotal = new Label();
        lblValorReceber = new Label();
        nudValorReceber = new NumericUpDown();
        btnReceberTotal = new Button();
        grpImpostos = new GroupBox();
        lblIcms = new Label();
        lblIbsCbs = new Label();
        lblAvisoImpostos = new Label();
        grpImpostos.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvComponentes).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudValorReceber).BeginInit();
        SuspendLayout();
        //
        // lblComponentes
        //
        lblComponentes.AutoSize = true;
        lblComponentes.Location = new Point(12, 12);
        lblComponentes.Name = "lblComponentes";
        lblComponentes.Size = new Size(207, 15);
        lblComponentes.TabIndex = 0;
        lblComponentes.Text = "Componentes do valor da prestação *";
        //
        // dgvComponentes
        //
        dgvComponentes.AllowUserToAddRows = true;
        dgvComponentes.AllowUserToDeleteRows = true;
        dgvComponentes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvComponentes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvComponentes.Location = new Point(12, 30);
        dgvComponentes.Name = "dgvComponentes";
        dgvComponentes.RowHeadersWidth = 24;
        dgvComponentes.Size = new Size(500, 180);
        dgvComponentes.TabIndex = 1;
        dgvComponentes.CellEndEdit += dgvComponentes_CellEndEdit;
        dgvComponentes.DataError += dgvComponentes_DataError;
        //
        // lblTotal
        //
        lblTotal.AutoSize = true;
        lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTotal.Location = new Point(12, 218);
        lblTotal.Name = "lblTotal";
        lblTotal.Size = new Size(180, 15);
        lblTotal.TabIndex = 2;
        lblTotal.Text = "Total da prestação: R$ 0,00";
        //
        // lblValorReceber
        //
        lblValorReceber.AutoSize = true;
        lblValorReceber.Location = new Point(12, 248);
        lblValorReceber.Name = "lblValorReceber";
        lblValorReceber.Size = new Size(99, 15);
        lblValorReceber.TabIndex = 3;
        lblValorReceber.Text = "Valor a receber *";
        //
        // nudValorReceber
        //
        nudValorReceber.DecimalPlaces = 2;
        nudValorReceber.Location = new Point(12, 266);
        nudValorReceber.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        nudValorReceber.Name = "nudValorReceber";
        nudValorReceber.Size = new Size(140, 23);
        nudValorReceber.TabIndex = 4;
        nudValorReceber.TextAlign = HorizontalAlignment.Right;
        nudValorReceber.ThousandsSeparator = true;
        //
        // btnReceberTotal
        //
        btnReceberTotal.Location = new Point(160, 264);
        btnReceberTotal.Name = "btnReceberTotal";
        btnReceberTotal.Size = new Size(120, 27);
        btnReceberTotal.TabIndex = 5;
        btnReceberTotal.Text = "&Usar o total";
        btnReceberTotal.UseVisualStyleBackColor = true;
        btnReceberTotal.Click += btnReceberTotal_Click;
        //
        // grpImpostos
        //
        grpImpostos.Controls.Add(lblIcms);
        grpImpostos.Controls.Add(lblIbsCbs);
        grpImpostos.Controls.Add(lblAvisoImpostos);
        grpImpostos.Location = new Point(12, 306);
        grpImpostos.Name = "grpImpostos";
        grpImpostos.Size = new Size(500, 104);
        grpImpostos.TabIndex = 6;
        grpImpostos.Text = "Impostos";
        //
        // lblIcms
        //
        lblIcms.AutoSize = true;
        lblIcms.Location = new Point(12, 24);
        lblIcms.Name = "lblIcms";
        lblIcms.Size = new Size(330, 15);
        lblIcms.TabIndex = 0;
        lblIcms.Text = "ICMS: Simples Nacional, grupo ICMSSN (CST 90, indSN 1).";
        //
        // lblIbsCbs
        //
        lblIbsCbs.AutoSize = true;
        lblIbsCbs.Location = new Point(12, 48);
        lblIbsCbs.Name = "lblIbsCbs";
        lblIbsCbs.Size = new Size(352, 15);
        lblIbsCbs.TabIndex = 1;
        lblIbsCbs.Text = "IBS/CBS: a calcular na implementação, conforme a NT 2026.005.";
        //
        // lblAvisoImpostos
        //
        lblAvisoImpostos.AutoSize = true;
        lblAvisoImpostos.ForeColor = SystemColors.GrayText;
        lblAvisoImpostos.Location = new Point(12, 72);
        lblAvisoImpostos.Name = "lblAvisoImpostos";
        lblAvisoImpostos.Size = new Size(262, 15);
        lblAvisoImpostos.TabIndex = 2;
        lblAvisoImpostos.Text = "No protótipo os impostos são só informativos.";
        //
        // UcCteValores
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lblComponentes);
        Controls.Add(dgvComponentes);
        Controls.Add(lblTotal);
        Controls.Add(lblValorReceber);
        Controls.Add(nudValorReceber);
        Controls.Add(btnReceberTotal);
        Controls.Add(grpImpostos);
        Name = "UcCteValores";
        Size = new Size(680, 480);
        ((System.ComponentModel.ISupportInitialize)dgvComponentes).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudValorReceber).EndInit();
        grpImpostos.ResumeLayout(false);
        grpImpostos.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblComponentes;
    private DataGridView dgvComponentes;
    private Label lblTotal;
    private Label lblValorReceber;
    private NumericUpDown nudValorReceber;
    private Button btnReceberTotal;
    private GroupBox grpImpostos;
    private Label lblIcms;
    private Label lblIbsCbs;
    private Label lblAvisoImpostos;
}
