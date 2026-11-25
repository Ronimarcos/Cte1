using System.Globalization;
using System.Xml;
using Emissor.DFe.Leiaute;
using Emissor.DFe.Leiaute.Mdfe;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using static Emissor.DFe.FormatacaoLeiaute;

namespace Emissor.DFe.Mdfe;

/// <summary>Tudo o que a montagem do XML do MDF-e precisa, já lido do banco.</summary>
/// <param name="Mdfe">MDF-e a montar.</param>
/// <param name="Empresa">Empresa emitente.</param>
/// <param name="Serie">Série do MDF-e.</param>
/// <param name="Numero">Número do MDF-e (na P4b, o próximo da série, sem reserva).</param>
/// <param name="Veiculos">Veículos da empresa, pelo código.</param>
/// <param name="Proprietarios">Proprietários de veículos de terceiros, pelo código.</param>
/// <param name="Motoristas">Motoristas da empresa, pelo código.</param>
/// <param name="Apolices">Apólices de seguro da empresa, pelo código.</param>
/// <param name="Municipio">Município pelo código IBGE, ou <c>null</c>.</param>
/// <param name="Ambiente">Ambiente do XML (<c>tpAmb</c>).</param>
/// <param name="DataHoraEmissao">Data e hora da emissão, com o fuso (<c>dhEmi</c>).</param>
/// <param name="VersaoAplicativo">Versão do emissor (<c>verProc</c>, até 20 caracteres).</param>
public sealed record DadosEmissaoMdfe(
    ManifestoEletronico Mdfe,
    Empresa Empresa,
    SerieDocumento Serie,
    int Numero,
    IReadOnlyDictionary<int, Veiculo> Veiculos,
    IReadOnlyDictionary<int, ProprietarioVeiculo> Proprietarios,
    IReadOnlyDictionary<int, Motorista> Motoristas,
    IReadOnlyDictionary<int, ApoliceSeguro> Apolices,
    Func<int, Municipio?> Municipio,
    TipoAmbiente Ambiente,
    DateTimeOffset DataHoraEmissao,
    string VersaoAplicativo);

/// <summary>Resultado da montagem: o MDF-e nas classes do leiaute e a chave, ou as pendências que impediram.</summary>
/// <param name="Documento">MDF-e montado (sem assinatura), ou <c>null</c> com pendências.</param>
/// <param name="Chave">Chave de acesso.</param>
/// <param name="Pendencias">O que impediu a montagem.</param>
public sealed record MdfeMontado(TmdFe? Documento, string Chave, IReadOnlyList<string> Pendencias);

/// <summary>
/// Converte o MDF-e do domínio nas classes geradas do XSD do MDF-e 3.00b (modal rodoviário). Valores sem tela seguem
/// as decisões da P4b2 (§8.2 do levantamento): emissão normal, emissor próprio, modal rodoviário. Ficam fora o QR Code
/// (P4c), a carga lotação e o contratante/pagamento do frete (pendência 6).
/// </summary>
public static class MontadorMdfe
{
    /// <summary>Versão do leiaute do MDF-e.</summary>
    public const string Versao = "3.00";

    /// <summary>Versão do leiaute do modal rodoviário.</summary>
    public const string VersaoModal = "3.00";

    /// <summary>Namespace do leiaute do MDF-e.</summary>
    public const string NamespaceMdfe = "http://www.portalfiscal.inf.br/mdfe";

    private static readonly CultureInfo Invariante = CultureInfo.InvariantCulture;

