using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição da série de numeração (§5.3).
/// O próximo número só pode ser informado ao criar a série (para continuar a numeração de outro emissor);
/// depois ele avança apenas com a emissão dos documentos.
/// </summary>
public partial class FormEdicaoSerie : Form
{
    private readonly SeriePrototipo _serie;
    private readonly Func<SeriePrototipo, bool> _existeDuplicada;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="serie">Cópia editável do registro.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="existeDuplicada">Indica se já existe outra série com o mesmo modelo, número e ambiente.</param>
    internal FormEdicaoSerie(SeriePrototipo serie, bool novo, Func<SeriePrototipo, bool> existeDuplicada)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _serie = serie;
        _existeDuplicada = existeDuplicada;
        Text = novo ? "Nova série de documento" : "Editar série de documento";

        cboModelo.Carregar(OpcoesCadastro.ModelosDocumento);
        cboAmbiente.Carregar(OpcoesCadastro.Ambientes);
        nudSerie.Maximum = SeriePrototipo.SerieMaxima;
        nudProximoNumero.Maximum = SeriePrototipo.NumeroMaximo;

        lblProximoNumero.Enabled = novo;
        nudProximoNumero.Enabled = novo;
        lblAvisoNumero.Text = novo
            ? "Informe o próximo número só se estiver continuando a numeração de outro emissor."
            : "O próximo número avança apenas com a emissão dos documentos.";

        CarregarCampos();
    }

    private void btnSalvar_Click(object? sender, EventArgs e) => Salvar();

    private void CarregarCampos()
    {
        cboModelo.Selecionar(_serie.Modelo);
        nudSerie.Value = _serie.Serie ?? 1;
        cboAmbiente.Selecionar<bool>(_serie.Homologacao);
        nudProximoNumero.Value = _serie.ProximoNumero ?? 1;
    }

    private void LerCampos()
    {
        _serie.Modelo = cboModelo.ValorSelecionado<ModeloDocumento>();
        _serie.Serie = (int)nudSerie.Value;
        _serie.Homologacao = cboAmbiente.ValorSelecionado<bool>() ?? true;
        _serie.ProximoNumero = (int)nudProximoNumero.Value;
    }

    /// <summary>Copia os campos para o registro e fecha com OK se estiver tudo preenchido e a série não for repetida.</summary>
    private void Salvar()
    {
        LerCampos();
        if (AvisosCadastro.HaPendencias(this, Text, _serie.ValidarObrigatorios()))
        {
            return;
        }

        if (_existeDuplicada(_serie))
        {
            MessageBox.Show(this,
                $"Já existe a série {_serie.Serie} do modelo {_serie.ModeloDescricao} no ambiente {_serie.AmbienteDescricao}.",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DialogResult = DialogResult.OK;
    }
}
