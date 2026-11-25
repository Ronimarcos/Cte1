using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição da empresa emitente (§5.1), gravada no banco. O emitente tem CNPJ (inclusive
/// alfanumérico) ou CPF. A gravação acontece aqui, pelo delegado recebido, para que uma recusa do banco
/// (ex.: CNPJ já cadastrado) mantenha o diálogo aberto com o que foi digitado.
/// </summary>
public partial class FormEdicaoEmpresa : Form
{
    private readonly Empresa _empresa;
    private readonly Func<Empresa, Task> _gravar;
    private readonly bool _producaoOriginal;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="empresa">Cópia editável da empresa.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="municipios">Municípios do IBGE para a escolha do endereço.</param>
    /// <param name="gravar">Inclui ou altera a empresa no banco.</param>
    internal FormEdicaoEmpresa(Empresa empresa, bool novo, CatalogoMunicipios municipios, Func<Empresa, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _empresa = empresa;
        _gravar = gravar;
        _producaoOriginal = empresa.AmbientePadrao == TipoAmbiente.Producao;
        Text = novo ? "Nova empresa" : "Editar empresa";
        cboCrt.Carregar(OpcoesCadastro.Crts);
        cboRegimeIbsCbs.Carregar(OpcoesCadastro.RegimesIbsCbs);
        cboAmbiente.Carregar(OpcoesCadastro.Ambientes);
        ucEndereco.CarregarMunicipios(municipios.Opcoes);
        CarregarCampos();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e) => await SalvarAsync();

    /// <summary>O regime de IBS/CBS é opcional: a tecla Delete limpa a escolha.</summary>
    private void cboRegimeIbsCbs_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            cboRegimeIbsCbs.SelectedIndex = -1;
            e.Handled = true;
        }
    }

    /// <summary>CNPJ aceita letras (CNPJ alfanumérico) e tem até 18 caracteres com máscara; CPF tem até 14.</summary>
    private void rdoDocumento_CheckedChanged(object? sender, EventArgs e) => txtCnpj.MaxLength = rdoCpf.Checked ? 14 : 18;

    private void CarregarCampos()
    {
        txtRazaoSocial.Text = _empresa.RazaoSocial;
        txtNomeFantasia.Text = _empresa.NomeFantasia;
        rdoCpf.Checked = _empresa.Cpf.Length > 0;
        rdoCnpj.Checked = !rdoCpf.Checked;
        txtCnpj.Text = rdoCpf.Checked ? Formatador.Cpf(_empresa.Cpf) : Formatador.Cnpj(_empresa.Cnpj);
        txtIe.Text = _empresa.Ie;
        txtIeSt.Text = _empresa.IeSt;
        txtRntrc.Text = _empresa.Rntrc;
        cboCrt.Selecionar(_empresa.Crt);
        cboRegimeIbsCbs.Selecionar(_empresa.RegimeIbsCbs);
        ucEndereco.Logradouro = _empresa.Endereco.Logradouro;
        ucEndereco.Numero = _empresa.Endereco.Numero;
        ucEndereco.Complemento = _empresa.Endereco.Complemento;
        ucEndereco.Bairro = _empresa.Endereco.Bairro;
        ucEndereco.MunicipioCodigo = _empresa.Endereco.MunicipioCodigo;
        ucEndereco.Cep = Formatador.Cep(_empresa.Endereco.Cep);
        txtFone.Text = _empresa.Fone;
        txtEmail.Text = _empresa.Email;
        cboAmbiente.Selecionar<bool>(_empresa.AmbientePadrao == TipoAmbiente.Homologacao);
    }

    /// <summary>Copia os campos para a empresa, já sem máscara (o banco guarda só letras e dígitos).</summary>
    private void LerCampos()
    {
        _empresa.RazaoSocial = txtRazaoSocial.Text.Trim();
        _empresa.NomeFantasia = txtNomeFantasia.Text.Trim();
        _empresa.Cnpj = rdoCnpj.Checked ? Formatos.SoAlfanumericos(txtCnpj.Text) : string.Empty;
        _empresa.Cpf = rdoCpf.Checked ? Formatos.SoDigitos(txtCnpj.Text) : string.Empty;
        _empresa.Ie = Formatos.SoDigitos(txtIe.Text);
        _empresa.IeSt = Formatos.SoDigitos(txtIeSt.Text);
        _empresa.Rntrc = Formatos.SoDigitos(txtRntrc.Text);
        _empresa.Crt = cboCrt.ValorSelecionado<Crt>();
        _empresa.RegimeIbsCbs = cboRegimeIbsCbs.ValorSelecionado<RegimeIbsCbs>();
        _empresa.Endereco.Logradouro = ucEndereco.Logradouro;
        _empresa.Endereco.Numero = ucEndereco.Numero;
        _empresa.Endereco.Complemento = ucEndereco.Complemento;
        _empresa.Endereco.Bairro = ucEndereco.Bairro;
        _empresa.Endereco.MunicipioCodigo = ucEndereco.MunicipioCodigo;
        _empresa.Endereco.Cep = Formatos.SoDigitos(ucEndereco.Cep);
        _empresa.Fone = Formatos.SoDigitos(txtFone.Text);
        _empresa.Email = txtEmail.Text.Trim();

        // Sem escolha, mantém homologação: o ambiente seguro é o padrão.
        _empresa.AmbientePadrao = cboAmbiente.ValorSelecionado<bool>() is false ? TipoAmbiente.Producao : TipoAmbiente.Homologacao;
    }

    /// <summary>
    /// Valida, pede confirmação ao passar para produção e grava. Se o banco recusar, mostra o motivo e
    /// mantém o diálogo aberto.
    /// </summary>
    private async Task SalvarAsync()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _empresa.Validar()))
        {
            return;
        }

        if (!_producaoOriginal && _empresa.AmbientePadrao == TipoAmbiente.Producao)
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

        btnSalvar.Enabled = false;
        UseWaitCursor = true;
        try
        {
            await _gravar(_empresa);
            DialogResult = DialogResult.OK;
        }
        catch (RegraNegocioException erro)
        {
            AvisosCadastro.HaPendencias(this, Text, erro.Pendencias);
        }
        finally
        {
            UseWaitCursor = false;
            btnSalvar.Enabled = true;
        }
    }
}