    /// <summary>Monta o MDF-e; as pendências dizem o que falta nos cadastros ou no documento.</summary>
    /// <param name="dados">MDF-e, empresa e cadastros.</param>
    public static MdfeMontado Montar(DadosEmissaoMdfe dados)
    {
        var mdfe = dados.Mdfe;
        var empresa = dados.Empresa;
        var pendencias = new List<string>(mdfe.ValidarObrigatorios());
        pendencias.AddRange(mdfe.ValidarCarregamentoDescarga(codigo => dados.Municipio(codigo)?.Uf ?? string.Empty));

        if (!Formatos.CnpjValido(empresa.Cnpj))
        {
            pendencias.Add("A empresa precisa de CNPJ para emitir MDF-e (a chave de acesso usa o CNPJ)");
        }

        var municipioEmpresa = empresa.Endereco.MunicipioCodigo is { } codigoEmpresa ? dados.Municipio(codigoEmpresa) : null;
        if (municipioEmpresa is null)
        {
            pendencias.Add("Município da empresa");
        }

        foreach (var documento in mdfe.Descargas.SelectMany(d => d.Documentos).Where(d => d.CteId is not null))
        {
            pendencias.Add($"CT-e nº {documento.CteId} emitido neste sistema ainda não tem chave de acesso: só pode ir no MDF-e depois de autorizado (P4c). Para testar, informe a chave do CT-e");
        }

        var tracao = VeiculoDoCadastro(dados, mdfe.VeiculoTracaoId, "Veículo de tração", pendencias);
        if (tracao is not null)
        {
            if (tracao.Tipo != TipoVeiculo.Tracao) pendencias.Add($"O veículo {tracao.Placa} não é de tração");
            if (tracao.TipoRodado is null) pendencias.Add($"Tipo de rodado do veículo {tracao.Placa}");
            if (tracao.TipoCarroceria is null) pendencias.Add($"Tipo de carroceria do veículo {tracao.Placa}");
            ConferirProprietario(dados, tracao, pendencias);
        }

        var reboques = new List<Veiculo>();
        foreach (var id in mdfe.ReboquesIds)
        {
            if (VeiculoDoCadastro(dados, id, "Reboque", pendencias) is not { } reboque)
            {
                continue;
            }

            if (reboque.Tipo != TipoVeiculo.Reboque) pendencias.Add($"O veículo {reboque.Placa} não é reboque");
            if (reboque.CapacidadeKg is null) pendencias.Add($"Capacidade em kg do reboque {reboque.Placa} (obrigatória no MDF-e)");
            if (reboque.TipoCarroceria is null) pendencias.Add($"Tipo de carroceria do reboque {reboque.Placa}");
            ConferirProprietario(dados, reboque, pendencias);
            reboques.Add(reboque);
        }

        var condutores = new List<Motorista>();
        foreach (var id in mdfe.CondutoresIds)
        {
            if (dados.Motoristas.TryGetValue(id, out var motorista))
            {
                condutores.Add(motorista);
            }
            else
            {
                pendencias.Add($"Condutor: motorista {id} não encontrado no cadastro");
            }
        }

        ApoliceSeguro? apolice = null;
        if (mdfe.ApoliceId is { } apoliceId && !dados.Apolices.TryGetValue(apoliceId, out apolice))
        {
            pendencias.Add($"Apólice {apoliceId} não encontrada no cadastro");
        }

        if (pendencias.Count > 0 || municipioEmpresa is null || tracao is null)
        {
            return new MdfeMontado(null, string.Empty, pendencias.Distinct().ToList());
        }

        var codigoUf = municipioEmpresa.Codigo / 100_000;
        var codigoNumerico = ChaveAcesso.GerarCodigoNumerico(dados.Numero);
        var chave = ChaveAcesso.Montar(codigoUf, dados.DataHoraEmissao.DateTime, empresa.Cnpj, 58, dados.Serie.Serie, dados.Numero, 1, codigoNumerico);

        var infMdfe = new TmdFeInfMdFe
        {
            Versao = Versao,
            Id = "MDFe" + chave,
            Ide = Identificacao(dados, codigoUf, codigoNumerico, chave),
            Emit = Emitente(empresa, municipioEmpresa),
            InfModal = new TmdFeInfMdFeInfModal
            {
                VersaoModal = VersaoModal,
                Any = Elemento(Rodoviario(dados, tracao, reboques, condutores), NamespaceMdfe),
            },
            Tot = new TmdFeInfMdFeTot
            {
                QcTe = mdfe.QuantidadeCte > 0 ? Inteiro(mdfe.QuantidadeCte) : null,
                QnFe = mdfe.QuantidadeNfe > 0 ? Inteiro(mdfe.QuantidadeNfe) : null,
                VCarga = Valor(mdfe.ValorCarga ?? 0),
                CUnid = LeiauteEnum.De<TmdFeInfMdFeTotCUnid>((int)mdfe.UnidadePeso!.Value, 2),
                QCarga = Quantidade(mdfe.PesoCarga ?? 0),
            },
        };

        foreach (var descarga in mdfe.Descargas)
        {
            var municipio = dados.Municipio(descarga.MunicipioCodigo)!;
            var grupo = new TmdFeInfMdFeInfDocInfMunDescarga
            {
                CMunDescarga = Inteiro(municipio.Codigo),
                XMunDescarga = Texto(municipio.Nome),
            };
            foreach (var documento in descarga.Documentos)
            {
                if (documento.Tipo == TipoDocumentoMdfe.Cte)
                {
                    grupo.InfCTe.Add(new TmdFeInfMdFeInfDocInfMunDescargaInfCTe { ChCTe = documento.Chave });
                }
                else
                {
                    grupo.InfNFe.Add(new TmdFeInfMdFeInfDocInfMunDescargaInfNFe { ChNFe = documento.Chave });
                }
            }

            infMdfe.InfDoc.Add(grupo);
        }

        if (apolice is not null)
        {
            infMdfe.Seg.Add(Seguro(empresa, apolice, mdfe.Averbacoes));
        }

        if (!string.IsNullOrWhiteSpace(mdfe.ProdutoPredominante))
        {
            infMdfe.ProdPred = new TmdFeInfMdFeProdPred
            {
                TpCarga = LeiauteEnum.De<TmdFeInfMdFeProdPredTpCarga>((int)mdfe.TipoCarga!.Value, 2),
                XProd = Texto(mdfe.ProdutoPredominante),
            };
        }

        if (!string.IsNullOrWhiteSpace(mdfe.InformacoesComplementares))
        {
            infMdfe.InfAdic = new TmdFeInfMdFeInfAdic { InfCpl = Texto(mdfe.InformacoesComplementares) };
        }

        return new MdfeMontado(new TmdFe { InfMdFe = infMdfe }, chave, []);
    }

