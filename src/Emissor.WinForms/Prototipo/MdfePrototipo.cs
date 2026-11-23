using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// MDF-e fictício do protótipo da emissão (modal rodoviário). Guarda só o que as telas pedem; montagem do XML,
/// chave, assinatura, transmissão e eventos entram com o motor DF-e, a partir dos XSD do MDF-e 3.00b.
/// Contratante e pagamento do frete (<c>infContratante</c>, <c>infPag</c>) ficam fora até a pendência 6 do levantamento.
/// </summary>
public sealed partial class MdfePrototipo : RegistroPrototipo
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Empresa emitente.</summary>
    public int EmpresaId { get; set; }

    /// <summary>Situação no protótipo.</summary>
    public SituacaoMdfe Situacao { get; set; } = SituacaoMdfe.EmDigitacao;

    // ---- Etapa 1: identificação (ide) ----

    /// <summary>Tipo do emitente (<c>ide/tpEmit</c>).</summary>
    public TipoEmitenteMdfe? TipoEmitente { get; set; } = TipoEmitenteMdfe.PrestadorServico;

    /// <summary>Tipo do transportador (<c>ide/tpTransp</c>), opcional.</summary>
    public TipoTransportador? TipoTransportador { get; set; }

    /// <summary>Série escolhida no cadastro de séries (<c>ide/serie</c>).</summary>
    public int? SerieId { get; set; }

    /// <summary>Número do MDF-e (<c>ide/nMDF</c>); definido só na transmissão.</summary>
    public int? Numero { get; set; }

    /// <summary>Data de emissão (<c>ide/dhEmi</c>).</summary>
    public DateTime DataEmissao { get; set; } = DateTime.Today;

    /// <summary>Início previsto da viagem (<c>ide/dhIniViagem</c>), opcional.</summary>
    public DateTime? InicioViagem { get; set; }

    /// <summary>UF de início (<c>ide/UFIni</c>).</summary>
    public string UfInicio { get; set; } = "MT";

    /// <summary>UF de fim (<c>ide/UFFim</c>).</summary>
    public string UfFim { get; set; } = string.Empty;

    /// <summary>UFs do percurso, em ordem, sem as de início e fim (<c>ide/infPercurso/UFPer</c>, até 25).</summary>
    public List<string> Percurso { get; set; } = [];

    // ---- Etapa 2: carregamento e descarga ----

    /// <summary>Municípios de carregamento (<c>ide/infMunCarrega</c>, 1 a 50).</summary>
    public List<int> MunicipiosCarregamento { get; set; } = [];

    /// <summary>Municípios de descarga com os documentos (<c>infDoc/infMunDescarga</c>).</summary>
    public List<DescargaMdfePrototipo> Descargas { get; set; } = [];

    // ---- Etapa 3: veículos, condutores e CIOT (rodo) ----

    /// <summary>Veículo de tração (<c>rodo/veicTracao</c>).</summary>
    public int? VeiculoTracaoId { get; set; }

    /// <summary>Reboques (<c>rodo/veicReboque</c>, até 3).</summary>
    public List<int> ReboquesIds { get; set; } = [];

    /// <summary>Condutores (<c>veicTracao/condutor</c>, 1 a 10).</summary>
    public List<int> CondutoresIds { get; set; } = [];

    /// <summary>CIOT da viagem (<c>rodo/infANTT/infCIOT</c>).</summary>
    public List<CiotPrototipo> Ciots { get; set; } = [];

    // ---- Etapa 4: carga, seguro e totais ----

    /// <summary>Tipo de carga do produto predominante (<c>prodPred/tpCarga</c>).</summary>
    public TipoCargaMdfe? TipoCarga { get; set; }

    /// <summary>Descrição do produto predominante (<c>prodPred/xProd</c>, até 120).</summary>
    public string ProdutoPredominante { get; set; } = string.Empty;

    /// <summary>Apólice do cadastro usada no grupo <c>seg</c>.</summary>
    public int? ApoliceId { get; set; }

    /// <summary>Números de averbação (<c>seg/nAver</c>, até 40 caracteres cada).</summary>
    public List<string> Averbacoes { get; set; } = [];

    /// <summary>Valor total da carga (<c>tot/vCarga</c>).</summary>
    public decimal? ValorCarga { get; set; }

    /// <summary>Unidade do peso (<c>tot/cUnid</c>).</summary>
    public UnidadePesoMdfe? UnidadePeso { get; set; } = UnidadePesoMdfe.Quilograma;

    /// <summary>Peso bruto total (<c>tot/qCarga</c>).</summary>
    public decimal? PesoCarga { get; set; }

    // ---- Etapa 5 ----

    /// <summary>Informações complementares (<c>infAdic/infCpl</c>, até 5.000).</summary>
    public string InformacoesComplementares { get; set; } = string.Empty;

    // ---- Regras derivadas ----

    /// <summary>Quantidade de CT-e (<c>tot/qCTe</c>).</summary>
    public int QuantidadeCte => Descargas.Sum(d => d.Documentos.Count(x => x.Tipo == TipoDocumentoMdfe.Cte));

    /// <summary>Quantidade de NF-e (<c>tot/qNFe</c>).</summary>
    public int QuantidadeNfe => Descargas.Sum(d => d.Documentos.Count(x => x.Tipo == TipoDocumentoMdfe.Nfe));

    /// <summary>
    /// CIOT obrigatório (NT MDF-e 2026.001, rejeição 684): prestador de serviço (1), CT-e globalizado (3)
    /// ou carga própria (2) com tipo do transportador informado. Ver §6.1 do levantamento.
    /// </summary>
    public bool ExigeCiot =>
        TipoEmitente is TipoEmitenteMdfe.PrestadorServico or TipoEmitenteMdfe.CteGlobalizado
        || (TipoEmitente == TipoEmitenteMdfe.CargaPropria && TipoTransportador is not null);

    /// <summary>Seguro exigido para quem transporta carga de terceiros (Lei 11.442/2007, citada na anotação do <c>respSeg</c>).</summary>
    public bool ExigeSeguro => TipoEmitente is TipoEmitenteMdfe.PrestadorServico or TipoEmitenteMdfe.CteGlobalizado;

    // ---- Descrições ----

    /// <summary>Número para exibição.</summary>
    public string NumeroDescricao => Numero?.ToString(Brasil) ?? "—";

    /// <summary>Número da série para exibição.</summary>
    public string SerieDescricao => SerieId is null ? string.Empty : DadosFicticios.Series.ObterPorId(SerieId.Value)?.Serie?.ToString(Brasil) ?? string.Empty;

    /// <summary>Placa da tração.</summary>
    public string PlacaTracao => VeiculoTracaoId is null ? string.Empty : DadosFicticios.Veiculos.ObterPorId(VeiculoTracaoId.Value)?.Placa ?? string.Empty;

    /// <summary>Percurso para exibição (ex.: "MT → MS → SP").</summary>
    public string PercursoDescricao => string.Join(" → ", new[] { UfInicio }.Concat(Percurso).Append(UfFim).Where(u => u.Length > 0));

    /// <summary>Situação para exibição.</summary>
    public string SituacaoDescricao => Situacao == SituacaoMdfe.ProntoParaTransmitir ? "Pronto para transmitir" : "Em digitação";

    // ---- Validações por etapa ----

    /// <summary>Pendências da etapa 1 (identificação e percurso).</summary>
    public IReadOnlyList<string> ValidarIdentificacao()
    {
        var faltando = new List<string>();
        if (TipoEmitente is null) faltando.Add("Tipo do emitente");
        if (SerieId is null) faltando.Add("Série");
        if (!OpcoesCadastro.Ufs.Contains(UfInicio)) faltando.Add("UF de início");
        if (!OpcoesCadastro.Ufs.Contains(UfFim)) faltando.Add("UF de fim");
        if (Percurso.Any(u => !OpcoesCadastro.Ufs.Contains(u))) faltando.Add("UFs do percurso: use siglas válidas separadas por vírgula");
        if (Percurso.Contains(UfInicio) || Percurso.Contains(UfFim)) faltando.Add("UFs do percurso: não repita a UF de início nem a de fim");
        if (Percurso.Count > 25) faltando.Add("UFs do percurso: no máximo 25");
        if (InicioViagem is not null && InicioViagem.Value.Date < DataEmissao.Date) faltando.Add("Início da viagem não pode ser antes da emissão");
        return faltando;
    }

    /// <summary>Pendências da etapa 2 (carregamento e descarga).</summary>
    public IReadOnlyList<string> ValidarCarregamentoDescarga()
    {
        var faltando = new List<string>();
        if (MunicipiosCarregamento.Count == 0) faltando.Add("Ao menos um município de carregamento");
        if (MunicipiosCarregamento.Count > 50) faltando.Add("No máximo 50 municípios de carregamento");
        if (MunicipiosCarregamento.Any(c => DadosFicticios.ObterMunicipio(c)?.Uf != UfInicio)) faltando.Add($"Municípios de carregamento devem ser da UF de início ({UfInicio})");
        if (Descargas.Count == 0) faltando.Add("Ao menos um município de descarga");
        if (Descargas.Any(d => DadosFicticios.ObterMunicipio(d.MunicipioCodigo)?.Uf != UfFim)) faltando.Add($"Municípios de descarga devem ser da UF de fim ({UfFim})");
        if (Descargas.Any(d => d.Documentos.Count == 0)) faltando.Add("Cada município de descarga precisa de ao menos um documento");
        if (Descargas.SelectMany(d => d.Documentos).Any(x => x.CteId is null && !ChaveValida().IsMatch(x.Chave)))
        {
            faltando.Add("Chaves de documentos com 44 dígitos");
        }

        if (TipoEmitente != TipoEmitenteMdfe.CargaPropria && QuantidadeNfe > 0)
        {
            faltando.Add("NF-e diretamente no MDF-e só para carga própria (tipo do emitente 2); prestador vincula CT-e");
        }

        return faltando;
    }

    /// <summary>Pendências da etapa 3 (veículos, condutores e CIOT).</summary>
    public IReadOnlyList<string> ValidarVeiculos()
    {
        var faltando = new List<string>();
        if (VeiculoTracaoId is null) faltando.Add("Veículo de tração");
        if (ReboquesIds.Count > 3) faltando.Add("No máximo 3 reboques");
        if (ReboquesIds.Distinct().Count() != ReboquesIds.Count) faltando.Add("Reboque repetido");
        if (CondutoresIds.Count == 0) faltando.Add("Ao menos um condutor");
        if (CondutoresIds.Count > 10) faltando.Add("No máximo 10 condutores");
        if (ExigeCiot && Ciots.Count == 0) faltando.Add("CIOT (obrigatório para este tipo de emitente — NT 2026.001, rejeição 684)");
        if (Ciots.Any(c => !CiotValido().IsMatch(c.Ciot) || string.IsNullOrWhiteSpace(c.CpfCnpjResponsavel)))
        {
            faltando.Add("CIOT com 12 dígitos e CPF/CNPJ do responsável");
        }

        return faltando;
    }

    /// <summary>Pendências da etapa 4 (carga, seguro e totais).</summary>
    public IReadOnlyList<string> ValidarCargaSeguro()
    {
        var faltando = new List<string>();
        if (TipoCarga is null) faltando.Add("Tipo de carga");
        if (string.IsNullOrWhiteSpace(ProdutoPredominante)) faltando.Add("Produto predominante");
        if (ExigeSeguro && ApoliceId is null) faltando.Add("Apólice de seguro (transporte de carga de terceiros)");
        if (Averbacoes.Any(a => a.Length > 40)) faltando.Add("Averbação com no máximo 40 caracteres");
        if (ValorCarga is null or <= 0) faltando.Add("Valor total da carga");
        if (UnidadePeso is null) faltando.Add("Unidade do peso");
        if (PesoCarga is null or <= 0) faltando.Add("Peso bruto da carga");
        return faltando;
    }

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios() =>
        [.. ValidarIdentificacao(), .. ValidarCarregamentoDescarga(), .. ValidarVeiculos(), .. ValidarCargaSeguro()];

    /// <summary>Texto da conferência (etapa 5).</summary>
    public string MontarResumo()
    {
        var texto = new StringBuilder();
        texto.AppendLine("IDENTIFICAÇÃO");
        texto.AppendLine($"  Emitente: {OpcoesCadastro.Texto(OpcoesCadastro.TiposEmitenteMdfe, TipoEmitente)}");
        texto.AppendLine($"  Transportador: {(TipoTransportador is null ? "não informado" : OpcoesCadastro.Texto(OpcoesCadastro.TiposTransportador, TipoTransportador))}");
        texto.AppendLine($"  Série: {SerieDescricao}    Número: {Numero?.ToString(Brasil) ?? "definido na transmissão"}    Emissão: {DataEmissao:dd/MM/yyyy}");
        texto.AppendLine($"  Início da viagem: {(InicioViagem is null ? "não informado" : InicioViagem.Value.ToString("dd/MM/yyyy HH:mm", Brasil))}");
        texto.AppendLine($"  Percurso: {PercursoDescricao}");
        texto.AppendLine();
        texto.AppendLine("CARREGAMENTO");
        foreach (var codigo in MunicipiosCarregamento) texto.AppendLine($"  {DadosFicticios.ObterMunicipio(codigo)?.Descricao}");
        texto.AppendLine("DESCARGA");
        foreach (var descarga in Descargas)
        {
            texto.AppendLine($"  {DadosFicticios.ObterMunicipio(descarga.MunicipioCodigo)?.Descricao}");
            foreach (var documento in descarga.Documentos) texto.AppendLine($"    {(documento.Tipo == TipoDocumentoMdfe.Cte ? "CT-e" : "NF-e")} {documento.ChaveDescricao}");
        }

        texto.AppendLine();
        texto.AppendLine("VEÍCULOS E CONDUTORES");
        texto.AppendLine($"  Tração: {PlacaTracao}");
        foreach (var id in ReboquesIds) texto.AppendLine($"  Reboque: {DadosFicticios.Veiculos.ObterPorId(id)?.Placa}");
        foreach (var id in CondutoresIds)
        {
            var motorista = DadosFicticios.Motoristas.ObterPorId(id);
            texto.AppendLine($"  Condutor: {motorista?.Nome} ({motorista?.Cpf})");
        }

        foreach (var ciot in Ciots) texto.AppendLine($"  CIOT: {ciot.Ciot} (responsável {ciot.CpfCnpjResponsavel})");
        if (Ciots.Count == 0) texto.AppendLine($"  CIOT: não informado{(ExigeCiot ? " (OBRIGATÓRIO)" : string.Empty)}");
        texto.AppendLine();
        texto.AppendLine("CARGA, SEGURO E TOTAIS");
        texto.AppendLine($"  Produto: {OpcoesCadastro.Texto(OpcoesCadastro.TiposCargaMdfe, TipoCarga)} - {ProdutoPredominante}");
        var apolice = ApoliceId is null ? null : DadosFicticios.Apolices.ObterPorId(ApoliceId.Value);
        texto.AppendLine($"  Seguro: {(apolice is null ? "não informado" : $"{apolice.NomeSeguradora}, apólice {apolice.NumeroApolice} ({apolice.ResponsavelDescricao})")}");
        foreach (var averbacao in Averbacoes) texto.AppendLine($"  Averbação: {averbacao}");
        texto.AppendLine($"  CT-e: {QuantidadeCte}    NF-e: {QuantidadeNfe}");
        texto.AppendLine($"  Valor da carga: {(ValorCarga ?? 0).ToString("C2", Brasil)}    Peso: {(PesoCarga ?? 0).ToString("N4", Brasil)} {OpcoesCadastro.Texto(OpcoesCadastro.UnidadesPesoMdfe, UnidadePeso)}");
        return texto.ToString();
    }

    /// <inheritdoc />
    protected override void CopiarColecoes()
    {
        Percurso = [.. Percurso];
        MunicipiosCarregamento = [.. MunicipiosCarregamento];
        Descargas = Descargas.Select(d => d.Copiar()).ToList();
        ReboquesIds = [.. ReboquesIds];
        CondutoresIds = [.. CondutoresIds];
        Ciots = Ciots.Select(c => c.Copiar()).ToList();
        Averbacoes = [.. Averbacoes];
    }

    [GeneratedRegex("^[0-9]{44}$")]
    private static partial Regex ChaveValida();

    [GeneratedRegex("^[0-9]{12}$")]
    private static partial Regex CiotValido();
}
