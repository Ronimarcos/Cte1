using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição de participante (§5.4), usado pelo cadastro e pelo assistente de CT-e.
/// Grava no banco pelo delegado recebido e continua aberto se o banco recusar.
/// </summary>
public partial class FormEdicaoParticipante : Form
{
    private readonly Participante _participante;
    private readonly Func<Participante, Task> _gravar;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="participante">Cópia editável do participante.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="municipios">Municípios do IBGE; <c>null</c> usa a lista reduzida do protótipo.</param>
    /// <param name="gravar">Inclui ou altera o participante.</param>
    internal FormEdicaoParticipante(Participante participante, bool novo, IReadOnlyList<Opcao<int>>? municipios, Func<Participante, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _participante = participante;
        _gravar = gravar;
        Text = novo ? "Novo participante" : "Editar participante";
        cboIndicadorIe.Carregar(OpcoesCadastro.IndicadoresIe);
        if (municipios is not null)
        {
            ucEndereco.CarregarMunicipios(municipios);
        }

        CarregarCampos();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        LerCampos();
        if (await GravacaoDialogo.GravarAsync(this, btnSalvar, _participante, _gravar))
        {
            DialogResult = DialogResult.OK;
        }
    }

    private void CarregarCampos()
    {
        txtCnpjCpf.Text = Formatador.CnpjCpf(_participante.CnpjCpf);
        cboIndicadorIe.Selecionar(_participante.IndicadorIe);
        txtIe.Text = _participante.Ie;
        txtRazaoSocial.Text = _participante.RazaoSocial;
        txtNomeFantasia.Text = _participante.NomeFantasia;
        ucEndereco.Logradouro = _participante.Endereco.Logradouro;
        ucEndereco.Numero = _participante.Endereco.Numero;
        ucEndereco.Complemento = _participante.Endereco.Complemento;
        ucEndereco.Bairro = _participante.Endereco.Bairro;
        ucEndereco.MunicipioCodigo = _participante.Endereco.MunicipioCodigo;
        ucEndereco.Cep = Formatador.Cep(_participante.Endereco.Cep);
        txtFone.Text = _participante.Fone;
        txtEmail.Text = _participante.Email;
    }

    /// <summary>Copia os campos para o participante, sem máscara (CNPJ aceita letras: CNPJ alfanumérico).</summary>
    private void LerCampos()
    {
        _participante.CnpjCpf = Formatos.SoAlfanumericos(txtCnpjCpf.Text);
        _participante.IndicadorIe = cboIndicadorIe.ValorSelecionado<IndicadorIe>();
        _participante.Ie = Formatos.SoDigitos(txtIe.Text);
        _participante.RazaoSocial = txtRazaoSocial.Text.Trim();
        _participante.NomeFantasia = txtNomeFantasia.Text.Trim();
        _participante.Endereco.Logradouro = ucEndereco.Logradouro;
        _participante.Endereco.Numero = ucEndereco.Numero;
        _participante.Endereco.Complemento = ucEndereco.Complemento;
        _participante.Endereco.Bairro = ucEndereco.Bairro;
        _participante.Endereco.MunicipioCodigo = ucEndereco.MunicipioCodigo;
        _participante.Endereco.Cep = Formatos.SoDigitos(ucEndereco.Cep);
        _participante.Fone = Formatos.SoDigitos(txtFone.Text);
        _participante.Email = txtEmail.Text.Trim();
    }
}
