using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Emissor.DFe.Leiaute;
using Emissor.DFe.Validacao;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using LeiauteCte = Emissor.DFe.Leiaute.Cte;
using MontadoresCte = Emissor.DFe.Cte;
using MontadoresMdfe = Emissor.DFe.Mdfe;
using LeiauteMdfe = Emissor.DFe.Leiaute.Mdfe;

namespace Emissor.DFe.Webservices;

/// <summary>Resultado da consulta do status do serviço.</summary>
/// <param name="Documento">"CT-e" ou "MDF-e".</param>
/// <param name="Servidor">Servidor consultado.</param>
/// <param name="Codigo">Código da situação (<c>cStat</c>); 107 = serviço em operação.</param>
/// <param name="Motivo">Descrição da situação (<c>xMotivo</c>).</param>
/// <param name="UfAtendida">Código da UF que atendeu (<c>cUF</c>).</param>
/// <param name="Recebimento">Data e hora do recebimento na SEFAZ (<c>dhRecbto</c>).</param>
/// <param name="TempoMedio">Tempo médio de resposta em segundos (<c>tMed</c>), se informado.</param>
/// <param name="Observacao">Observação da SEFAZ (<c>xObs</c>), se informada.</param>
/// <param name="VersaoAplicativo">Versão do aplicativo da SEFAZ (<c>verAplic</c>).</param>
public sealed record StatusServico(
    string Documento,
    string Servidor,
    int Codigo,
    string Motivo,
    string UfAtendida,
    string Recebimento,
    string? TempoMedio,
    string? Observacao,
    string VersaoAplicativo)
{
    /// <summary>Código "Serviço em Operação" (MOC CT-e 4.00 e MDF-e 3.00b, §4.6.6).</summary>
    public const int EmOperacaoCodigo = 107;

    /// <summary>Indica se o serviço está em operação.</summary>
    public bool EmOperacao => Codigo == EmOperacaoCodigo;
}

/// <summary>
/// Consulta do status do serviço (MOC CT-e 4.00 §4.6 e MDF-e 3.00b): mensagens <c>consStatServCTe</c> e
/// <c>consStatServMDFe</c> montadas pelas classes do XSD, validadas antes do envio, em XML sem compactação.
/// </summary>
public static class ServicoStatus
{
    /// <summary>
    /// Namespace do elemento de dados do status do CT-e. O MOC só traz o exemplo da recepção, sem o "V4"
    /// (<c>…/cte/wsdl/CTeRecepcaoSinc</c>); aqui se segue o nome do serviço da tabela oficial (<c>CTeStatusServicoV4</c>).
    /// A confirmar no WSDL ou na primeira chamada real (pendência 11 do levantamento).
    /// </summary>
    public const string NamespaceStatusCte = "http://www.portalfiscal.inf.br/cte/wsdl/CTeStatusServicoV4";

    /// <summary>Namespace do elemento de dados do status do MDF-e (padrão do MOC 3.00b: <c>…/mdfe/wsdl/&lt;serviço&gt;</c>).</summary>
    public const string NamespaceStatusMdfe = "http://www.portalfiscal.inf.br/mdfe/wsdl/MDFeStatusServico";

    /// <summary>Mensagem de consulta de status do CT-e, validada contra o XSD.</summary>
    /// <param name="codigoUf">Código IBGE da UF (ex.: 51 para MT).</param>
    /// <param name="ambiente">Ambiente.</param>
    public static XmlElement MensagemCte(int codigoUf, TipoAmbiente ambiente)
    {
        var consulta = new LeiauteCte.TConsStatServ
        {
            Versao = "4.00",
            TpAmb = LeiauteEnum.De<LeiauteCte.TAmb>((int)ambiente),
            Cuf = LeiauteEnum.De<LeiauteCte.TCodUfIbge>(codigoUf, 2),
            XServ = "STATUS",
        };
        return Validada(FormatacaoLeiaute.Serializar(consulta, MontadoresCte.MontadorCte.NamespaceCte).DocumentElement!, ValidadorXsd.SchemaConsStatServCte);
    }

