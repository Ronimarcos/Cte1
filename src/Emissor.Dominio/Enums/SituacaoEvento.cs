namespace Emissor.Dominio.Enums;

/// <summary>Situação de um evento (cancelamento, CC-e, encerramento, inclusão de condutor) no emissor.</summary>
public enum SituacaoEvento
{
    /// <summary>XML montado e assinado; ainda não enviado à SEFAZ.</summary>
    Gerado = 0,

    /// <summary>Enviado à SEFAZ sem resposta conclusiva (ex.: falha de comunicação); consultar antes de reenviar.</summary>
    Transmitido = 1,

    /// <summary>Registrado pela SEFAZ (cStat 135 ou 136), com protocolo.</summary>
    Registrado = 2,

    /// <summary>Rejeitado pela SEFAZ; o motivo fica gravado.</summary>
    Rejeitado = 3,
}
