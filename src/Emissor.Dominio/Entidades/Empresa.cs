using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// Empresa emitente (§5.1). O emitente tem CNPJ ou CPF — exatamente um dos dois.
/// Os demais cadastros e os documentos pertencem a uma empresa (multiempresa).
/// </summary>
public class Empresa : EntidadeBase
{
    /// <summary>Razão social (<c>emit/xNome</c>, até 60).</summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome fantasia (<c>emit/xFant</c>, até 60).</summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>CNPJ alfanumérico sem máscara (<c>emit/CNPJ</c>); vazio quando o emitente é pessoa física.</summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>CPF sem máscara (<c>emit/CPF</c>); vazio quando o emitente é pessoa jurídica.</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>Inscrição Estadual (<c>emit/IE</c>).</summary>
    public string Ie { get; set; } = string.Empty;

    /// <summary>IE do substituto tributário (<c>emit/IEST</c>).</summary>
    public string IeSt { get; set; } = string.Empty;

    /// <summary>Código de Regime Tributário (<c>emit/CRT</c>).</summary>
    public Crt? Crt { get; set; } = Enums.Crt.SimplesNacional;

    /// <summary>Regime do IBS/CBS (<c>emit/tpRegimeIBSCBS</c>, NT 2026.005), opcional.</summary>
    public RegimeIbsCbs? RegimeIbsCbs { get; set; }

    /// <summary>RNTRC da empresa (8 dígitos).</summary>
    public string Rntrc { get; set; } = string.Empty;

    /// <summary>Endereço (<c>enderEmit</c>).</summary>
    public Endereco Endereco { get; set; } = new();

    /// <summary>Telefone só com dígitos (<c>enderEmit/fone</c>).</summary>
    public string Fone { get; set; } = string.Empty;

    /// <summary>E-mail para envio de XML e DACTE.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Ambiente padrão de emissão (<c>ide/tpAmb</c>); o padrão é homologação.</summary>
    public TipoAmbiente AmbientePadrao { get; set; } = TipoAmbiente.Homologacao;

    /// <summary>CNPJ ou CPF, o que estiver preenchido.</summary>
    public string Documento => Cnpj.Length > 0 ? Cnpj : Cpf;

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (string.IsNullOrWhiteSpace(RazaoSocial)) faltando.Add("Razão social");
        if ((Cnpj.Length > 0) == (Cpf.Length > 0)) faltando.Add("Informe o CNPJ ou o CPF (só um dos dois)");
        else if (Cnpj.Length > 0 && !Formatos.CnpjValido(Cnpj)) faltando.Add("CNPJ no formato do leiaute (12 letras ou dígitos + 2 dígitos)");
        else if (Cpf.Length > 0 && !Formatos.CpfValido(Cpf)) faltando.Add("CPF com 11 dígitos");
        if (Ie.Length > 0 && !Formatos.IeValida(Ie)) faltando.Add("Inscrição Estadual com 2 a 14 dígitos");
        if (IeSt.Length > 0 && !Formatos.IeValida(IeSt)) faltando.Add("IE do substituto tributário com 2 a 14 dígitos");
        if (Crt is null) faltando.Add("CRT");
        if (!Formatos.RntrcValido(Rntrc)) faltando.Add("RNTRC com 8 dígitos");
        Endereco.Validar(faltando);
        if (Fone.Length > 0 && !Formatos.FoneValido(Fone)) faltando.Add("Telefone com 6 a 14 dígitos");
        if (Email.Length > 0 && !Formatos.EmailValido(Email)) faltando.Add("E-mail válido");
        return faltando;
    }
}
