using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Eventos;

/// <summary>
/// Encerramento do MDF-e: data, UF e município do encerramento. Sugere a UF de descarregamento final e o último
/// município de descarga do MDF-e.
/// </summary>
public partial class FormEncerramentoMdfe : Form
{
    private readonly EventoDocumento _evento;
    private readonly CatalogoMunicipios _municipios;

    /// <summary>Cria a tela para o evento já identificado (tipo, documento e sequência).</summary>
    /// <param name="evento">Evento a preencher.</param>
    /// <param name="documento">Descrição do documento.</param>
    /// <param name="municipios">Municípios do IBGE.</param>
    /// <param name="mdfe">MDF-e, para sugerir o local do encerramento.</param>
    internal FormEncerramentoMdfe(EventoDocumento evento, string documento, CatalogoMunicipios municipios, ManifestoEletronico? mdfe)
    {
        InitializeComponent();
        _evento = evento;
        _municipios = municipios;
        Text = evento.Tipo.Descricao();
        lblDocumento.Text = documento;
        lblAviso.Text = TelaEvento.AvisoTeste;
        dtpData.MaxDate = DateTime.Today;
        dtpData.Value = DateTime.Today;
        cboUf.Carregar(Formatos.Ufs.Order().Select(uf => new Opcao<string>(uf, uf)));
        cboMunicipio.ConfigurarBuscaPorTexto();

        var municipioSugerido = mdfe?.Descargas.LastOrDefault()?.MunicipioCodigo;
        var uf = _municipios.Obter(municipioSugerido)?.Uf ?? mdfe?.UfFim;
        if (!string.IsNullOrEmpty(uf))
        {
            cboUf.SelectedItem = cboUf.Items.OfType<Opcao<string>>().FirstOrDefault(o => o.Valor == uf);
            cboMunicipio.Selecionar(municipioSugerido);
        }
    }

    private void cboUf_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var uf = (cboUf.SelectedItem as Opcao<string>)?.Valor;
        cboMunicipio.Carregar(uf is null ? [] : _municipios.OpcoesDaUf(uf));
    }

    private void btnGerar_Click(object? sender, EventArgs e)
    {
        _evento.DataEncerramento = dtpData.Value.Date;
        _evento.MunicipioEncerramentoCodigo = cboMunicipio.ValorSelecionado<int>();
        if (TelaEvento.Conferir(this, _evento))
        {
            DialogResult = DialogResult.OK;
        }
    }
}
