using System.Xml;
using Emissor.DFe.Leiaute;
using Emissor.DFe.Leiaute.Cte;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using static Emissor.DFe.FormatacaoLeiaute;

namespace Emissor.DFe.Cte;

/// <summary>Tudo o que a montagem do XML do CT-e precisa, já lido do banco.</summary>
/// <param name="Cte">CT-e a montar.</param>
/// <param name="Empresa">Empresa emitente.</param>
/// <param name="Serie">Série do CT-e.</param>
/// <param name="Numero">Número do CT-e (na P4b, o próximo da série, sem reserva).</param>
/// <param name="Participantes">Participantes da empresa, pelo código.</param>
/// <param name="Municipio">Município pelo código IBGE, ou <c>null</c>.</param>
/// <param name="Ambiente">Ambiente do XML (<c>tpAmb</c>).</param>
/// <param name="DataHoraEmissao">Data e hora da emissão, com o fuso (<c>dhEmi</c>).</param>
/// <param name="VersaoAplicativo">Versão do emissor (<c>verProc</c>, até 20 caracteres).</param>
public sealed record DadosEmissaoCte(
    ConhecimentoTransporte Cte,
    Empresa Empresa,
    SerieDocumento Serie,
    int Numero,
    IReadOnlyDictionary<int, Participante> Participantes,
    Func<int, Municipio?> Municipio,
    TipoAmbiente Ambiente,
    DateTimeOffset DataHoraEmissao,
    string VersaoAplicativo);

/// <summary>Resultado da montagem: o CT-e nas classes do leiaute e a chave, ou as pendências que impediram.</summary>
/// <param name="Documento">CT-e montado (sem assinatura), ou <c>null</c> com pendências.</param>
/// <param name="Chave">Chave de acesso.</param>
/// <param name="Pendencias">O que impediu a montagem.</param>
public sealed record CteMontado(TcTe? Documento, string Chave, IReadOnlyList<string> Pendencias);

/// <summary>
/// Converte o CT-e do domínio nas classes geradas do XSD do CT-e 4.00 (modal rodoviário). Valores sem tela seguem
/// as decisões da P4b (§8.2 do levantamento): impressão retrato, emissão normal, emissor próprio, não retira, ICMS do
/// Simples Nacional (CST 90). IBS/CBS e QR Code ficam fora até a pendência 9 e a P4c.
/// </summary>
public static class MontadorCte
{
    /// <summary>Versão do leiaute do CT-e.</summary>
    public const string Versao = "4.00";

    /// <summary>Versão do leiaute do modal rodoviário.</summary>
    public const string VersaoModal = "4.00";

    /// <summary>Namespace do leiaute do CT-e.</summary>
    public const string NamespaceCte = "http://www.portalfiscal.inf.br/cte";

    /// <summary>
    /// IBS/CBS do Simples Nacional sem regime regular (NT 2026.005, item 2.2): CST e cClassTrib do cadastro da empresa
    /// e o grupo gTribSN com as alíquotas efetivas e os valores sobre o valor total da prestação (a mesma receita
    /// bruta do item que a NT indica); sem o grupo gIBSCBS, proibido nesse caso (rejeição 1100).
    /// </summary>
    /// <param name="empresa">Emitente, com as alíquotas conferidas antes da montagem.</param>
    /// <param name="valorPrestacao">Valor total da prestação (<c>vTPrest</c>).</param>
    public static TTribCTe IbsCbsSimples(Empresa empresa, decimal valorPrestacao)
    {
        var aliquotaIbs = empresa.AliquotaIbsSimples ?? 0;
        var aliquotaCbs = empresa.AliquotaCbsSimples ?? 0;
        return new TTribCTe
        {
            Cst = empresa.CstIbsCbs,
            CClassTrib = empresa.ClassTribIbsCbs,
            GTribSn = new TTribSn
            {
                Gibssn = new TTribSnGibssn { Pibssn = Aliquota(aliquotaIbs), Vibssn = Valor(ValorTributo(valorPrestacao, aliquotaIbs)) },
                Gcbssn = new TTribSnGcbssn { Pcbssn = Aliquota(aliquotaCbs), Vcbssn = Valor(ValorTributo(valorPrestacao, aliquotaCbs)) },
            },
        };
    }

