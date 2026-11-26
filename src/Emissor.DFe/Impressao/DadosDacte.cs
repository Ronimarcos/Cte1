using System.Globalization;
using System.Xml;
using Emissor.DFe.Webservices;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Impressao;

/// <summary>Pessoa de um quadro do DACTE (emitente, remetente, destinatário, expedidor, recebedor, tomador).</summary>
/// <param name="Nome">Nome ou razão social.</param>
/// <param name="Endereco">Logradouro, número, complemento e bairro.</param>
/// <param name="Municipio">Município.</param>
/// <param name="Uf">UF.</param>
/// <param name="Cep">CEP formatado.</param>
/// <param name="Documento">CNPJ ou CPF formatado.</param>
/// <param name="Ie">Inscrição estadual.</param>
/// <param name="Pais">País.</param>
/// <param name="Fone">Telefone.</param>
public sealed record PessoaDacte(string Nome, string Endereco, string Municipio, string Uf, string Cep, string Documento, string Ie, string Pais, string Fone)
{
    /// <summary>Pessoa vazia (quadro sem informação).</summary>
    public static PessoaDacte Vazia { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
}

/// <summary>Quantidade da carga (<c>infQ</c>) já convertida para exibição.</summary>
/// <param name="Descricao">Tipo da medida (<c>tpMed</c>).</param>
/// <param name="Valor">Quantidade com a unidade (ex.: "1.500,000 KG").</param>
public sealed record QuantidadeDacte(string Descricao, string Valor);

/// <summary>Documento originário (NF-e, NF ou outros).</summary>
/// <param name="Tipo">Tipo (NF-e, NF, outros).</param>
/// <param name="Identificacao">Chave, ou série e número.</param>
public sealed record DocumentoOriginarioDacte(string Tipo, string Identificacao);

/// <summary>
/// Conteúdo do DACTE lido do XML do CT-e (<c>CTe</c> ou <c>cteProc</c>), conforme a correlação de campos do Anexo II
/// do MOC (item 3). Os campos vêm prontos para impressão (formatados em pt-BR).
/// </summary>
public sealed class DadosDacte
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Ambiente (<c>tpAmb</c>).</summary>
    public TipoAmbiente Ambiente { get; init; }

    /// <summary>Chave de acesso (44 posições).</summary>
    public string Chave { get; init; } = string.Empty;

    /// <summary>Chave em grupos de 4, para leitura.</summary>
    public string ChaveFormatada => string.Join(' ', Enumerable.Range(0, (Chave.Length + 3) / 4).Select(i => Chave.Substring(i * 4, Math.Min(4, Chave.Length - i * 4))));

    /// <summary>Protocolo de autorização com data e hora; vazio se o XML não tem <c>protCTe</c> autorizado.</summary>
    public string Protocolo { get; init; } = string.Empty;

    /// <summary>Conteúdo do QR Code.</summary>
    public string QrCode { get; init; } = string.Empty;

    /// <summary>Modal (ex.: RODOVIÁRIO).</summary>
    public string Modal { get; init; } = string.Empty;

    /// <summary>Modelo (<c>mod</c>).</summary>
    public string Modelo { get; init; } = string.Empty;

    /// <summary>Série.</summary>
    public string Serie { get; init; } = string.Empty;

    /// <summary>Número.</summary>
    public string Numero { get; init; } = string.Empty;

    /// <summary>Data e hora de emissão.</summary>
    public string DataHoraEmissao { get; init; } = string.Empty;

    /// <summary>Tipo do CT-e.</summary>
    public string TipoCte { get; init; } = string.Empty;

    /// <summary>Tipo do serviço.</summary>
    public string TipoServico { get; init; } = string.Empty;

    /// <summary>Papel do tomador (remetente, expedidor...).</summary>
    public string TomadorPapel { get; init; } = string.Empty;

    /// <summary>Indicador de CT-e globalizado.</summary>
    public string Globalizado { get; init; } = string.Empty;

    /// <summary>CFOP e natureza da operação.</summary>
    public string CfopNatureza { get; init; } = string.Empty;

    /// <summary>Início da prestação (UF - município).</summary>
    public string Inicio { get; init; } = string.Empty;

    /// <summary>Término da prestação (UF - município).</summary>
    public string Termino { get; init; } = string.Empty;

    /// <summary>Emitente.</summary>
    public PessoaDacte Emitente { get; init; } = PessoaDacte.Vazia;

