using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Participante fictício (remetente, destinatário, expedidor, recebedor ou tomador), com os campos do §5.4.
/// O papel é definido no documento, não no cadastro.
/// </summary>
public sealed class ParticipantePrototipo : RegistroPrototipo
{
    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }

    /// <summary>CNPJ ou CPF formatado (<c>*/CNPJ</c> ou <c>*/CPF</c>).</summary>
    public string CnpjCpf { get; set; } = string.Empty;

    /// <summary>Indicador da Inscrição Estadual (<c>ide/indIEToma</c> quando tomador).</summary>
    public IndicadorIe? IndicadorIe { get; set; }

    /// <summary>Inscrição Estadual (<c>*/IE</c>).</summary>
    public string Ie { get; set; } = string.Empty;

    /// <summary>Razão social ou nome (<c>*/xNome</c>).</summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome fantasia (<c>*/xFant</c>).</summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>Logradouro.</summary>
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>Número.</summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>Complemento.</summary>
    public string Complemento { get; set; } = string.Empty;

    /// <summary>Bairro.</summary>
    public string Bairro { get; set; } = string.Empty;

    /// <summary>Código IBGE do município.</summary>
    public int? MunicipioCodigo { get; set; }

    /// <summary>CEP.</summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>Telefone (<c>*/fone</c>).</summary>
    public string Fone { get; set; } = string.Empty;

    /// <summary>E-mail (<c>*/email</c>).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Município no formato "Nome/UF".</summary>
    public string MunicipioDescricao => DadosFicticios.ObterMunicipio(MunicipioCodigo)?.Descricao ?? string.Empty;

    /// <summary>Descrição do indicador de IE para exibição.</summary>
    public string IndicadorIeDescricao => OpcoesCadastro.Texto(OpcoesCadastro.IndicadoresIe, IndicadorIe);

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (string.IsNullOrWhiteSpace(CnpjCpf)) faltando.Add("CNPJ/CPF");
        if (IndicadorIe is null) faltando.Add("Indicador da IE");
        if (IndicadorIe == Dominio.Enums.IndicadorIe.Contribuinte && string.IsNullOrWhiteSpace(Ie)) faltando.Add("Inscrição Estadual (obrigatória para contribuinte)");
        if (string.IsNullOrWhiteSpace(RazaoSocial)) faltando.Add("Razão social");
        if (string.IsNullOrWhiteSpace(Logradouro)) faltando.Add("Logradouro");
        if (string.IsNullOrWhiteSpace(Numero)) faltando.Add("Número");
        if (string.IsNullOrWhiteSpace(Bairro)) faltando.Add("Bairro");
        if (MunicipioCodigo is null) faltando.Add("Município");
        return faltando;
    }
}
