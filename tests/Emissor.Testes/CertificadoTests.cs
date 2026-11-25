using System.Formats.Asn1;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Emissor.DFe.Certificados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Testes;

/// <summary>
/// Leitura, proteção e regras do certificado digital. Os certificados são criados aqui, autoassinados e só em
/// memória: nenhum certificado real entra no repositório.
/// </summary>
public class CertificadoTests
{
    private const string Senha = "senha-de-teste";

    [Fact]
    public void LerA1_ExtraiCnpjDoNomeAlternativoIcpBrasil()
    {
        var pfx = CriarPfx("TRANSPORTADORA TESTE LTDA:99999999000199", cnpjNoNomeAlternativo: "11111111000111");

        var dados = LeitorCertificado.LerA1(pfx, Senha);

        Assert.Equal("11111111000111", dados.Documento);
        Assert.Equal("TRANSPORTADORA TESTE LTDA", dados.Titular);
        Assert.True(dados.TemChavePrivada);
        Assert.Matches("^[0-9A-F]{40}$", dados.Thumbprint);
        Assert.True(dados.ValidoAte > dados.ValidoDe);
    }

    [Fact]
    public void LerA1_SemNomeAlternativo_UsaODocumentoDoNome()
    {
        var pfx = CriarPfx("TRANSPORTADORA TESTE LTDA:22222222000122", cnpjNoNomeAlternativo: null);
        Assert.Equal("22222222000122", LeitorCertificado.LerA1(pfx, Senha).Documento);
    }

    [Fact]
    public void LerA1_SenhaErrada_ERecusada()
    {
        var pfx = CriarPfx("TESTE:11111111000111", cnpjNoNomeAlternativo: null);
        var erro = Assert.Throws<RegraNegocioException>(() => LeitorCertificado.LerA1(pfx, "senha-errada"));
        Assert.Contains("senha", erro.Message);
    }

    [Fact]
    public void ProvedorA1_AbreOCertificadoGuardadoProtegido()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var pfx = CriarPfx("TESTE:11111111000111", cnpjNoNomeAlternativo: null);
        var protetor = new ProtetorDpapi();
        var pfxProtegido = protetor.Proteger(pfx);
        var senhaProtegida = protetor.Proteger(Encoding.UTF8.GetBytes(Senha));

        Assert.NotEqual(pfx, pfxProtegido);
        Assert.DoesNotContain(Senha, Encoding.UTF8.GetString(senhaProtegida));

        using var certificado = new ProvedorCertificadoA1(pfxProtegido, senhaProtegida, protetor).ObterCertificado();
        Assert.True(certificado.HasPrivateKey);
    }

    [Theory]
    [InlineData("11111111000111", "11111111000111", true)]
    [InlineData("11111111000111", "11111111000299", true)]
    [InlineData("11111111000111", "22222222000122", false)]
    [InlineData("33333333333", "33333333333", true)]
    [InlineData("11111111000111", "33333333333", false)]
    public void ValidarTitular_ExigeAMesmaRaizDeCnpjOuOMesmoCpf(string documentoEmpresa, string documentoCertificado, bool aceito)
    {
        var empresa = documentoEmpresa.Length == 14 ? new Empresa { Cnpj = documentoEmpresa } : new Empresa { Cpf = documentoEmpresa };
        var certificado = new CertificadoDigital { Tipo = TipoCertificado.A3, CnpjTitular = documentoCertificado };
        Assert.Equal(aceito, certificado.ValidarTitular(empresa).Count == 0);
    }

    /// <summary>Cria um .pfx autoassinado com o nome informado e, opcionalmente, o CNPJ no nome alternativo ICP-Brasil.</summary>
    private static byte[] CriarPfx(string nome, string? cnpjNoNomeAlternativo)
    {
        using var chave = RSA.Create(2048);
        var pedido = new CertificateRequest($"CN={nome}", chave, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        if (cnpjNoNomeAlternativo is not null)
        {
            pedido.CertificateExtensions.Add(new X509Extension("2.5.29.17", NomeAlternativoIcp(cnpjNoNomeAlternativo), critical: false));
        }

        using var certificado = pedido.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));
        return certificado.Export(X509ContentType.Pfx, Senha);
    }

    /// <summary>SEQUENCE { [0] { OID 2.16.76.1.3.3, [0] { OCTET STRING "cnpj" } } }, como nos certificados ICP-Brasil.</summary>
    private static byte[] NomeAlternativoIcp(string cnpj)
    {
        var escritor = new AsnWriter(AsnEncodingRules.DER);
        var outroNome = new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true);
        using (escritor.PushSequence())
        using (escritor.PushSequence(outroNome))
        {
            escritor.WriteObjectIdentifier(LeitorCertificado.OidCnpj);
            using (escritor.PushSequence(outroNome))
            {
                escritor.WriteOctetString(Encoding.ASCII.GetBytes(cnpj));
            }
        }

        return escritor.Encode();
    }
}
