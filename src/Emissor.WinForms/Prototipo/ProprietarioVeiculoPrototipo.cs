using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Proprietário fictício de veículo de terceiro (grupo <c>prop</c> do MDF-e), com os campos do §5.6.
/// </summary>
public sealed class ProprietarioVeiculoPrototipo : RegistroPrototipo
{
    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }

    /// <summary>CNPJ ou CPF formatado (<c>prop/CNPJ</c> ou <c>prop/CPF</c>).</summary>
    public string CnpjCpf { get; set; } = string.Empty;

    /// <summary>RNTRC do proprietário (<c>prop/RNTRC</c>).</summary>
    public string Rntrc { get; set; } = string.Empty;

    /// <summary>Nome ou razão social (<c>prop/xNome</c>).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Tipo do proprietário (<c>prop/tpProp</c>).</summary>
    public TipoProprietario? TipoProprietario { get; set; }

    /// <summary>Descrição do tipo para exibição.</summary>
    public string TipoProprietarioDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposProprietario, TipoProprietario);

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (string.IsNullOrWhiteSpace(CnpjCpf)) faltando.Add("CNPJ/CPF");
        if (string.IsNullOrWhiteSpace(Rntrc)) faltando.Add("RNTRC");
        if (string.IsNullOrWhiteSpace(Nome)) faltando.Add("Nome");
        if (TipoProprietario is null) faltando.Add("Tipo do proprietário");
        return faltando;
    }
}
