using Emissor.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emissor.Dados.Configuracoes;

/// <summary>
/// Auditoria: sem chaves estrangeiras de propósito (o histórico continua legível mesmo depois que o usuário, a empresa
/// ou o registro forem excluídos). Índices para a consulta por data, usuário e empresa.
/// </summary>
internal sealed class ConfiguracaoAuditoria : IEntityTypeConfiguration<RegistroAuditoria>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("Auditoria");
        builder.Property(a => a.UsuarioLogin).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Entidade).HasMaxLength(40).IsRequired();
        builder.Property(a => a.Descricao).HasMaxLength(RegistroAuditoria.TamanhoDescricao).IsRequired();
        builder.HasIndex(a => a.DataHora);
        builder.HasIndex(a => new { a.UsuarioId, a.DataHora });
        builder.HasIndex(a => new { a.EmpresaId, a.DataHora });
    }
}
