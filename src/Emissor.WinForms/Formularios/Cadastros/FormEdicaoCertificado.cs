using System.Text;
using Emissor.DFe.Certificados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição do certificado digital (§5.2), gravado no banco.
/// A1: escolhe o arquivo .pfx e digita a senha; o certificado é aberto na hora (senha errada é recusada) e o
/// arquivo e a senha são gravados criptografados. A3: escolhe o certificado instalado no Windows (token ou
/// cartão), localizado depois pelo thumbprint. Titular, CNPJ e validade são lidos do próprio certificado.
/// </summary>
public partial class FormEdicaoCertificado : Form
{
    private readonly CertificadoDigital _certificado;
    private readonly Empresa _empresa;
    private readonly IProtetorSegredos _protetor;
    private readonly Func<CertificadoDigital, Task> _gravar;

    /// <summary>Arquivo .pfx escolhido nesta edição (ainda aberto, só em memória).</summary>
    private byte[]? _pfx;

    /// <summary>Dados lidos do certificado escolhido nesta edição.</summary>
    private DadosCertificado? _lido;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="certificado">Cópia editável do certificado.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="empresa">Empresa emitente (o certificado precisa ter a mesma raiz de CNPJ).</param>
    /// <param name="protetor">Criptografia do arquivo e da senha do A1.</param>
    /// <param name="gravar">Inclui ou altera no banco.</param>
    internal FormEdicaoCertificado(CertificadoDigital certificado, bool novo, Empresa empresa, IProtetorSegredos protetor,
        Func<CertificadoDigital, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _certificado = certificado;
        _empresa = empresa;
        _protetor = protetor;
        _gravar = gravar;
        Text = novo ? "Novo certificado digital" : "Editar certificado digital";
        cboTipo.Carregar(OpcoesCadastro.TiposCertificado);

        // Os dados do titular vêm do certificado: só para conferência.
        txtTitular.ReadOnly = true;
        txtCnpjTitular.ReadOnly = true;
        txtThumbprint.ReadOnly = true;
        dtpValidoDe.Enabled = false;
        dtpValidoAte.Enabled = false;
        cboTipo.Enabled = novo;
        txtSenha.Leave += txtSenha_Leave;
        lblAvisoPrototipo.Text = "Titular, CNPJ e validade são lidos do próprio certificado. O arquivo e a senha do A1 " +
            "são guardados criptografados; o A3 é localizado no Windows pelo thumbprint.";

        CarregarCampos();
        AtualizarCamposPorTipo();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e) => await SalvarAsync();

