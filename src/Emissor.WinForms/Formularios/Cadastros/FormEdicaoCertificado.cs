using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição do certificado digital (§5.2).
/// No protótipo o arquivo .pfx e a senha não são lidos nem guardados: titular, CNPJ e validade são digitados.
/// Na versão final esses dados virão do próprio certificado, e arquivo e senha serão guardados criptografados.
/// </summary>
public partial class FormEdicaoCertificado : Form
{
    private readonly CertificadoPrototipo _certificado;
    private readonly bool _novo;
    private readonly string _arquivoOriginal;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="certificado">Cópia editável do registro.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    internal FormEdicaoCertificado(CertificadoPrototipo certificado, bool novo)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _certificado = certificado;
        _novo = novo;
        _arquivoOriginal = certificado.CaminhoArquivo;
        Text = novo ? "Novo certificado digital" : "Editar certificado digital";
        cboTipo.Carregar(OpcoesCadastro.TiposCertificado);
        CarregarCampos();
        AtualizarCamposPorTipo();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    private void cboTipo_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarCamposPorTipo();

    private void btnProcurarArquivo_Click(object? sender, EventArgs e) => EscolherArquivo();

    /// <summary>A1 usa arquivo e senha; A3 usa o thumbprint do repositório do Windows.</summary>
    private void AtualizarCamposPorTipo()
    {
        var tipo = cboTipo.ValorSelecionado<TipoCertificado>();
        var a1 = tipo == TipoCertificado.A1;
        var a3 = tipo == TipoCertificado.A3;

        lblArquivo.Enabled = a1;
        txtArquivo.Enabled = a1;
        btnProcurarArquivo.Enabled = a1;
        lblSenha.Enabled = a1;
        txtSenha.Enabled = a1;
        lblThumbprint.Enabled = a3;
        txtThumbprint.Enabled = a3;
    }

    /// <summary>Guarda apenas o caminho escolhido; o protótipo não abre o arquivo.</summary>
    private void EscolherArquivo()
    {
        using var dialogo = new OpenFileDialog
        {
            Title = "Escolher certificado A1",
            Filter = "Certificado A1 (*.pfx;*.p12)|*.pfx;*.p12",
            CheckFileExists = true,
        };

        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            txtArquivo.Text = dialogo.FileName;
        }
    }

    private void CarregarCampos()
    {
        cboTipo.Selecionar(_certificado.Tipo);
        txtArquivo.Text = _certificado.CaminhoArquivo;
        txtThumbprint.Text = _certificado.Thumbprint;
        txtTitular.Text = _certificado.Titular;
        txtCnpjTitular.Text = _certificado.CnpjTitular;
        dtpValidoDe.Value = _certificado.ValidoDe ?? DateTime.Today;
        dtpValidoAte.Value = _certificado.ValidoAte ?? DateTime.Today.AddYears(1);
    }

    private void LerCampos()
    {
        _certificado.Tipo = cboTipo.ValorSelecionado<TipoCertificado>();

        // Só guarda o dado que vale para o tipo escolhido.
        _certificado.CaminhoArquivo = _certificado.Tipo == TipoCertificado.A1 ? txtArquivo.Text.Trim() : string.Empty;
        _certificado.Thumbprint = _certificado.Tipo == TipoCertificado.A3
            ? txtThumbprint.Text.Replace(" ", string.Empty).Trim().ToUpperInvariant()
            : string.Empty;
        _certificado.Titular = txtTitular.Text.Trim();
        _certificado.CnpjTitular = txtCnpjTitular.Text.Trim();
        _certificado.ValidoDe = dtpValidoDe.Value.Date;
        _certificado.ValidoAte = dtpValidoAte.Value.Date;
    }

    /// <summary>
    /// A senha é pedida para um A1 novo ou com outro arquivo. No protótipo ela só é conferida como preenchida
    /// e é descartada ao fechar a tela.
    /// </summary>
    private IReadOnlyList<string> CamposFaltando()
    {
        var faltando = _certificado.ValidarObrigatorios().ToList();
        var arquivoNovo = _novo || !string.Equals(_certificado.CaminhoArquivo, _arquivoOriginal, StringComparison.OrdinalIgnoreCase);
        if (_certificado.Tipo == TipoCertificado.A1 && arquivoNovo && txtSenha.TextLength == 0)
        {
            faltando.Add("Senha do certificado");
        }

        return faltando;
    }

    /// <summary>Copia os campos para o registro e fecha com OK se os obrigatórios estiverem preenchidos.</summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, CamposFaltando()))
        {
            return;
        }

        txtSenha.Clear();
        DialogResult = DialogResult.OK;
    }
}