    /// <summary>Remetente.</summary>
    public PessoaDacte Remetente { get; init; } = PessoaDacte.Vazia;

    /// <summary>Destinatário.</summary>
    public PessoaDacte Destinatario { get; init; } = PessoaDacte.Vazia;

    /// <summary>Expedidor.</summary>
    public PessoaDacte Expedidor { get; init; } = PessoaDacte.Vazia;

    /// <summary>Recebedor.</summary>
    public PessoaDacte Recebedor { get; init; } = PessoaDacte.Vazia;

    /// <summary>Tomador do serviço.</summary>
    public PessoaDacte Tomador { get; init; } = PessoaDacte.Vazia;

    /// <summary>Produto predominante.</summary>
    public string ProdutoPredominante { get; init; } = string.Empty;

    /// <summary>Outras características da carga.</summary>
    public string OutrasCaracteristicas { get; init; } = string.Empty;

    /// <summary>Valor total da carga.</summary>
    public string ValorCarga { get; init; } = string.Empty;

    /// <summary>Quantidades da carga.</summary>
    public IReadOnlyList<QuantidadeDacte> Quantidades { get; init; } = [];

    /// <summary>Componentes do valor da prestação (nome e valor).</summary>
    public IReadOnlyList<(string Nome, string Valor)> Componentes { get; init; } = [];

    /// <summary>Valor total da prestação.</summary>
    public string ValorTotalPrestacao { get; init; } = string.Empty;

    /// <summary>Valor a receber.</summary>
    public string ValorReceber { get; init; } = string.Empty;

    /// <summary>Situação tributária do ICMS.</summary>
    public string SituacaoTributaria { get; init; } = string.Empty;

    /// <summary>Base de cálculo do ICMS.</summary>
    public string BaseCalculo { get; init; } = string.Empty;

    /// <summary>Alíquota do ICMS.</summary>
    public string Aliquota { get; init; } = string.Empty;

    /// <summary>Valor do ICMS.</summary>
    public string ValorIcms { get; init; } = string.Empty;

    /// <summary>Percentual de redução da base de cálculo.</summary>
    public string ReducaoBase { get; init; } = string.Empty;

    /// <summary>Documentos originários.</summary>
    public IReadOnlyList<DocumentoOriginarioDacte> Documentos { get; init; } = [];

    /// <summary>Observações gerais (<c>xObs</c>, <c>ObsCont</c> e <c>ObsFisco</c>).</summary>
    public string Observacoes { get; init; } = string.Empty;

    /// <summary>RNTRC da empresa (modal rodoviário).</summary>
    public string Rntrc { get; init; } = string.Empty;

    /// <summary>Lê o XML do CT-e assinado (<c>CTe</c>) ou o processado (<c>cteProc</c>, com o protocolo).</summary>
    /// <param name="xml">Conteúdo do arquivo.</param>
    /// <exception cref="RegraNegocioException">O XML não é de um CT-e.</exception>
    public static DadosDacte Ler(string xml)
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

        var cte = Primeiro(documento, "infCte") ?? throw new RegraNegocioException("O XML não é de um CT-e (falta o grupo infCte).");
        var ide = Primeiro(cte, "ide");
        var ambiente = Texto(ide, "tpAmb") == "1" ? TipoAmbiente.Producao : TipoAmbiente.Homologacao;
        var chave = cte.GetAttribute("Id").Replace("CTe", string.Empty, StringComparison.Ordinal);
        var normal = Primeiro(cte, "infCTeNorm");
        var carga = Primeiro(normal, "infCarga");
        var icms = Primeiro(Primeiro(cte, "imp"), "ICMS")?.ChildNodes.OfType<XmlElement>().FirstOrDefault();

        var pessoas = new Dictionary<string, PessoaDacte>
        {
            ["rem"] = Pessoa(Primeiro(cte, "rem"), "enderReme"),
            ["exped"] = Pessoa(Primeiro(cte, "exped"), "enderExped"),
            ["receb"] = Pessoa(Primeiro(cte, "receb"), "enderReceb"),
            ["dest"] = Pessoa(Primeiro(cte, "dest"), "enderDest"),
        };
        var (papel, tomador) = LerTomador(ide, pessoas);

