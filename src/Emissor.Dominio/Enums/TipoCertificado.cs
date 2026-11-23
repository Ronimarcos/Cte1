namespace Emissor.Dominio.Enums;

/// <summary>
/// Tipo do certificado digital ICP-Brasil usado na assinatura (§5.2 do levantamento).
/// Valores internos do sistema; não é tag do leiaute.
/// </summary>
public enum TipoCertificado
{
    /// <summary>A1: arquivo .pfx guardado criptografado, com senha também criptografada.</summary>
    A1 = 1,

    /// <summary>A3: token ou cartão, acessado pelo repositório do Windows pelo thumbprint.</summary>
    A3 = 3,
}
