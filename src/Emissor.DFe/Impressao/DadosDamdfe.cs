using System.Xml;
using Emissor.DFe.Webservices;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using static Emissor.DFe.Impressao.ImpressaoComum;

namespace Emissor.DFe.Impressao;

/// <summary>Veículo do DAMDFE (tração ou reboque).</summary>
/// <param name="Placa">Placa.</param>
/// <param name="Rntrc">RNTRC do proprietário (terceiro) ou da empresa.</param>
public sealed record VeiculoDamdfe(string Placa, string Rntrc);

/// <summary>Condutor do DAMDFE.</summary>
/// <param name="Cpf">CPF formatado.</param>
/// <param name="Nome">Nome.</param>
public sealed record CondutorDamdfe(string Cpf, string Nome);

/// <summary>Vale-pedágio (<c>valePed/disp</c>).</summary>
/// <param name="Responsavel">CNPJ ou CPF de quem paga.</param>
/// <param name="Fornecedor">CNPJ do fornecedor.</param>
/// <param name="Comprovante">Número do comprovante de compra.</param>
public sealed record ValePedagioDamdfe(string Responsavel, string Fornecedor, string Comprovante);

/// <summary>Documento vinculado ao MDF-e (CT-e ou NF-e), com o município de descarregamento.</summary>
/// <param name="Tipo">CT-e ou NF-e.</param>
/// <param name="Chave">Chave de acesso.</param>
/// <param name="MunicipioDescarga">Município de descarregamento.</param>
public sealed record DocumentoDamdfe(string Tipo, string Chave, string MunicipioDescarga);

/// <summary>CIOT do MDF-e (<c>infANTT/infCIOT</c>).</summary>
/// <param name="Ciot">Código do CIOT.</param>
/// <param name="Responsavel">CPF ou CNPJ do responsável pela geração.</param>
public sealed record CiotDamdfe(string Ciot, string Responsavel);

/// <summary>
/// Conteúdo do DAMDFE lido do XML do MDF-e (<c>MDFe</c> ou <c>mdfeProc</c>), conforme o modelo do modal rodoviário do
/// Anexo II do MOC MDF-e (item 2.7.1). Além do leiaute padrão, traz os documentos vinculados e o CIOT, impressos
/// abaixo dele (o manual permite campos adicionais abaixo do leiaute padrão).
/// </summary>
public sealed class DadosDamdfe
{
    /// <summary>Ambiente (<c>tpAmb</c>).</summary>
    public TipoAmbiente Ambiente { get; init; }

    /// <summary>Emissão em contingência off-line (<c>tpEmis</c> 2).</summary>
    public bool Contingencia { get; init; }

    /// <summary>Chave de acesso (44 posições).</summary>
    public string Chave { get; init; } = string.Empty;

    /// <summary>Chave em grupos de 4, para leitura.</summary>
    public string ChaveFormatada =>
        string.Join(' ', Enumerable.Range(0, (Chave.Length + 3) / 4).Select(i => Chave.Substring(i * 4, Math.Min(4, Chave.Length - i * 4))));

    /// <summary>Protocolo de autorização com data e hora; vazio se o XML não tem <c>protMDFe</c> autorizado.</summary>
    public string Protocolo { get; init; } = string.Empty;

    /// <summary>Conteúdo do QR Code.</summary>
    public string QrCode { get; init; } = string.Empty;

    /// <summary>Nome do emitente.</summary>
    public string EmitenteNome { get; init; } = string.Empty;

    /// <summary>Endereço do emitente (logradouro, número, complemento e bairro).</summary>
    public string EmitenteEndereco { get; init; } = string.Empty;

    /// <summary>Município, UF e CEP do emitente.</summary>
    public string EmitenteMunicipio { get; init; } = string.Empty;

    /// <summary>CNPJ ou CPF do emitente, formatado.</summary>
    public string EmitenteDocumento { get; init; } = string.Empty;

    /// <summary>IE do emitente.</summary>
    public string EmitenteIe { get; init; } = string.Empty;

    /// <summary>RNTRC da empresa (<c>infANTT/RNTRC</c>).</summary>
    public string EmitenteRntrc { get; init; } = string.Empty;

    /// <summary>Modelo.</summary>
    public string Modelo { get; init; } = string.Empty;

    /// <summary>Série.</summary>
    public string Serie { get; init; } = string.Empty;

    /// <summary>Número.</summary>
    public string Numero { get; init; } = string.Empty;

