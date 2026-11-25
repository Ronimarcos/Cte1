using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Emissor.DFe.Webservices;

/// <summary>
/// Confiança nos servidores da SEFAZ pela cadeia ICP-Brasil, sem depender das raízes instaladas no Windows.
/// As raízes oficiais (baixadas do repositório AC-Raiz do ITI em 25/11/2026) vão embutidas no assembly; a validação
/// TLS continua obrigatória: nome do servidor, validade, cadeia até uma dessas raízes e revogação quando consultável.
/// </summary>
public static class ConfiancaIcpBrasil
{
    private static readonly Lazy<X509Certificate2Collection> RaizesCarregadas = new(CarregarRaizes);

    /// <summary>Raízes ICP-Brasil vigentes embutidas (v4, v5, v6, v7, v10, v12 e v13).</summary>
    public static X509Certificate2Collection Raizes => RaizesCarregadas.Value;

    /// <summary>
    /// Validação do certificado do servidor para o <see cref="SslClientAuthenticationOptions"/>: aceita só servidor
    /// com o nome certo e cadeia completa até uma raiz ICP-Brasil.
    /// </summary>
    /// <param name="remetente">Não usado.</param>
    /// <param name="certificado">Certificado apresentado pelo servidor.</param>
    /// <param name="cadeiaRecebida">Cadeia montada pelo sistema (traz os intermediários enviados pelo servidor).</param>
    /// <param name="erros">Erros apontados pela validação padrão do Windows.</param>
    /// <returns>Verdadeiro se o servidor é confiável.</returns>
    public static bool ValidarServidor(object? remetente, X509Certificate? certificado, X509Chain? cadeiaRecebida, SslPolicyErrors erros)
    {
        if (certificado is null || erros.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch) || erros.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
        {
            return false;
        }

        using var servidor = new X509Certificate2(certificado);
        return CadeiaValida(servidor, cadeiaRecebida?.ChainElements.Select(e => e.Certificate));
    }

    /// <summary>Monta a cadeia do certificado até uma raiz ICP-Brasil embutida.</summary>
    /// <param name="servidor">Certificado do servidor.</param>
    /// <param name="intermediarios">Certificados intermediários disponíveis (os enviados pelo servidor).</param>
    /// <returns>Verdadeiro se a cadeia termina numa raiz ICP-Brasil e não tem erros.</returns>
    public static bool CadeiaValida(X509Certificate2 servidor, IEnumerable<X509Certificate2>? intermediarios)
    {
        using var cadeia = new X509Chain();
        cadeia.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        cadeia.ChainPolicy.CustomTrustStore.AddRange(Raizes);
        if (intermediarios is not null)
        {
            foreach (var intermediario in intermediarios)
            {
                cadeia.ChainPolicy.ExtraStore.Add(intermediario);
            }
        }

        // Revogação consultada quando o servidor de LCR responde; indisponível não bloqueia a emissão.
        cadeia.ChainPolicy.RevocationMode = X509RevocationMode.Online;
        cadeia.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
        cadeia.ChainPolicy.VerificationFlags =
            X509VerificationFlags.IgnoreEndRevocationUnknown | X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown;
        cadeia.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromSeconds(10);

        return cadeia.Build(servidor);
    }

    private static X509Certificate2Collection CarregarRaizes()
    {
        var assembly = typeof(ConfiancaIcpBrasil).Assembly;
        var raizes = new X509Certificate2Collection();
        foreach (var recurso in assembly.GetManifestResourceNames().Where(n => n.EndsWith(".crt", StringComparison.OrdinalIgnoreCase)))
        {
            using var fluxo = assembly.GetManifestResourceStream(recurso)!;
            using var leitor = new StreamReader(fluxo);
            raizes.Add(X509Certificate2.CreateFromPem(leitor.ReadToEnd()));
        }

        if (raizes.Count == 0)
        {
            throw new InvalidOperationException("As raízes ICP-Brasil não foram encontradas no aplicativo.");
        }

        return raizes;
    }
}
