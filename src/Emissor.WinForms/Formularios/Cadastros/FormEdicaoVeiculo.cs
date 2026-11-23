using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição de veículo (§5.5). O tipo de rodado só vale para tração
/// e o proprietário só é pedido quando o veículo não é próprio.
/// </summary>
public partial class FormEdicaoVeiculo : Form
{
    private readonly VeiculoPrototipo _veiculo;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="veiculo">Cópia editável do registro.</param>
    /// <param name="proprietarios">Proprietários de terceiros da empresa ativa.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    internal FormEdicaoVeiculo(VeiculoPrototipo veiculo, IEnumerable<Opcao<int>> proprietarios, bool novo)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _veiculo = veiculo;
        Text = novo ? "Novo veículo" : "Editar veículo";
        cboTipo.Carregar(OpcoesCadastro.TiposVeiculo);
        cboTipoRodado.Carregar(OpcoesCadastro.TiposRodado);
        cboTipoCarroceria.Carregar(OpcoesCadastro.TiposCarroceria);
        cboProprietario.Carregar(proprietarios);
        cboUf.Items.AddRange([.. OpcoesCadastro.Ufs]);
        CarregarCampos();
        AtualizarCamposDependentes();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    private void cboTipo_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarCamposDependentes();

    private void chkProprio_CheckedChanged(object? sender, EventArgs e) => AtualizarCamposDependentes();

    /// <summary>Habilita o tipo de rodado só para tração e o proprietário só para veículo de terceiro.</summary>
    private void AtualizarCamposDependentes()
    {
        var tracao = cboTipo.ValorSelecionado<TipoVeiculo>() == TipoVeiculo.Tracao;
        cboTipoRodado.Enabled = tracao;
        if (!tracao)
        {
            cboTipoRodado.SelectedIndex = -1;
        }

        cboProprietario.Enabled = !chkProprio.Checked;
        if (chkProprio.Checked)
        {
            cboProprietario.SelectedIndex = -1;
        }
    }

    private void CarregarCampos()
    {
        cboTipo.Selecionar(_veiculo.Tipo);
        txtPlaca.Text = _veiculo.Placa;
        txtRenavam.Text = _veiculo.Renavam;
        txtCodigoInterno.Text = _veiculo.CodigoInterno;
        cboUf.SelectedItem = string.IsNullOrEmpty(_veiculo.Uf) ? null : _veiculo.Uf;
        nudTara.Value = _veiculo.TaraKg;
        nudCapacidadeKg.Value = _veiculo.CapacidadeKg;
        nudCapacidadeM3.Value = _veiculo.CapacidadeM3;
        cboTipoRodado.Selecionar(_veiculo.TipoRodado);
        cboTipoCarroceria.Selecionar(_veiculo.TipoCarroceria);
        chkProprio.Checked = _veiculo.Proprio;
        cboProprietario.Selecionar(_veiculo.ProprietarioId);
    }

    private void LerCampos()
    {
        _veiculo.Tipo = cboTipo.ValorSelecionado<TipoVeiculo>();
        _veiculo.Placa = txtPlaca.Text.Trim().ToUpperInvariant();
        _veiculo.Renavam = txtRenavam.Text.Trim();
        _veiculo.CodigoInterno = txtCodigoInterno.Text.Trim();
        _veiculo.Uf = cboUf.SelectedItem as string ?? string.Empty;
        _veiculo.TaraKg = (int)nudTara.Value;
        _veiculo.CapacidadeKg = (int)nudCapacidadeKg.Value;
        _veiculo.CapacidadeM3 = (int)nudCapacidadeM3.Value;
        _veiculo.TipoRodado = cboTipoRodado.ValorSelecionado<TipoRodado>();
        _veiculo.TipoCarroceria = cboTipoCarroceria.ValorSelecionado<TipoCarroceria>();
        _veiculo.Proprio = chkProprio.Checked;
        _veiculo.ProprietarioId = chkProprio.Checked ? null : cboProprietario.ValorSelecionado<int>();
    }

    /// <summary>Copia os campos para o registro e fecha com OK se os obrigatórios estiverem preenchidos.</summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _veiculo.ValidarObrigatorios()))
        {
            return;
        }

        DialogResult = DialogResult.OK;
    }
}
