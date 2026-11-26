using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Eventos;

/// <summary>
/// Inclusão de condutor no MDF-e: escolhe um motorista do cadastro (preenche nome e CPF) ou digita os dados.
/// </summary>
public partial class FormInclusaoCondutor : Form
{
    private const int Digitar = 0;

    private readonly EventoDocumento _evento;
    private readonly IReadOnlyList<Motorista> _motoristas;

    /// <summary>Cria a tela para o evento já identificado (tipo, documento e sequência).</summary>
    /// <param name="evento">Evento a preencher.</param>
    /// <param name="documento">Descrição do documento.</param>
    /// <param name="motoristas">Motoristas cadastrados da empresa.</param>
    internal FormInclusaoCondutor(EventoDocumento evento, string documento, IReadOnlyList<Motorista> motoristas)
    {
        InitializeComponent();
        _evento = evento;
        _motoristas = motoristas.Where(m => m.Ativo).OrderBy(m => m.Nome).ToList();
        Text = $"{evento.Tipo.Descricao()} nº {evento.Sequencia}";
        lblDocumento.Text = documento;
        lblAviso.Text = TelaEvento.AvisoTeste;
        cboMotorista.Carregar(new[] { new Opcao<int>(Digitar, "(digitar nome e CPF)") }
            .Concat(_motoristas.Select(m => new Opcao<int>(m.Id, $"{m.Nome} — CPF {m.Cpf}"))));
        cboMotorista.SelectedIndex = 0;
    }

    private void cboMotorista_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_motoristas?.FirstOrDefault(m => m.Id == cboMotorista.ValorSelecionado<int>()) is { } motorista)
        {
            txtNome.Text = motorista.Nome;
            txtCpf.Text = motorista.Cpf;
        }
    }

    private void btnGerar_Click(object? sender, EventArgs e)
    {
        _evento.NomeCondutor = RegrasEvento.Normalizar(txtNome.Text);
        _evento.CpfCondutor = Formatos.SoDigitos(txtCpf.Text);
        if (TelaEvento.Conferir(this, _evento))
        {
            DialogResult = DialogResult.OK;
        }
    }
}
