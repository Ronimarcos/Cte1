using System.ComponentModel;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Bloco de endereço reaproveitado pelos cadastros de empresa e de participante:
/// logradouro, número, complemento, bairro, município e CEP.
/// </summary>
public partial class UcEndereco : UserControl
{
    /// <summary>Cria o bloco; a lista de municípios é carregada pela tela com <see cref="CarregarMunicipios"/>.</summary>
    public UcEndereco()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Troca a lista de municípios (ex.: os 5.571 do IBGE vindos do banco). Com uma lista grande, o campo passa a
    /// aceitar digitação com sugestões ("Cuia" → "Cuiabá/MT"), e o texto digitado é conferido ao sair do campo.
    /// </summary>
    /// <param name="municipios">Opções "Nome/UF" com o código IBGE.</param>
    public void CarregarMunicipios(IReadOnlyList<Opcao<int>> municipios)
    {
        var atual = MunicipioCodigo;
        cboMunicipio.DropDownStyle = ComboBoxStyle.DropDown;
        cboMunicipio.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        cboMunicipio.AutoCompleteSource = AutoCompleteSource.ListItems;
        cboMunicipio.Carregar(municipios);
        cboMunicipio.Selecionar(atual);
        cboMunicipio.Leave -= cboMunicipio_Leave;
        cboMunicipio.Leave += cboMunicipio_Leave;
    }

    /// <summary>Seleciona o município cujo texto foi digitado por inteiro; senão, limpa o campo.</summary>
    private void cboMunicipio_Leave(object? sender, EventArgs e)
    {
        if (cboMunicipio.SelectedItem is not null && cboMunicipio.Text == cboMunicipio.SelectedItem.ToString())
        {
            return;
        }

        var indice = cboMunicipio.FindStringExact(cboMunicipio.Text);
        cboMunicipio.SelectedIndex = indice;
        if (indice < 0)
        {
            cboMunicipio.Text = string.Empty;
        }
    }

    /// <summary>Logradouro.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Logradouro
    {
        get => txtLogradouro.Text.Trim();
        set => txtLogradouro.Text = value;
    }

    /// <summary>Número.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Numero
    {
        get => txtNumero.Text.Trim();
        set => txtNumero.Text = value;
    }

    /// <summary>Complemento.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Complemento
    {
        get => txtComplemento.Text.Trim();
        set => txtComplemento.Text = value;
    }

    /// <summary>Bairro.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Bairro
    {
        get => txtBairro.Text.Trim();
        set => txtBairro.Text = value;
    }

    /// <summary>Código IBGE do município selecionado.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? MunicipioCodigo
    {
        get => cboMunicipio.ValorSelecionado<int>();
        set => cboMunicipio.Selecionar(value);
    }

    /// <summary>CEP.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Cep
    {
        get => txtCep.Text.Trim();
        set => txtCep.Text = value;
    }
}