    /// <summary>Serializa o MDF-e montado (sem assinatura) num documento XML, sem os namespaces xsi/xsd.</summary>
    /// <param name="documento">MDF-e montado.</param>
    public static XmlDocument Serializar(TmdFe documento) => FormatacaoLeiaute.Serializar(documento, NamespaceMdfe);

    private static TmdFeInfMdFeIde Identificacao(DadosEmissaoMdfe dados, int codigoUf, int codigoNumerico, string chave)
    {
        var mdfe = dados.Mdfe;
        var ide = new TmdFeInfMdFeIde
        {
            Cuf = LeiauteEnum.De<TCodUfIbge>(codigoUf, 2),
            TpAmb = LeiauteEnum.De<TAmb>((int)dados.Ambiente),
            TpEmit = LeiauteEnum.De<TEmit>((int)mdfe.TipoEmitente!.Value),
            TpTransp = mdfe.TipoTransportador is { } transportador ? LeiauteEnum.De<TTransp>((int)transportador) : null,
            Mod = TModMd.Item58,
            Serie = Inteiro(dados.Serie.Serie),
            Nmdf = Inteiro(dados.Numero),
            Cmdf = codigoNumerico.ToString("00000000", Invariante),
            Cdv = chave[^1..],
            Modal = TModalMd.Item1,
            DhEmi = DataHora(dados.DataHoraEmissao),
            TpEmis = TmdFeInfMdFeIdeTpEmis.Item1,
            ProcEmi = TmdFeInfMdFeIdeProcEmi.Item0,
            VerProc = dados.VersaoAplicativo.Length > 20 ? dados.VersaoAplicativo[..20] : dados.VersaoAplicativo,
            UfIni = LeiauteEnum.De<TUf>(mdfe.UfInicio),
            UfFim = LeiauteEnum.De<TUf>(mdfe.UfFim),
            DhIniViagem = mdfe.InicioViagem is { } inicio ? DataHora(new DateTimeOffset(inicio)) : null,
        };

        foreach (var codigo in mdfe.MunicipiosCarregamento)
        {
            var municipio = dados.Municipio(codigo)!;
            ide.InfMunCarrega.Add(new TmdFeInfMdFeIdeInfMunCarrega { CMunCarrega = Inteiro(municipio.Codigo), XMunCarrega = Texto(municipio.Nome) });
        }

        foreach (var uf in mdfe.Percurso)
        {
            ide.InfPercurso.Add(new TmdFeInfMdFeIdeInfPercurso { UfPer = LeiauteEnum.De<TUf>(uf) });
        }

        return ide;
    }

