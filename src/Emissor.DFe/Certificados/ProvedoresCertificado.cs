using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Certificados;

/// <summary>
/// Entrega o certificado pronto para assinar os documentos (a assinatura do XML entra com o motor DF-e, fatia P4).
/// Há uma implementação para o A1 (arquivo criptografado no banco) e outra para o A3 (repositório do Windows).
/// </summary>
public interface IProvedorCertificado
{
    /// <summary>Abre o certificado com a chave privada. Quem chama deve descartá-lo após o uso.</summary>
    /// <exception cref="RegraNegocioException">Certificado não encontrado, sem chave privada ou ilegível.</exception>
    X509Certificate2 ObterCertificado();
}

/// <summary>Certificado A1: descriptografa o arquivo e a senha gravados no banco e abre o .pfx em memória.</summary>
public sealed class ProvedorCertificadoA1 : IProvedorCertificado
{
    private readonly byte[] _pfxProtegido;
    private readonly byte[] _senhaProtegida;
    private readonly IProtetorSegredos _protetor;

    /// <summary>Cria o provedor sobre os dados protegidos.</summary>
    /// <param name="pfxProtegido">Arquivo .pfx criptografado.</param>
    /// <param name="senhaProtegida">Senha criptografada.</param>
    /// <param name="protetor">Quem descriptografa.</param>
    public ProvedorCertificadoA1(byte[] pfxProtegido, byte[] senhaProtegida, IProtetorSegredos protetor)
    {
        _pfxProtegido = pfxProtegido;
        _senhaProtegida = senhaProtegida;
        _protetor = protetor;
    }

    /// <inheritdoc />
    public X509Certificate2 ObterCertificado()
    {
        var senha = Encoding.UTF8.GetString(_protetor.Desproteger(_senhaProtegida));
        return LeitorCertificado.AbrirA1(_protetor.Desproteger(_pfxProtegido), senha);
    }
}

/// <summary>
/// Certificado A3 (token ou cartão): localizado pelo thumbprint no repositório pessoal do Windows.
/// O PIN é pedido pelo próprio driver do token na hora de assinar.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class ProvedorCertificadoA3 : IProvedorCertificado
{
    private readonly string _thumbprint;

    /// <summary>Cria o provedor para o certificado com o thumbprint informado.</summary>
    /// <param name="thumbprint">Thumbprint (40 caracteres hexadecimais).</param>
    public ProvedorCertificadoA3(string thumbprint) => _thumbprint = thumbprint;

    /// <inheritdoc />
    public X509Certificate2 ObterCertificado()
    {
        using var repositorio = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        repositorio.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        var encontrados = repositorio.Certificates.Find(X509FindType.FindByThumbprint, _thumbprint, validOnly: false);
        var certificado = encontrados.FirstOrDefault(c => c.HasPrivateKey)
            ?? throw new RegraNegocioException("O certificado A3 não foi encontrado no Windows. Conecte o token ou cartão e confira o driver.");
        foreach (var outro in encontrados.Where(c => !ReferenceEquals(c, certificado)))
        {
            outro.Dispose();
        }

        return certificado;
    }
}

/// <summary>Escolhe o provedor conforme o tipo do certificado cadastrado.</summary>
public static class ProvedoresCertificado
{
    /// <summary>Provedor do certificado cadastrado.</summary>
    /// <param name="certificado">Certificado do banco.</param>
    /// <param name="protetor">Quem descriptografa o A1.</param>
    [SupportedOSPlatform("windows")]
    public static IProvedorCertificado Criar(CertificadoDigital certificado, IProtetorSegredos protetor) => certificado.Tipo switch
    {
        TipoCertificado.A1 when certificado.ArquivoPfxCriptografado is { } pfx && certificado.SenhaCriptografada is { } senha
            => new ProvedorCertificadoA1(pfx, senha, protetor),
        TipoCertificado.A3 => new ProvedorCertificadoA3(certificado.Thumbprint),
        _ => throw new RegraNegocioException("Certificado sem os dados necessários para assinar."),
    };
}
