using System.Globalization;
using System.Xml;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.DFe.Leiaute;
using LeiauteCteEvento = Emissor.DFe.Leiaute.CteEvento;
using LeiauteMdfeEvento = Emissor.DFe.Leiaute.MdfeEvento;

namespace Emissor.DFe.Eventos;

/// <summary>Dados para montar um evento.</summary>
/// <param name="Evento">Evento com os dados informados na tela (tipo, sequência, justificativa, correções...).</param>
/// <param name="Empresa">Empresa emitente (autora do evento).</param>
/// <param name="Chave">Chave de acesso do documento.</param>
/// <param name="ProtocoloDocumento">Protocolo de autorização do documento (cancelamento e encerramento).</param>
/// <param name="Ambiente">Ambiente (<c>tpAmb</c>).</param>
/// <param name="DataHora">Data e hora do evento, com o fuso (<c>dhEvento</c>).</param>
public sealed record DadosEvento(
    EventoDocumento Evento,
    Empresa Empresa,
    string Chave,
    string ProtocoloDocumento,
    TipoAmbiente Ambiente,
    DateTimeOffset DataHora);

/// <summary>Evento montado e serializado, ainda sem assinatura.</summary>
/// <param name="Documento">XML do evento (<c>eventoCTe</c> ou <c>eventoMDFe</c>); nulo se houve pendências.</param>
/// <param name="SchemaEnvelope">XSD do envelope.</param>
/// <param name="SchemaDetalhe">XSD do detalhe que vai em <c>detEvento</c>.</param>
/// <param name="Pendencias">O que impediu a montagem.</param>
public sealed record EventoMontado(XmlDocument? Documento, string SchemaEnvelope, string SchemaDetalhe, IReadOnlyList<string> Pendencias);

/// <summary>
/// Monta os eventos do CT-e (versão 4.00) e do MDF-e (versão 3.00) com as classes geradas dos XSD. O detalhe de cada
/// evento é serializado à parte e colocado em <c>detEvento</c>, que no envelope aceita qualquer conteúdo. O Id segue o
/// MOC: "ID" + tipo do evento + chave + sequência (3 dígitos no CT-e; 2 no MDF-e até 99).
/// </summary>
public static class MontadorEvento
{
    /// <summary>Versão do leiaute dos eventos do CT-e.</summary>
    public const string VersaoCte = "4.00";

    /// <summary>Versão do leiaute dos eventos do MDF-e.</summary>
    public const string VersaoMdfe = "3.00";

    private const string NamespaceCte = "http://www.portalfiscal.inf.br/cte";
    private const string NamespaceMdfe = "http://www.portalfiscal.inf.br/mdfe";

    /// <summary>Monta o evento conforme o tipo.</summary>
    /// <param name="dados">Evento, empresa, chave e protocolo do documento.</param>
    public static EventoMontado Montar(DadosEvento dados)
    {
        var pendencias = new List<string>(RegrasEvento.ValidarDados(dados.Evento));
        if (!ChaveAcesso.Valida(dados.Chave))
        {
            pendencias.Add("A chave de acesso do documento é inválida.");
        }

        var precisaProtocolo = dados.Evento.Tipo is TipoEvento.CancelamentoCte or TipoEvento.CancelamentoMdfe or TipoEvento.EncerramentoMdfe;
        if (precisaProtocolo && (dados.ProtocoloDocumento.Length != 15 || !dados.ProtocoloDocumento.All(char.IsAsciiDigit)))
        {
            pendencias.Add("O protocolo de autorização do documento deve ter 15 dígitos.");
        }

        if (dados.Empresa.Documento.Length == 0)
        {
            pendencias.Add("A empresa não tem CNPJ nem CPF.");
        }

        if (pendencias.Count > 0)
        {
            return new EventoMontado(null, string.Empty, string.Empty, pendencias);
        }

        return dados.Evento.Tipo.Modelo() == ModeloDocumento.Cte ? MontarCte(dados) : MontarMdfe(dados);
    }

