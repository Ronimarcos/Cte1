using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.WinForms.Formularios.Eventos;

/// <summary>Cancelamento do CT-e ou do MDF-e: pede a justificativa (15 a 255 caracteres).</summary>
public partial class FormEventoCancelamento : Form
{
    private readonly EventoDocumento _evento;

    /// <summary>Cria a tela para o evento já identificado (tipo, documento e sequência).</summary>
    /// <param name="evento">Evento a preencher.</param>
    /// <param name="documento">Descrição do documento.</param>
    internal FormEventoCancelamento(EventoDocumento evento, string documento)
    {
        InitializeComponent();
        _evento = evento;
        Text = evento.Tipo.Descricao();
        lblDocumento.Text = documento;
        lblAviso.Text = TelaEvento.AvisoTeste;
        txtJustificativa.Text = evento.Justificativa;
        AtualizarContador();
    }

    private void txtJustificativa_TextChanged(object? sender, EventArgs e) => AtualizarContador();

    private void btnGerar_Click(object? sender, EventArgs e)
    {
        _evento.Justificativa = RegrasEvento.Normalizar(txtJustificativa.Text);
        if (TelaEvento.Conferir(this, _evento))
        {
            DialogResult = DialogResult.OK;
        }
    }

    private void AtualizarContador()
    {
        var tamanho = RegrasEvento.Normalizar(txtJustificativa.Text).Length;
        lblContador.Text = $"{tamanho} de {RegrasEvento.JustificativaMaxima} (mínimo {RegrasEvento.JustificativaMinima})";
        lblContador.ForeColor = tamanho < RegrasEvento.JustificativaMinima ? Color.Firebrick : SystemColors.ControlText;
    }
}