    /// <summary>Valor do tributo: base × alíquota (%), arredondado em 2 casas (meio para cima).</summary>
    /// <param name="baseCalculo">Base de cálculo.</param>
    /// <param name="aliquota">Alíquota em %.</param>
    public static decimal ValorTributo(decimal baseCalculo, decimal aliquota) =>
        decimal.Round(baseCalculo * aliquota / 100m, 2, MidpointRounding.AwayFromZero);

    /// <summary>Alíquota no formato <c>TDec_0302_04</c> (2 a 4 casas, ponto decimal).</summary>
    private static string Aliquota(decimal aliquota) => aliquota.ToString("0.00##", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Monta o CT-e; as pendências dizem o que falta nos cadastros ou no documento.</summary>
    /// <param name="dados">CT-e, empresa e cadastros.</param>
    public static CteMontado Montar(DadosEmissaoCte dados)
    {
        var pendencias = new List<string>(dados.Cte.ValidarObrigatorios());
        var empresa = dados.Empresa;
        if (empresa.Crt != Crt.SimplesNacional)
        {
            pendencias.Add("Nesta versão o XML só é montado para empresa do Simples Nacional (CRT 1)");
        }
        else if (!empresa.IbsCbsPeloSimples)
        {
            pendencias.Add("Nesta versão o IBS/CBS só é tratado para o Simples Nacional sem opção pelo regime regular: deixe o \"Regime IBS/CBS\" da empresa em branco");
        }
        else if (empresa.AliquotaIbsSimples is null || empresa.AliquotaCbsSimples is null)
        {
            pendencias.Add("Informe no cadastro da empresa as alíquotas efetivas do IBS e da CBS no Simples Nacional");
        }

        if (!Dominio.Regras.Formatos.CnpjValido(empresa.Cnpj))
        {
            pendencias.Add("A empresa precisa de CNPJ para emitir CT-e (a chave de acesso usa o CNPJ)");
        }

        var municipioEmpresa = MunicipioObrigatorio(dados, empresa.Endereco.MunicipioCodigo, "Município da empresa", pendencias);
        var inicio = MunicipioObrigatorio(dados, dados.Cte.MunicipioInicioCodigo, "Município de início da prestação", pendencias);
        var fim = MunicipioObrigatorio(dados, dados.Cte.MunicipioFimCodigo, "Município de término da prestação", pendencias);
        var remetente = ParticipanteOpcional(dados, dados.Cte.RemetenteId, "Remetente", pendencias);
        var destinatario = ParticipanteOpcional(dados, dados.Cte.DestinatarioId, "Destinatário", pendencias);
        var expedidor = ParticipanteOpcional(dados, dados.Cte.ExpedidorId, "Expedidor", pendencias);
        var recebedor = ParticipanteOpcional(dados, dados.Cte.RecebedorId, "Recebedor", pendencias);
        var tomador = ParticipanteOpcional(dados, dados.Cte.TomadorId, "Tomador", pendencias);
        foreach (var participante in new[] { remetente, destinatario, expedidor, recebedor, tomador }.OfType<Participante>().Distinct())
        {
            if (dados.Municipio(participante.Endereco.MunicipioCodigo ?? 0) is null)
            {
                pendencias.Add($"Município do participante {participante.RazaoSocial}");
            }
        }

        if (pendencias.Count > 0 || municipioEmpresa is null || inicio is null || fim is null || tomador is null)
        {
            return new CteMontado(null, string.Empty, pendencias);
        }

        var codigoUf = municipioEmpresa.Codigo / 100_000;
        var codigoNumerico = ChaveAcesso.GerarCodigoNumerico(dados.Numero);
        var chave = ChaveAcesso.Montar(codigoUf, dados.DataHoraEmissao.DateTime, empresa.Cnpj, 57, dados.Serie.Serie, dados.Numero, 1, codigoNumerico);
        var cte = dados.Cte;

        var infCte = new TcTeInfCte
        {
            Versao = Versao,
            Id = "CTe" + chave,
            Ide = new TcTeInfCteIde
            {
                Cuf = LeiauteEnum.De<TCodUfIbge>(codigoUf, 2),
                Cct = codigoNumerico.ToString("00000000", System.Globalization.CultureInfo.InvariantCulture),
                Cfop = cte.Cfop,
                NatOp = Texto(cte.NaturezaOperacao),
                Mod = TModCt.Item57,
                Serie = dados.Serie.Serie.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Nct = dados.Numero.ToString(System.Globalization.CultureInfo.InvariantCulture),
                DhEmi = DataHora(dados.DataHoraEmissao),
                TpImp = TcTeInfCteIdeTpImp.Item1,
                TpEmis = TcTeInfCteIdeTpEmis.Item1,
                Cdv = chave[^1..],
                TpAmb = LeiauteEnum.De<TAmb>((int)dados.Ambiente),
                TpCTe = LeiauteEnum.De<TFinCTe>((int)cte.Tipo!.Value),
                ProcEmi = TProcEmiCTe.Item0,
                VerProc = dados.VersaoAplicativo.Length > 20 ? dados.VersaoAplicativo[..20] : dados.VersaoAplicativo,
                CMunEnv = municipioEmpresa.Codigo.ToString(System.Globalization.CultureInfo.InvariantCulture),
                XMunEnv = Texto(municipioEmpresa.Nome),
                UfEnv = LeiauteEnum.De<TUf>(municipioEmpresa.Uf),
                Modal = TModTransp.Item01,
                TpServ = LeiauteEnum.De<TcTeInfCteIdeTpServ>((int)cte.TipoServico!.Value),
                CMunIni = inicio.Codigo.ToString(System.Globalization.CultureInfo.InvariantCulture),
                XMunIni = Texto(inicio.Nome),
                UfIni = LeiauteEnum.De<TUf>(inicio.Uf),
                CMunFim = fim.Codigo.ToString(System.Globalization.CultureInfo.InvariantCulture),
                XMunFim = Texto(fim.Nome),
                UfFim = LeiauteEnum.De<TUf>(fim.Uf),
                Retira = TcTeInfCteIdeRetira.Item1,
                IndIeToma = LeiauteEnum.De<TcTeInfCteIdeIndIeToma>((int)(tomador.IndicadorIe ?? IndicadorIe.NaoContribuinte)),
            },
            Emit = Emitente(empresa, municipioEmpresa),
            VPrest = new TcTeInfCteVPrest
            {
                VtPrest = Valor(cte.ValorTotalPrestacao),
                VRec = Valor(cte.ValorReceber ?? 0),
            },
            Imp = new TcTeInfCteImp
            {
                Icms = new TImp { Icmssn = new TImpIcmssn { Cst = TImpIcmssnCst.Item90, IndSn = TImpIcmssnIndSn.Item1 } },
                Ibscbs = IbsCbsSimples(empresa, cte.ValorTotalPrestacao),

                // NT 2026.004: em 2026 o total do DF-e repete o valor da prestação (sem somar IBS e CBS); no Simples
                // sem regime regular não há gIBSCBS a somar.
                VTotDFe = Valor(cte.ValorTotalPrestacao),
            },
        };

        foreach (var componente in cte.Componentes)
        {
            infCte.VPrest.Comp.Add(new TcTeInfCteVPrestComp { XNome = Texto(componente.Nome), VComp = Valor(componente.Valor) });
        }

        if (cte.Tomador == TomadorServico.Outros)
        {
            infCte.Ide.Toma4 = Tomador4(tomador, dados);
        }
        else
        {
            infCte.Ide.Toma3 = new TcTeInfCteIdeToma3 { Toma = LeiauteEnum.De<TcTeInfCteIdeToma3Toma>((int)cte.Tomador!.Value) };
        }

        if (!string.IsNullOrWhiteSpace(cte.Observacoes))
        {
            infCte.Compl = new TcTeInfCteCompl { XObs = Texto(cte.Observacoes) };
        }

        if (remetente is not null)
        {
            infCte.Rem = new TcTeInfCteRem
            {
                Cnpj = Cnpj(remetente), Cpf = Cpf(remetente), Ie = Ie(remetente), XNome = Texto(remetente.RazaoSocial),
                XFant = Opcional(remetente.NomeFantasia), Fone = Opcional(remetente.Fone), Email = Opcional(remetente.Email),
                EnderReme = Endereco(remetente, dados),
            };
        }

        if (expedidor is not null)
        {
            infCte.Exped = new TcTeInfCteExped
            {
                Cnpj = Cnpj(expedidor), Cpf = Cpf(expedidor), Ie = Ie(expedidor), XNome = Texto(expedidor.RazaoSocial),
                Fone = Opcional(expedidor.Fone), Email = Opcional(expedidor.Email), EnderExped = Endereco(expedidor, dados),
            };
        }

        if (recebedor is not null)
        {
            infCte.Receb = new TcTeInfCteReceb
            {
                Cnpj = Cnpj(recebedor), Cpf = Cpf(recebedor), Ie = Ie(recebedor), XNome = Texto(recebedor.RazaoSocial),
                Fone = Opcional(recebedor.Fone), Email = Opcional(recebedor.Email), EnderReceb = Endereco(recebedor, dados),
            };
        }

        if (destinatario is not null)
        {
            infCte.Dest = new TcTeInfCteDest
            {
                Cnpj = Cnpj(destinatario), Cpf = Cpf(destinatario), Ie = Ie(destinatario), XNome = Texto(destinatario.RazaoSocial),
                Fone = Opcional(destinatario.Fone), Email = Opcional(destinatario.Email), EnderDest = Endereco(destinatario, dados),
            };
        }

        if (cte.EhComplemento)
        {
            infCte.InfCteComp.Add(new TcTeInfCteInfCteComp { ChCTe = cte.ChaveCteComplementado });
        }
        else
        {
            infCte.InfCTeNorm = Normal(cte, empresa);
        }

        return new CteMontado(new TcTe { InfCte = infCte }, chave, []);
    }

    /// <summary>Serializa o CT-e montado (sem assinatura) num documento XML, sem os namespaces xsi/xsd.</summary>
    /// <param name="documento">CT-e montado.</param>
    public static XmlDocument Serializar(TcTe documento) => FormatacaoLeiaute.Serializar(documento, NamespaceCte);

    private static TcTeInfCteInfCTeNorm Normal(ConhecimentoTransporte cte, Empresa empresa)
    {
        var normal = new TcTeInfCteInfCTeNorm
        {
            InfCarga = new TcTeInfCteInfCTeNormInfCarga
            {
                VCarga = Valor(cte.ValorCarga ?? 0),
                ProPred = Texto(cte.ProdutoPredominante),
                XOutCat = Opcional(cte.OutrasCaracteristicas),
            },
            InfModal = new TcTeInfCteInfCTeNormInfModal
            {
                VersaoModal = VersaoModal,
                Any = Rodoviario(empresa),
            },
        };

        foreach (var quantidade in cte.Quantidades)
        {
            normal.InfCarga.InfQ.Add(new TcTeInfCteInfCTeNormInfCargaInfQ
            {
                CUnid = LeiauteEnum.De<TcTeInfCteInfCTeNormInfCargaInfQcUnid>((int)quantidade.Unidade, 2),
                TpMed = Texto(quantidade.TipoMedida),
                QCarga = Quantidade(quantidade.Quantidade),
            });
        }

        if (cte.DocumentosNfe.Count > 0)
        {
            normal.InfDoc = new TcTeInfCteInfCTeNormInfDoc();
            foreach (var documento in cte.DocumentosNfe)
            {
                normal.InfDoc.InfNFe.Add(new TcTeInfCteInfCTeNormInfDocInfNFe { Chave = documento.Chave });
            }
        }

        if (cte.ExigeDocumentoAnterior)
        {
            foreach (var anterior in cte.DocumentosAnteriores)
            {
                var documento = Dominio.Regras.Formatos.SoAlfanumericos(anterior.CnpjCpfEmitente);
                var emitente = new TcTeInfCteInfCTeNormDocAntEmiDocAnt
                {
                    Cnpj = documento.Length == 14 ? documento : null,
                    Cpf = documento.Length == 11 ? documento : null,
                    XNome = Texto(anterior.NomeEmitente),
                };
                var identificacao = new TcTeInfCteInfCTeNormDocAntEmiDocAntIdDocAnt();
                identificacao.IdDocAntEle.Add(new TcTeInfCteInfCTeNormDocAntEmiDocAntIdDocAntIdDocAntEle { ChCTe = anterior.ChaveCte });
                emitente.IdDocAnt.Add(identificacao);
                normal.DocAnt.Add(emitente);
            }
        }

        return normal;
    }

    /// <summary>Grupo <c>rodo</c> do modal rodoviário, serializado à parte para ir em <c>infModal</c>.</summary>
    private static XmlElement Rodoviario(Empresa empresa) => Elemento(new Rodo { Rntrc = empresa.Rntrc }, NamespaceCte);

    private static TcTeInfCteEmit Emitente(Empresa empresa, Municipio municipio) => new()
    {
        Cnpj = empresa.Cnpj,
        Ie = Opcional(empresa.Ie),
        Iest = Opcional(empresa.IeSt),
        XNome = Texto(empresa.RazaoSocial),
        XFant = Opcional(empresa.NomeFantasia),
        EnderEmit = new TEndeEmi
        {
            XLgr = Texto(empresa.Endereco.Logradouro),
            Nro = Texto(empresa.Endereco.Numero),
            XCpl = Opcional(empresa.Endereco.Complemento),
            XBairro = Texto(empresa.Endereco.Bairro),
            CMun = municipio.Codigo.ToString(System.Globalization.CultureInfo.InvariantCulture),
            XMun = Texto(municipio.Nome),
            Cep = Opcional(empresa.Endereco.Cep),
            Uf = LeiauteEnum.De<TufSemEx>(municipio.Uf),
            Fone = Opcional(empresa.Fone),
        },
        Crt = LeiauteEnum.De<Tcrt>((int)empresa.Crt!.Value),
    };

    private static TcTeInfCteIdeToma4 Tomador4(Participante tomador, DadosEmissaoCte dados) => new()
    {
        Toma = TcTeInfCteIdeToma4Toma.Item4,
        Cnpj = Cnpj(tomador),
        Cpf = Cpf(tomador),
        Ie = Ie(tomador),
        XNome = Texto(tomador.RazaoSocial),
        XFant = Opcional(tomador.NomeFantasia),
        Fone = Opcional(tomador.Fone),
        EnderToma = Endereco(tomador, dados),
        Email = Opcional(tomador.Email),
    };

    private static TEndereco Endereco(Participante participante, DadosEmissaoCte dados)
    {
        var municipio = dados.Municipio(participante.Endereco.MunicipioCodigo!.Value)!;
        return new TEndereco
        {
            XLgr = Texto(participante.Endereco.Logradouro),
            Nro = Texto(participante.Endereco.Numero),
            XCpl = Opcional(participante.Endereco.Complemento),
            XBairro = Texto(participante.Endereco.Bairro),
            CMun = municipio.Codigo.ToString(System.Globalization.CultureInfo.InvariantCulture),
            XMun = Texto(municipio.Nome),
            Cep = Opcional(participante.Endereco.Cep),
            Uf = LeiauteEnum.De<TUf>(municipio.Uf),
        };
    }

    private static string? Cnpj(Participante participante) => participante.CnpjCpf.Length == 14 ? participante.CnpjCpf : null;

    private static string? Cpf(Participante participante) => participante.CnpjCpf.Length == 11 ? participante.CnpjCpf : null;

    /// <summary>IE só para contribuinte; isento e não contribuinte vão sem o campo.</summary>
    private static string? Ie(Participante participante) =>
        participante.IndicadorIe == IndicadorIe.Contribuinte ? Opcional(participante.Ie) : null;

    private static Municipio? MunicipioObrigatorio(DadosEmissaoCte dados, int? codigo, string descricao, List<string> pendencias)
    {
        if (codigo is null)
        {
            return null;
        }

        var municipio = dados.Municipio(codigo.Value);
        if (municipio is null)
        {
            pendencias.Add($"{descricao}: código {codigo} não encontrado na lista do IBGE");
        }

        return municipio;
    }

    private static Participante? ParticipanteOpcional(DadosEmissaoCte dados, int? id, string papel, List<string> pendencias)
    {
        if (id is null)
        {
            return null;
        }

        if (!dados.Participantes.TryGetValue(id.Value, out var participante))
        {
            pendencias.Add($"{papel}: participante {id} não encontrado no cadastro");
        }

        return participante;
    }

}