    /// <summary>Id do evento (<c>infEvento/@Id</c>).</summary>
    /// <param name="tipo">Tipo do evento.</param>
    /// <param name="chave">Chave do documento.</param>
    /// <param name="sequencia">Número sequencial.</param>
    public static string Id(TipoEvento tipo, string chave, int sequencia)
    {
        var digitos = tipo.Modelo() == ModeloDocumento.Cte || sequencia > 99 ? 3 : 2;
        return "ID" + tipo.Codigo() + chave + sequencia.ToString(new string('0', digitos), CultureInfo.InvariantCulture);
    }

    private static EventoMontado MontarCte(DadosEvento dados)
    {
        var evento = dados.Evento;
        XmlElement detalhe;
        string schemaDetalhe;
        if (evento.Tipo == TipoEvento.CancelamentoCte)
        {
            detalhe = FormatacaoLeiaute.Elemento(new LeiauteCteEvento.EvCancCTe
            {
                DescEvento = LeiauteCteEvento.EvCancCTeDescEvento.Cancelamento,
                NProt = dados.ProtocoloDocumento,
                XJust = RegrasEvento.Normalizar(evento.Justificativa),
            }, NamespaceCte);
            schemaDetalhe = Validacao.ValidadorXsd.SchemaEvCancCte;
        }
        else
        {
            var carta = new LeiauteCteEvento.EvCCeCTe
            {
                DescEvento = LeiauteCteEvento.EvCCeCTeDescEvento.CartaDeCorreção,
                XCondUso = LeiauteCteEvento.EvCCeCTeXCondUso
                    .ACartaDeCorreçãoÉDisciplinadaPeloArt58BDoConvênioSinief0689FicaPermitidaAUtilizaçãoDeCartaDeCorreçãoParaRegularizaçãoDeErroOcorridoNaEmissãoDeDocumentosFiscaisRelativosÀPrestaçãoDeServiçoDeTransporteDesdeQueOErroNãoEstejaRelacionadoComIAsVariáveisQueDeterminamOValorDoImpostoTaisComoBaseDeCálculoAlíquotaDiferençaDePreçoQuantidadeValorDaPrestaçãoIiACorreçãoDeDadosCadastraisQueImpliqueMudançaDoEmitenteTomadorRemetenteOuDoDestinatárioIiiADataDeEmissãoOuDeSaída,
            };
            foreach (var correcao in evento.Correcoes)
            {
                carta.InfCorrecao.Add(new LeiauteCteEvento.EvCCeCTeInfCorrecao
                {
                    GrupoAlterado = correcao.Grupo.Trim(),
                    CampoAlterado = correcao.Campo.Trim(),
                    ValorAlterado = RegrasEvento.Normalizar(correcao.Valor),
                    NroItemAlterado = correcao.NumeroItem?.ToString("00", CultureInfo.InvariantCulture),
                });
            }

            detalhe = FormatacaoLeiaute.Elemento(carta, NamespaceCte);
            schemaDetalhe = Validacao.ValidadorXsd.SchemaEvCceCte;
        }

        var documento = new LeiauteCteEvento.TEvento
        {
            Versao = VersaoCte,
            InfEvento = new LeiauteCteEvento.TEventoInfEvento
            {
                Id = Id(evento.Tipo, dados.Chave, evento.Sequencia),
                COrgao = LeiauteEnum.De<LeiauteCteEvento.TcOrgaoIbge>(dados.Chave[..2]),
                TpAmb = LeiauteEnum.De<LeiauteCteEvento.TAmb>((int)dados.Ambiente),
                Cnpj = dados.Empresa.Cnpj.Length > 0 ? dados.Empresa.Cnpj : null,
                Cpf = dados.Empresa.Cnpj.Length > 0 ? null : dados.Empresa.Cpf,
                ChCTe = dados.Chave,
                DhEvento = FormatacaoLeiaute.DataHora(dados.DataHora),
                TpEvento = evento.Tipo.Codigo(),
                NSeqEvento = FormatacaoLeiaute.Inteiro(evento.Sequencia),
                DetEvento = new LeiauteCteEvento.TEventoInfEventoDetEvento { VersaoEvento = VersaoCte, Any = detalhe },
            },
        };

        return new EventoMontado(FormatacaoLeiaute.Serializar(documento, NamespaceCte), Validacao.ValidadorXsd.SchemaEventoCte, schemaDetalhe, []);
    }

