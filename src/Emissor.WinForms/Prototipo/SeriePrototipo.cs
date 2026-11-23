using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Série de numeração fictícia (§5.3). Chave única: empresa, modelo, série e ambiente.
/// Na versão real o próximo número avança dentro de uma transação, ao reservar o número do documento.
/// </summary>
public sealed class SeriePrototipo : RegistroPrototipo
{
    /// <summary>Maior série aceita pelo leiaute (<c>TSerie</c>: 0 a 999).</summary>
    public const int SerieMaxima = 999;

    /// <summary>Maior número de documento aceito pelo leiaute (<c>nCT</c>/<c>nMDF</c>: 1 a 999.999.999).</summary>
    public const int NumeroMaximo = 999_999_999;

    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }

    /// <summary>Modelo do documento (<c>ide/mod</c>).</summary>
    public ModeloDocumento? Modelo { get; set; }

    /// <summary>Número da série (<c>ide/serie</c>).</summary>
    public int? Serie { get; set; }

    /// <summary>Verdadeiro para homologação (<c>tpAmb</c> 2); a numeração é separada por ambiente.</summary>
    public bool Homologacao { get; set; } = true;

    /// <summary>Próximo número a usar (<c>ide/nCT</c> ou <c>ide/nMDF</c>).</summary>
    public int? ProximoNumero { get; set; } = 1;

    /// <summary>Descrição do modelo para exibição.</summary>
    public string ModeloDescricao => OpcoesCadastro.Texto(OpcoesCadastro.ModelosDocumento, Modelo);

    /// <summary>Descrição do ambiente para exibição.</summary>
    public string AmbienteDescricao => OpcoesCadastro.Texto(OpcoesCadastro.Ambientes, Homologacao);

    /// <summary>Indica se tem a mesma chave (modelo, série e ambiente) que outra série da mesma empresa.</summary>
    /// <param name="outra">Série comparada.</param>
    public bool MesmaChave(SeriePrototipo outra) =>
        outra.EmpresaId == EmpresaId && outra.Modelo == Modelo && outra.Serie == Serie && outra.Homologacao == Homologacao;

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (Modelo is null) faltando.Add("Modelo");
        if (Serie is null or < 0 or > SerieMaxima) faltando.Add($"Série (0 a {SerieMaxima})");
        if (ProximoNumero is null or < 1 or > NumeroMaximo) faltando.Add($"Próximo número (1 a {NumeroMaximo:N0})");
        return faltando;
    }
}
