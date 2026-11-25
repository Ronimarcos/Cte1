using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Formularios.Cte;
using Emissor.WinForms.Formularios.Mdfe;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios;

/// <summary>
/// Janela principal (MDI): menu de navegação e barra de status com a empresa e o ambiente ativos.
/// </summary>
public partial class FormPrincipal : Form
{
    private const string TituloAplicacao = "Emissor CT-e / MDF-e";

    /// <summary>Opções de menu que já têm tela de cadastro; as demais abrem a tela provisória.</summary>
    private readonly Dictionary<ToolStripItem, Func<IFonteCadastro>> _cadastros;

    /// <summary>Opções de menu que abrem telas próprias (fora do padrão de cadastro).</summary>
    private readonly Dictionary<ToolStripItem, Action> _telas;

    private readonly ServicoEmpresas _servicoEmpresas;
    private readonly ServicosCadastro _servicos;
    private readonly CatalogoMunicipios _municipios;

    /// <summary>Empresa ativa, como gravada no banco.</summary>
    private Empresa _empresa;

    /// <summary>
    /// Mesma empresa no formato do protótipo, usado pelos assistentes de CT-e/MDF-e e pelos cadastros ainda em
    /// memória (ponte provisória até a fatia P2c).
    /// </summary>
    private EmpresaPrototipo _empresaAtiva;

    /// <summary>Cria a janela principal já com a empresa escolhida na seleção inicial.</summary>
    /// <param name="empresa">Empresa com que o usuário vai trabalhar.</param>
    /// <param name="servicoEmpresas">Operações de empresa no banco.</param>
    /// <param name="servicos">Operações dos demais cadastros no banco.</param>
    /// <param name="municipios">Municípios do IBGE.</param>
    public FormPrincipal(Empresa empresa, ServicoEmpresas servicoEmpresas, ServicosCadastro servicos, CatalogoMunicipios municipios)
    {
        InitializeComponent();
        _servicoEmpresas = servicoEmpresas;
        _servicos = servicos;
        _municipios = municipios;
        _empresa = empresa;
        _empresaAtiva = EmpresaPrototipo.DeEntidade(empresa);

        // As funções leem _empresaAtiva na hora de abrir, então seguem a troca de empresa.
        _cadastros = new()
        {
            [mnuCadastrosEmpresas] = () => new FonteEmpresas(_servicoEmpresas, _municipios, _empresa.Id, RecarregarEmpresaAtivaAsync),
            [mnuCadastrosParticipantes] = () => new FonteParticipantes(_servicos.Participantes, _municipios, _empresa.Id),
            [mnuCadastrosVeiculos] = () => new FonteVeiculos(_servicos.Veiculos, _servicos.Proprietarios, _empresa.Id),
            [mnuCadastrosProprietarios] = () => new FonteProprietarios(_servicos.Proprietarios, _empresa.Id),
            [mnuCadastrosCertificados] = () => new FonteCertificados(_empresaAtiva.Id),
            [mnuCadastrosSeries] = () => new FonteSeries(_servicos.Series, _empresa.Id),
            [mnuCadastrosMotoristas] = () => new FonteMotoristas(_servicos.Motoristas, _empresa.Id),
            [mnuCadastrosApolices] = () => new FonteApolices(_servicos.Apolices, _empresa.Id),
        };

        _telas = new()
        {
            [mnuCteEmitir] = EmitirCte,
            [mnuCteConsultar] = AbrirConsultaCte,
            [mnuMdfeEmitir] = EmitirMdfe,
            [mnuMdfeConsultar] = AbrirConsultaMdfe,
        };

        ConfigurarAtalhosMenu();

        AtualizarEmpresaAtiva();
    }

    /// <summary>
    /// Evento compartilhado pelas opções de menu de telas. Abre o cadastro quando a opção já tem tela;
    /// senão abre a tela provisória, cujo título vem da propriedade <c>Tag</c> do item (ou do texto, se não houver Tag).
    /// </summary>
    private void AbrirTelaEmConstrucao_Click(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem item)
        {
            return;
        }