    private void cboTipo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Trocar o tipo descarta o que foi lido do outro tipo.
        _pfx = null;
        _lido = null;
        txtArquivo.Clear();
        txtSenha.Clear();
        MostrarDados(null);
        AtualizarCamposPorTipo();
    }

    private async void btnProcurarArquivo_Click(object? sender, EventArgs e)
    {
        if (cboTipo.ValorSelecionado<TipoCertificado>() == TipoCertificado.A3)
        {
            await EscolherNoWindowsAsync();
        }
        else
        {
            await EscolherArquivoAsync();
        }
    }

    /// <summary>Com arquivo escolhido e senha digitada, o A1 é aberto ao sair do campo da senha.</summary>
    private void txtSenha_Leave(object? sender, EventArgs e)
    {
        if (_pfx is not null && txtSenha.TextLength > 0)
        {
            LerA1(avisar: true);
        }
    }

    /// <summary>A1 usa arquivo e senha; A3 escolhe o certificado instalado no Windows pelo mesmo botão.</summary>
    private void AtualizarCamposPorTipo()
    {
        var tipo = cboTipo.ValorSelecionado<TipoCertificado>();
        var a1 = tipo == TipoCertificado.A1;
        btnProcurarArquivo.Enabled = tipo is not null;
        btnProcurarArquivo.Text = tipo == TipoCertificado.A3 ? "&Escolher no Windows..." : "&Procurar...";
        lblArquivo.Enabled = a1;
        txtArquivo.Enabled = a1;
        lblSenha.Enabled = a1;
        txtSenha.Enabled = a1;
    }

    private async Task EscolherArquivoAsync()
    {
        using var dialogo = new OpenFileDialog
        {
            Title = "Escolher certificado A1",
            Filter = "Certificado A1 (*.pfx;*.p12)|*.pfx;*.p12",
            CheckFileExists = true,
        };
        if (dialogo.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _pfx = await File.ReadAllBytesAsync(dialogo.FileName);
        }
        catch (IOException erro)
        {
            MessageBox.Show(this, $"Não foi possível ler o arquivo.{Environment.NewLine}{erro.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _lido = null;
        txtArquivo.Text = dialogo.FileName;
        MostrarDados(null);
        if (txtSenha.TextLength > 0)
        {
            LerA1(avisar: true);
        }
        else
        {
            txtSenha.Focus();
        }
    }

    private async Task EscolherNoWindowsAsync()
    {
        IReadOnlyList<DadosCertificado> instalados;
        UseWaitCursor = true;
        try
        {
            instalados = await Task.Run(LeitorCertificado.ListarRepositorioWindows);
        }
        catch (Exception erro)
        {
            MessageBox.Show(this, $"Não foi possível ler os certificados do Windows.{Environment.NewLine}{erro.Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        finally
        {
            UseWaitCursor = false;
        }

        using var tela = new FormSelecaoCertificadoWindows(instalados);
        if (tela.ShowDialog(this) == DialogResult.OK && tela.Selecionado is { } escolhido)
        {
            _lido = escolhido;
            MostrarDados(escolhido);
        }
    }

    /// <summary>Abre o A1 com a senha digitada; se falhar, avisa (quando pedido) e devolve falso.</summary>
    private bool LerA1(bool avisar)
    {
        if (_pfx is null)
        {
            return false;
        }

        try
        {
            _lido = LeitorCertificado.LerA1(_pfx, txtSenha.Text);
            MostrarDados(_lido);
            return true;
        }
        catch (RegraNegocioException erro)
        {
            _lido = null;
            MostrarDados(null);
            if (avisar)
            {
                MessageBox.Show(this, erro.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return false;
        }
    }

    private void CarregarCampos()
    {
        cboTipo.Selecionar(_certificado.Tipo);
        txtArquivo.Text = _certificado.ArquivoPfxCriptografado is { Length: > 0 } ? "(arquivo guardado criptografado no banco)" : string.Empty;
        txtThumbprint.Text = _certificado.Thumbprint;
        txtTitular.Text = _certificado.Titular;
        txtCnpjTitular.Text = Formatador.CnpjCpf(_certificado.CnpjTitular);
        if (_certificado.ValidoAte > _certificado.ValidoDe)
        {
            dtpValidoDe.Value = _certificado.ValidoDe;
            dtpValidoAte.Value = _certificado.ValidoAte;
        }
    }

    private void MostrarDados(DadosCertificado? dados)
    {
        txtTitular.Text = dados?.Titular ?? string.Empty;
        txtCnpjTitular.Text = Formatador.CnpjCpf(dados?.Documento);
        txtThumbprint.Text = dados?.Thumbprint ?? string.Empty;
        if (dados is not null)
        {
            dtpValidoDe.Value = dados.ValidoDe;
            dtpValidoAte.Value = dados.ValidoAte;
        }
    }

    /// <summary>
    /// Copia para o registro o que foi lido do certificado escolhido nesta edição. Sem certificado novo, mantém
    /// o que já estava gravado.
    /// </summary>
    private void LerCampos()
    {
        _certificado.Tipo = cboTipo.ValorSelecionado<TipoCertificado>();
        if (_lido is null)
        {
            return;
        }

        _certificado.Titular = _lido.Titular;
        _certificado.CnpjTitular = _lido.Documento;
        _certificado.ValidoDe = _lido.ValidoDe;
        _certificado.ValidoAte = _lido.ValidoAte;
        _certificado.Thumbprint = _lido.Thumbprint;
        if (_certificado.Tipo == TipoCertificado.A1 && _pfx is not null)
        {
            _certificado.ArquivoPfxCriptografado = _protetor.Proteger(_pfx);
            _certificado.SenhaCriptografada = _protetor.Proteger(Encoding.UTF8.GetBytes(txtSenha.Text));
        }
        else
        {
            _certificado.ArquivoPfxCriptografado = null;
            _certificado.SenhaCriptografada = null;
        }
    }

    /// <summary>Confere o certificado (titular da empresa, dentro da validade) e grava no banco.</summary>
    private async Task SalvarAsync()
    {
        if (_pfx is not null && _lido is null && !LerA1(avisar: true))
        {
            return;
        }

        LerCampos();
        var pendencias = new List<string>();
        if (_certificado.Tipo is not null && _lido is null && _certificado.Id == 0)
        {
            pendencias.Add(_certificado.Tipo == TipoCertificado.A1
                ? "Escolha o arquivo do certificado e digite a senha"
                : "Escolha o certificado instalado no Windows");
        }

        if (pendencias.Count == 0)
        {
            pendencias.AddRange(_certificado.Validar());
            if (_certificado.CnpjTitular.Length > 0)
            {
                pendencias.AddRange(_certificado.ValidarTitular(_empresa));
            }

            if (_certificado.ValidoAte > _certificado.ValidoDe && _certificado.SituacaoEm(DateTime.Today) == SituacaoCertificado.Vencido)
            {
                pendencias.Add($"O certificado venceu em {_certificado.ValidoAte:dd/MM/yyyy} e não serve para assinar");
            }
        }

        if (AvisosCadastro.HaPendencias(this, Text, pendencias))
        {
            return;
        }

        if (await GravacaoDialogo.GravarAsync(this, btnSalvar, _certificado, _gravar))
        {
            txtSenha.Clear();
            DialogResult = DialogResult.OK;
        }
    }
}