    private static TmdFeInfMdFeEmit Emitente(Empresa empresa, Municipio municipio) => new()
    {
        Cnpj = empresa.Cnpj,
        Ie = Opcional(empresa.Ie),
        XNome = Texto(empresa.RazaoSocial),
        XFant = Opcional(empresa.NomeFantasia),
        EnderEmit = new TEndeEmi
        {
            XLgr = Texto(empresa.Endereco.Logradouro),
            Nro = Texto(empresa.Endereco.Numero),
            XCpl = Opcional(empresa.Endereco.Complemento),
            XBairro = Texto(empresa.Endereco.Bairro),
            CMun = Inteiro(municipio.Codigo),
            XMun = Texto(municipio.Nome),
            Cep = Opcional(empresa.Endereco.Cep),
            Uf = LeiauteEnum.De<TUf>(municipio.Uf),
            Fone = Opcional(empresa.Fone),
            Email = Opcional(empresa.Email),
        },
    };

    /// <summary>Seguro da carga: responsável (emitente ou contratante), seguradora, apólice e averbações.</summary>
    private static TmdFeInfMdFeSeg Seguro(Empresa empresa, ApoliceSeguro apolice, IEnumerable<string> averbacoes)
    {
        var responsavel = apolice.Responsavel == ResponsavelSeguro.Contratante ? apolice.CnpjCpfResponsavel : empresa.Cnpj;
        var seguro = new TmdFeInfMdFeSeg
        {
            InfResp = new TmdFeInfMdFeSegInfResp
            {
                RespSeg = LeiauteEnum.De<TmdFeInfMdFeSegInfRespRespSeg>((int)apolice.Responsavel!.Value),
                Cnpj = responsavel.Length == 14 ? responsavel : null,
                Cpf = responsavel.Length == 11 ? responsavel : null,
            },
            InfSeg = new TmdFeInfMdFeSegInfSeg { XSeg = Texto(apolice.NomeSeguradora), Cnpj = apolice.CnpjSeguradora },
            NApol = Texto(apolice.NumeroApolice),
        };
        foreach (var averbacao in averbacoes.Where(a => !string.IsNullOrWhiteSpace(a)))
        {
            seguro.NAver.Add(Texto(averbacao));
        }

        return seguro;
    }

