using Emissor.Dominio.Enums;
using Emissor.Dominio.Entidades;
using System.Globalization;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>Diálogo para marcar CT-e emitidos por este sistema e vinculá-los a um município de descarga do MDF-e.</summary>
public partial class FormSelecaoCtes : Form
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Cria o diálogo com os CT-e disponíveis.</summary>
    /// <param name="ctes">CT-e que podem ser vinculados (prontos e ainda não usados neste MDF-e).</param>
    /// <param name="cadastros">Cadastros da empresa, para os nomes dos participantes.</param>
    internal FormSelecaoCtes(IEnumerable<ConhecimentoTransporte> ctes, CadastrosEmissao cadastros)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnOk);
        foreach (var cte in ctes)
        {
            clbCtes.Items.Add(new Opcao<int>(cte.Id,
                $"{cte.DataEmissao:dd/MM/yyyy} - {cte.TipoDescricao} - {cadastros.NomeParticipante(cte.RemetenteId)} → {cadastros.NomeParticipante(cte.DestinatarioId)} - {cte.ValorTotalPrestacao.ToString("C2", Brasil)}"));
        }

        lblVazio.Visible = clbCtes.Items.Count == 0;
    }

    /// <summary>CT-e marcados; preenchido quando o diálogo retorna OK.</summary>
    internal IReadOnlyList<int> Selecionados { get; private set; } = [];

    private void btnOk_Click(object? sender, EventArgs e)
    {
        Selecionados = clbCtes.CheckedItems.OfType<Opcao<int>>().Select(o => o.Valor).ToList();
        DialogResult = DialogResult.OK;
    }
}
