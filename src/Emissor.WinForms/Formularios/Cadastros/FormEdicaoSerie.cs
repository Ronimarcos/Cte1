using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Diálogo de inclusão e edição da série de numeração (§5.3), gravada no banco.
/// O próximo número só pode ser informado ao criar a série (para continuar a numeração de outro emissor);
/// depois ele avança apenas com a emissão dos documentos. A série repetida (mesmo modelo, número e ambiente)
/// é recusada pelo banco, e o diálogo continua aberto com a mensagem.
/// </summary>
public partial class FormEdicaoSerie : Form
{
    private readonly SerieDocumento _serie;
    private readonly Func<SerieDocumento, Task> _gravar;

    /// <summary>Cria o diálogo sobre a cópia do registro.</summary>
    /// <param name="serie">Cópia editável da série.</param>
    /// <param name="novo">Verdadeiro para inclusão.</param>
    /// <param name="gravar">Inclui ou altera no banco.</param>
    internal FormEdicaoSerie(SerieDocumento serie, bool novo, Func<SerieDocumento, Task> gravar)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnSalvar);
        _serie = serie;
        _gravar = gravar;
        Text = novo ? "Nova série de documento" : "Editar série de documento";

        cboModelo.Carregar(OpcoesCadastro.ModelosDocumento);
        cboAmbiente.Carregar(OpcoesCadastro.Ambientes);
        nudSerie.Maximum = SerieDocumento.SerieMaxima;
        nudProximoNumero.Maximum = SerieDocumento.NumeroMaximo;

        lblProximoNumero.Enabled = novo;
        nudProximoNumero.Enabled = novo;
        lblAvisoNumero.Text = novo
            ? "Informe o próximo número só se estiver continuando a numeração de outro emissor."
            : "O próximo número avança apenas com a emissão dos documentos.";

        CarregarCampos();
    }

    private async void btnSalvar_Click(object? sender, EventArgs e)
    {
        LerCampos();
        if (await GravacaoDialogo.GravarAsync(this, btnSalvar, _serie, _gravar))
        {
            DialogResult = DialogResult.OK;
        }
    }

    private void CarregarCampos()
    {
        cboModelo.Selecionar(_serie.Modelo);
        nudSerie.Value = Math.Clamp(_serie.Serie, 0, SerieDocumento.SerieMaxima);
        cboAmbiente.Selecionar<bool>(_serie.Ambiente == TipoAmbiente.Homologacao);
        nudProximoNumero.Value = Math.Clamp(_serie.ProximoNumero, 1, SerieDocumento.NumeroMaximo);
    }

    private void LerCampos()
    {
        _serie.Modelo = cboModelo.ValorSelecionado<ModeloDocumento>();
        _serie.Serie = (int)nudSerie.Value;

        // Sem escolha, fica em homologação: o ambiente seguro é o padrão.
        _serie.Ambiente = cboAmbiente.ValorSelecionado<bool>() is false ? TipoAmbiente.Producao : TipoAmbiente.Homologacao;
        _serie.ProximoNumero = (int)nudProximoNumero.Value;
    }
}
