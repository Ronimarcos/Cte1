using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Empresa (emitente) fictícia usada no protótipo, com os campos do §5.1 do levantamento.
/// Será substituída pela entidade <c>Empresa</c> do domínio quando a persistência for implementada.
/// </summary>
public sealed class EmpresaPrototipo : RegistroPrototipo
{
    /// <summary>Razão social (<c>emit/xNome</c>).</summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome fantasia (<c>emit/xFant</c>).</summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>CNPJ formatado para exibição (<c>emit/CNPJ</c>).</summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>Inscrição Estadual (<c>emit/IE</c>).</summary>
    public string Ie { get; set; } = string.Empty;

    /// <summary>Inscrição Estadual do substituto tributário (<c>emit/IEST</c>).</summary>
    public string IeSt { get; set; } = string.Empty;

    /// <summary>Código de Regime Tributário (<c>emit/CRT</c>).</summary>
    public Crt? Crt { get; set; } = Dominio.Enums.Crt.SimplesNacional;

    /// <summary>Regime de apuração do IBS e da CBS (<c>emit/tpRegimeIBSCBS</c>), opcional.</summary>
    public RegimeIbsCbs? RegimeIbsCbs { get; set; }

    /// <summary>RNTRC da empresa (<c>rodo/RNTRC</c> e <c>infANTT/RNTRC</c>).</summary>
    public string Rntrc { get; set; } = string.Empty;

    /// <summary>Logradouro (<c>enderEmit/xLgr</c>).</summary>
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>Número (<c>enderEmit/nro</c>).</summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>Complemento (<c>enderEmit/xCpl</c>).</summary>
    public string Complemento { get; set; } = string.Empty;

    /// <summary>Bairro (<c>enderEmit/xBairro</c>).</summary>
    public string Bairro { get; set; } = string.Empty;

    /// <summary>Código IBGE do município (<c>enderEmit/cMun</c>).</summary>
    public int? MunicipioCodigo { get; set; }

    /// <summary>CEP (<c>enderEmit/CEP</c>).</summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>Telefone (<c>enderEmit/fone</c>).</summary>
    public string Fone { get; set; } = string.Empty;

    /// <summary>E-mail para envio de XML e DACTE.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Verdadeiro quando o ambiente padrão é homologação (<c>tpAmb</c> 2).</summary>
    public bool Homologacao { get; set; } = true;

    /// <summary>UF do emitente, obtida do município.</summary>
    public string Uf => DadosFicticios.ObterMunicipio(MunicipioCodigo)?.Uf ?? string.Empty;

    /// <summary>Município no formato "Nome/UF".</summary>
    public string MunicipioDescricao => DadosFicticios.ObterMunicipio(MunicipioCodigo)?.Descricao ?? string.Empty;

    /// <summary>Descrição do ambiente para exibição em grades e na barra de status.</summary>
    public string AmbienteDescricao => Homologacao ? "Homologação" : "Produção";

    /// <summary>Descrição do CRT para exibição.</summary>
    public string CrtDescricao => OpcoesCadastro.Texto(OpcoesCadastro.Crts, Crt);

    /// <summary>
    /// Ponte provisória (fatia P2a): converte a empresa gravada no banco para o formato que os assistentes de
    /// CT-e e MDF-e e os cadastros ainda em memória usam. Some na fatia P2c.
    /// </summary>
    /// <param name="empresa">Empresa do banco.</param>
    public static EmpresaPrototipo DeEntidade(Empresa empresa) => new()
    {
        Id = empresa.Id,
        RazaoSocial = empresa.RazaoSocial,
        NomeFantasia = empresa.NomeFantasia,
        Cnpj = empresa.Documento,
        Ie = empresa.Ie,
        IeSt = empresa.IeSt,
        Crt = empresa.Crt,
        RegimeIbsCbs = empresa.RegimeIbsCbs,
        Rntrc = empresa.Rntrc,
        Logradouro = empresa.Endereco.Logradouro,
        Numero = empresa.Endereco.Numero,
        Complemento = empresa.Endereco.Complemento,
        Bairro = empresa.Endereco.Bairro,
        MunicipioCodigo = empresa.Endereco.MunicipioCodigo,
        Cep = empresa.Endereco.Cep,
        Fone = empresa.Fone,
        Email = empresa.Email,
        Homologacao = empresa.AmbientePadrao == TipoAmbiente.Homologacao,
    };

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (string.IsNullOrWhiteSpace(RazaoSocial)) faltando.Add("Razão social");
        if (string.IsNullOrWhiteSpace(Cnpj)) faltando.Add("CNPJ");
        if (Crt is null) faltando.Add("CRT");
        if (string.IsNullOrWhiteSpace(Rntrc)) faltando.Add("RNTRC");
        if (string.IsNullOrWhiteSpace(Logradouro)) faltando.Add("Logradouro");
        if (string.IsNullOrWhiteSpace(Numero)) faltando.Add("Número");
        if (string.IsNullOrWhiteSpace(Bairro)) faltando.Add("Bairro");
        if (MunicipioCodigo is null) faltando.Add("Município");
        return faltando;
    }
}
