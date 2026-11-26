using System.Globalization;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Formularios.Eventos;

/// <summary>
/// Carta de correção do CT-e: uma linha por correção, com o grupo e o campo do leiaute, o número do item (quando o
/// grupo se repete) e o valor correto. Os campos proibidos pelo MOC (item 13) são recusados na conferência.
/// </summary>
public partial class FormCartaCorrecao : Form
{
    private const string ColunaGrupo = "Grupo";
    private const string ColunaCampo = "Campo";
    private const string ColunaItem = "Item";
    private const string ColunaValor = "Valor";

    private readonly EventoDocumento _evento;

    /// <summary>Cria a tela para o evento já identificado (tipo, documento e sequência).</summary>
    /// <param name="evento">Evento a preencher.</param>
    /// <param name="documento">Descrição do documento.</param>
    internal FormCartaCorrecao(EventoDocumento evento, string documento)
    {
        InitializeComponent();
        _evento = evento;
        Text = $"{evento.Tipo.Descricao()} nº {evento.Sequencia}";
        lblDocumento.Text = documento;
        lblAviso.Text = TelaEvento.AvisoTeste;
        CriarColunas();
    }

    private void btnGerar_Click(object? sender, EventArgs e)
    {
        dgvCorrecoes.EndEdit();
        var correcoes = new List<CorrecaoCte>();
        var pendencias = new List<string>();
        foreach (DataGridViewRow linha in dgvCorrecoes.Rows)
        {
            if (linha.IsNewRow)
            {
                continue;
            }

            var grupo = Texto(linha, ColunaGrupo);
            var campo = Texto(linha, ColunaCampo);
            var item = Texto(linha, ColunaItem);
            var valor = Texto(linha, ColunaValor);
            if (grupo.Length + campo.Length + item.Length + valor.Length == 0)
            {
                continue;
            }

            int? numeroItem = null;
            if (item.Length > 0)
            {
                if (int.TryParse(item, NumberStyles.None, CultureInfo.InvariantCulture, out var numero))
                {
                    numeroItem = numero;
                }
                else
                {
                    pendencias.Add($"Correção {correcoes.Count + 1}: o item deve ser um número de 1 a 99.");
                }
            }

            correcoes.Add(new CorrecaoCte { Grupo = grupo, Campo = campo, Valor = valor, NumeroItem = numeroItem });
        }

        _evento.Correcoes = correcoes;
        if (pendencias.Count > 0)
        {
            Cadastros.AvisosCadastro.HaPendencias(this, Text, pendencias);
            return;
        }

        if (TelaEvento.Conferir(this, _evento))
        {
            DialogResult = DialogResult.OK;
        }
    }

    private static string Texto(DataGridViewRow linha, string coluna) => (linha.Cells[coluna].Value as string ?? string.Empty).Trim();

    private void CriarColunas()
    {
        dgvCorrecoes.Columns.Add(new DataGridViewTextBoxColumn { Name = ColunaGrupo, HeaderText = "Grupo", FillWeight = 16, MaxInputLength = 20 });
        dgvCorrecoes.Columns.Add(new DataGridViewTextBoxColumn { Name = ColunaCampo, HeaderText = "Campo", FillWeight = 16, MaxInputLength = 20 });
        dgvCorrecoes.Columns.Add(new DataGridViewTextBoxColumn { Name = ColunaItem, HeaderText = "Item", FillWeight = 8, MaxInputLength = 2 });
        dgvCorrecoes.Columns.Add(new DataGridViewTextBoxColumn { Name = ColunaValor, HeaderText = "Valor correto", FillWeight = 60, MaxInputLength = 500 });
    }
}
