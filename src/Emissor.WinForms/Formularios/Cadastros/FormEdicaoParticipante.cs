using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Diálogo de inclusão e edição de participante (§5.4).</summary>
public partial class FormEdicaoParticipante : Form
{
    private readonly ParticipantePrototipo _participante;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="participante">Cópia editável do registro.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    internal FormEdicaoParticipante(ParticipantePrototipo participante, bool novo)
    {
        InitializeComponent();
        _participante = participante;
        Text = novo ? "Novo participante" : "Editar participante";
        cboIndicadorIe.Carregar(OpcoesCadastro.IndicadoresIe);
        CarregarCampos();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    private void CarregarCampos()
    {
        txtCnpjCpf.Text = _participante.CnpjCpf;
        cboIndicadorIe.Selecionar(_participante.IndicadorIe);
        txtIe.Text = _participante.Ie;
        txtRazaoSocial.Text = _participante.RazaoSocial;
        txtNomeFantasia.Text = _participante.NomeFantasia;
        ucEndereco.Logradouro = _participante.Logradouro;
        ucEndereco.Numero = _participante.Numero;
        ucEndereco.Complemento = _participante.Complemento;
        ucEndereco.Bairro = _participante.Bairro;
        ucEndereco.MunicipioCodigo = _participante.MunicipioCodigo;
        ucEndereco.Cep = _participante.Cep;
        txtFone.Text = _participante.Fone;
        txtEmail.Text = _participante.Email;
    }

    private void LerCampos()
    {
        _participante.CnpjCpf = txtCnpjCpf.Text.Trim();
        _participante.IndicadorIe = cboIndicadorIe.ValorSelecionado<IndicadorIe>();
        _participante.Ie = txtIe.Text.Trim();
        _participante.RazaoSocial = txtRazaoSocial.Text.Trim();
        _participante.NomeFantasia = txtNomeFantasia.Text.Trim();
        _participante.Logradouro = ucEndereco.Logradouro;
        _participante.Numero = ucEndereco.Numero;
        _participante.Complemento = ucEndereco.Complemento;
        _participante.Bairro = ucEndereco.Bairro;
        _participante.MunicipioCodigo = ucEndereco.MunicipioCodigo;
        _participante.Cep = ucEndereco.Cep;
        _participante.Fone = txtFone.Text.Trim();
        _participante.Email = txtEmail.Text.Trim();
    }

    /// <summary>Copia os campos para o registro e fecha com OK se os obrigatórios estiverem preenchidos.</summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _participante.ValidarObrigatorios()))
        {
            return;
        }

        DialogResult = DialogResult.OK;
    }
}
