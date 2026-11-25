using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Acesso;

/// <summary>Linha da consulta de auditoria.</summary>
internal sealed class LinhaAuditoria
{
    /// <summary>Data e hora.</summary>
    public DateTime DataHora { get; init; }

    /// <summary>Login.</summary>
    public string Usuario { get; init; } = string.Empty;

    /// <summary>Empresa.</summary>
    public string Empresa { get; init; } = string.Empty;

    /// <summary>Ação para exibição.</summary>
    public string Acao { get; init; } = string.Empty;

    /// <summary>Registro afetado ("Participante 12").</summary>
    public string Registro { get; init; } = string.Empty;

    /// <summary>Detalhe.</summary>
    public string Descricao { get; init; } = string.Empty;
}

/// <summary>
/// Consulta da auditoria (só administrador, janela MDI): filtro por período, usuário, empresa, ação e texto. Só
/// leitura: a auditoria não é alterada nem excluída.
/// </summary>
public partial class FormConsultaAuditoria : Form
{
    /// <summary>Textos das ações.</summary>
    internal static readonly IReadOnlyDictionary<AcaoAuditoria, string> Acoes = new Dictionary<AcaoAuditoria, string>
    {
        [AcaoAuditoria.Inclusao] = "Inclusão",
        [AcaoAuditoria.Alteracao] = "Alteração",
        [AcaoAuditoria.Exclusao] = "Exclusão",
        [AcaoAuditoria.LoginAceito] = "Login",
        [AcaoAuditoria.LoginRecusado] = "Login recusado",
        [AcaoAuditoria.LoginBloqueado] = "Login bloqueado",
        [AcaoAuditoria.LoginInativo] = "Login de inativo",
        [AcaoAuditoria.PrimeiroAdministrador] = "Primeiro administrador",
        [AcaoAuditoria.TrocaSenha] = "Troca de senha",
        [AcaoAuditoria.EntradaEmpresa] = "Entrada na empresa",
        [AcaoAuditoria.TrocaUsuario] = "Troca de usuário",
        [AcaoAuditoria.XmlTeste] = "XML de teste",
        [AcaoAuditoria.ConsultaSefaz] = "Consulta à SEFAZ",
    };

    private const int Todos = -1;

    private readonly ServicoAuditoria _auditoria;
    private readonly ServicoAcesso _acesso;
    private readonly ServicoEmpresas _empresas;
    private Dictionary<int, string> _nomesEmpresas = [];
    private bool _carregado;
    private int _versao;

    /// <summary>Cria a consulta.</summary>
    /// <param name="auditoria">Consulta da auditoria.</param>
    /// <param name="acesso">Lista de usuários.</param>
    /// <param name="empresas">Lista de empresas.</param>
    internal FormConsultaAuditoria(ServicoAuditoria auditoria, ServicoAcesso acesso, ServicoEmpresas empresas)
    {
        InitializeComponent();
        _auditoria = auditoria;
        _acesso = acesso;
        _empresas = empresas;
        CriarColunas();
        dtpDe.Value = DateTime.Today.AddDays(-7);
        dtpAte.Value = DateTime.Today;
        cboAcao.Carregar(new[] { new Opcao<int>(Todos, "Todas") }.Concat(Acoes.Select(a => new Opcao<int>((int)a.Key, a.Value))));
        cboAcao.SelectedIndex = 0;
        Load += async (_, _) => await CarregarListasAsync();
    }

    /// <summary>Esc fecha a consulta.</summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            Close();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private async void Filtro_Alterado(object? sender, EventArgs e) => await RecarregarAsync();

    private void btnFechar_Click(object? sender, EventArgs e) => Close();

    private async Task CarregarListasAsync()
    {
        try
        {
            var usuarios = await _acesso.ListarUsuariosAsync();
            var empresas = await _empresas.ListarResumosAsync();
            _nomesEmpresas = empresas.ToDictionary(e => e.Id, e => e.NomeFantasia.Length > 0 ? e.NomeFantasia : e.RazaoSocial);
            cboUsuario.Carregar(new[] { new Opcao<int>(Todos, "Todos") }.Concat(usuarios.OrderBy(u => u.Login).Select(u => new Opcao<int>(u.Id, u.Login))));
            cboUsuario.SelectedIndex = 0;
            cboEmpresa.Carregar(new[] { new Opcao<int>(Todos, "Todas") }.Concat(empresas.Select(e => new Opcao<int>(e.Id, e.RazaoSocial))));
            cboEmpresa.SelectedIndex = 0;
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler os filtros.{Environment.NewLine}{erro.GetBaseException().Message}", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        _carregado = true;
        await RecarregarAsync();
    }

    private async Task RecarregarAsync()
    {
        if (!_carregado)
        {
            return;
        }

        var versao = ++_versao;
        var filtro = new FiltroAuditoria(
            dtpDe.Value.Date,
            dtpAte.Value.Date,
            Valor(cboUsuario),
            Valor(cboEmpresa),
            Valor(cboAcao) is { } acao ? (AcaoAuditoria)acao : null,
            txtPesquisa.Text);
        IReadOnlyList<RegistroAuditoria> registros;
        try
        {
            registros = await _auditoria.ConsultarAsync(filtro);
        }
        catch (RegraNegocioException erro)
        {
            MessageBox.Show(this, string.Join(Environment.NewLine, erro.Pendencias), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler a auditoria.{Environment.NewLine}{erro.GetBaseException().Message}", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (versao != _versao || IsDisposed)
        {
            return;
        }

        dgvAuditoria.DataSource = registros.Select(r => new LinhaAuditoria
        {
            DataHora = r.DataHora,
            Usuario = r.UsuarioLogin,
            Empresa = r.EmpresaId is { } id ? _nomesEmpresas.GetValueOrDefault(id, $"#{id}") : string.Empty,
            Acao = Acoes.GetValueOrDefault(r.Acao, r.Acao.ToString()),
            Registro = r.RegistroId is { } registro ? $"{r.Entidade} {registro}" : r.Entidade,
            Descricao = r.Descricao,
        }).ToList();
        lblTotal.Text = registros.Count >= Dados.Auditoria.LimiteConsulta
            ? $"{registros.Count} registros (limite atingido: refine o filtro)"
            : registros.Count == 1 ? "1 registro" : $"{registros.Count} registros";
    }

    private static int? Valor(ComboBox combo) => combo.ValorSelecionado<int>() is { } valor && valor != Todos ? valor : null;

    private void CriarColunas()
    {
        dgvAuditoria.AutoGenerateColumns = false;
        Coluna(nameof(LinhaAuditoria.DataHora), "Data e hora", 11, "dd/MM/yyyy HH:mm:ss");
        Coluna(nameof(LinhaAuditoria.Usuario), "Usuário", 9);
        Coluna(nameof(LinhaAuditoria.Empresa), "Empresa", 12);
        Coluna(nameof(LinhaAuditoria.Acao), "Ação", 10);
        Coluna(nameof(LinhaAuditoria.Registro), "Registro", 11);
        Coluna(nameof(LinhaAuditoria.Descricao), "Detalhe", 47);
    }

    private void Coluna(string propriedade, string titulo, float peso, string? formato = null) =>
        dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propriedade,
            HeaderText = titulo,
            FillWeight = peso,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle { Format = formato ?? string.Empty, WrapMode = DataGridViewTriState.True },
        });
}
