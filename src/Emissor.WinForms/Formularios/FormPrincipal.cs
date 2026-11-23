using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios;

/// <summary>
/// Janela principal (MDI): menu de navegação e barra de status com a empresa e o ambiente ativos.
/// </summary>
public partial class FormPrincipal : Form
{
    private const string TituloAplicacao = "Emissor CT-e / MDF-e";

    /// <summary>Opções de menu que já têm tela de cadastro; as demais abrem a tela provisória.</summary>
    private readonly Dictionary<ToolStripItem, Func<IFonteCadastro>> _cadastros;

    private EmpresaPrototipo _empresaAtiva;

    /// <summary>Cria a janela principal já com a empresa escolhida na seleção inicial.</summary>
    /// <param name="empresaAtiva">Empresa com que o usuário vai trabalhar.</param>
    public FormPrincipal(EmpresaPrototipo empresaAtiva)
    {
        InitializeComponent();
        _empresaAtiva = empresaAtiva;

        // As funções leem _empresaAtiva na hora de abrir, então seguem a troca de empresa.
        _cadastros = new()
        {
            [mnuCadastrosEmpresas] = () => new FonteEmpresas(_empresaAtiva.Id, RecarregarEmpresaAtiva),
            [mnuCadastrosParticipantes] = () => new FonteParticipantes(_empresaAtiva.Id),
            [mnuCadastrosVeiculos] = () => new FonteVeiculos(_empresaAtiva.Id),
            [mnuCadastrosProprietarios] = () => new FonteProprietarios(_empresaAtiva.Id),
            [mnuCadastrosCertificados] = () => new FonteCertificados(_empresaAtiva.Id),
            [mnuCadastrosSeries] = () => new FonteSeries(_empresaAtiva.Id),
            [mnuCadastrosMotoristas] = () => new FonteMotoristas(_empresaAtiva.Id),
            [mnuCadastrosApolices] = () => new FonteApolices(_empresaAtiva.Id),
        };

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

    /// <summary>Relê a empresa ativa depois de uma alteração no cadastro de empresas e atualiza título e barra de status.</summary>
    private void RecarregarEmpresaAtiva()
    {
        if (DadosFicticios.Empresas.ObterPorId(_empresaAtiva.Id) is { } atualizada)
        {
            _empresaAtiva = atualizada;
            AtualizarEmpresaAtiva();
        }
    }

    private void mnuSistemaTrocarEmpresa_Click(object? sender, EventArgs e) => TrocarEmpresa();

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
    private void TrocarEmpresa()
    {
        using var selecao = new FormSelecaoEmpresa(DadosFicticios.Empresas.Listar(), _empresaAtiva);
        if (selecao.ShowDialog(this) != DialogResult.OK || selecao.EmpresaSelecionada is null)
        {
            return;
        }

        if (selecao.EmpresaSelecionada.Id == _empresaAtiva.Id)
        {
            return;
        }

        // As telas abertas pertencem à empresa anterior: fecha todas antes de trocar.
        foreach (var tela in MdiChildren)
        {
            tela.Close();
        }

        _empresaAtiva = selecao.EmpresaSelecionada;
        AtualizarEmpresaAtiva();
    }

    /// <summary>Atualiza título da janela e barra de status; o ambiente fica em destaque para evitar confusão com produção.</summary>
    private void AtualizarEmpresaAtiva()
    {
        var homologacao = _empresaAtiva.Homologacao;

        Text = $"{TituloAplicacao} — {_empresaAtiva.RazaoSocial}";
        lblStatusEmpresa.Text = $"Empresa: {_empresaAtiva.RazaoSocial} ({_empresaAtiva.Uf})";
        lblStatusCnpj.Text = $"CNPJ: {_empresaAtiva.Cnpj}";
        lblStatusAmbiente.Text = homologacao ? "HOMOLOGAÇÃO" : "PRODUÇÃO";
        lblStatusAmbiente.BackColor = homologacao ? Color.Gold : Color.Firebrick;
        lblStatusAmbiente.ForeColor = homologacao ? Color.Black : Color.White;
    }
}