    private static EventoMontado MontarMdfe(DadosEvento dados)
    {
        var evento = dados.Evento;
        XmlElement detalhe;
        string schemaDetalhe;
        switch (evento.Tipo)
        {
            case TipoEvento.CancelamentoMdfe:
                detalhe = FormatacaoLeiaute.Elemento(new LeiauteMdfeEvento.EvCancMdFe
                {
                    DescEvento = LeiauteMdfeEvento.EvCancMdFeDescEvento.Cancelamento,
                    NProt = dados.ProtocoloDocumento,
                    XJust = RegrasEvento.Normalizar(evento.Justificativa),
                }, NamespaceMdfe);
                schemaDetalhe = Validacao.ValidadorXsd.SchemaEvCancMdfe;
                break;
            case TipoEvento.EncerramentoMdfe:
                var municipio = evento.MunicipioEncerramentoCodigo!.Value;
                detalhe = FormatacaoLeiaute.Elemento(new LeiauteMdfeEvento.EvEncMdFe
                {
                    DescEvento = LeiauteMdfeEvento.EvEncMdFeDescEvento.Encerramento,
                    NProt = dados.ProtocoloDocumento,
                    DtEnc = evento.DataEncerramento!.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Cuf = LeiauteEnum.De<LeiauteMdfeEvento.TCodUfIbgeEx>(municipio / 100_000, 2),
                    CMun = FormatacaoLeiaute.Inteiro(municipio),
                }, NamespaceMdfe);
                schemaDetalhe = Validacao.ValidadorXsd.SchemaEvEncMdfe;
                break;
            default:
                detalhe = FormatacaoLeiaute.Elemento(new LeiauteMdfeEvento.EvIncCondutorMdFe
                {
                    DescEvento = LeiauteMdfeEvento.EvIncCondutorMdFeDescEvento.InclusaoCondutor,
                    Condutor = new LeiauteMdfeEvento.EvIncCondutorMdFeCondutor
                    {
                        XNome = RegrasEvento.Normalizar(evento.NomeCondutor),
                        Cpf = evento.CpfCondutor,
                    },
                }, NamespaceMdfe);
                schemaDetalhe = Validacao.ValidadorXsd.SchemaEvIncCondutorMdfe;
                break;
        }

        var documento = new LeiauteMdfeEvento.TEvento
        {
            Versao = VersaoMdfe,
            InfEvento = new LeiauteMdfeEvento.TEventoInfEvento
            {
                Id = Id(evento.Tipo, dados.Chave, evento.Sequencia),
                COrgao = LeiauteEnum.De<LeiauteMdfeEvento.TcOrgaoIbge>(dados.Chave[..2]),
                TpAmb = LeiauteEnum.De<LeiauteMdfeEvento.TAmb>((int)dados.Ambiente),
                Cnpj = dados.Empresa.Cnpj.Length > 0 ? dados.Empresa.Cnpj : null,
                Cpf = dados.Empresa.Cnpj.Length > 0 ? null : dados.Empresa.Cpf,
                ChMdFe = dados.Chave,
                DhEvento = FormatacaoLeiaute.DataHora(dados.DataHora),
                TpEvento = evento.Tipo.Codigo(),
                NSeqEvento = FormatacaoLeiaute.Inteiro(evento.Sequencia),
                DetEvento = new LeiauteMdfeEvento.TEventoInfEventoDetEvento { VersaoEvento = VersaoMdfe, Any = detalhe },
            },
        };

        return new EventoMontado(FormatacaoLeiaute.Serializar(documento, NamespaceMdfe), Validacao.ValidadorXsd.SchemaEventoMdfe, schemaDetalhe, []);
    }
}
