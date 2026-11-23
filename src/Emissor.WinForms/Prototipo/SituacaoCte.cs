namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Situação do CT-e no protótipo. Não há situações de SEFAZ (autorizado, rejeitado, cancelado) porque
/// o protótipo não transmite: elas entram quando o motor DF-e for implementado.
/// </summary>
public enum SituacaoCte
{
    /// <summary>Rascunho salvo; pode faltar informação.</summary>
    EmDigitacao,

    /// <summary>Todas as etapas conferidas; aguarda a transmissão (ainda não implementada).</summary>
    ProntoParaTransmitir,
}
