using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Emissor.DFe.Webservices;

/// <summary>
/// Cliente HTTPS para os webservices da SEFAZ: apresenta o certificado da empresa (autenticação mútua exigida pelos
/// autorizadores) e só aceita servidores com cadeia ICP-Brasil (<see cref="ConfiancaIcpBrasil"/>). A verificação TLS
/// nunca é desligada.
/// </summary>
public static class ClienteHttpSefaz
{
    /// <summary>Tempo limite padrão de uma chamada.</summary>
    public static readonly TimeSpan TempoLimitePadrao = TimeSpan.FromSeconds(60);

    /// <summary>Cria o cliente com o certificado da empresa.</summary>
    /// <param name="certificadoCliente">Certificado com chave privada (A1 aberto ou A3 do Windows); <c>null</c> só em diagnóstico.</param>
    /// <param name="tempoLimite">Tempo limite da chamada; padrão de 60 segundos.</param>
    public static HttpClient Criar(X509Certificate2? certificadoCliente, TimeSpan? tempoLimite = null)
    {
        var manipulador = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                RemoteCertificateValidationCallback = ConfiancaIcpBrasil.ValidarServidor,
                ClientCertificates = certificadoCliente is null ? null : [certificadoCliente],
            },
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        };

        return new HttpClient(manipulador, disposeHandler: true) { Timeout = tempoLimite ?? TempoLimitePadrao };
    }
}
