using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Veículo fictício (tração ou reboque) usado no MDF-e, com os campos do §5.5.
/// </summary>
public sealed class VeiculoPrototipo : RegistroPrototipo
{
    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }

    /// <summary>Tração ou reboque.</summary>
    public TipoVeiculo? Tipo { get; set; }

    /// <summary>Código interno (<c>cInt</c>).</summary>
    public string CodigoInterno { get; set; } = string.Empty;

    /// <summary>Placa (<c>placa</c>).</summary>
    public string Placa { get; set; } = string.Empty;

    /// <summary>RENAVAM (<c>RENAVAM</c>).</summary>
    public string Renavam { get; set; } = string.Empty;

    /// <summary>Tara em kg (<c>tara</c>).</summary>
    public int TaraKg { get; set; }

    /// <summary>Capacidade em kg (<c>capKG</c>); obrigatória para reboque.</summary>
    public int CapacidadeKg { get; set; }

    /// <summary>Capacidade em m³ (<c>capM3</c>).</summary>
    public int CapacidadeM3 { get; set; }

    /// <summary>Tipo de rodado (<c>tpRod</c>); obrigatório para tração.</summary>
    public TipoRodado? TipoRodado { get; set; }

    /// <summary>Tipo de carroceria (<c>tpCar</c>).</summary>
    public TipoCarroceria? TipoCarroceria { get; set; }

    /// <summary>UF de licenciamento (<c>UF</c>).</summary>
    public string Uf { get; set; } = string.Empty;

    /// <summary>Verdadeiro quando o veículo é da própria empresa.</summary>
    public bool Proprio { get; set; } = true;

    /// <summary>Proprietário, quando o veículo é de terceiro (<c>prop</c>).</summary>
    public int? ProprietarioId { get; set; }

    /// <summary>Descrição do tipo para exibição.</summary>
    public string TipoDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposVeiculo, Tipo);

    /// <summary>Descrição do rodado para exibição.</summary>
    public string TipoRodadoDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposRodado, TipoRodado);

    /// <summary>Descrição da carroceria para exibição.</summary>
    public string TipoCarroceriaDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposCarroceria, TipoCarroceria);

    /// <summary>"Próprio" ou o nome do proprietário de terceiro.</summary>
    public string ProprietarioDescricao => Proprio
        ? "Próprio"
        : DadosFicticios.Proprietarios.Listar(p => p.Id == ProprietarioId).FirstOrDefault()?.Nome ?? "(não informado)";

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (Tipo is null) faltando.Add("Tipo (tração ou reboque)");
        if (string.IsNullOrWhiteSpace(Placa)) faltando.Add("Placa");
        if (TaraKg <= 0) faltando.Add("Tara (kg)");
        if (Tipo == TipoVeiculo.Reboque && CapacidadeKg <= 0) faltando.Add("Capacidade (kg) (obrigatória para reboque)");
        if (Tipo == TipoVeiculo.Tracao && TipoRodado is null) faltando.Add("Tipo de rodado (obrigatório para tração)");
        if (TipoCarroceria is null) faltando.Add("Tipo de carroceria");
        if (!Proprio && ProprietarioId is null) faltando.Add("Proprietário (obrigatório para veículo de terceiro)");
        return faltando;
    }
}
