using System.Text.RegularExpressions;
using Emissor.Dominio.Enums;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// CT-e (modelo 57, modal rodoviário) emitido pela empresa. Guarda o que o assistente de emissão pede; a montagem
/// do XML, a chave de acesso, a assinatura e a transmissão entram com o motor DF-e (P4b e P4c), a partir dos XSD
/// do NT 2026.005. As validações estão separadas por etapa do assistente.
/// </summary>
public partial class ConhecimentoTransporte : DocumentoFiscal
{
    // ---- Etapa 1: identificação (grupo ide) ----

    /// <summary>Tipo do CT-e (<c>ide/tpCTe</c>).</summary>
    public TipoCte? Tipo { get; set; } = TipoCte.Normal;

    /// <summary>Tipo do serviço (<c>ide/tpServ</c>).</summary>
    public TipoServicoCte? TipoServico { get; set; } = TipoServicoCte.Normal;

    /// <summary>CFOP da prestação (<c>ide/CFOP</c>, 4 dígitos).</summary>
    public string Cfop { get; set; } = string.Empty;

    /// <summary>Natureza da operação (<c>ide/natOp</c>).</summary>
    public string NaturezaOperacao { get; set; } = string.Empty;

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
    public List<QuantidadeCarga> Quantidades { get; set; } = [];

    /// <summary>NF-e transportadas (<c>infDoc/infNFe</c>).</summary>
    public List<DocumentoNfeCte> DocumentosNfe { get; set; } = [];

    /// <summary>Documentos anteriores (<c>docAnt</c>), em subcontratação e redespacho.</summary>
    public List<DocumentoAnteriorCte> DocumentosAnteriores { get; set; } = [];

    /// <summary>Chave do CT-e complementado (<c>infCteComp/chCTe</c>), no CT-e de complemento.</summary>
    public string ChaveCteComplementado { get; set; } = string.Empty;

    // ---- Etapa 4: valores (vPrest) ----

    /// <summary>Componentes do valor da prestação (<c>vPrest/Comp</c>).</summary>
    public List<ComponenteFrete> Componentes { get; set; } = [];

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

    /// <inheritdoc />
    public override void CopiarItensDe(DocumentoFiscal origem)
    {
        var cte = (ConhecimentoTransporte)origem;
        Quantidades = cte.Quantidades.Select(q => q.Copiar()).ToList();
        DocumentosNfe = cte.DocumentosNfe.Select(d => d.Copiar()).ToList();
        DocumentosAnteriores = cte.DocumentosAnteriores.Select(d => d.Copiar()).ToList();
        Componentes = cte.Componentes.Select(c => c.Copiar()).ToList();
    }

    [GeneratedRegex("^[0-9]{44}$")]
    private static partial Regex ChaveValida();

    [GeneratedRegex("^[0-9]{4}$")]
    private static partial Regex CfopValido();
}

/// <summary>Quantidade da carga (grupo <c>infCarga/infQ</c>). Propriedades com setter público para edição em grade.</summary>
public class QuantidadeCarga
{
    /// <summary>Unidade de medida (<c>cUnid</c>).</summary>
    public UnidadeMedidaCarga Unidade { get; set; } = UnidadeMedidaCarga.Quilograma;

    /// <summary>Tipo da medida (<c>tpMed</c>), ex.: "PESO BRUTO".</summary>
    public string TipoMedida { get; set; } = string.Empty;

    /// <summary>Quantidade (<c>qCarga</c>).</summary>
    public decimal Quantidade { get; set; }

    /// <summary>Cópia independente da linha.</summary>
    public QuantidadeCarga Copiar() => new() { Unidade = Unidade, TipoMedida = TipoMedida, Quantidade = Quantidade };
}

/// <summary>NF-e transportada (grupo <c>infDoc/infNFe</c>).</summary>
public class DocumentoNfeCte
{
    /// <summary>Chave de acesso da NF-e com 44 dígitos (<c>chave</c>).</summary>
    public string Chave { get; set; } = string.Empty;

    /// <summary>Cópia independente da linha.</summary>
    public DocumentoNfeCte Copiar() => new() { Chave = Chave };
}

/// <summary>
/// Documento de transporte anterior (grupo <c>docAnt/emiDocAnt</c>), usado em subcontratação e redespacho.
/// Considera só o CT-e eletrônico do emitente anterior.
/// </summary>
public class DocumentoAnteriorCte
{
    /// <summary>CNPJ ou CPF do emitente do documento anterior.</summary>
    public string CnpjCpfEmitente { get; set; } = string.Empty;

    /// <summary>Razão social ou nome do emitente (<c>xNome</c>).</summary>
    public string NomeEmitente { get; set; } = string.Empty;

    /// <summary>Chave do CT-e anterior com 44 dígitos (<c>idDocAntEle/chCTe</c>).</summary>
    public string ChaveCte { get; set; } = string.Empty;

    /// <summary>Cópia independente da linha.</summary>
    public DocumentoAnteriorCte Copiar() => new() { CnpjCpfEmitente = CnpjCpfEmitente, NomeEmitente = NomeEmitente, ChaveCte = ChaveCte };
}

/// <summary>Componente do valor da prestação (grupo <c>vPrest/Comp</c>).</summary>
public class ComponenteFrete
{
    /// <summary>Nome do componente (<c>xNome</c>, até 15 caracteres), ex.: "FRETE PESO".</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Valor do componente (<c>vComp</c>).</summary>
    public decimal Valor { get; set; }

    /// <summary>Cópia independente da linha.</summary>
    public ComponenteFrete Copiar() => new() { Nome = Nome, Valor = Valor };
}
