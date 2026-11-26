using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// Evento de um CT-e ou MDF-e (cancelamento, carta de correção, encerramento, inclusão de condutor). Guarda os dados
/// informados, o XML assinado e, depois da transmissão (P4d2), o retorno da SEFAZ. O documento é identificado pelo
/// modelo do tipo de evento e pelo código interno (tabela de CT-e ou de MDF-e).
/// </summary>
public class EventoDocumento : EntidadeDaEmpresa
{
    /// <summary>Tipo do evento.</summary>
    public TipoEvento Tipo { get; set; }

    /// <summary>Código interno do CT-e ou do MDF-e (conforme o modelo do tipo).</summary>
    public int DocumentoId { get; set; }

    /// <summary>Número sequencial do evento para o mesmo tipo e documento (<c>nSeqEvento</c>).</summary>
    public int Sequencia { get; set; } = 1;

    /// <summary>Data e hora do evento (<c>dhEvento</c>).</summary>
    public DateTime DataHora { get; set; }

    /// <summary>Justificativa do cancelamento (<c>xJust</c>, 15 a 255 caracteres).</summary>
    public string Justificativa { get; set; } = string.Empty;

    /// <summary>Correções da carta de correção (<c>infCorrecao</c>).</summary>
    public List<CorrecaoCte> Correcoes { get; set; } = [];

    /// <summary>Data do encerramento do MDF-e (<c>dtEnc</c>).</summary>
    public DateTime? DataEncerramento { get; set; }

    /// <summary>Código IBGE do município de encerramento (<c>cMun</c>; a UF sai dos dois primeiros dígitos).</summary>
    public int? MunicipioEncerramentoCodigo { get; set; }

    /// <summary>Nome do condutor incluído (<c>xNome</c>).</summary>
    public string NomeCondutor { get; set; } = string.Empty;

    /// <summary>CPF do condutor incluído, só dígitos.</summary>
    public string CpfCondutor { get; set; } = string.Empty;

    /// <summary>Situação do evento.</summary>
    public SituacaoEvento Situacao { get; set; } = SituacaoEvento.Gerado;

    /// <summary>XML do evento assinado (<c>eventoCTe</c> ou <c>eventoMDFe</c>).</summary>
    public string Xml { get; set; } = string.Empty;

    /// <summary>Protocolo de registro do evento (preenchido pela SEFAZ).</summary>
    public string Protocolo { get; set; } = string.Empty;

    /// <summary>Código de status do retorno (<c>cStat</c>).</summary>
    public int? CodigoStatus { get; set; }

    /// <summary>Motivo do retorno (<c>xMotivo</c>).</summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>Data e hora do registro na SEFAZ (<c>dhRegEvento</c>).</summary>
    public DateTime? RegistradoEm { get; set; }

    /// <inheritdoc />
    public override IReadOnlyList<string> Validar() => RegrasEvento.ValidarDados(this);
}

/// <summary>Uma correção da carta de correção do CT-e.</summary>
public class CorrecaoCte
{
    /// <summary>Grupo do leiaute alterado (<c>grupoAlterado</c>, ex.: <c>ide</c>).</summary>
    public string Grupo { get; set; } = string.Empty;

    /// <summary>Campo alterado (<c>campoAlterado</c>, ex.: <c>xObs</c>).</summary>
    public string Campo { get; set; } = string.Empty;

    /// <summary>Valor correto (<c>valorAlterado</c>, até 500 caracteres).</summary>
    public string Valor { get; set; } = string.Empty;

    /// <summary>Número do item, quando o grupo se repete (<c>nroItemAlterado</c>, 1 a 99).</summary>
    public int? NumeroItem { get; set; }
}