    /// <summary>Data e hora de emissão.</summary>
    public string DataHoraEmissao { get; init; } = string.Empty;

    /// <summary>UF de carregamento.</summary>
    public string UfCarregamento { get; init; } = string.Empty;

    /// <summary>UF de descarregamento.</summary>
    public string UfDescarregamento { get; init; } = string.Empty;

    /// <summary>Quantidade de CT-e.</summary>
    public string QuantidadeCte { get; init; } = "0";

    /// <summary>Quantidade de NF-e.</summary>
    public string QuantidadeNfe { get; init; } = "0";

    /// <summary>Peso total em quilos.</summary>
    public string PesoTotalKg { get; init; } = string.Empty;

    /// <summary>Veículo de tração e reboques.</summary>
    public IReadOnlyList<VeiculoDamdfe> Veiculos { get; init; } = [];

    /// <summary>Condutores.</summary>
    public IReadOnlyList<CondutorDamdfe> Condutores { get; init; } = [];

    /// <summary>Vale-pedágio.</summary>
    public IReadOnlyList<ValePedagioDamdfe> ValesPedagio { get; init; } = [];

    /// <summary>Observações (<c>infAdFisco</c> e <c>infCpl</c>).</summary>
    public string Observacoes { get; init; } = string.Empty;

    /// <summary>Documentos vinculados (campo adicional, abaixo do leiaute padrão).</summary>
    public IReadOnlyList<DocumentoDamdfe> Documentos { get; init; } = [];

    /// <summary>CIOT (campo adicional, abaixo do leiaute padrão).</summary>
    public IReadOnlyList<CiotDamdfe> Ciots { get; init; } = [];

    /// <summary>Lê o XML do MDF-e assinado (<c>MDFe</c>) ou o processado (<c>mdfeProc</c>, com o protocolo).</summary>
    /// <param name="xml">Conteúdo do arquivo.</param>
    /// <exception cref="RegraNegocioException">O XML não é de um MDF-e.</exception>
    public static DadosDamdfe Ler(string xml)
    {
        var documento = new XmlDocument { PreserveWhitespace = false, XmlResolver = null };
        try
        {
            documento.LoadXml(xml);
        }
        catch (XmlException erro)
        {
            throw new RegraNegocioException($"O arquivo não é um XML válido: {erro.Message}");
        }

        var mdfe = Primeiro(documento, "infMDFe") ?? throw new RegraNegocioException("O XML não é de um MDF-e (falta o grupo infMDFe).");
        var ide = Primeiro(mdfe, "ide");
        var emitente = Primeiro(mdfe, "emit");
        var endereco = Primeiro(emitente, "enderEmit");
        var rodo = Primeiro(Primeiro(mdfe, "infModal"), "rodo");
        var antt = Primeiro(rodo, "infANTT");
        var rntrcEmpresa = Texto(antt, "RNTRC");
        var tracao = Primeiro(rodo, "veicTracao");
        var totais = Primeiro(mdfe, "tot");
        var ambiente = Texto(ide, "tpAmb") == "1" ? TipoAmbiente.Producao : TipoAmbiente.Homologacao;
        var chave = mdfe.GetAttribute("Id").Replace("MDFe", string.Empty, StringComparison.Ordinal);
        var cnpj = Texto(emitente, "CNPJ");

        return new DadosDamdfe
        {
            Ambiente = ambiente,
            Contingencia = Texto(ide, "tpEmis") == "2",
            Chave = chave,
            Protocolo = LerProtocolo(documento),
            QrCode = LerQrCode(documento, chave, ambiente),
            EmitenteNome = Texto(emitente, "xNome"),
            EmitenteEndereco = string.Join(", ", new[] { Texto(endereco, "xLgr"), Texto(endereco, "nro"), Texto(endereco, "xCpl"), Texto(endereco, "xBairro") }
                .Where(p => p.Length > 0)),
            EmitenteMunicipio = $"{Texto(endereco, "xMun")} - {Texto(endereco, "UF")}   CEP {Cep(Texto(endereco, "CEP"))}",
            EmitenteDocumento = cnpj.Length > 0 ? FormatarCnpj(cnpj) : FormatarCpf(Texto(emitente, "CPF")),
            EmitenteIe = Texto(emitente, "IE"),
            EmitenteRntrc = rntrcEmpresa,
            Modelo = Texto(ide, "mod"),
            Serie = Texto(ide, "serie"),
            Numero = Texto(ide, "nMDF"),
            DataHoraEmissao = DataHora(Texto(ide, "dhEmi")),
            UfCarregamento = Texto(ide, "UFIni"),
            UfDescarregamento = Texto(ide, "UFFim"),
            QuantidadeCte = Primeiro(Texto(totais, "qCTe"), "0"),
            QuantidadeNfe = Primeiro(Texto(totais, "qNFe"), "0"),
            PesoTotalKg = PesoKg(totais),
            Veiculos = new[] { tracao }.Concat(Filhos(rodo, "veicReboque"))
                .Where(v => v is not null)
                .Select(v => new VeiculoDamdfe(Texto(v, "placa"), Primeiro(Texto(Primeiro(v, "prop"), "RNTRC"), rntrcEmpresa)))
                .ToList(),
            Condutores = Filhos(tracao, "condutor").Select(c => new CondutorDamdfe(FormatarCpf(Texto(c, "CPF")), Texto(c, "xNome"))).ToList(),
            ValesPedagio = Filhos(Primeiro(antt, "valePed"), "disp")
                .Select(d => new ValePedagioDamdfe(
                    Texto(d, "CNPJPg") is { Length: > 0 } pagador ? FormatarCnpj(pagador) : FormatarCpf(Texto(d, "CPFPg")),
                    FormatarCnpj(Texto(d, "CNPJForn")),
                    Texto(d, "nCompra")))
                .ToList(),
            Observacoes = string.Join(Environment.NewLine,
                new[] { Texto(Primeiro(mdfe, "infAdic"), "infAdFisco"), Texto(Primeiro(mdfe, "infAdic"), "infCpl") }.Where(t => t.Length > 0)),
            Documentos = LerDocumentos(Primeiro(mdfe, "infDoc")),
            Ciots = Filhos(antt, "infCIOT")
                .Select(c => new CiotDamdfe(Texto(c, "CIOT"), Texto(c, "CNPJ") is { Length: > 0 } cnpjCiot ? FormatarCnpj(cnpjCiot) : FormatarCpf(Texto(c, "CPF"))))
                .ToList(),
        };
    }

