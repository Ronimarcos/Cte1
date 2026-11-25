using System.Reflection;
using System.Xml.Serialization;

namespace Emissor.DFe.Leiaute;

/// <summary>
/// Converte o código do leiaute (ex.: "57", "MT", "01") no valor do enum gerado dos XSD, que tem nomes como
/// <c>Item57</c>. A busca é pelo <see cref="XmlEnumAttribute"/>, que guarda o código exato do XSD.
/// </summary>
public static class LeiauteEnum
{
    /// <summary>Valor do enum gerado para o código do leiaute.</summary>
    /// <typeparam name="T">Enum gerado dos XSD.</typeparam>
    /// <param name="codigo">Código como aparece no XML.</param>
    /// <exception cref="ArgumentException">Código que o XSD não aceita.</exception>
    public static T De<T>(string codigo) where T : struct, Enum
    {
        foreach (var campo in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var nome = campo.GetCustomAttribute<XmlEnumAttribute>()?.Name ?? campo.Name;
            if (nome == codigo)
            {
                return (T)campo.GetValue(null)!;
            }
        }

        throw new ArgumentException($"O código \"{codigo}\" não é aceito pelo leiaute em {typeof(T).Name}.", nameof(codigo));
    }

    /// <summary>Valor do enum gerado para um código numérico.</summary>
    /// <typeparam name="T">Enum gerado dos XSD.</typeparam>
    /// <param name="codigo">Código numérico.</param>
    /// <param name="digitos">Quantidade de dígitos no XML (ex.: 2 para "01").</param>
    public static T De<T>(int codigo, int digitos = 1) where T : struct, Enum =>
        De<T>(codigo.ToString(new string('0', digitos), System.Globalization.CultureInfo.InvariantCulture));
}
