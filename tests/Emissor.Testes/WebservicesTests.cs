using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Emissor.DFe.Webservices;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Testes;

/// <summary>Tabela de webservices (só homologação) e confiança TLS pela cadeia ICP-Brasil. Sem acesso à rede.</summary>
public class WebservicesTests
{
    [Fact]
    public void Raizes_SaoAsSeteVigentesDoIti()
    {
        var raizes = ConfiancaIcpBrasil.Raizes;
        Assert.Equal(7, raizes.Count);
        Assert.All(raizes, r => Assert.Contains("Autoridade Certificadora Raiz Brasileira", r.Subject, StringComparison.Ordinal));
        Assert.All(raizes, r => Assert.Equal(r.Subject, r.Issuer));
        Assert.Contains(raizes.Cast<X509Certificate2>(), r => r.Subject.Contains("v10", StringComparison.Ordinal));
    }

    [Fact]
    public void CertificadoForaDaIcpBrasil_NaoEhAceito()
    {
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=servidor.falso.exemplo", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var autoassinado = pedido.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(30));

        Assert.False(ConfiancaIcpBrasil.CadeiaValida(autoassinado, null));
    }

    [Theory]
    [InlineData(ServicoSefaz.CteStatusServico, "https://homologacao.sefaz.mt.gov.br/ctews2/services/CTeStatusServicoV4")]
    [InlineData(ServicoSefaz.CteRecepcaoSinc, "https://homologacao.sefaz.mt.gov.br/ctews2/services/CTeRecepcaoSincV4")]
    [InlineData(ServicoSefaz.MdfeStatusServico, "https://mdfe-homologacao.svrs.rs.gov.br/ws/MDFeStatusServico/MDFeStatusServico.asmx")]
    [InlineData(ServicoSefaz.MdfeRecepcaoSinc, "https://mdfe-homologacao.svrs.rs.gov.br/ws/MDFeRecepcaoSinc/MDFeRecepcaoSinc.asmx")]
    public void Homologacao_TemOsEnderecosConferidos(ServicoSefaz servico, string esperado)
    {
        Assert.Equal(new Uri(esperado), TabelaWebservices.Endereco("MT", servico, TipoAmbiente.Homologacao));
    }

    [Fact]
    public void Producao_EhRecusada()
    {
        var erro = Assert.Throws<RegraNegocioException>(() => TabelaWebservices.Endereco("MT", ServicoSefaz.CteStatusServico, TipoAmbiente.Producao));
        Assert.Contains("produção não está liberada", erro.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UfSemEnderecoConferido_EhRecusada()
    {
        Assert.Throws<RegraNegocioException>(() => TabelaWebservices.Endereco("SP", ServicoSefaz.CteStatusServico, TipoAmbiente.Homologacao));
    }

    [Fact]
    public void Mdfe_UsaOMesmoAutorizadorParaQualquerUf()
    {
        Assert.Equal(
            TabelaWebservices.Endereco("MT", ServicoSefaz.MdfeStatusServico, TipoAmbiente.Homologacao),
            TabelaWebservices.Endereco("SP", ServicoSefaz.MdfeStatusServico, TipoAmbiente.Homologacao));
    }
}
