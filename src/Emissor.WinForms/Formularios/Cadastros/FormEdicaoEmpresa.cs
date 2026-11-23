using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Diálogo de inclusão e edição da empresa emitente (§5.1).</summary>
public partial class FormEdicaoEmpresa : Form
{
    private readonly EmpresaPrototipo _empresa;
    private readonly bool _homologacaoOriginal;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="empresa">Cópia editável do registro.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    internal FormEdicaoEmpresa(EmpresaPrototipo empresa, bool novo)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _empresa = empresa;
        _homologacaoOriginal = empresa.Homologacao;
        Text = novo ? "Nova empresa" : "Editar empresa";
        cboCrt.Carregar(OpcoesCadastro.Crts);
        cboRegimeIbsCbs.Carregar(OpcoesCadastro.RegimesIbsCbs);
        cboAmbiente.Carregar(OpcoesCadastro.Ambientes);
        CarregarCampos();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    /// <summary>O regime de IBS/CBS é opcional: a tecla Delete limpa a escolha.</summary>
    private void cboRegimeIbsCbs_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            cboRegimeIbsCbs.SelectedIndex = -1;
            e.Handled = true;
        }
    }

    private void CarregarCampos()
    {
        txtRazaoSocial.Text = _empresa.RazaoSocial;
        txtNomeFantasia.Text = _empresa.NomeFantasia;
        txtCnpj.Text = _empresa.Cnpj;
        txtIe.Text = _empresa.Ie;
        txtIeSt.Text = _empresa.IeSt;
        txtRntrc.Text = _empresa.Rntrc;
        cboCrt.Selecionar(_empresa.Crt);
        cboRegimeIbsCbs.Selecionar(_empresa.RegimeIbsCbs);
        ucEndereco.Logradouro = _empresa.Logradouro;
        ucEndereco.Numero = _empresa.Numero;
        ucEndereco.Complemento = _empresa.Complemento;
        ucEndereco.Bairro = _empresa.Bairro;
        ucEndereco.MunicipioCodigo = _empresa.MunicipioCodigo;
        ucEndereco.Cep = _empresa.Cep;
        txtFone.Text = _empresa.Fone;
        txtEmail.Text = _empresa.Email;
        cboAmbiente.Selecionar<bool>(_empresa.Homologacao);
    }

    private void LerCampos()
    {
        _empresa.RazaoSocial = txtRazaoSocial.Text.Trim();
        _empresa.NomeFantasia = txtNomeFantasia.Text.Trim();
        _empresa.Cnpj = txtCnpj.Text.Trim();
        _empresa.Ie = txtIe.Text.Trim();
        _empresa.IeSt = txtIeSt.Text.Trim();
        _empresa.Rntrc = txtRntrc.Text.Trim();
        _empresa.Crt = cboCrt.ValorSelecionado<Crt>();
        _empresa.RegimeIbsCbs = cboRegimeIbsCbs.ValorSelecionado<RegimeIbsCbs>();
        _empresa.Logradouro = ucEndereco.Logradouro;
        _empresa.Numero = ucEndereco.Numero;
        _empresa.Complemento = ucEndereco.Complemento;
        _empresa.Bairro = ucEndereco.Bairro;
        _empresa.MunicipioCodigo = ucEndereco.MunicipioCodigo;
        _empresa.Cep = ucEndereco.Cep;
        _empresa.Fone = txtFone.Text.Trim();
        _empresa.Email = txtEmail.Text.Trim();

        // Sem escolha, mantém homologação: o ambiente seguro é o padrão.
        _empresa.Homologacao = cboAmbiente.ValorSelecionado<bool>() ?? true;
    }

    /// <summary>
    /// Copia os campos para o registro e fecha com OK se os obrigatórios estiverem preenchidos.
    /// A troca para produção exige confirmação, pois documentos emitidos nela têm validade fiscal.
    /// </summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _empresa.ValidarObrigatorios()))
        {
            return;
        }

        if (_homologacaoOriginal && !_empresa.Homologacao)
        {
            var resposta = MessageBox.Show(this,
                "O ambiente padrão será alterado para PRODUÇÃO. Os documentos emitidos nesse ambiente terão validade fiscal." +
                $"{Environment.NewLine}{Environment.NewLine}Confirma a alteração?",
                Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (resposta != DialogResult.Yes)
            {
                return;
            }
        }

        DialogResult = DialogResult.OK;
    }
}
