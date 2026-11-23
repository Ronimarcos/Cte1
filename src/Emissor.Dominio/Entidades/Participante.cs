using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// Participante do CT-e (§5.4): remetente, destinatário, expedidor, recebedor ou tomador.
/// O papel é definido em cada documento, não no cadastro.
/// </summary>
public class Participante : EntidadeDaEmpresa
{
    /// <summary>CNPJ (alfanumérico, 14) ou CPF (11) sem máscara (<c>*/CNPJ</c> ou <c>*/CPF</c>).</summary>
    public string CnpjCpf { get; set; } = string.Empty;

    /// <summary>Indicador da IE (<c>ide/indIEToma</c> quando tomador).</summary>
    public IndicadorIe? IndicadorIe { get; set; }

    /// <summary>Inscrição Estadual (<c>*/IE</c>), obrigatória para contribuinte.</summary>
    public string Ie { get; set; } = string.Empty;

    /// <summary>Razão social ou nome (<c>*/xNome</c>, até 60).</summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome fantasia (<c>*/xFant</c>, até 60).</summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>Endereço (<c>*/ender*</c>).</summary>
    public Endereco Endereco { get; set; } = new();

    /// <summary>Telefone só com dígitos (<c>*/fone</c>).</summary>
    public string Fone { get; set; } = string.Empty;

    /// <summary>E-mail (<c>*/email</c>).</summary>
    public string Email { get; set; } = string.Empty;

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (!Formatos.CnpjOuCpfValido(CnpjCpf)) faltando.Add("CNPJ ou CPF no formato do leiaute");
        if (IndicadorIe is null) faltando.Add("Indicador da IE");
        if (IndicadorIe == Enums.IndicadorIe.Contribuinte && !Formatos.IeValida(Ie)) faltando.Add("Inscrição Estadual (obrigatória para contribuinte, 2 a 14 dígitos)");
        if (string.IsNullOrWhiteSpace(RazaoSocial)) faltando.Add("Razão social");
        Endereco.Validar(faltando);
        if (Fone.Length > 0 && !Formatos.FoneValido(Fone)) faltando.Add("Telefone com 6 a 14 dígitos");
        if (Email.Length > 0 && !Formatos.EmailValido(Email)) faltando.Add("E-mail válido");
        return faltando;
    }
}
