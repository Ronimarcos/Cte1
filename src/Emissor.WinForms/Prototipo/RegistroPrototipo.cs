namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Base dos registros fictícios do protótipo, com os campos comuns da convenção do §5 do levantamento.
/// Será substituída pelas entidades do domínio quando a persistência (EF Core) for implementada.
/// </summary>
public abstract class RegistroPrototipo
{
    /// <summary>Identificador interno.</summary>
    public int Id { get; set; }

    /// <summary>Indica se o registro está ativo.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Data e hora da inclusão.</summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>Data e hora da última alteração.</summary>
    public DateTime AlteradoEm { get; set; }

    /// <summary>Cria uma cópia rasa do registro, usada para editar sem afetar a lista até o usuário salvar.</summary>
    public RegistroPrototipo Clonar() => (RegistroPrototipo)MemberwiseClone();

    /// <summary>Lista os campos obrigatórios não preenchidos; vazia quando o registro pode ser salvo.</summary>
    public abstract IReadOnlyList<string> ValidarObrigatorios();
}
