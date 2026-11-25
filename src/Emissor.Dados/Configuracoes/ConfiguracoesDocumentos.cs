using Emissor.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emissor.Dados.Configuracoes;

/// <summary>
/// CT-e: série, participantes e municípios como chaves estrangeiras sem exclusão em cascata (cadastro usado em
/// documento não pode ser excluído). Quantidades, NF-e, documentos anteriores e componentes ficam em tabelas
/// filhas, apagadas junto com o CT-e.
/// </summary>
internal sealed class ConfiguracaoConhecimentoTransporte : ConfiguracaoDaEmpresa<ConhecimentoTransporte>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<ConhecimentoTransporte> builder)
    {
        builder.ToTable("Ctes");
        builder.Property(c => c.Cfop).HasMaxLength(4).IsRequired();
        builder.Property(c => c.NaturezaOperacao).HasMaxLength(60).IsRequired();
        builder.Property(c => c.ProdutoPredominante).HasMaxLength(60).IsRequired();
        builder.Property(c => c.OutrasCaracteristicas).HasMaxLength(30).IsRequired();
        builder.Property(c => c.ChaveCteComplementado).HasMaxLength(44).IsRequired();
        builder.Property(c => c.Observacoes).HasMaxLength(2000).IsRequired();

        builder.HasOne<SerieDocumento>().WithMany().HasForeignKey(c => c.SerieId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Participante>().WithMany().HasForeignKey(c => c.RemetenteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Participante>().WithMany().HasForeignKey(c => c.DestinatarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Participante>().WithMany().HasForeignKey(c => c.ExpedidorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Participante>().WithMany().HasForeignKey(c => c.RecebedorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Participante>().WithMany().HasForeignKey(c => c.TomadorOutrosId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Municipio>().WithMany().HasForeignKey(c => c.MunicipioInicioCodigo).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Municipio>().WithMany().HasForeignKey(c => c.MunicipioFimCodigo).OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(c => c.Quantidades, q =>
        {
            q.ToTable("CteQuantidades");
            ChavesFilhas.Configurar(q);
            q.Property(x => x.TipoMedida).HasMaxLength(20).IsRequired();
        });
        builder.OwnsMany(c => c.DocumentosNfe, d =>
        {
            d.ToTable("CteNfes");
            ChavesFilhas.Configurar(d);
            d.Property(x => x.Chave).HasMaxLength(44).IsRequired();
        });
        builder.OwnsMany(c => c.DocumentosAnteriores, d =>
        {
            d.ToTable("CteDocumentosAnteriores");
            ChavesFilhas.Configurar(d);
            d.Property(x => x.CnpjCpfEmitente).HasMaxLength(14).IsRequired();
            d.Property(x => x.NomeEmitente).HasMaxLength(60).IsRequired();
            d.Property(x => x.ChaveCte).HasMaxLength(44).IsRequired();
        });
        builder.OwnsMany(c => c.Componentes, x =>
        {
            x.ToTable("CteComponentes");
            ChavesFilhas.Configurar(x);
            x.Property(c => c.Nome).HasMaxLength(15).IsRequired();
        });

        builder.Ignore(c => c.EmDigitacao);
        builder.HasIndex(c => new { c.EmpresaId, c.DataEmissao });
    }
}

/// <summary>
/// MDF-e: série, veículo de tração e apólice como chaves estrangeiras sem exclusão em cascata. Descargas (com os
/// documentos) e CIOT ficam em tabelas filhas; o CT-e vinculado numa descarga não pode ser excluído. Percurso,
/// municípios de carregamento, reboques, condutores e averbações são listas simples gravadas em JSON na própria
/// tabela.
/// </summary>
internal sealed class ConfiguracaoManifestoEletronico : ConfiguracaoDaEmpresa<ManifestoEletronico>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<ManifestoEletronico> builder)
    {
        builder.ToTable("Mdfes");
        builder.Property(m => m.UfInicio).HasMaxLength(2).IsRequired();
        builder.Property(m => m.UfFim).HasMaxLength(2).IsRequired();
        builder.Property(m => m.ProdutoPredominante).HasMaxLength(120).IsRequired();
        builder.Property(m => m.InformacoesComplementares).HasMaxLength(5000).IsRequired();

        builder.HasOne<SerieDocumento>().WithMany().HasForeignKey(m => m.SerieId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Veiculo>().WithMany().HasForeignKey(m => m.VeiculoTracaoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApoliceSeguro>().WithMany().HasForeignKey(m => m.ApoliceId).OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(m => m.Descargas, d =>
        {
            d.ToTable("MdfeDescargas");
            ChavesFilhas.Configurar(d);
            d.HasOne<Municipio>().WithMany().HasForeignKey(x => x.MunicipioCodigo).OnDelete(DeleteBehavior.Restrict);
            d.OwnsMany(x => x.Documentos, doc =>
            {
                doc.ToTable("MdfeDocumentos");
                ChavesFilhas.Configurar(doc);
                doc.Property(x => x.Chave).HasMaxLength(44).IsRequired();
                doc.HasOne<ConhecimentoTransporte>().WithMany().HasForeignKey(x => x.CteId).OnDelete(DeleteBehavior.Restrict);
            });
        });
        builder.OwnsMany(m => m.Ciots, c =>
        {
            c.ToTable("MdfeCiots");
            ChavesFilhas.Configurar(c);
            c.Property(x => x.Ciot).HasMaxLength(12).IsRequired();
            c.Property(x => x.CpfCnpjResponsavel).HasMaxLength(18).IsRequired();
        });

        builder.Ignore(m => m.EmDigitacao);
        builder.Ignore(m => m.PercursoDescricao);
        builder.HasIndex(m => new { m.EmpresaId, m.DataEmissao });
    }
}

/// <summary>
/// Chave das tabelas filhas: um <c>Id</c> autonumerado próprio (o SQLite só gera números em chave de uma coluna),
/// com a chave estrangeira para o documento à parte.
/// </summary>
internal static class ChavesFilhas
{
    /// <summary>Configura a chave do item.</summary>
    /// <typeparam name="TDono">Documento (ou descarga) dono do item.</typeparam>
    /// <typeparam name="TItem">Item.</typeparam>
    /// <param name="item">Construtor do item.</param>
    public static void Configurar<TDono, TItem>(OwnedNavigationBuilder<TDono, TItem> item) where TDono : class where TItem : class
    {
        item.Property<int>("Id").ValueGeneratedOnAdd();
        item.HasKey("Id");
    }
}
