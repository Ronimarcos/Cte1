using System.ComponentModel;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Bloco de endereço reaproveitado pelos cadastros de empresa e de participante:
/// logradouro, número, complemento, bairro, município e CEP.
/// </summary>
public partial class UcEndereco : UserControl
{
    /// <summary>Cria o bloco já com a lista de municípios.</summary>
    public UcEndereco()
    {
        InitializeComponent();
        cboMunicipio.Carregar(DadosFicticios.Municipios.Select(m => new Opcao<int>(m.Codigo, m.Descricao)));
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
