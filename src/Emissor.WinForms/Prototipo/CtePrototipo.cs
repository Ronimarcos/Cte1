using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// CT-e fictício do protótipo da emissão (modal rodoviário). Guarda só o que as telas pedem; a montagem do XML,
/// a chave de acesso, a assinatura e a transmissão entram com o motor DF-e, a partir dos XSD do NT 2026.005.
/// As validações estão separadas por etapa do assistente.
/// </summary>
public sealed partial class CtePrototipo : RegistroPrototipo
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Empresa emitente.</summary>
    public int EmpresaId { get; set; }

    /// <summary>Situação no protótipo.</summary>
    public SituacaoCte Situacao { get; set; } = SituacaoCte.EmDigitacao;

    // ---- Etapa 1: identificação (grupo ide) ----

    /// <summary>Tipo do CT-e (<c>ide/tpCTe</c>).</summary>
    public TipoCte? Tipo { get; set; } = TipoCte.Normal;

    /// <summary>Tipo do serviço (<c>ide/tpServ</c>).</summary>
    public TipoServicoCte? TipoServico { get; set; } = TipoServicoCte.Normal;

    /// <summary>Série escolhida no cadastro de séries (<c>ide/serie</c>).</summary>
    public int? SerieId { get; set; }

    /// <summary>Número do CT-e (<c>ide/nCT</c>); definido só na transmissão, que o protótipo não faz.</summary>
    public int? Numero { get; set; }

    /// <summary>CFOP da prestação (<c>ide/CFOP</c>, 4 dígitos).</summary>
    public string Cfop { get; set; } = string.Empty;

    /// <summary>Natureza da operação (<c>ide/natOp</c>).</summary>
    public string NaturezaOperacao { get; set; } = string.Empty;

    /// <summary>Data de emissão (<c>ide/dhEmi</c>).</summary>
    public DateTime DataEmissao { get; set; } = DateTime.Today;

    // ---- Etapa 2: participantes ----

    /// <summary>Remetente (<c>rem</c>).</summary>
    public int? RemetenteId { get; set; }

    /// <summary>Destinatário (<c>dest</c>).</summary>
    public int? DestinatarioId { get; set; }

    /// <summary>Expedidor (<c>exped</c>), opcional.</summary>
    public int? ExpedidorId { get; set; }

    /// <summary>Recebedor (<c>receb</c>), opcional.</summary>
    public int? RecebedorId { get; set; }

    /// <summary>Tomador do serviço (<c>toma3/toma</c> ou <c>toma4/toma</c>).</summary>
    public TomadorServico? Tomador { get; set; } = TomadorServico.Remetente;

    /// <summary>Tomador quando é "4 - Outros" (<c>toma4</c>).</summary>
    public int? TomadorOutrosId { get; set; }

    /// <summary>Município de início da prestação (<c>ide/cMunIni</c>).</summary>
    public int? MunicipioInicioCodigo { get; set; }

    /// <summary>Município de término da prestação (<c>ide/cMunFim</c>).</summary>
    public int? MunicipioFimCodigo { get; set; }

    // ---- Etapa 3: carga e documentos (infCTeNorm / infCteComp) ----

    /// <summary>Valor total da carga (<c>infCarga/vCarga</c>).</summary>
    public decimal? ValorCarga { get; set; }

    /// <summary>Produto predominante (<c>infCarga/proPred</c>).</summary>
    public string ProdutoPredominante { get; set; } = string.Empty;

    /// <summary>Outras características da carga (<c>infCarga/xOutCat</c>).</summary>
    public string OutrasCaracteristicas { get; set; } = string.Empty;

    /// <summary>Quantidades da carga (<c>infCarga/infQ</c>).</summary>
    public List<QuantidadeCargaPrototipo> Quantidades { get; set; } = [];

    /// <summary>NF-e transportadas (<c>infDoc/infNFe</c>).</summary>
    public List<DocumentoNfePrototipo> DocumentosNfe { get; set; } = [];

    /// <summary>Documentos anteriores (<c>docAnt</c>), em subcontratação e redespacho.</summary>
    public List<DocumentoAnteriorPrototipo> DocumentosAnteriores { get; set; } = [];

    /// <summary>Chave do CT-e complementado (<c>infCteComp/chCTe</c>), no CT-e de complemento.</summary>
    public string ChaveCteComplementado { get; set; } = string.Empty;

    // ---- Etapa 4: valores (vPrest) ----

    /// <summary>Componentes do valor da prestação (<c>vPrest/Comp</c>).</summary>
    public List<ComponenteFretePrototipo> Componentes { get; set; } = [];

    /// <summary>Valor a receber (<c>vPrest/vRec</c>).</summary>
    public decimal? ValorReceber { get; set; }

    // ---- Etapa 5: conferência ----

    /// <summary>Observações gerais (<c>compl/xObs</c>).</summary>
    public string Observacoes { get; set; } = string.Empty;

    // ---- Regras derivadas ----

    /// <summary>Valor total da prestação (<c>vPrest/vTPrest</c>): soma dos componentes.</summary>
    public decimal ValorTotalPrestacao => Componentes.Sum(c => c.Valor);

    /// <summary>Indica se o CT-e é de complemento de valores (usa <c>infCteComp</c> em vez de <c>infCTeNorm</c>).</summary>
    public bool EhComplemento => Tipo == TipoCte.Complemento;

    /// <summary>Subcontratação e redespachos exigem o documento anterior (<c>docAnt</c>).</summary>
    public bool ExigeDocumentoAnterior =>
        !EhComplemento && TipoServico is TipoServicoCte.Subcontratacao or TipoServicoCte.Redespacho or TipoServicoCte.RedespachoIntermediario;

    // ---- Descrições para a lista e a conferência ----

    /// <summary>Número para exibição.</summary>
    public string NumeroDescricao => Numero?.ToString(Brasil) ?? "—";

    /// <summary>Número da série para exibição.</summary>
    public string SerieDescricao => ObterSerie()?.Serie?.ToString(Brasil) ?? string.Empty;

    /// <summary>Tipo do CT-e para exibição.</summary>
    public string TipoDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposCte, Tipo);

    /// <summary>Tipo do serviço para exibição.</summary>
    public string TipoServicoDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposServicoCte, TipoServico);

    /// <summary>Nome do remetente.</summary>
    public string RemetenteNome => NomeParticipante(RemetenteId);

    /// <summary>Nome do destinatário.</summary>
    public string DestinatarioNome => NomeParticipante(DestinatarioId);

    /// <summary>Nome do tomador, conforme o papel escolhido.</summary>
    public string TomadorNome => NomeParticipante(TomadorId);

    /// <summary>Situação para exibição.</summary>
    public string SituacaoDescricao => Situacao switch
    {
        SituacaoCte.ProntoParaTransmitir => "Pronto para transmitir",
        _ => "Em digitação",
    };

    /// <summary>Participante que é o tomador, conforme o campo <see cref="Tomador"/>.</summary>
    public int? TomadorId => Tomador switch
    {
        TomadorServico.Remetente => RemetenteId,
        TomadorServico.Expedidor => ExpedidorId,
        TomadorServico.Recebedor => RecebedorId,
        TomadorServico.Destinatario => DestinatarioId,
        TomadorServico.Outros => TomadorOutrosId,
        _ => null,
    };

    /// <summary>Série escolhida, ou <c>null</c>.</summary>
    public SeriePrototipo? ObterSerie() => SerieId is null ? null : DadosFicticios.Series.ObterPorId(SerieId.Value);

    // ---- Validações por etapa ----

    /// <summary>Pendências da etapa 1 (identificação).</summary>
    public IReadOnlyList<string> ValidarIdentificacao()
    {
        var faltando = new List<string>();
        if (Tipo is null) faltando.Add("Tipo do CT-e");
        if (TipoServico is null) faltando.Add("Tipo do serviço");
        if (SerieId is null) faltando.Add("Série");
        if (!CfopValido().IsMatch(Cfop)) faltando.Add("CFOP (4 dígitos)");
        if (string.IsNullOrWhiteSpace(NaturezaOperacao)) faltando.Add("Natureza da operação");
        return faltando;
    }

    /// <summary>Pendências da etapa 2 (participantes e percurso).</summary>
    public IReadOnlyList<string> ValidarParticipantes()
    {
        var faltando = new List<string>();
        if (RemetenteId is null) faltando.Add("Remetente");
        if (DestinatarioId is null) faltando.Add("Destinatário");
        if (Tomador is null) faltando.Add("Tomador do serviço");
        else if (Tomador == TomadorServico.Expedidor && ExpedidorId is null) faltando.Add("Expedidor (é o tomador)");
        else if (Tomador == TomadorServico.Recebedor && RecebedorId is null) faltando.Add("Recebedor (é o tomador)");
        else if (Tomador == TomadorServico.Outros && TomadorOutrosId is null) faltando.Add("Tomador (outros)");
        if (MunicipioInicioCodigo is null) faltando.Add("Município de início da prestação");
        if (MunicipioFimCodigo is null) faltando.Add("Município de término da prestação");
        return faltando;
    }

    /// <summary>Pendências da etapa 3 (carga e documentos), que dependem do tipo do CT-e e do serviço.</summary>
    public IReadOnlyList<string> ValidarCargaDocumentos()
    {
        var faltando = new List<string>();
        if (EhComplemento)
        {
            if (!ChaveValida().IsMatch(ChaveCteComplementado)) faltando.Add("Chave do CT-e complementado (44 dígitos)");
            return faltando;
        }

        if (ValorCarga is null or <= 0) faltando.Add("Valor da carga");
        if (string.IsNullOrWhiteSpace(ProdutoPredominante)) faltando.Add("Produto predominante");
        if (Quantidades.Count == 0) faltando.Add("Ao menos uma quantidade da carga");
        if (Quantidades.Any(q => q.Quantidade <= 0 || string.IsNullOrWhiteSpace(q.TipoMedida)))
        {
            faltando.Add("Quantidades: tipo da medida e quantidade maior que zero");
        }

        if (DocumentosNfe.Count == 0) faltando.Add("Ao menos uma NF-e");
        if (DocumentosNfe.Any(d => !ChaveValida().IsMatch(d.Chave))) faltando.Add("Chaves de NF-e com 44 dígitos");

        if (ExigeDocumentoAnterior)
        {
            if (DocumentosAnteriores.Count == 0) faltando.Add("Documento anterior (subcontratação/redespacho)");
            if (DocumentosAnteriores.Any(d => string.IsNullOrWhiteSpace(d.CnpjCpfEmitente)
                || string.IsNullOrWhiteSpace(d.NomeEmitente) || !ChaveValida().IsMatch(d.ChaveCte)))
            {
                faltando.Add("Documentos anteriores: emitente e chave do CT-e com 44 dígitos");
            }
        }

        return faltando;
    }

    /// <summary>Pendências da etapa 4 (valores).</summary>
    public IReadOnlyList<string> ValidarValores()
    {
        var faltando = new List<string>();
        if (Componentes.Count == 0) faltando.Add("Ao menos um componente do frete");
        if (Componentes.Any(c => string.IsNullOrWhiteSpace(c.Nome) || c.Valor < 0)) faltando.Add("Componentes: nome e valor não negativo");
        if (ValorTotalPrestacao <= 0) faltando.Add("Valor total da prestação maior que zero");
        if (ValorReceber is null or < 0) faltando.Add("Valor a receber");
        else if (ValorReceber > ValorTotalPrestacao) faltando.Add("Valor a receber não pode passar do total da prestação");
        return faltando;
    }

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios() =>
        [.. ValidarIdentificacao(), .. ValidarParticipantes(), .. ValidarCargaDocumentos(), .. ValidarValores()];

    /// <summary>Texto da conferência (etapa 5).</summary>
    public string MontarResumo()
    {
        var texto = new StringBuilder();
        texto.AppendLine("IDENTIFICAÇÃO");
        texto.AppendLine($"  Tipo: {TipoDescricao}    Serviço: {TipoServicoDescricao}");
        texto.AppendLine($"  Série: {SerieDescricao}    Número: {Numero?.ToString(Brasil) ?? "definido na transmissão"}    Emissão: {DataEmissao:dd/MM/yyyy}");
        texto.AppendLine($"  CFOP: {Cfop} - {NaturezaOperacao}");
        texto.AppendLine();
        texto.AppendLine("PARTICIPANTES");
        texto.AppendLine($"  Remetente: {RemetenteNome}");
        texto.AppendLine($"  Destinatário: {DestinatarioNome}");
        if (ExpedidorId is not null) texto.AppendLine($"  Expedidor: {NomeParticipante(ExpedidorId)}");
        if (RecebedorId is not null) texto.AppendLine($"  Recebedor: {NomeParticipante(RecebedorId)}");
        texto.AppendLine($"  Tomador: {OpcoesCadastro.Texto(OpcoesCadastro.Tomadores, Tomador)} - {TomadorNome}");
        texto.AppendLine($"  Percurso: {DescricaoMunicipio(MunicipioInicioCodigo)} → {DescricaoMunicipio(MunicipioFimCodigo)}");
        texto.AppendLine();

        if (EhComplemento)
        {
            texto.AppendLine("CT-E COMPLEMENTADO");
            texto.AppendLine($"  Chave: {ChaveCteComplementado}");
        }
        else
        {
            texto.AppendLine("CARGA");
            texto.AppendLine($"  Valor: {Moeda(ValorCarga ?? 0)}    Produto predominante: {ProdutoPredominante}");
            if (!string.IsNullOrWhiteSpace(OutrasCaracteristicas)) texto.AppendLine($"  Outras características: {OutrasCaracteristicas}");
            foreach (var quantidade in Quantidades)
            {
                var unidade = OpcoesCadastro.Texto(OpcoesCadastro.UnidadesMedidaCarga, quantidade.Unidade);
                texto.AppendLine($"  {quantidade.TipoMedida}: {quantidade.Quantidade.ToString("N4", Brasil)} ({unidade})");
            }

            texto.AppendLine();
            texto.AppendLine($"DOCUMENTOS ({DocumentosNfe.Count} NF-e)");
            foreach (var documento in DocumentosNfe) texto.AppendLine($"  NF-e {documento.Chave}");
            foreach (var anterior in DocumentosAnteriores)
            {
                texto.AppendLine($"  Doc. anterior: {anterior.NomeEmitente} ({anterior.CnpjCpfEmitente}) CT-e {anterior.ChaveCte}");
            }
        }

        texto.AppendLine();
        texto.AppendLine("VALORES");
        foreach (var componente in Componentes) texto.AppendLine($"  {componente.Nome}: {Moeda(componente.Valor)}");
        texto.AppendLine($"  Total da prestação: {Moeda(ValorTotalPrestacao)}    A receber: {Moeda(ValorReceber ?? 0)}");
        texto.AppendLine("  ICMS: Simples Nacional (grupo ICMSSN, CST 90, indSN 1)");
        texto.AppendLine("  IBS/CBS: a calcular na implementação (NT 2026.005)");
        return texto.ToString();
    }

    /// <inheritdoc />
    protected override void CopiarColecoes()
    {
        Quantidades = Quantidades.Select(q => q.Copiar()).ToList();
        DocumentosNfe = DocumentosNfe.Select(d => d.Copiar()).ToList();
        DocumentosAnteriores = DocumentosAnteriores.Select(d => d.Copiar()).ToList();
        Componentes = Componentes.Select(c => c.Copiar()).ToList();
    }

    private static string NomeParticipante(int? id) =>
        id is null ? string.Empty : DadosFicticios.Participantes.ObterPorId(id.Value)?.RazaoSocial ?? string.Empty;

    private static string DescricaoMunicipio(int? codigo) => DadosFicticios.ObterMunicipio(codigo)?.Descricao ?? "?";

    private static string Moeda(decimal valor) => valor.ToString("C2", Brasil);

    [GeneratedRegex("^[0-9]{44}$")]
    private static partial Regex ChaveValida();

    [GeneratedRegex("^[0-9]{4}$")]
    private static partial Regex CfopValido();
}