        if (_cadastros.TryGetValue(item, out var criarFonte))
        {
            AbrirCadastro(criarFonte());
            return;
        }

        if (_telas.TryGetValue(item, out var abrirTela))
        {
            abrirTela();
            return;
        }

        AbrirTelaEmConstrucao(item.Tag as string ?? item.Text?.Replace("&", string.Empty) ?? string.Empty);
    }

    /// <summary>Abre a lista do cadastro ou, se já estiver aberta, apenas a traz para frente.</summary>
    private void AbrirCadastro(IFonteCadastro fonte)
    {
        var telaAberta = MdiChildren.OfType<FormListaCadastro>().FirstOrDefault(f => f.Titulo == fonte.Titulo);
        if (telaAberta is not null)
        {
            telaAberta.Activate();
            return;
        }

        var tela = new FormListaCadastro(fonte) { MdiParent = this };
        tela.Show();
    }

    /// <summary>
    /// Atalhos diretos dos menus (aparecem ao lado de cada item). Definidos aqui, e não no Designer,
    /// para manter o <c>FormPrincipal.Designer.cs</c> como foi validado. A lista completa está em <see cref="AtalhosTeclado"/>.
    /// </summary>
    private void ConfigurarAtalhosMenu()
    {
        mnuCteEmitir.ShortcutKeys = Keys.Control | Keys.T;
        mnuCteConsultar.ShortcutKeys = Keys.Control | Keys.Shift | Keys.T;
        mnuMdfeEmitir.ShortcutKeys = Keys.Control | Keys.M;
        // Ctrl+Shift+M não serve: o Microsoft Teams usa essa tecla (microfone) em todo o Windows.
        mnuMdfeConsultar.ShortcutKeys = Keys.Control | Keys.Shift | Keys.D;
        mnuCadastrosParticipantes.ShortcutKeys = Keys.Control | Keys.Shift | Keys.P;
        mnuCadastrosVeiculos.ShortcutKeys = Keys.Control | Keys.Shift | Keys.V;
        mnuCadastrosMotoristas.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
        mnuSistemaTrocarEmpresa.ShortcutKeys = Keys.Control | Keys.Shift | Keys.E;

        // Alt+F4 já é tratado pelo Windows; aqui só aparece no menu.
        mnuSistemaSair.ShortcutKeyDisplayString = "Alt+F4";
    }

    /// <summary>F1 mostra a lista de atalhos, também quando o foco está numa janela interna (MDI).</summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F1)
        {
            AtalhosTeclado.MostrarAjuda(this);
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>Abre o assistente de emissão para um CT-e novo; ao salvar, atualiza a consulta se estiver aberta.</summary>
    private void EmitirCte()
    {
        var cte = new CtePrototipo { EmpresaId = _empresaAtiva.Id };
        using var tela = new FormEmissaoCte(cte, _empresaAtiva);
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            MdiChildren.OfType<FormConsultaCte>().FirstOrDefault()?.Recarregar(cte.Id);
        }
    }

    /// <summary>Abre a consulta de CT-e ou, se já estiver aberta, apenas a traz para frente.</summary>
    private void AbrirConsultaCte()
    {
        var telaAberta = MdiChildren.OfType<FormConsultaCte>().FirstOrDefault();
        if (telaAberta is not null)
        {
            telaAberta.Activate();
            return;
        }

        var tela = new FormConsultaCte(_empresaAtiva) { MdiParent = this };
        tela.Show();
    }

    /// <summary>Abre o assistente de emissão para um MDF-e novo; ao salvar, atualiza a consulta se estiver aberta.</summary>
    private void EmitirMdfe()
    {
        var mdfe = new MdfePrototipo { EmpresaId = _empresaAtiva.Id };
        using var tela = new FormEmissaoMdfe(mdfe, _empresaAtiva);
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            MdiChildren.OfType<FormConsultaMdfe>().FirstOrDefault()?.Recarregar(mdfe.Id);
        }
    }

    /// <summary>Abre a consulta de MDF-e ou, se já estiver aberta, apenas a traz para frente.</summary>
    private void AbrirConsultaMdfe()
    {
        var telaAberta = MdiChildren.OfType<FormConsultaMdfe>().FirstOrDefault();
        if (telaAberta is not null)
        {
            telaAberta.Activate();
            return;
        }

        var tela = new FormConsultaMdfe(_empresaAtiva) { MdiParent = this };
        tela.Show();
    }

    /// <summary>Relê a empresa ativa depois de uma alteração no cadastro de empresas e atualiza título e barra de status.</summary>
    private async Task RecarregarEmpresaAtivaAsync()
    {
        if (await _servicoEmpresas.ObterAsync(_empresa.Id) is { } atualizada)
        {
            DefinirEmpresa(atualizada);
        }
    }

    /// <summary>Guarda a empresa ativa nos dois formatos e atualiza título e barra de status.</summary>
    private void DefinirEmpresa(Empresa empresa)
    {
        _empresa = empresa;
        _empresaAtiva = EmpresaPrototipo.DeEntidade(empresa);
        AtualizarEmpresaAtiva();
    }

    private async void mnuSistemaTrocarEmpresa_Click(object? sender, EventArgs e) => await TrocarEmpresaAsync();

    private void mnuSistemaSair_Click(object? sender, EventArgs e) => Close();

    /// <summary>Abre a tela provisória ou, se já estiver aberta, apenas a traz para frente.</summary>
    private void AbrirTelaEmConstrucao(string titulo)
    {
        var telaAberta = MdiChildren.OfType<FormEmConstrucao>().FirstOrDefault(f => f.Titulo == titulo);
        if (telaAberta is not null)
        {
            telaAberta.Activate();
            return;
        }

        var tela = new FormEmConstrucao(titulo) { MdiParent = this };
        tela.Show();
    }

    /// <summary>Exibe a seleção de empresa e, se outra for escolhida, fecha as telas abertas e troca a empresa ativa.</summary>
    private async Task TrocarEmpresaAsync()
    {
        IReadOnlyList<EmpresaResumo> empresas;
        try
        {
            empresas = await _servicoEmpresas.ListarResumosAsync();
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler as empresas.{Environment.NewLine}{erro.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using var selecao = new FormSelecaoEmpresa(empresas, _empresa.Id);
        if (selecao.ShowDialog(this) != DialogResult.OK || selecao.EmpresaSelecionada is null)
        {
            return;
        }

        if (selecao.EmpresaSelecionada.Id == _empresa.Id
            || await _servicoEmpresas.ObterAsync(selecao.EmpresaSelecionada.Id) is not { } escolhida)
        {
            return;
        }

        // As telas abertas pertencem à empresa anterior: fecha todas antes de trocar.
        foreach (var tela in MdiChildren)
        {
            tela.Close();
        }

        DefinirEmpresa(escolhida);
    }

    /// <summary>Atualiza título da janela e barra de status; o ambiente fica em destaque para evitar confusão com produção.</summary>
    private void AtualizarEmpresaAtiva()
    {
        var homologacao = _empresa.AmbientePadrao == TipoAmbiente.Homologacao;
        var uf = _municipios.Obter(_empresa.Endereco.MunicipioCodigo)?.Uf ?? string.Empty;

        Text = $"{TituloAplicacao} — {_empresa.RazaoSocial}";
        lblStatusEmpresa.Text = $"Empresa: {_empresa.RazaoSocial} ({uf})";
        lblStatusCnpj.Text = _empresa.Cpf.Length > 0 ? $"CPF: {Formatador.Cpf(_empresa.Cpf)}" : $"CNPJ: {Formatador.Cnpj(_empresa.Cnpj)}";
        lblStatusAmbiente.Text = homologacao ? "HOMOLOGAÇÃO" : "PRODUÇÃO";
        lblStatusAmbiente.BackColor = homologacao ? Color.Gold : Color.Firebrick;
        lblStatusAmbiente.ForeColor = homologacao ? Color.Black : Color.White;
    }
}
