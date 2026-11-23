using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>Item de uma lista de escolha: o valor gravado e o texto exibido ao usuário.</summary>
/// <typeparam name="T">Tipo do valor.</typeparam>
/// <param name="Valor">Valor gravado no cadastro.</param>
/// <param name="Texto">Texto exibido na lista.</param>
public sealed record Opcao<T>(T Valor, string Texto)
{
    /// <inheritdoc />
    public override string ToString() => Texto;
}

/// <summary>
/// Textos das listas de escolha dos cadastros, com o código do leiaute à frente para facilitar a conferência.
/// </summary>
public static class OpcoesCadastro
{
    /// <summary>Siglas das 27 UFs.</summary>
    public static IReadOnlyList<string> Ufs { get; } =
    [
        "AC", "AL", "AM", "AP", "BA", "CE", "DF", "ES", "GO", "MA", "MG", "MS", "MT", "PA",
        "PB", "PE", "PI", "PR", "RJ", "RN", "RO", "RR", "RS", "SC", "SE", "SP", "TO",
    ];

    /// <summary>Código de Regime Tributário (CRT).</summary>
    public static IReadOnlyList<Opcao<Crt>> Crts { get; } =
    [
        new(Crt.SimplesNacional, "1 - Simples Nacional"),
        new(Crt.SimplesNacionalExcessoSublimite, "2 - Simples Nacional, excesso de sublimite"),
        new(Crt.RegimeNormal, "3 - Regime Normal"),
        new(Crt.SimplesNacionalMei, "4 - Simples Nacional - MEI"),
    ];

    /// <summary>Regime de apuração do IBS e da CBS.</summary>
    public static IReadOnlyList<Opcao<RegimeIbsCbs>> RegimesIbsCbs { get; } =
    [
        new(RegimeIbsCbs.SimplesNacionalComRegimeRegular, "1 - Simples Nacional com regime regular do IBS/CBS"),
        new(RegimeIbsCbs.NormalOpcaoPendenteSimplesNacional, "2 - Regime Normal, opção pendente pelo CRT 1, sem regime regular"),
        new(RegimeIbsCbs.NormalOpcaoPendenteSimplesNacionalExcesso, "3 - Regime Normal, opção pendente pelo CRT 2, sem regime regular"),
        new(RegimeIbsCbs.NormalOpcaoPendenteComRegimeRegular, "4 - Regime Normal, opção pendente pelo Simples, com regime regular"),
    ];

    /// <summary>Ambiente padrão de emissão (verdadeiro = homologação).</summary>
    public static IReadOnlyList<Opcao<bool>> Ambientes { get; } =
    [
        new(true, "2 - Homologação"),
        new(false, "1 - Produção"),
    ];

    /// <summary>Indicador da Inscrição Estadual.</summary>
    public static IReadOnlyList<Opcao<IndicadorIe>> IndicadoresIe { get; } =
    [
        new(IndicadorIe.Contribuinte, "1 - Contribuinte do ICMS"),
        new(IndicadorIe.Isento, "2 - Contribuinte isento de inscrição"),
        new(IndicadorIe.NaoContribuinte, "9 - Não contribuinte"),
    ];

    /// <summary>Papel do veículo no MDF-e.</summary>
    public static IReadOnlyList<Opcao<TipoVeiculo>> TiposVeiculo { get; } =
    [
        new(TipoVeiculo.Tracao, "Tração"),
        new(TipoVeiculo.Reboque, "Reboque"),
    ];

    /// <summary>Tipo de rodado (somente veículo de tração).</summary>
    public static IReadOnlyList<Opcao<TipoRodado>> TiposRodado { get; } =
    [
        new(TipoRodado.Truck, "01 - Truck"),
        new(TipoRodado.Toco, "02 - Toco"),
        new(TipoRodado.CavaloMecanico, "03 - Cavalo mecânico"),
        new(TipoRodado.Van, "04 - Van"),
        new(TipoRodado.Utilitario, "05 - Utilitário"),
        new(TipoRodado.Outros, "06 - Outros"),
    ];

    /// <summary>Tipo de carroceria.</summary>
    public static IReadOnlyList<Opcao<TipoCarroceria>> TiposCarroceria { get; } =
    [
        new(TipoCarroceria.NaoAplicavel, "00 - Não aplicável"),
        new(TipoCarroceria.Aberta, "01 - Aberta"),
        new(TipoCarroceria.FechadaBau, "02 - Fechada/baú"),
        new(TipoCarroceria.Graneleira, "03 - Graneleira"),
        new(TipoCarroceria.PortaConteiner, "04 - Porta-contêiner"),
        new(TipoCarroceria.Sider, "05 - Sider"),
    ];

    /// <summary>Tipo do proprietário do veículo de terceiro.</summary>
    public static IReadOnlyList<Opcao<TipoProprietario>> TiposProprietario { get; } =
    [
        new(TipoProprietario.TacAgregado, "0 - TAC agregado"),
        new(TipoProprietario.TacIndependente, "1 - TAC independente"),
        new(TipoProprietario.Outros, "2 - Outros"),
    ];

    /// <summary>Texto de exibição de um valor, ou vazio quando não informado.</summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="opcoes">Lista de opções.</param>
    /// <param name="valor">Valor a descrever.</param>
    public static string Texto<T>(IReadOnlyList<Opcao<T>> opcoes, T? valor) where T : struct =>
        valor is null ? string.Empty : opcoes.FirstOrDefault(o => o.Valor.Equals(valor.Value))?.Texto ?? valor.Value.ToString() ?? string.Empty;
}
