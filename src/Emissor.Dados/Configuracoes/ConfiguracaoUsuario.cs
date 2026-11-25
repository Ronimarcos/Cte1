using Emissor.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emissor.Dados.Configuracoes;

/// <summary>
/// Usuários: login único; empresas liberadas em tabela filha, com chave estrangeira para a empresa sem exclusão em
/// cascata (empresa liberada para algum usuário não pode ser excluída sem antes tirar a liberação).
/// </summary>
internal sealed class ConfiguracaoUsuario : IEntityTypeConfiguration<Usuario>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.Property(u => u.Login).HasMaxLength(30).IsRequired();
        builder.Property(u => u.Nome).HasMaxLength(60).IsRequired();
        builder.Property(u => u.SenhaHash).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Perfil).IsRequired();
        builder.HasIndex(u => u.Login).IsUnique();
        builder.Ignore(u => u.Administrador);

        builder.OwnsMany(u => u.Empresas, e =>
        {
            e.ToTable("UsuarioEmpresas");
            ChavesFilhas.Configurar(e);
            e.HasOne<Empresa>().WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex("UsuarioId", nameof(UsuarioEmpresa.EmpresaId)).IsUnique();
        });
    }
}
