using System.Security.Cryptography;
using Emissor.Dominio.Regras;

namespace Emissor.DFe;

/// <summary>
/// Chave de acesso de 44 posições do CT-e e do MDF-e (tipo <c>TChDFe</c>: <c>[0-9]{6}[A-Z0-9]{12}[0-9]{26}</c>):
/// cUF (2), ano e mês da emissão (4), CNPJ do emitente (14, alfanumérico), modelo (2), série (3), número (9),
/// tipo de emissão (1), código numérico (8) e dígito verificador (1).
/// </summary>
public static class ChaveAcesso
{
    /// <summary>Monta a chave com o dígito verificador.</summary>
    /// <param name="codigoUf">Código IBGE da UF do emitente (ex.: 51 para MT).</param>
    /// <param name="emissao">Data da emissão.</param>
    /// <param name="cnpj">CNPJ do emitente, sem máscara (pode ser alfanumérico).</param>
    /// <param name="modelo">57 (CT-e) ou 58 (MDF-e).</param>
    /// <param name="serie">Série (0 a 999).</param>
    /// <param name="numero">Número (1 a 999.999.999).</param>
    /// <param name="tipoEmissao">Tipo de emissão (1 = normal).</param>
    /// <param name="codigoNumerico">Código numérico de 8 dígitos (<c>cCT</c>/<c>cMDF</c>).</param>
    /// <returns>Chave com 44 posições.</returns>
    /// <exception cref="ArgumentException">Algum componente fora do formato.</exception>
    public static string Montar(int codigoUf, DateTime emissao, string cnpj, int modelo, int serie, int numero, int tipoEmissao, int codigoNumerico)
    {
        if (!Formatos.CnpjValido(cnpj))
        {
            throw new ArgumentException("CNPJ do emitente fora do formato do leiaute.", nameof(cnpj));
        }

        if (serie is < 0 or > 999 || numero is < 1 or > 999_999_999 || codigoNumerico is < 0 or > 99_999_999)
        {
            throw new ArgumentException("Série, número ou código numérico fora da faixa do leiaute.");
        }

        var semDigito = $"{codigoUf:00}{emissao:yyMM}{cnpj}{modelo:00}{serie:000}{numero:000000000}{tipoEmissao}{codigoNumerico:00000000}";
        return semDigito + CalcularDigito(semDigito);
    }

    /// <summary>
    /// Dígito verificador pelo módulo 11, com pesos de 2 a 9 da direita para a esquerda. Cada posição vale o código
    /// ASCII menos 48: dígitos valem 0 a 9 e as letras do CNPJ alfanumérico valem 17 (A) a 42 (Z).
    /// Resto 0 ou 1 resulta em dígito 0.
    /// </summary>
    /// <param name="semDigito">As 43 primeiras posições da chave.</param>
    public static int CalcularDigito(string semDigito)
    {
        var soma = 0;
        var peso = 2;
        for (var i = semDigito.Length - 1; i >= 0; i--)
        {
            soma += (semDigito[i] - '0') * peso;
            peso = peso == 9 ? 2 : peso + 1;
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    /// <summary>Indica se a chave tem o formato e o dígito verificador corretos.</summary>
    /// <param name="chave">Chave com 44 posições.</param>
    public static bool Valida(string? chave) =>
        chave is { Length: 44 }
        && chave.Take(6).All(char.IsAsciiDigit)
        && chave.Skip(6).Take(12).All(c => char.IsAsciiDigit(c) || char.IsAsciiLetterUpper(c))
        && chave.Skip(18).All(char.IsAsciiDigit)
        && CalcularDigito(chave[..43]) == chave[43] - '0';

    /// <summary>Código numérico aleatório de 8 dígitos, diferente do número do documento.</summary>
    /// <param name="numero">Número do documento.</param>
    public static int GerarCodigoNumerico(int numero)
    {
        int codigo;
        do
        {
            codigo = RandomNumberGenerator.GetInt32(0, 100_000_000);
        }
        while (codigo == numero);

        return codigo;
    }
}
