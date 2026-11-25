using Emissor.Dominio.Enums;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// Linha da auditoria: quem fez o quê, em qual empresa e quando. Só é incluída; o banco recusa alteração e exclusão
/// (gatilhos da migração <c>Auditoria</c>). Nunca guarda senha nem conteúdo de certificado.
/// </summary>
public class RegistroAuditoria
{
    /// <summary>Tamanho máximo da descrição.</summary>
    public const int TamanhoDescricao = 4000;

    /// <summary>Identificador.</summary>
    public long Id { get; set; }

    /// <summary>Data e hora da ação.</summary>
    public DateTime DataHora { get; set; }

    /// <summary>Usuário que fez a ação (<c>null</c> em tentativa de login com usuário inexistente).</summary>
    public int? UsuarioId { get; set; }

    /// <summary>Login no momento da ação (ou o login digitado, na tentativa recusada).</summary>
    public string UsuarioLogin { get; set; } = string.Empty;

    /// <summary>Empresa da ação, quando houver.</summary>
    public int? EmpresaId { get; set; }

    /// <summary>Ação.</summary>
    public AcaoAuditoria Acao { get; set; }

    /// <summary>Tipo do registro afetado (ex.: "Participante", "CT-e"), quando houver.</summary>
    public string Entidade { get; set; } = string.Empty;

    /// <summary>Código do registro afetado, quando houver.</summary>
    public int? RegistroId { get; set; }

    /// <summary>Detalhe: campos alterados com o valor anterior e o novo, ou o texto do evento.</summary>
    public string Descricao { get; set; } = string.Empty;
}
