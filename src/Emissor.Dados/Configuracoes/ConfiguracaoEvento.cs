using Emissor.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emissor.Dados.Configuracoes;

/// <summary>
/// Eventos dos documentos. O documento é apontado pelo tipo (CT-e ou MDF-e) e pelo código interno, sem chave
/// estrangeira (o mesmo campo serve às duas tabelas); a sequência é única por documento e tipo. As correções da CC-e
/// ficam numa tabela filha. O município de encerramento é chave estrangeira para a tabela do IBGE.
/// </summary>
internal sealed class ConfiguracaoEvento : ConfiguracaoDaEmpresa<EventoDocumento>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<EventoDocumento> builder)
    {
        builder.ToTable("Eventos");
        builder.Property(e => e.Justificativa).HasMaxLength(255).IsRequired();
        builder.Property(e => e.NomeCondutor).HasMaxLength(60).IsRequired();
        builder.Property(e => e.CpfCondutor).HasMaxLength(11).IsRequired();
        builder.Property(e => e.Xml).IsRequired();
        builder.Property(e => e.Protocolo).HasMaxLength(15).IsRequired();
        builder.Property(e => e.Motivo).HasMaxLength(255).IsRequired();
        builder.HasOne<Municipio>().WithMany().HasForeignKey(e => e.MunicipioEncerramentoCodigo).OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(e => e.Correcoes, c =>
        {
            c.ToTable("EventoCorrecoes");
            ChavesFilhas.Configurar(c);
            c.Property(x => x.Grupo).HasMaxLength(20).IsRequired();
            c.Property(x => x.Campo).HasMaxLength(20).IsRequired();
            c.Property(x => x.Valor).HasMaxLength(500).IsRequired();
        });

        builder.HasIndex(e => new { e.EmpresaId, e.Tipo, e.DocumentoId, e.Sequencia }).IsUnique();
    }
}
