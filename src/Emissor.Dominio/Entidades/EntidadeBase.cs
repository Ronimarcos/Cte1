namespace Emissor.Dominio.Entidades;

/// <summary>
/// Base das entidades de cadastro, com os campos comuns da convenção do §5 do levantamento.
/// </summary>
public abstract class EntidadeBase
{
    /// <summary>Identificador interno (gerado pelo banco).</summary>
    public int Id { get; set; }

    /// <summary>Indica se o registro está ativo; inativos não aparecem para novos documentos.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Data e hora da inclusão (preenchida pela camada de dados).</summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>Data e hora da última alteração (preenchida pela camada de dados).</summary>
    public DateTime AlteradoEm { get; set; }

    /// <summary>Lista o que impede gravar o registro; vazia quando ele está válido.</summary>
    public abstract IReadOnlyList<string> Validar();
}

/// <summary>Entidade de cadastro que pertence a uma empresa emitente (multiempresa).</summary>
public abstract class EntidadeDaEmpresa : EntidadeBase
{
    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }
}
