using System.Globalization;
using System.Text;

namespace Emissor.DFe.Impressao;

/// <summary>
/// Código de barras CODE-128C da chave de acesso, conforme o Anexo II do MOC CT-e (DACTE, item 2.16): caractere de
/// início C (105), a chave em pares de dígitos, dígito verificador pelo módulo 103 e caractere de parada. Cada símbolo
/// tem 11 módulos (a parada, 13), alternando barra e espaço a partir de uma barra.
/// </summary>
public static class CodigoBarras128C
{
    /// <summary>Valor do caractere de início do conjunto C.</summary>
    public const int InicioC = 105;

    /// <summary>Margem clara mínima em módulos, de cada lado (item 2.16: 10 módulos).</summary>
    public const int MargemClara = 10;

    /// <summary>Larguras (barra, espaço, barra...) dos valores 0 a 105, iguais à tabela do item 7 do manual.</summary>
    private static readonly string[] Padroes =
    [
        "212222", "222122", "222221", "121223", "121322", "131222", "122213", "122312", "132212", "221213",
        "221312", "231212", "112232", "122132", "122231", "113222", "123122", "123221", "223211", "221132",
        "221231", "213212", "223112", "312131", "311222", "321122", "321221", "312212", "322112", "322211",
        "212123", "212321", "232121", "111323", "131123", "131321", "112313", "132113", "132311", "211313",
        "231113", "231311", "112133", "112331", "132131", "113123", "113321", "133121", "313121", "211331",
        "231131", "213113", "213311", "213131", "311123", "311321", "331121", "312113", "312311", "332111",
        "314111", "221411", "431111", "111224", "111422", "121124", "121421", "141122", "141221", "112214",
        "112412", "122114", "122411", "142112", "142211", "241211", "221114", "413111", "241112", "134111",
        "111242", "121142", "121241", "114212", "124112", "124211", "411212", "421112", "421211", "212141",
        "214121", "412121", "111143", "111341", "131141", "114113", "114311", "411113", "411311", "113141",
        "114131", "311141", "411131", "211412", "211214", "211232",
    ];

    private const string Parada = "2331112";

    /// <summary>Dígito verificador (módulo 103) da sequência numérica, com o início C de peso 1.</summary>
    /// <param name="digitos">Quantidade par de dígitos.</param>
    public static int DigitoVerificador(string digitos)
    {
        var valores = Pares(digitos);
        var soma = InicioC;
        for (var i = 0; i < valores.Count; i++)
        {
            soma += valores[i] * (i + 1);
        }

        return soma % 103;
    }

    /// <summary>Larguras de barra e espaço, em módulos, do símbolo completo (sem a margem clara).</summary>
    /// <param name="digitos">Quantidade par de dígitos (a chave de acesso tem 44).</param>
    /// <returns>Texto com as larguras alternadas, começando por barra (ex.: "211232...2331112").</returns>
    /// <exception cref="ArgumentException">Texto vazio, ímpar ou com caractere que não é dígito.</exception>
    public static string Larguras(string digitos)
    {
        var larguras = new StringBuilder(Padroes[InicioC]);
        foreach (var valor in Pares(digitos))
        {
            larguras.Append(Padroes[valor]);
        }

        larguras.Append(Padroes[DigitoVerificador(digitos)]).Append(Parada);
        return larguras.ToString();
    }

    /// <summary>
    /// Desenho em SVG, com a margem clara de 10 módulos de cada lado, na proporção informada (a imagem é esticada
    /// pelo PDF para a área reservada).
    /// </summary>
    /// <param name="digitos">Quantidade par de dígitos.</param>
    /// <param name="altura">Altura em módulos.</param>
    public static string Svg(string digitos, int altura = 50)
    {
        var larguras = Larguras(digitos);
        var total = larguras.Sum(c => c - '0') + 2 * MargemClara;
        var svg = new StringBuilder();
        svg.Append(CultureInfo.InvariantCulture,
            $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {total} {altura}\" preserveAspectRatio=\"none\" shape-rendering=\"crispEdges\">");
        var x = MargemClara;
        for (var i = 0; i < larguras.Length; i++)
        {
            var largura = larguras[i] - '0';
            if (i % 2 == 0)
            {
                svg.Append(CultureInfo.InvariantCulture, $"<rect x=\"{x}\" y=\"0\" width=\"{largura}\" height=\"{altura}\" fill=\"#000\"/>");
            }

            x += largura;
        }

        return svg.Append("</svg>").ToString();
    }

    private static List<int> Pares(string digitos)
    {
        if (string.IsNullOrEmpty(digitos) || digitos.Length % 2 != 0 || !digitos.All(char.IsAsciiDigit))
        {
            throw new ArgumentException("O CODE-128C exige uma quantidade par de dígitos.", nameof(digitos));
        }

        var valores = new List<int>(digitos.Length / 2);
        for (var i = 0; i < digitos.Length; i += 2)
        {
            valores.Add((digitos[i] - '0') * 10 + (digitos[i + 1] - '0'));
        }

        return valores;
    }
}