    private static string LerProtocolo(XmlDocument documento)
    {
        var protocolo = Primeiro(documento, "infProt");
        return Texto(protocolo, "cStat") == "100"
            ? $"{Texto(protocolo, "nProt")} - {DataHora(Texto(protocolo, "dhRecbto"))}"
            : string.Empty;
    }

    /// <summary>
    /// QR Code do XML (<c>qrCodMDFe</c>); no XML de teste, sem <c>infMDFeSupl</c>, a URL da tabela oficial com
    /// <c>chMDFe</c> e <c>tpAmb</c> (MOC MDF-e 3.00b, item 9.2.1), só em homologação.
    /// </summary>
    private static string LerQrCode(XmlDocument documento, string chave, TipoAmbiente ambiente)
    {
        var doXml = Texto(Primeiro(documento, "infMDFeSupl"), "qrCodMDFe");
        if (doXml.Length > 0)
        {
            return doXml;
        }

        if (ambiente != TipoAmbiente.Homologacao || chave.Length != 44)
        {
            return string.Empty;
        }

        try
        {
            return $"{TabelaWebservices.Endereco(UfDaChave(chave), ServicoSefaz.MdfeQrCode, TipoAmbiente.Homologacao)}?chMDFe={chave}&tpAmb=2";
        }
        catch (RegraNegocioException)
        {
            return string.Empty;
        }
    }

    /// <summary>Peso bruto total em quilos: unidade 01 (KG) como vem; 02 (TON) × 1.000.</summary>
    private static string PesoKg(XmlElement? totais)
    {
        var peso = Decimal(Texto(totais, "qCarga"));
        if (Texto(totais, "cUnid") == "02")
        {
            peso *= 1000m;
        }

        return peso.ToString("N2", Brasil);
    }

    private static IReadOnlyList<DocumentoDamdfe> LerDocumentos(XmlElement? infDoc)
    {
        var lista = new List<DocumentoDamdfe>();
        foreach (var descarga in Filhos(infDoc, "infMunDescarga"))
        {
            var municipio = Texto(descarga, "xMunDescarga");
            lista.AddRange(Filhos(descarga, "infCTe").Select(d => new DocumentoDamdfe("CT-e", Texto(d, "chCTe"), municipio)));
            lista.AddRange(Filhos(descarga, "infNFe").Select(d => new DocumentoDamdfe("NF-e", Texto(d, "chNFe"), municipio)));
        }

        return lista;
    }
}
