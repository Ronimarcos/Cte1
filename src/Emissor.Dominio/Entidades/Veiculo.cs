using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Dominio.Entidades;

/// <summary>Veículo de tração ou reboque (§5.5), usado nos grupos <c>veicTracao</c> e <c>veicReboque</c> do MDF-e.</summary>
public class Veiculo : EntidadeDaEmpresa
{
    /// <summary>Maior valor aceito em tara e capacidades (6 dígitos no leiaute).</summary>
    public const int ValorMaximo = 999_999;

    /// <summary>Tração ou reboque.</summary>
    public TipoVeiculo? Tipo { get; set; }

    /// <summary>Código interno (<c>cInt</c>, até 10).</summary>
    public string CodigoInterno { get; set; } = string.Empty;

    /// <summary>Placa sem hífen, em maiúsculas (<c>placa</c>).</summary>
    public string Placa { get; set; } = string.Empty;

    /// <summary>RENAVAM (<c>RENAVAM</c>), opcional.</summary>
    public string Renavam { get; set; } = string.Empty;

    /// <summary>Tara em kg (<c>tara</c>).</summary>
    public int TaraKg { get; set; }

    /// <summary>Capacidade em kg (<c>capKG</c>), obrigatória para reboque.</summary>
    public int? CapacidadeKg { get; set; }

    /// <summary>Capacidade em m³ (<c>capM3</c>), opcional.</summary>
    public int? CapacidadeM3 { get; set; }

    /// <summary>Tipo de rodado (<c>tpRod</c>), só para tração.</summary>
    public TipoRodado? TipoRodado { get; set; }

    /// <summary>Tipo de carroceria (<c>tpCar</c>).</summary>
    public TipoCarroceria? TipoCarroceria { get; set; }

    /// <summary>UF de licenciamento (<c>UF</c>), opcional.</summary>
    public string Uf { get; set; } = string.Empty;

    /// <summary>Verdadeiro quando o veículo é da própria empresa.</summary>
    public bool Proprio { get; set; } = true;

    /// <summary>Proprietário do veículo de terceiro (<c>prop</c>).</summary>
    public int? ProprietarioId { get; set; }

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar()
    {
        var faltando = new List<string>();
        if (Tipo is null) faltando.Add("Tipo (tração ou reboque)");
        if (!Formatos.PlacaValida(Placa)) faltando.Add("Placa no formato do leiaute (ex.: ABC1234 ou ABC1D23)");
        if (Renavam.Length > 0 && Formatos.SoDigitos(Renavam).Length is < 9 or > 11) faltando.Add("RENAVAM com 9 a 11 dígitos");
        if (TaraKg is <= 0 or > ValorMaximo) faltando.Add("Tara (kg) entre 1 e 999.999");
        if (Tipo == TipoVeiculo.Reboque && CapacidadeKg is null or <= 0) faltando.Add("Capacidade em kg (obrigatória para reboque)");
        if (CapacidadeKg is < 0 or > ValorMaximo || CapacidadeM3 is < 0 or > ValorMaximo) faltando.Add("Capacidades entre 0 e 999.999");
        if (Tipo == TipoVeiculo.Tracao && TipoRodado is null) faltando.Add("Tipo de rodado (obrigatório para tração)");
        if (TipoCarroceria is null) faltando.Add("Tipo de carroceria");
        if (Uf.Length > 0 && !Formatos.UfValida(Uf)) faltando.Add("UF válida");
        if (!Proprio && ProprietarioId is null) faltando.Add("Proprietário (veículo de terceiro)");
        return faltando;
    }
}
