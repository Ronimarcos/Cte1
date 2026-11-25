namespace Emissor.WinForms.Formularios;

partial class FormPrincipal
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
        mnuPrincipal = new MenuStrip();
        mnuCadastros = new ToolStripMenuItem();
        mnuCadastrosEmpresas = new ToolStripMenuItem();
        mnuCadastrosCertificados = new ToolStripMenuItem();
        mnuCadastrosSeries = new ToolStripMenuItem();
        mnuCadastrosSeparador1 = new ToolStripSeparator();
        mnuCadastrosParticipantes = new ToolStripMenuItem();
        mnuCadastrosSeparador2 = new ToolStripSeparator();
        mnuCadastrosVeiculos = new ToolStripMenuItem();
        mnuCadastrosProprietarios = new ToolStripMenuItem();
        mnuCadastrosMotoristas = new ToolStripMenuItem();
        mnuCadastrosApolices = new ToolStripMenuItem();
        mnuCte = new ToolStripMenuItem();
        mnuCteEmitir = new ToolStripMenuItem();
        mnuCteConsultar = new ToolStripMenuItem();
        mnuMdfe = new ToolStripMenuItem();
        mnuMdfeEmitir = new ToolStripMenuItem();
        mnuMdfeConsultar = new ToolStripMenuItem();
        mnuSistema = new ToolStripMenuItem();
        mnuSistemaStatusSefaz = new ToolStripMenuItem();
        mnuSistemaUsuarios = new ToolStripMenuItem();
        mnuSistemaAuditoria = new ToolStripMenuItem();
        mnuSistemaSeparador2 = new ToolStripSeparator();
        mnuSistemaTrocarSenha = new ToolStripMenuItem();
        mnuSistemaTrocarUsuario = new ToolStripMenuItem();
        mnuSistemaTrocarEmpresa = new ToolStripMenuItem();
        mnuSistemaSeparador1 = new ToolStripSeparator();
        mnuSistemaSair = new ToolStripMenuItem();
        stsPrincipal = new StatusStrip();
        lblStatusEmpresa = new ToolStripStatusLabel();
        lblStatusUsuario = new ToolStripStatusLabel();
        lblStatusCnpj = new ToolStripStatusLabel();
        lblStatusAmbiente = new ToolStripStatusLabel();
        mnuPrincipal.SuspendLayout();
        stsPrincipal.SuspendLayout();
        SuspendLayout();
        //
        // mnuPrincipal
        //
        mnuPrincipal.Items.AddRange(new ToolStripItem[] { mnuCadastros, mnuCte, mnuMdfe, mnuSistema });
        mnuPrincipal.Location = new Point(0, 0);
        mnuPrincipal.Name = "mnuPrincipal";
        mnuPrincipal.Size = new Size(1024, 24);
        mnuPrincipal.TabIndex = 0;
        //
        // mnuCadastros
        //
        mnuCadastros.DropDownItems.AddRange(new ToolStripItem[] { mnuCadastrosEmpresas, mnuCadastrosCertificados, mnuCadastrosSeries, mnuCadastrosSeparador1, mnuCadastrosParticipantes, mnuCadastrosSeparador2, mnuCadastrosVeiculos, mnuCadastrosProprietarios, mnuCadastrosMotoristas, mnuCadastrosApolices });
        mnuCadastros.Name = "mnuCadastros";
        mnuCadastros.Size = new Size(71, 20);
        mnuCadastros.Text = "&Cadastros";
        //
        // mnuCadastrosEmpresas
        //
        mnuCadastrosEmpresas.Name = "mnuCadastrosEmpresas";
        mnuCadastrosEmpresas.Size = new Size(220, 22);
        mnuCadastrosEmpresas.Tag = "Cadastro de empresas";
        mnuCadastrosEmpresas.Text = "&Empresas";
        mnuCadastrosEmpresas.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCadastrosCertificados
        //
        mnuCadastrosCertificados.Name = "mnuCadastrosCertificados";
        mnuCadastrosCertificados.Size = new Size(220, 22);
        mnuCadastrosCertificados.Tag = "Cadastro de certificados digitais";
        mnuCadastrosCertificados.Text = "&Certificados digitais";
        mnuCadastrosCertificados.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCadastrosSeries
        //
        mnuCadastrosSeries.Name = "mnuCadastrosSeries";
        mnuCadastrosSeries.Size = new Size(220, 22);
        mnuCadastrosSeries.Tag = "Cadastro de séries de documentos";
        mnuCadastrosSeries.Text = "&Séries de documentos";
        mnuCadastrosSeries.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCadastrosSeparador1
        //
        mnuCadastrosSeparador1.Name = "mnuCadastrosSeparador1";
        mnuCadastrosSeparador1.Size = new Size(217, 6);
        //
        // mnuCadastrosParticipantes
        //
        mnuCadastrosParticipantes.Name = "mnuCadastrosParticipantes";
        mnuCadastrosParticipantes.Size = new Size(220, 22);
        mnuCadastrosParticipantes.Tag = "Cadastro de participantes";
        mnuCadastrosParticipantes.Text = "&Participantes";
        mnuCadastrosParticipantes.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCadastrosSeparador2
        //
        mnuCadastrosSeparador2.Name = "mnuCadastrosSeparador2";
        mnuCadastrosSeparador2.Size = new Size(217, 6);
        //
        // mnuCadastrosVeiculos
        //
        mnuCadastrosVeiculos.Name = "mnuCadastrosVeiculos";
        mnuCadastrosVeiculos.Size = new Size(220, 22);
        mnuCadastrosVeiculos.Tag = "Cadastro de veículos";
        mnuCadastrosVeiculos.Text = "&Veículos";
        mnuCadastrosVeiculos.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCadastrosProprietarios
        //
        mnuCadastrosProprietarios.Name = "mnuCadastrosProprietarios";
        mnuCadastrosProprietarios.Size = new Size(220, 22);
        mnuCadastrosProprietarios.Tag = "Cadastro de proprietários de veículos";
        mnuCadastrosProprietarios.Text = "Proprietários de veíc&ulos";
        mnuCadastrosProprietarios.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCadastrosMotoristas
        //
        mnuCadastrosMotoristas.Name = "mnuCadastrosMotoristas";
        mnuCadastrosMotoristas.Size = new Size(220, 22);
        mnuCadastrosMotoristas.Tag = "Cadastro de motoristas";
        mnuCadastrosMotoristas.Text = "&Motoristas";
        mnuCadastrosMotoristas.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCadastrosApolices
        //
        mnuCadastrosApolices.Name = "mnuCadastrosApolices";
        mnuCadastrosApolices.Size = new Size(220, 22);
        mnuCadastrosApolices.Tag = "Cadastro de apólices de seguro";
        mnuCadastrosApolices.Text = "&Apólices de seguro";
        mnuCadastrosApolices.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCte
        //
        mnuCte.DropDownItems.AddRange(new ToolStripItem[] { mnuCteEmitir, mnuCteConsultar });
        mnuCte.Name = "mnuCte";
        mnuCte.Size = new Size(46, 20);
        mnuCte.Text = "C&T-e";
        //
        // mnuCteEmitir
        //
        mnuCteEmitir.Name = "mnuCteEmitir";
        mnuCteEmitir.Size = new Size(180, 22);
        mnuCteEmitir.Tag = "Emissão de CT-e";
        mnuCteEmitir.Text = "&Emitir CT-e";
        mnuCteEmitir.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuCteConsultar
        //
        mnuCteConsultar.Name = "mnuCteConsultar";
        mnuCteConsultar.Size = new Size(180, 22);
        mnuCteConsultar.Tag = "Consulta de CT-e";
        mnuCteConsultar.Text = "&Consultar CT-e";
        mnuCteConsultar.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuMdfe
        //
        mnuMdfe.DropDownItems.AddRange(new ToolStripItem[] { mnuMdfeEmitir, mnuMdfeConsultar });
        mnuMdfe.Name = "mnuMdfe";
        mnuMdfe.Size = new Size(54, 20);
        mnuMdfe.Text = "&MDF-e";
        //
        // mnuMdfeEmitir
        //
        mnuMdfeEmitir.Name = "mnuMdfeEmitir";
        mnuMdfeEmitir.Size = new Size(180, 22);
        mnuMdfeEmitir.Tag = "Emissão de MDF-e";
        mnuMdfeEmitir.Text = "&Emitir MDF-e";
        mnuMdfeEmitir.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuMdfeConsultar
        //
        mnuMdfeConsultar.Name = "mnuMdfeConsultar";
        mnuMdfeConsultar.Size = new Size(180, 22);
        mnuMdfeConsultar.Tag = "Consulta de MDF-e";
        mnuMdfeConsultar.Text = "&Consultar MDF-e";
        mnuMdfeConsultar.Click += AbrirTelaEmConstrucao_Click;
        //
        // mnuSistema
        //
        mnuSistema.DropDownItems.AddRange(new ToolStripItem[] { mnuSistemaStatusSefaz, mnuSistemaUsuarios, mnuSistemaAuditoria, mnuSistemaSeparador2, mnuSistemaTrocarEmpresa, mnuSistemaTrocarSenha, mnuSistemaTrocarUsuario, mnuSistemaSeparador1, mnuSistemaSair });
        mnuSistema.Name = "mnuSistema";
        mnuSistema.Size = new Size(60, 20);
        mnuSistema.Text = "&Sistema";
        //
        // mnuSistemaStatusSefaz
        //
        mnuSistemaStatusSefaz.Name = "mnuSistemaStatusSefaz";
        mnuSistemaStatusSefaz.Size = new Size(250, 22);
        mnuSistemaStatusSefaz.Text = "Status da SEFA&Z (homologação)";
        mnuSistemaStatusSefaz.Click += mnuSistemaStatusSefaz_Click;
        //
        // mnuSistemaUsuarios
        //
        mnuSistemaUsuarios.Name = "mnuSistemaUsuarios";
        mnuSistemaUsuarios.Size = new Size(250, 22);
        mnuSistemaUsuarios.Text = "&Usuários";
        mnuSistemaUsuarios.Click += mnuSistemaUsuarios_Click;
        //
        // mnuSistemaAuditoria
        //
        mnuSistemaAuditoria.Name = "mnuSistemaAuditoria";
        mnuSistemaAuditoria.Size = new Size(250, 22);
        mnuSistemaAuditoria.Text = "&Auditoria";
        mnuSistemaAuditoria.Click += mnuSistemaAuditoria_Click;
        //
        // mnuSistemaSeparador2
        //
        mnuSistemaSeparador2.Name = "mnuSistemaSeparador2";
        mnuSistemaSeparador2.Size = new Size(247, 6);
        //
        // mnuSistemaTrocarSenha
        //
        mnuSistemaTrocarSenha.Name = "mnuSistemaTrocarSenha";
        mnuSistemaTrocarSenha.Size = new Size(250, 22);
        mnuSistemaTrocarSenha.Text = "Trocar s&enha";
        mnuSistemaTrocarSenha.Click += mnuSistemaTrocarSenha_Click;
        //
        // mnuSistemaTrocarUsuario
        //
        mnuSistemaTrocarUsuario.Name = "mnuSistemaTrocarUsuario";
        mnuSistemaTrocarUsuario.Size = new Size(250, 22);
        mnuSistemaTrocarUsuario.Text = "Trocar us&uário";
        mnuSistemaTrocarUsuario.Click += mnuSistemaTrocarUsuario_Click;
        //
        // mnuSistemaTrocarEmpresa
        //
        mnuSistemaTrocarEmpresa.Name = "mnuSistemaTrocarEmpresa";
        mnuSistemaTrocarEmpresa.Size = new Size(180, 22);
        mnuSistemaTrocarEmpresa.Text = "&Trocar empresa";
        mnuSistemaTrocarEmpresa.Click += mnuSistemaTrocarEmpresa_Click;
        //
        // mnuSistemaSeparador1
        //
        mnuSistemaSeparador1.Name = "mnuSistemaSeparador1";
        mnuSistemaSeparador1.Size = new Size(177, 6);
        //
        // mnuSistemaSair
        //
        mnuSistemaSair.Name = "mnuSistemaSair";
        mnuSistemaSair.Size = new Size(180, 22);
        mnuSistemaSair.Text = "Sai&r";
        mnuSistemaSair.Click += mnuSistemaSair_Click;
        //
        // stsPrincipal
        //
        stsPrincipal.Items.AddRange(new ToolStripItem[] { lblStatusEmpresa, lblStatusUsuario, lblStatusCnpj, lblStatusAmbiente });
        stsPrincipal.Location = new Point(0, 746);
        stsPrincipal.Name = "stsPrincipal";
        stsPrincipal.Size = new Size(1024, 22);
        stsPrincipal.TabIndex = 1;
        //
        // lblStatusEmpresa
        //
        lblStatusEmpresa.Name = "lblStatusEmpresa";
        lblStatusEmpresa.Size = new Size(809, 17);
        lblStatusEmpresa.Spring = true;
        lblStatusEmpresa.Text = "Empresa:";
        lblStatusEmpresa.TextAlign = ContentAlignment.MiddleLeft;
        //
        // lblStatusUsuario
        //
        lblStatusUsuario.BorderSides = ToolStripStatusLabelBorderSides.Left;
        lblStatusUsuario.Name = "lblStatusUsuario";
        lblStatusUsuario.Size = new Size(50, 17);
        lblStatusUsuario.Text = "Usuário:";
        //
        // lblStatusCnpj
        //
        lblStatusCnpj.BorderSides = ToolStripStatusLabelBorderSides.Left;
        lblStatusCnpj.Name = "lblStatusCnpj";
        lblStatusCnpj.Size = new Size(40, 17);
        lblStatusCnpj.Text = "CNPJ:";
        //
        // lblStatusAmbiente
        //
        lblStatusAmbiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatusAmbiente.Name = "lblStatusAmbiente";
        lblStatusAmbiente.Padding = new Padding(8, 0, 8, 0);
        lblStatusAmbiente.Size = new Size(90, 17);
        lblStatusAmbiente.Text = "AMBIENTE";
        //
        // FormPrincipal
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1024, 768);
        Controls.Add(stsPrincipal);
        Controls.Add(mnuPrincipal);
        IsMdiContainer = true;
        MainMenuStrip = mnuPrincipal;
        Name = "FormPrincipal";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Emissor CT-e / MDF-e";
        WindowState = FormWindowState.Maximized;
        mnuPrincipal.ResumeLayout(false);
        mnuPrincipal.PerformLayout();
        stsPrincipal.ResumeLayout(false);
        stsPrincipal.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip mnuPrincipal;
    private ToolStripMenuItem mnuCadastros;
    private ToolStripMenuItem mnuCadastrosEmpresas;
    private ToolStripMenuItem mnuCadastrosCertificados;
    private ToolStripMenuItem mnuCadastrosSeries;
    private ToolStripSeparator mnuCadastrosSeparador1;
    private ToolStripMenuItem mnuCadastrosParticipantes;
    private ToolStripSeparator mnuCadastrosSeparador2;
    private ToolStripMenuItem mnuCadastrosVeiculos;
    private ToolStripMenuItem mnuCadastrosProprietarios;
    private ToolStripMenuItem mnuCadastrosMotoristas;
    private ToolStripMenuItem mnuCadastrosApolices;
    private ToolStripMenuItem mnuCte;
    private ToolStripMenuItem mnuCteEmitir;
    private ToolStripMenuItem mnuCteConsultar;
    private ToolStripMenuItem mnuMdfe;
    private ToolStripMenuItem mnuMdfeEmitir;
    private ToolStripMenuItem mnuMdfeConsultar;
    private ToolStripMenuItem mnuSistema;
    private ToolStripMenuItem mnuSistemaStatusSefaz;
    private ToolStripMenuItem mnuSistemaUsuarios;
    private ToolStripMenuItem mnuSistemaAuditoria;
    private ToolStripSeparator mnuSistemaSeparador2;
    private ToolStripMenuItem mnuSistemaTrocarSenha;
    private ToolStripMenuItem mnuSistemaTrocarUsuario;
    private ToolStripMenuItem mnuSistemaTrocarEmpresa;
    private ToolStripSeparator mnuSistemaSeparador1;
    private ToolStripMenuItem mnuSistemaSair;
    private StatusStrip stsPrincipal;
    private ToolStripStatusLabel lblStatusEmpresa;
    private ToolStripStatusLabel lblStatusUsuario;
    private ToolStripStatusLabel lblStatusCnpj;
    private ToolStripStatusLabel lblStatusAmbiente;
}