    /// <summary>Grupo <c>rodo</c>: RNTRC e CIOT (ANTT), veículo de tração com condutores e reboques.</summary>
    private static Rodo Rodoviario(DadosEmissaoMdfe dados, Veiculo tracao, IReadOnlyList<Veiculo> reboques, IReadOnlyList<Motorista> condutores)
    {
        var antt = new RodoInfAntt { Rntrc = Opcional(dados.Empresa.Rntrc) };
        foreach (var ciot in dados.Mdfe.Ciots)
        {
            var responsavel = Formatos.SoAlfanumericos(ciot.CpfCnpjResponsavel);
            antt.InfCiot.Add(new RodoInfAnttInfCiot
            {
                Ciot = ciot.Ciot,
                Cpf = responsavel.Length == 11 ? responsavel : null,
                Cnpj = responsavel.Length == 14 ? responsavel : null,
            });
        }

        var veiculo = new RodoVeicTracao
        {
            CInt = Opcional(tracao.CodigoInterno),
            Placa = tracao.Placa,
            Renavam = Opcional(tracao.Renavam),
            Tara = Inteiro(tracao.TaraKg),
            CapKg = tracao.CapacidadeKg is { } capacidade ? Inteiro(capacidade) : null,
            CapM3 = tracao.CapacidadeM3 is { } volume ? Inteiro(volume) : null,
            TpRod = LeiauteEnum.De<RodoVeicTracaoTpRod>((int)tracao.TipoRodado!.Value, 2),
            TpCar = LeiauteEnum.De<RodoVeicTracaoTpCar>((int)tracao.TipoCarroceria!.Value, 2),
            Uf = Formatos.UfValida(tracao.Uf) ? LeiauteEnum.De<TUf>(tracao.Uf) : null,
        };
        if (!tracao.Proprio && Proprietario(dados, tracao) is { } dono)
        {
            veiculo.Prop = new RodoVeicTracaoProp
            {
                Cpf = dono.CnpjCpf.Length == 11 ? dono.CnpjCpf : null,
                Cnpj = dono.CnpjCpf.Length == 14 ? dono.CnpjCpf : null,
                Rntrc = dono.Rntrc,
                XNome = Texto(dono.Nome),
                Ie = dono.Ie,
                Uf = LeiauteEnum.De<TUf>(dono.Uf),
                TpProp = LeiauteEnum.De<RodoVeicTracaoPropTpProp>((int)dono.TipoProprietario!.Value),
            };
        }

        foreach (var condutor in condutores)
        {
            veiculo.Condutor.Add(new RodoVeicTracaoCondutor { XNome = Texto(condutor.Nome), Cpf = condutor.Cpf });
        }

        var rodo = new Rodo { InfAntt = antt, VeicTracao = veiculo };
        foreach (var reboque in reboques)
        {
            var grupo = new RodoVeicReboque
            {
                CInt = Opcional(reboque.CodigoInterno),
                Placa = reboque.Placa,
                Renavam = Opcional(reboque.Renavam),
                Tara = Inteiro(reboque.TaraKg),
                CapKg = Inteiro(reboque.CapacidadeKg!.Value),
                CapM3 = reboque.CapacidadeM3 is { } volumeReboque ? Inteiro(volumeReboque) : null,
                TpCar = LeiauteEnum.De<RodoVeicReboqueTpCar>((int)reboque.TipoCarroceria!.Value, 2),
                Uf = Formatos.UfValida(reboque.Uf) ? LeiauteEnum.De<TUf>(reboque.Uf) : null,
            };
            if (!reboque.Proprio && Proprietario(dados, reboque) is { } donoReboque)
            {
                grupo.Prop = new RodoVeicReboqueProp
                {
                    Cpf = donoReboque.CnpjCpf.Length == 11 ? donoReboque.CnpjCpf : null,
                    Cnpj = donoReboque.CnpjCpf.Length == 14 ? donoReboque.CnpjCpf : null,
                    Rntrc = donoReboque.Rntrc,
                    XNome = Texto(donoReboque.Nome),
                    Ie = donoReboque.Ie,
                    Uf = LeiauteEnum.De<TUf>(donoReboque.Uf),
                    TpProp = LeiauteEnum.De<RodoVeicReboquePropTpProp>((int)donoReboque.TipoProprietario!.Value),
                };
            }

            rodo.VeicReboque.Add(grupo);
        }

        return rodo;
    }

    private static Veiculo? VeiculoDoCadastro(DadosEmissaoMdfe dados, int? id, string papel, List<string> pendencias)
    {
        if (id is null)
        {
            return null;
        }

        if (!dados.Veiculos.TryGetValue(id.Value, out var veiculo))
        {
            pendencias.Add($"{papel}: veículo {id} não encontrado no cadastro");
        }

        return veiculo;
    }

    /// <summary>Veículo de terceiro precisa do proprietário com UF (grupo <c>prop</c>).</summary>
    private static void ConferirProprietario(DadosEmissaoMdfe dados, Veiculo veiculo, List<string> pendencias)
    {
        if (veiculo.Proprio)
        {
            return;
        }

        var dono = Proprietario(dados, veiculo);
        if (dono is null)
        {
            pendencias.Add($"Proprietário do veículo {veiculo.Placa} não encontrado no cadastro");
        }
        else if (!Formatos.UfValida(dono.Uf))
        {
            pendencias.Add($"UF do proprietário {dono.Nome} (complete o cadastro do proprietário)");
        }
    }

    private static ProprietarioVeiculo? Proprietario(DadosEmissaoMdfe dados, Veiculo veiculo) =>
        veiculo.ProprietarioId is { } id && dados.Proprietarios.TryGetValue(id, out var dono) ? dono : null;
}
