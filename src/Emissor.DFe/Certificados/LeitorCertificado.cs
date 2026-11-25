using System.Formats.Asn1;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Certificados;

/// <summary>Dados lidos de um certificado digital, para o cadastro.</summary>
/// <param name="Titular">Nome do titular (parte do nome do certificado antes do documento).</param>
/// <param name="Documento">CNPJ (14) ou CPF (11) do titular, sem máscara; vazio se não encontrado.</param>
/// <param name="ValidoDe">Início da validade.</param>
/// <param name="ValidoAte">Fim da validade.</param>
/// <param name="Thumbprint">Impressão digital SHA-1 em hexadecimal maiúsculo (40 caracteres).</param>
/// <param name="Emissor">Autoridade certificadora que emitiu.</param>
/// <param name="TemChavePrivada">Verdadeiro quando o certificado pode assinar.</param>
public sealed record DadosCertificado(
    string Titular, string Documento, DateTime ValidoDe, DateTime ValidoAte, string Thumbprint, string Emissor, bool TemChavePrivada);

/// <summary>
/// Leitura de certificados ICP-Brasil: abre o A1 (.pfx com senha), lista os A3 e A1 instalados no repositório
/// pessoal do Windows e extrai titular, CNPJ/CPF, validade e thumbprint.
/// </summary>
public static class LeitorCertificado
{
    /// <summary>OID do CNPJ da pessoa jurídica no nome alternativo do certificado ICP-Brasil.</summary>
    public const string OidCnpj = "2.16.76.1.3.3";

    /// <summary>OID dos dados da pessoa física (nascimento + CPF + ...) no nome alternativo ICP-Brasil.</summary>
    public const string OidDadosPessoaFisica = "2.16.76.1.3.1";

    private const string OidNomeAlternativo = "2.5.29.17";

    /// <summary>Abre o arquivo .pfx/.p12 com a senha; recusa senha errada, arquivo inválido ou sem chave privada.</summary>
    /// <param name="pfx">Conteúdo do arquivo.</param>
    /// <param name="senha">Senha do arquivo.</param>
    /// <exception cref="RegraNegocioException">Senha incorreta, arquivo inválido ou sem chave privada.</exception>
    public static X509Certificate2 AbrirA1(byte[] pfx, string senha)
    {
        X509Certificate2 certificado;
        try
        {
            certificado = X509CertificateLoader.LoadPkcs12(pfx, senha, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
        }
        catch (CryptographicException)
        {
            throw new RegraNegocioException("Não foi possível abrir o certificado: a senha está incorreta ou o arquivo não é um certificado A1 (.pfx) válido.");
        }

        if (!certificado.HasPrivateKey)
        {
            certificado.Dispose();
            throw new RegraNegocioException("O arquivo não contém a chave privada do certificado e não serve para assinar.");
        }

        return certificado;
    }

    /// <summary>Lê os dados do certificado A1.</summary>
    /// <param name="pfx">Conteúdo do arquivo.</param>
    /// <param name="senha">Senha do arquivo.</param>
    public static DadosCertificado LerA1(byte[] pfx, string senha)
    {
        using var certificado = AbrirA1(pfx, senha);
        return Ler(certificado);
    }

    /// <summary>
    /// Certificados com chave privada do repositório pessoal do usuário no Windows (onde ficam os A3 de token ou
    /// cartão e os A1 instalados). Listar não pede o PIN; ele só é pedido ao assinar.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static IReadOnlyList<DadosCertificado> ListarRepositorioWindows()
    {
        using var repositorio = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        repositorio.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        var lista = new List<DadosCertificado>();
        foreach (var certificado in repositorio.Certificates)
        {
            using (certificado)
            {
                if (certificado.HasPrivateKey)
                {
                    lista.Add(Ler(certificado));
                }
            }
        }

        return lista.OrderBy(c => c.Titular).ThenByDescending(c => c.ValidoAte).ToList();
    }

    /// <summary>Extrai os dados de exibição e de cadastro do certificado.</summary>
    /// <param name="certificado">Certificado aberto.</param>
    public static DadosCertificado Ler(X509Certificate2 certificado)
    {
        var nome = certificado.GetNameInfo(X509NameType.SimpleName, forIssuer: false);
        var documento = DocumentoDoNomeAlternativo(certificado) ?? DocumentoDoNome(nome) ?? string.Empty;
        var titular = nome.Contains(':') ? nome[..nome.LastIndexOf(':')].Trim() : nome.Trim();
        return new DadosCertificado(
            titular,
            documento,
            certificado.NotBefore,
            certificado.NotAfter,
            certificado.Thumbprint.ToUpperInvariant(),
            certificado.GetNameInfo(X509NameType.SimpleName, forIssuer: true),
            certificado.HasPrivateKey);
    }

    /// <summary>
    /// CNPJ (OID 2.16.76.1.3.3) ou CPF (posições 9 a 19 do OID 2.16.76.1.3.1) gravados no nome alternativo,
    /// como determina o padrão ICP-Brasil. Retorna <c>null</c> se não houver ou se a extensão for ilegível.
    /// </summary>
    private static string? DocumentoDoNomeAlternativo(X509Certificate2 certificado)
    {
        var extensao = certificado.Extensions[OidNomeAlternativo];
        if (extensao is null)
        {
            return null;
        }

        try
        {
            string? cpf = null;
            var nomes = new AsnReader(extensao.RawData, AsnEncodingRules.DER).ReadSequence();
            var outroNome = new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true);
            while (nomes.HasData)
            {
                if (nomes.PeekTag() != outroNome)
                {
                    nomes.ReadEncodedValue();
                    continue;
                }

                var item = nomes.ReadSequence(outroNome);
                var oid = item.ReadObjectIdentifier();
                var valor = LerValor(item.ReadSequence(outroNome));
                if (oid == OidCnpj && Formatos.CnpjValido(Formatos.SoAlfanumericos(valor)))
                {
                    return Formatos.SoAlfanumericos(valor);
                }

                if (oid == OidDadosPessoaFisica && valor.Length >= 19 && Formatos.CpfValido(valor.Substring(8, 11)))
                {
                    cpf = valor.Substring(8, 11);
                }
            }

            return cpf;
        }
        catch (AsnContentException)
        {
            return null;
        }
    }

    /// <summary>O valor do outro nome ICP-Brasil vem como texto (OCTET STRING com ASCII ou cadeia de caracteres).</summary>
    private static string LerValor(AsnReader valor)
    {
        var tag = valor.PeekTag();
        if (tag.HasSameClassAndValue(Asn1Tag.PrimitiveOctetString))
        {
            return System.Text.Encoding.ASCII.GetString(valor.ReadOctetString());
        }

        return tag.TagClass == TagClass.Universal
            ? valor.ReadCharacterString((UniversalTagNumber)tag.TagValue)
            : string.Empty;
    }

    /// <summary>Nos certificados ICP-Brasil o nome costuma ser "RAZÃO SOCIAL:CNPJ" ou "NOME:CPF".</summary>
    private static string? DocumentoDoNome(string nome)
    {
        var posicao = nome.LastIndexOf(':');
        if (posicao < 0)
        {
            return null;
        }

        var documento = Formatos.SoAlfanumericos(nome[(posicao + 1)..]);
        return Formatos.CnpjOuCpfValido(documento) ? documento : null;
    }
}
