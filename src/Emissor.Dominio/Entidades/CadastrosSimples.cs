using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Dominio.Entidades;

/// <summary>Proprietário de veículo de terceiro (§5.6), grupo <c>prop</c> do MDF-e.</summary>
public class ProprietarioVeiculo : EntidadeDaEmpresa
{
    /// <summary>CNPJ ou CPF sem máscara (<c>prop/CNPJ</c> ou <c>prop/CPF</c>).</summary>
    public string CnpjCpf { get; set; } = string.Empty;

    /// <summary>RNTRC (<c>prop/RNTRC</c>, 8 dígitos).</summary>
    public string Rntrc { get; set; } = string.Empty;

    /// <summary>Nome ou razão social (<c>prop/xNome</c>, até 60).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Tipo do proprietário (<c>prop/tpProp</c>).</summary>
    public TipoProprietario? TipoProprietario { get; set; }

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (!Formatos.CnpjOuCpfValido(CnpjCpf)) faltando.Add("CNPJ ou CPF no formato do leiaute");
        if (!Formatos.RntrcValido(Rntrc)) faltando.Add("RNTRC com 8 dígitos");
        if (string.IsNullOrWhiteSpace(Nome)) faltando.Add("Nome");
        if (TipoProprietario is null) faltando.Add("Tipo do proprietário");
        return faltando;
    }
}

/// <summary>Motorista (§5.7), condutor do MDF-e (<c>condutor</c>).</summary>
public class Motorista : EntidadeDaEmpresa
{
    /// <summary>Nome (<c>condutor/xNome</c>, até 60).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>CPF sem máscara (<c>condutor/CPF</c>).</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>Telefone para contato (uso interno).</summary>
    public string Telefone { get; set; } = string.Empty;

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (string.IsNullOrWhiteSpace(Nome)) faltando.Add("Nome");
        if (!Formatos.CpfValido(Cpf)) faltando.Add("CPF com 11 dígitos");
        return faltando;
    }
}

/// <summary>Apólice de seguro da carga (§5.8), grupo <c>seg</c> do MDF-e.</summary>
public class ApoliceSeguro : EntidadeDaEmpresa
{
    /// <summary>Responsável pelo seguro (<c>infResp/respSeg</c>).</summary>
    public ResponsavelSeguro? Responsavel { get; set; }

    /// <summary>CNPJ ou CPF do contratante (<c>infResp/CNPJ</c> ou <c>CPF</c>); só quando o responsável é o contratante.</summary>
    public string CnpjCpfResponsavel { get; set; } = string.Empty;

    /// <summary>Nome da seguradora (<c>infSeg/xSeg</c>, até 30).</summary>
    public string NomeSeguradora { get; set; } = string.Empty;

    /// <summary>CNPJ da seguradora (<c>infSeg/CNPJ</c>).</summary>
    public string CnpjSeguradora { get; set; } = string.Empty;

    /// <summary>Número da apólice (<c>nApol</c>, até 20).</summary>
    public string NumeroApolice { get; set; } = string.Empty;

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (Responsavel is null) faltando.Add("Responsável pelo seguro");
        if (Responsavel == ResponsavelSeguro.Contratante && !Formatos.CnpjOuCpfValido(CnpjCpfResponsavel)) faltando.Add("CNPJ/CPF do contratante");
        if (Responsavel == ResponsavelSeguro.Emitente && CnpjCpfResponsavel.Length > 0) faltando.Add("CNPJ/CPF do responsável só quando é o contratante");
        if (string.IsNullOrWhiteSpace(NomeSeguradora)) faltando.Add("Seguradora");
        if (!Formatos.CnpjValido(CnpjSeguradora)) faltando.Add("CNPJ da seguradora no formato do leiaute");
        if (string.IsNullOrWhiteSpace(NumeroApolice)) faltando.Add("Número da apólice");
        return faltando;
    }
}

/// <summary>
/// Série de numeração (§5.3). Chave única: empresa, modelo, série e ambiente.
/// O próximo número avança dentro de uma transação, ao reservar o número do documento.
/// </summary>
public class SerieDocumento : EntidadeDaEmpresa
{
    /// <summary>Maior série do leiaute (<c>TSerie</c>: 0 a 999).</summary>
    public const int SerieMaxima = 999;

    /// <summary>Maior número de documento (<c>nCT</c>/<c>nMDF</c>: 1 a 999.999.999).</summary>
    public const int NumeroMaximo = 999_999_999;

    /// <summary>Modelo (<c>ide/mod</c>).</summary>
    public ModeloDocumento? Modelo { get; set; }

    /// <summary>Série (<c>ide/serie</c>).</summary>
    public int Serie { get; set; } = 1;

    /// <summary>Ambiente (<c>ide/tpAmb</c>); a numeração é separada por ambiente.</summary>
    public TipoAmbiente Ambiente { get; set; } = TipoAmbiente.Homologacao;

    /// <summary>Próximo número a usar (<c>ide/nCT</c> ou <c>ide/nMDF</c>).</summary>
    public int ProximoNumero { get; set; } = 1;

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (Modelo is null) faltando.Add("Modelo");
        if (Serie is < 0 or > SerieMaxima) faltando.Add($"Série (0 a {SerieMaxima})");
        if (ProximoNumero is < 1 or > NumeroMaximo) faltando.Add("Próximo número (1 a 999.999.999)");
        return faltando;
    }
}
