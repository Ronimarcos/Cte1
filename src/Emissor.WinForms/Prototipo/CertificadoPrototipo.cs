using System.Text.RegularExpressions;
using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>Situação de validade do certificado, usada para destacar a linha na lista.</summary>
public enum SituacaoCertificado
{
    /// <summary>Validade não informada.</summary>
    Indefinida,

    /// <summary>Dentro da validade, com folga.</summary>
    Valido,

    /// <summary>Vence dentro do prazo de alerta.</summary>
    VencendoEmBreve,

    /// <summary>Já venceu.</summary>
    Vencido,
}

/// <summary>
/// Certificado digital fictício (§5.2). No protótipo o arquivo .pfx e a senha <b>não</b> são lidos nem guardados:
/// só o caminho do arquivo é exibido, e titular, CNPJ e validade são digitados. Na versão real esses dados
/// são lidos do próprio certificado, e arquivo e senha ficam criptografados.
/// </summary>
public sealed partial class CertificadoPrototipo : RegistroPrototipo
{
    /// <summary>Dias de antecedência para alertar o vencimento.</summary>
    public const int DiasAlertaVencimento = 30;

    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }

    /// <summary>Tipo do certificado (A1 ou A3).</summary>
    public TipoCertificado? Tipo { get; set; }

    /// <summary>Caminho do arquivo .pfx escolhido (somente A1; apenas exibido no protótipo).</summary>
    public string CaminhoArquivo { get; set; } = string.Empty;

    /// <summary>Thumbprint do certificado no repositório do Windows (somente A3).</summary>
    public string Thumbprint { get; set; } = string.Empty;

    /// <summary>Nome do titular do certificado.</summary>
    public string Titular { get; set; } = string.Empty;

    /// <summary>CNPJ do titular, formatado.</summary>
    public string CnpjTitular { get; set; } = string.Empty;

    /// <summary>Início da validade.</summary>
    public DateTime? ValidoDe { get; set; }

    /// <summary>Fim da validade.</summary>
    public DateTime? ValidoAte { get; set; }

    /// <summary>Descrição do tipo para exibição.</summary>
    public string TipoDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposCertificado, Tipo);

    /// <summary>Validade no formato "dd/MM/aaaa a dd/MM/aaaa".</summary>
    public string ValidadeDescricao =>
        ValidoDe is null || ValidoAte is null ? string.Empty : $"{ValidoDe:dd/MM/yyyy} a {ValidoAte:dd/MM/yyyy}";

    /// <summary>Situação de validade em relação à data de hoje.</summary>
    public SituacaoCertificado Situacao
    {
        get
        {
            if (ValidoAte is null)
            {
                return SituacaoCertificado.Indefinida;
            }

            var dias = (ValidoAte.Value.Date - DateTime.Today).Days;
            return dias < 0 ? SituacaoCertificado.Vencido
                : dias <= DiasAlertaVencimento ? SituacaoCertificado.VencendoEmBreve
                : SituacaoCertificado.Valido;
        }
    }

    /// <summary>Texto da situação para a lista (ex.: "Vence em 12 dias").</summary>
    public string SituacaoDescricao => Situacao switch
    {
        SituacaoCertificado.Vencido => "Vencido",
        SituacaoCertificado.VencendoEmBreve => DiasParaVencer switch
        {
            0 => "Vence hoje",
            1 => "Vence amanhã",
            var dias => $"Vence em {dias} dias",
        },
        SituacaoCertificado.Valido => "Válido",
        _ => string.Empty,
    };

    private int DiasParaVencer => ValidoAte is null ? 0 : (ValidoAte.Value.Date - DateTime.Today).Days;

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (Tipo is null) faltando.Add("Tipo do certificado");
        if (Tipo == TipoCertificado.A1 && string.IsNullOrWhiteSpace(CaminhoArquivo)) faltando.Add("Arquivo do certificado (.pfx)");
        if (Tipo == TipoCertificado.A3 && !ThumbprintValido().IsMatch(Thumbprint)) faltando.Add("Thumbprint (40 caracteres hexadecimais)");
        if (string.IsNullOrWhiteSpace(Titular)) faltando.Add("Titular");
        if (string.IsNullOrWhiteSpace(CnpjTitular)) faltando.Add("CNPJ do titular");
        if (ValidoDe is null || ValidoAte is null) faltando.Add("Validade");
        else if (ValidoAte < ValidoDe) faltando.Add("Validade (o fim deve ser depois do início)");
        return faltando;
    }

    [GeneratedRegex("^[0-9A-Fa-f]{40}$")]
    private static partial Regex ThumbprintValido();
}
