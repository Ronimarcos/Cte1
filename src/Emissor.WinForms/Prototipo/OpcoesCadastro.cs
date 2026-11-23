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

    /// <summary>Tipo do certificado digital.</summary>
    public static IReadOnlyList<Opcao<TipoCertificado>> TiposCertificado { get; } =
    [
        new(TipoCertificado.A1, "A1 - Arquivo (.pfx)"),
        new(TipoCertificado.A3, "A3 - Token ou cartão"),
    ];

    /// <summary>Modelo do documento fiscal.</summary>
    public static IReadOnlyList<Opcao<ModeloDocumento>> ModelosDocumento { get; } =
    [
        new(ModeloDocumento.Cte, "57 - CT-e"),
        new(ModeloDocumento.Mdfe, "58 - MDF-e"),
    ];

    /// <summary>Responsável pelo seguro da carga.</summary>
    public static IReadOnlyList<Opcao<ResponsavelSeguro>> ResponsaveisSeguro { get; } =
    [
        new(ResponsavelSeguro.Emitente, "1 - Emitente do MDF-e"),
        new(ResponsavelSeguro.Contratante, "2 - Contratante do transporte"),
    ];

    /// <summary>Tipo do CT-e.</summary>
    public static IReadOnlyList<Opcao<TipoCte>> TiposCte { get; } =
    [
        new(TipoCte.Normal, "0 - Normal"),
        new(TipoCte.Complemento, "1 - Complemento de valores"),
        new(TipoCte.Substituicao, "3 - Substituição"),
    ];

    /// <summary>Tipo do serviço do CT-e.</summary>
    public static IReadOnlyList<Opcao<TipoServicoCte>> TiposServicoCte { get; } =
    [
        new(TipoServicoCte.Normal, "0 - Normal"),
        new(TipoServicoCte.Subcontratacao, "1 - Subcontratação"),
        new(TipoServicoCte.Redespacho, "2 - Redespacho"),
        new(TipoServicoCte.RedespachoIntermediario, "3 - Redespacho intermediário"),
    ];

    /// <summary>Tomador do serviço.</summary>
    public static IReadOnlyList<Opcao<TomadorServico>> Tomadores { get; } =
    [
        new(TomadorServico.Remetente, "0 - Remetente"),
        new(TomadorServico.Expedidor, "1 - Expedidor"),
        new(TomadorServico.Recebedor, "2 - Recebedor"),
        new(TomadorServico.Destinatario, "3 - Destinatário"),
        new(TomadorServico.Outros, "4 - Outros"),
    ];

    /// <summary>Unidade de medida da quantidade de carga.</summary>
    public static IReadOnlyList<Opcao<UnidadeMedidaCarga>> UnidadesMedidaCarga { get; } =
    [
        new(UnidadeMedidaCarga.MetroCubico, "00 - M3"),
        new(UnidadeMedidaCarga.Quilograma, "01 - KG"),
        new(UnidadeMedidaCarga.Tonelada, "02 - TON"),
        new(UnidadeMedidaCarga.Unidade, "03 - UNIDADE"),
        new(UnidadeMedidaCarga.Litros, "04 - LITROS"),
        new(UnidadeMedidaCarga.Mmbtu, "05 - MMBTU"),
    ];

    /// <summary>Tipo do emitente do MDF-e.</summary>
    public static IReadOnlyList<Opcao<TipoEmitenteMdfe>> TiposEmitenteMdfe { get; } =
    [
        new(TipoEmitenteMdfe.PrestadorServico, "1 - Prestador de serviço de transporte"),
        new(TipoEmitenteMdfe.CargaPropria, "2 - Transportador de carga própria"),
        new(TipoEmitenteMdfe.CteGlobalizado, "3 - Prestador que emitirá CT-e globalizado"),
    ];

    /// <summary>Tipo do transportador (opcional no MDF-e).</summary>
    public static IReadOnlyList<Opcao<TipoTransportador>> TiposTransportador { get; } =
    [
        new(TipoTransportador.Etc, "1 - ETC (empresa)"),
        new(TipoTransportador.Tac, "2 - TAC (autônomo)"),
        new(TipoTransportador.Ctc, "3 - CTC (cooperativa)"),
    ];

    /// <summary>Tipo de carga do produto predominante.</summary>
    public static IReadOnlyList<Opcao<TipoCargaMdfe>> TiposCargaMdfe { get; } =
    [
        new(TipoCargaMdfe.GranelSolido, "01 - Granel sólido"),
        new(TipoCargaMdfe.GranelLiquido, "02 - Granel líquido"),
        new(TipoCargaMdfe.Frigorificada, "03 - Frigorificada"),
        new(TipoCargaMdfe.Conteinerizada, "04 - Conteinerizada"),
        new(TipoCargaMdfe.CargaGeral, "05 - Carga geral"),
        new(TipoCargaMdfe.Neogranel, "06 - Neogranel"),
        new(TipoCargaMdfe.PerigosaGranelSolido, "07 - Perigosa (granel sólido)"),
        new(TipoCargaMdfe.PerigosaGranelLiquido, "08 - Perigosa (granel líquido)"),
        new(TipoCargaMdfe.PerigosaFrigorificada, "09 - Perigosa (carga frigorificada)"),
        new(TipoCargaMdfe.PerigosaConteinerizada, "10 - Perigosa (conteinerizada)"),
        new(TipoCargaMdfe.PerigosaCargaGeral, "11 - Perigosa (carga geral)"),
        new(TipoCargaMdfe.GranelPressurizada, "12 - Granel pressurizada"),
    ];

    /// <summary>Unidade do peso bruto no MDF-e.</summary>
    public static IReadOnlyList<Opcao<UnidadePesoMdfe>> UnidadesPesoMdfe { get; } =
    [
        new(UnidadePesoMdfe.Quilograma, "01 - KG"),
        new(UnidadePesoMdfe.Tonelada, "02 - TON"),
    ];

    /// <summary>Texto de exibição de um valor, ou vazio quando não informado.</summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="opcoes">Lista de opções.</param>
    /// <param name="valor">Valor a descrever.</param>
    public static string Texto<T>(IReadOnlyList<Opcao<T>> opcoes, T? valor) where T : struct =>
        valor is null ? string.Empty : opcoes.FirstOrDefault(o => o.Valor.Equals(valor.Value))?.Texto ?? valor.Value.ToString() ?? string.Empty;
}
