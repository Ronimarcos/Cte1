using Emissor.Dominio.Entidades;
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

    /// <summary>
    /// Ponte provisória (até a fatia P2c): o diálogo de participante trabalha com a entidade do domínio, e o
    /// assistente de CT-e ainda com o protótipo.
    /// </summary>
    public Participante ParaEntidade() => new()
    {
        Id = Id,
        EmpresaId = EmpresaId,
        CnpjCpf = Dominio.Regras.Formatos.SoAlfanumericos(CnpjCpf),
        IndicadorIe = IndicadorIe,
        Ie = Ie,
        RazaoSocial = RazaoSocial,
        NomeFantasia = NomeFantasia,
        Endereco = new Endereco
        {
            Logradouro = Logradouro, Numero = Numero, Complemento = Complemento, Bairro = Bairro,
            MunicipioCodigo = MunicipioCodigo, Cep = Dominio.Regras.Formatos.SoDigitos(Cep),
        },
        Fone = Fone,
        Email = Email,
    };

    /// <summary>Copia para este participante do protótipo os dados editados na entidade.</summary>
    /// <param name="participante">Entidade editada no diálogo.</param>
    public void CopiarDe(Participante participante)
    {
        CnpjCpf = participante.CnpjCpf;
        IndicadorIe = participante.IndicadorIe;
        Ie = participante.Ie;
        RazaoSocial = participante.RazaoSocial;
        NomeFantasia = participante.NomeFantasia;
        Logradouro = participante.Endereco.Logradouro;
        Numero = participante.Endereco.Numero;
        Complemento = participante.Endereco.Complemento;
        Bairro = participante.Endereco.Bairro;
        MunicipioCodigo = participante.Endereco.MunicipioCodigo;
        Cep = participante.Endereco.Cep;
        Fone = participante.Fone;
        Email = participante.Email;
    }
}