    /// <summary>Mensagem de consulta de status do MDF-e, validada contra o XSD.</summary>
    /// <param name="ambiente">Ambiente.</param>
    public static XmlElement MensagemMdfe(TipoAmbiente ambiente)
    {
        var consulta = new LeiauteMdfe.TConsStatServ
        {
            Versao = "3.00",
            TpAmb = LeiauteEnum.De<LeiauteMdfe.TAmb>((int)ambiente),
            XServ = "STATUS",
        };
        return Validada(FormatacaoLeiaute.Serializar(consulta, MontadoresMdfe.MontadorMdfe.NamespaceMdfe).DocumentElement!, ValidadorXsd.SchemaConsStatServMdfe);
    }

    /// <summary>Consulta o status do serviço do CT-e no autorizador da UF.</summary>
    /// <param name="cliente">Cliente HTTPS com o certificado da empresa.</param>
    /// <param name="uf">UF do emitente.</param>
    /// <param name="codigoUf">Código IBGE da UF.</param>
    /// <param name="ambiente">Ambiente (só homologação é aceito pela tabela).</param>
    /// <param name="cancelamento">Cancelamento.</param>
    public static async Task<StatusServico> ConsultarCteAsync(HttpClient cliente, string uf, int codigoUf, TipoAmbiente ambiente, CancellationToken cancelamento = default)
    {
        var servico = new ServicoSoap(TabelaWebservices.Endereco(uf, ServicoSefaz.CteStatusServico, ambiente), NamespaceStatusCte, "cteDadosMsg", FormatoMensagem.Xml);
        var retorno = await ClienteSoap.EnviarAsync(cliente, servico, MensagemCte(codigoUf, ambiente), "retConsStatServCTe", cancelamento);
        var lido = Ler<LeiauteCte.TRetConsStatServ>(retorno);
        return new StatusServico("CT-e", servico.Endereco.Host, Codigo(lido.CStat), lido.XMotivo, Uf(lido.Cuf),
            lido.DhRecbto, lido.TMed, lido.XObs, lido.VerAplic);
    }

    /// <summary>Consulta o status do serviço do MDF-e (autorizador único, SVRS).</summary>
    /// <param name="cliente">Cliente HTTPS com o certificado da empresa.</param>
    /// <param name="uf">UF do emitente (não muda o autorizador).</param>
    /// <param name="ambiente">Ambiente (só homologação é aceito pela tabela).</param>
    /// <param name="cancelamento">Cancelamento.</param>
    public static async Task<StatusServico> ConsultarMdfeAsync(HttpClient cliente, string uf, TipoAmbiente ambiente, CancellationToken cancelamento = default)
    {
        var servico = new ServicoSoap(TabelaWebservices.Endereco(uf, ServicoSefaz.MdfeStatusServico, ambiente), NamespaceStatusMdfe, "mdfeDadosMsg", FormatoMensagem.Xml);
        var retorno = await ClienteSoap.EnviarAsync(cliente, servico, MensagemMdfe(ambiente), "retConsStatServMDFe", cancelamento);
        var lido = Ler<LeiauteMdfe.TRetConsStatServ>(retorno);
        return new StatusServico("MDF-e", servico.Endereco.Host, Codigo(lido.CStat), lido.XMotivo, Uf(lido.Cuf),
            lido.DhRecbto, lido.TMed, lido.XObs, lido.VerAplic);
    }

    private static XmlElement Validada(XmlElement mensagem, string schema)
    {
        var erros = ValidadorXsd.Validar(mensagem, schema);
        return erros.Count == 0 ? mensagem : throw new RegraNegocioException(erros);
    }

    private static T Ler<T>(XmlElement retorno)
    {
        try
        {
            using var leitor = new XmlNodeReader(retorno);
            return (T)new XmlSerializer(typeof(T)).Deserialize(leitor)!;
        }
        catch (InvalidOperationException erro)
        {
            throw new RegraNegocioException($"O retorno da SEFAZ não segue o leiaute: {erro.GetBaseException().Message}");
        }
    }

    private static int Codigo(string cStat) => int.TryParse(cStat, NumberStyles.None, CultureInfo.InvariantCulture, out var codigo) ? codigo : 0;

    private static string Uf<TEnum>(TEnum codigo) where TEnum : struct, Enum =>
        typeof(TEnum).GetField(codigo.ToString())?.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .OfType<XmlEnumAttribute>().FirstOrDefault()?.Name ?? codigo.ToString();
}