        return new DadosDacte
        {
            Ambiente = ambiente,
            Chave = chave,
            Protocolo = LerProtocolo(documento),
            QrCode = LerQrCode(documento, chave, ambiente),
            Modal = Texto(ide, "modal") switch
            {
                "01" => "RODOVIÁRIO",
                "02" => "AÉREO",
                "03" => "AQUAVIÁRIO",
                "04" => "FERROVIÁRIO",
                "05" => "DUTOVIÁRIO",
                "06" => "MULTIMODAL",
                var outro => outro,
            },
            Modelo = Texto(ide, "mod"),
            Serie = Texto(ide, "serie"),
            Numero = Texto(ide, "nCT"),
            DataHoraEmissao = DataHora(Texto(ide, "dhEmi")),
            TipoCte = Texto(ide, "tpCTe") switch { "0" => "NORMAL", "1" => "COMPLEMENTO DE VALORES", "3" => "SUBSTITUIÇÃO", var outro => outro },
            TipoServico = Texto(ide, "tpServ") switch
            {
                "0" => "NORMAL",
                "1" => "SUBCONTRATAÇÃO",
                "2" => "REDESPACHO",
                "3" => "REDESPACHO INTERMEDIÁRIO",
                "4" => "SERVIÇO VINCULADO A MULTIMODAL",
                var outro => outro,
            },
            TomadorPapel = papel,
            Globalizado = Texto(ide, "indGlobalizado") == "1" ? "SIM" : "NÃO",
            CfopNatureza = $"{Texto(ide, "CFOP")} - {Texto(ide, "natOp")}",
            Inicio = $"{Texto(ide, "UFIni")} - {Texto(ide, "xMunIni")}",
            Termino = $"{Texto(ide, "UFFim")} - {Texto(ide, "xMunFim")}",
            Emitente = Pessoa(Primeiro(cte, "emit"), "enderEmit"),
            Remetente = pessoas["rem"],
            Destinatario = pessoas["dest"],
            Expedidor = pessoas["exped"],
            Recebedor = pessoas["receb"],
            Tomador = tomador,
            ProdutoPredominante = Texto(carga, "proPred"),
            OutrasCaracteristicas = Texto(carga, "xOutCat"),
            ValorCarga = Dinheiro(Texto(carga, "vCarga")),
            Quantidades = Filhos(carga, "infQ").Select(Quantidade).ToList(),
            Componentes = Filhos(Primeiro(cte, "vPrest"), "Comp").Select(c => (Texto(c, "xNome"), Dinheiro(Texto(c, "vComp")))).ToList(),
            ValorTotalPrestacao = Dinheiro(Texto(Primeiro(cte, "vPrest"), "vTPrest")),
            ValorReceber = Dinheiro(Texto(Primeiro(cte, "vPrest"), "vRec")),
            SituacaoTributaria = LerSituacaoTributaria(icms),
            BaseCalculo = Dinheiro(Primeiro(Texto(icms, "vBC"), Texto(icms, "vBCSTRet"), Texto(icms, "vBCOutraUF"))),
            Aliquota = Percentual(Primeiro(Texto(icms, "pICMS"), Texto(icms, "pICMSSTRet"), Texto(icms, "pICMSOutraUF"))),
            ValorIcms = Dinheiro(Primeiro(Texto(icms, "vICMS"), Texto(icms, "vICMSSTRet"), Texto(icms, "vICMSOutraUF"))),
            ReducaoBase = Percentual(Primeiro(Texto(icms, "pRedBC"), Texto(icms, "pRedBCOutraUF"))),
            Documentos = LerDocumentos(Primeiro(normal, "infDoc"), Primeiro(normal, "docAnt")),
            Observacoes = LerObservacoes(Primeiro(cte, "compl")),
            Rntrc = Texto(Primeiro(Primeiro(normal, "infModal"), "rodo"), "RNTRC"),
        };
    }

    private static string LerProtocolo(XmlDocument documento)
    {
        var protocolo = Primeiro(documento, "infProt");
        return Texto(protocolo, "cStat") == "100"
            ? $"{Texto(protocolo, "nProt")} {DataHora(Texto(protocolo, "dhRecbto"))}"
            : string.Empty;
    }

    private static string LerQrCode(XmlDocument documento, string chave, TipoAmbiente ambiente)
    {
        var doXml = Texto(Primeiro(documento, "infCTeSupl"), "qrCodCTe");
        if (doXml.Length > 0)
        {
            return doXml;
        }

        // Sem infCTeSupl (XML de teste): URL de QR Code da UF autorizadora pela tabela oficial, só em homologação.
        var uf = chave.Length >= 2 && Ufs.TryGetValue(chave[..2], out var sigla) ? sigla : string.Empty;
        if (ambiente != TipoAmbiente.Homologacao || uf.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            return $"{TabelaWebservices.Endereco(uf, ServicoSefaz.CteQrCode, TipoAmbiente.Homologacao)}?chCTe={chave}&tpAmb=2";
        }
        catch (RegraNegocioException)
        {
            return string.Empty;
        }
    }

    private static (string Papel, PessoaDacte Pessoa) LerTomador(XmlElement? ide, IReadOnlyDictionary<string, PessoaDacte> pessoas)
    {
        var toma4 = Primeiro(ide, "toma4");
        if (toma4 is not null)
        {
            return ("OUTROS", Pessoa(toma4, "enderToma"));
        }

        return Texto(Primeiro(ide, "toma3"), "toma") switch
        {
            "0" => ("REMETENTE", pessoas["rem"]),
            "1" => ("EXPEDIDOR", pessoas["exped"]),
            "2" => ("RECEBEDOR", pessoas["receb"]),
            "3" => ("DESTINATÁRIO", pessoas["dest"]),
            _ => (string.Empty, PessoaDacte.Vazia),
        };
    }

    private static PessoaDacte Pessoa(XmlElement? grupo, string grupoEndereco)
    {
        if (grupo is null)
        {
            return PessoaDacte.Vazia;
        }

        var endereco = Primeiro(grupo, grupoEndereco);
        var linha = string.Join(", ", new[] { Texto(endereco, "xLgr"), Texto(endereco, "nro"), Texto(endereco, "xCpl"), Texto(endereco, "xBairro") }
            .Where(p => p.Length > 0));
        var cnpj = Texto(grupo, "CNPJ");
        var cpf = Texto(grupo, "CPF");
        return new PessoaDacte(
            Texto(grupo, "xNome"),
            linha,
            Texto(endereco, "xMun"),
            Texto(endereco, "UF"),
            Cep(Texto(endereco, "CEP")),
            cnpj.Length > 0 ? FormatarCnpj(cnpj) : FormatarCpf(cpf),
            Texto(grupo, "IE"),
            Primeiro(Texto(endereco, "xPais"), endereco is null ? string.Empty : "BRASIL"),
            Primeiro(Texto(grupo, "fone"), Texto(endereco, "fone")));
    }

    /// <summary>
    /// Quantidade conforme o item 3 do manual: pesos (unidade 01 = KG e 02 = TON) em quilos, com o fator 1 ou 1.000;
    /// demais unidades como vêm, com a sigla.
    /// </summary>
    private static QuantidadeDacte Quantidade(XmlElement infQ)
    {
        var quantidade = Decimal(Texto(infQ, "qCarga"));
        var (valor, unidade) = Texto(infQ, "cUnid") switch
        {
            "01" => (quantidade, "KG"),
            "02" => (quantidade * 1000m, "KG"),
            "00" => (quantidade, "M3"),
            "03" => (quantidade, "UN"),
            "04" => (quantidade, "LITROS"),
            "05" => (quantidade, "MMBTU"),
            var outra => (quantidade, outra),
        };
        return new QuantidadeDacte(Texto(infQ, "tpMed"), $"{valor.ToString("N4", Brasil)} {unidade}");
    }

    private static string LerSituacaoTributaria(XmlElement? icms)
    {
        if (icms is null)
        {
            return string.Empty;
        }

        if (icms.LocalName == "ICMSSN")
        {
            return $"{Texto(icms, "CST")} - SIMPLES NACIONAL".TrimStart(' ', '-');
        }

        return Texto(icms, "CST") switch
        {
            "00" => "00 - TRIBUTAÇÃO NORMAL",
            "20" => "20 - COM REDUÇÃO DE BC",
            "40" => "40 - ISENTA",
            "41" => "41 - NÃO TRIBUTADA",
            "51" => "51 - DIFERIDA",
            "60" => "60 - COBRADO POR SUBSTITUIÇÃO TRIBUTÁRIA",
            "90" => icms.LocalName == "ICMSOutraUF" ? "90 - ICMS OUTRA UF" : "90 - OUTROS",
            var outro => outro,
        };
    }

    private static IReadOnlyList<DocumentoOriginarioDacte> LerDocumentos(XmlElement? infDoc, XmlElement? docAnt)
    {
        var lista = new List<DocumentoOriginarioDacte>();
        lista.AddRange(Filhos(infDoc, "infNFe").Select(n => new DocumentoOriginarioDacte("NF-e", Texto(n, "chave"))));
        lista.AddRange(Filhos(infDoc, "infNF").Select(n => new DocumentoOriginarioDacte("NF", $"Série {Texto(n, "serie")} nº {Texto(n, "nDoc")}")));
        lista.AddRange(Filhos(infDoc, "infOutros").Select(n => new DocumentoOriginarioDacte("Outros", $"{Texto(n, "descOutros")} nº {Texto(n, "nDoc")}".Trim())));
        if (docAnt is not null)
        {
            lista.AddRange(docAnt.GetElementsByTagName("chCTe", docAnt.NamespaceURI).OfType<XmlElement>()
                .Select(c => new DocumentoOriginarioDacte("CT-e anterior", c.InnerText)));
        }

        return lista;
    }

    private static string LerObservacoes(XmlElement? compl)
    {
        var partes = new List<string>();
        if (Texto(compl, "xObs") is { Length: > 0 } observacao)
        {
            partes.Add(observacao);
        }

        partes.AddRange(Filhos(compl, "ObsCont").Concat(Filhos(compl, "ObsFisco"))
            .Select(o => $"{o.GetAttribute("xCampo")}: {Texto(o, "xTexto")}"));
        return string.Join(Environment.NewLine, partes);
    }

    private static string DataHora(string texto) =>
        DateTimeOffset.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data)
            ? data.ToString("dd/MM/yyyy HH:mm:ss", Brasil)
            : texto;

    private static string Dinheiro(string texto) => texto.Length == 0 ? string.Empty : Decimal(texto).ToString("N2", Brasil);

    private static string Percentual(string texto) => texto.Length == 0 ? string.Empty : Decimal(texto).ToString("N2", Brasil) + "%";

    private static decimal Decimal(string texto) =>
        decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var valor) ? valor : 0m;

    private static string Cep(string cep) => cep.Length == 8 ? $"{cep[..5]}-{cep[5..]}" : cep;

    private static string FormatarCnpj(string cnpj) =>
        cnpj.Length == 14 ? $"{cnpj[..2]}.{cnpj[2..5]}.{cnpj[5..8]}/{cnpj[8..12]}-{cnpj[12..]}" : cnpj;

    private static string FormatarCpf(string cpf) =>
        cpf.Length == 11 ? $"{cpf[..3]}.{cpf[3..6]}.{cpf[6..9]}-{cpf[9..]}" : cpf;

    private static string Primeiro(params string[] textos) => textos.FirstOrDefault(t => t.Length > 0) ?? string.Empty;

    private static XmlElement? Primeiro(XmlNode? pai, string nome) =>
        pai switch
        {
            null => null,
            XmlDocument documento => documento.GetElementsByTagName(nome).OfType<XmlElement>().FirstOrDefault(),
            _ => pai.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.LocalName == nome),
        };

    private static IEnumerable<XmlElement> Filhos(XmlElement? pai, string nome) =>
        pai?.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == nome) ?? [];

    private static string Texto(XmlElement? pai, string nome) => Primeiro(pai, nome)?.InnerText.Trim() ?? string.Empty;

    /// <summary>Código IBGE da UF (dois primeiros dígitos da chave) para a sigla.</summary>
    private static readonly IReadOnlyDictionary<string, string> Ufs = new Dictionary<string, string>
    {
        ["11"] = "RO", ["12"] = "AC", ["13"] = "AM", ["14"] = "RR", ["15"] = "PA", ["16"] = "AP", ["17"] = "TO",
        ["21"] = "MA", ["22"] = "PI", ["23"] = "CE", ["24"] = "RN", ["25"] = "PB", ["26"] = "PE", ["27"] = "AL", ["28"] = "SE", ["29"] = "BA",
        ["31"] = "MG", ["32"] = "ES", ["33"] = "RJ", ["35"] = "SP", ["41"] = "PR", ["42"] = "SC", ["43"] = "RS",
        ["50"] = "MS", ["51"] = "MT", ["52"] = "GO", ["53"] = "DF",
    };
}
