using System.Text.RegularExpressions;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Dominio.Entidades;

/// <summary>Situação de validade do certificado em relação a uma data.</summary>
public enum SituacaoCertificado
{
    /// <summary>Dentro da validade, com folga.</summary>
    Valido,

    /// <summary>Vence dentro do prazo de alerta.</summary>
    VencendoEmBreve,

    /// <summary>Já venceu.</summary>
    Vencido,

    /// <summary>Ainda não começou a valer.</summary>
    AindaNaoValido,
}

/// <summary>
/// Certificado digital da empresa (§5.2). O A1 guarda o arquivo .pfx e a senha <b>sempre criptografados</b>
/// (a criptografia entra na fatia P3, atrás de <c>IProvedorCertificado</c>); o A3 é localizado no repositório
/// do Windows pelo thumbprint. Titular, CNPJ e validade são lidos do próprio certificado.
/// </summary>
public partial class CertificadoDigital : EntidadeDaEmpresa
{
    /// <summary>Dias de antecedência para alertar o vencimento.</summary>
    public const int DiasAlertaVencimento = 30;

    /// <summary>A1 ou A3.</summary>
    public TipoCertificado? Tipo { get; set; }

    /// <summary>Conteúdo do .pfx criptografado (somente A1). Nunca gravar em texto puro.</summary>
    public byte[]? ArquivoPfxCriptografado { get; set; }

    /// <summary>Senha do .pfx criptografada (somente A1). Nunca gravar em texto puro.</summary>
    public byte[]? SenhaCriptografada { get; set; }

    /// <summary>Thumbprint (40 caracteres hexadecimais), usado para achar o A3 no repositório do Windows.</summary>
    public string Thumbprint { get; set; } = string.Empty;

    /// <summary>Nome do titular, lido do certificado.</summary>
    public string Titular { get; set; } = string.Empty;

    /// <summary>CNPJ do titular sem máscara, lido do certificado.</summary>
    public string CnpjTitular { get; set; } = string.Empty;

    /// <summary>Início da validade.</summary>
    public DateTime ValidoDe { get; set; }

    /// <summary>Fim da validade.</summary>
    public DateTime ValidoAte { get; set; }

    /// <summary>Situação da validade na data informada.</summary>
    /// <param name="hoje">Data de referência.</param>
    public SituacaoCertificado SituacaoEm(DateTime hoje)
    {
        if (hoje.Date < ValidoDe.Date) return SituacaoCertificado.AindaNaoValido;
        var dias = (ValidoAte.Date - hoje.Date).Days;
        return dias < 0 ? SituacaoCertificado.Vencido
            : dias <= DiasAlertaVencimento ? SituacaoCertificado.VencendoEmBreve
            : SituacaoCertificado.Valido;
    }

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (Tipo is null) faltando.Add("Tipo do certificado");
        if (Tipo == TipoCertificado.A1 && (ArquivoPfxCriptografado is not { Length: > 0 } || SenhaCriptografada is not { Length: > 0 }))
        {
            faltando.Add("Arquivo e senha do certificado A1");
        }

        if (!ThumbprintValido().IsMatch(Thumbprint)) faltando.Add("Thumbprint com 40 caracteres hexadecimais");
        if (string.IsNullOrWhiteSpace(Titular)) faltando.Add("Titular");
        if (!Formatos.CnpjOuCpfValido(CnpjTitular)) faltando.Add("CNPJ/CPF do titular");
        if (ValidoAte <= ValidoDe) faltando.Add("Validade (o fim deve ser depois do início)");
        return faltando;
    }

    [GeneratedRegex("^[0-9A-F]{40}$")]
    private static partial Regex ThumbprintValido();
}
