using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição de veículo (§5.5). O tipo de rodado só vale para tração
/// e o proprietário só é pedido quando o veículo não é próprio.
/// </summary>
public partial class FormEdicaoVeiculo : Form
{
    private readonly Veiculo _veiculo;
    private readonly Func<Veiculo, Task> _gravar;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="veiculo">Cópia editável do veículo.</param>
    /// <param name="proprietarios">Proprietários de terceiros da empresa ativa.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="gravar">Inclui ou altera o veículo no banco.</param>
    internal FormEdicaoVeiculo(Veiculo veiculo, IEnumerable<Opcao<int>> proprietarios, bool novo, Func<Veiculo, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _veiculo = veiculo;
        _gravar = gravar;
        Text = novo ? "Novo veículo" : "Editar veículo";
        cboTipo.Carregar(OpcoesCadastro.TiposVeiculo);
        cboTipoRodado.Carregar(OpcoesCadastro.TiposRodado);
        cboTipoCarroceria.Carregar(OpcoesCadastro.TiposCarroceria);
        cboProprietario.Carregar(proprietarios);
        cboUf.Items.AddRange([.. OpcoesCadastro.Ufs]);
        CarregarCampos();
        AtualizarCamposDependentes();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        LerCampos();
        if (await GravacaoDialogo.GravarAsync(this, btnSalvar, _veiculo, _gravar))
        {
            DialogResult = DialogResult.OK;
        }
    }

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
        nudTara.Value = Math.Min(nudTara.Maximum, _veiculo.TaraKg);
        nudCapacidadeKg.Value = Math.Min(nudCapacidadeKg.Maximum, _veiculo.CapacidadeKg ?? 0);
        nudCapacidadeM3.Value = Math.Min(nudCapacidadeM3.Maximum, _veiculo.CapacidadeM3 ?? 0);
        cboTipoRodado.Selecionar(_veiculo.TipoRodado);
        cboTipoCarroceria.Selecionar(_veiculo.TipoCarroceria);
        chkProprio.Checked = _veiculo.Proprio;
        cboProprietario.Selecionar(_veiculo.ProprietarioId);
    }

    /// <summary>Copia os campos para o veículo; placa sem hífen e em maiúsculas, capacidade zero vira "não informada".</summary>
    private void LerCampos()
    {
        _veiculo.Tipo = cboTipo.ValorSelecionado<TipoVeiculo>();
        _veiculo.Placa = Formatos.SoAlfanumericos(txtPlaca.Text);
        _veiculo.Renavam = Formatos.SoDigitos(txtRenavam.Text);
        _veiculo.CodigoInterno = txtCodigoInterno.Text.Trim();
        _veiculo.Uf = cboUf.SelectedItem as string ?? string.Empty;
        _veiculo.TaraKg = (int)nudTara.Value;
        _veiculo.CapacidadeKg = nudCapacidadeKg.Value > 0 ? (int)nudCapacidadeKg.Value : null;
        _veiculo.CapacidadeM3 = nudCapacidadeM3.Value > 0 ? (int)nudCapacidadeM3.Value : null;
        _veiculo.TipoRodado = cboTipoRodado.ValorSelecionado<TipoRodado>();
        _veiculo.TipoCarroceria = cboTipoCarroceria.ValorSelecionado<TipoCarroceria>();
        _veiculo.Proprio = chkProprio.Checked;
        _veiculo.ProprietarioId = chkProprio.Checked ? null : cboProprietario.ValorSelecionado<int>();
    }
}
