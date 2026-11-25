using Emissor.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emissor.Dados.Configuracoes;

/// <summary>
/// Campos comuns das entidades de cadastro de uma empresa: vínculo com a empresa sem exclusão em cascata
/// (empresa com cadastros não pode ser excluída).
/// </summary>
/// <typeparam name="T">Entidade configurada.</typeparam>
internal abstract class ConfiguracaoDaEmpresa<T> : IEntityTypeConfiguration<T> where T : EntidadeDaEmpresa
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasOne<Empresa>().WithMany().HasForeignKey(e => e.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        ConfigurarCampos(builder);
    }

    /// <summary>Tamanhos, índices e relações específicos da entidade.</summary>
    /// <param name="builder">Construtor do modelo da entidade.</param>
    protected abstract void ConfigurarCampos(EntityTypeBuilder<T> builder);
}

/// <summary>Endereço gravado nas colunas da própria tabela, com o município como chave estrangeira.</summary>
internal static class ConfiguracaoEndereco
{
    /// <summary>Mapeia o endereço da entidade.</summary>
    /// <typeparam name="T">Entidade dona do endereço.</typeparam>
    /// <param name="builder">Construtor do endereço.</param>
    public static void Mapear<T>(OwnedNavigationBuilder<T, Endereco> builder) where T : class
    {
        builder.Property(e => e.Logradouro).HasColumnName("Logradouro").HasMaxLength(255).IsRequired();
        builder.Property(e => e.Numero).HasColumnName("Numero").HasMaxLength(60).IsRequired();
        builder.Property(e => e.Complemento).HasColumnName("Complemento").HasMaxLength(60).IsRequired();
        builder.Property(e => e.Bairro).HasColumnName("Bairro").HasMaxLength(60).IsRequired();
        builder.Property(e => e.MunicipioCodigo).HasColumnName("MunicipioCodigo");
        builder.Property(e => e.Cep).HasColumnName("Cep").HasMaxLength(8).IsRequired();
        builder.HasOne<Municipio>().WithMany().HasForeignKey(e => e.MunicipioCodigo).OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>Municípios: chave é o código IBGE, sem geração automática.</summary>
internal sealed class ConfiguracaoMunicipio : IEntityTypeConfiguration<Municipio>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.ToTable("Municipios");
        builder.HasKey(m => m.Codigo);
        builder.Property(m => m.Codigo).ValueGeneratedNever();
        builder.Property(m => m.Nome).HasMaxLength(60).IsRequired();
        builder.Property(m => m.Uf).HasMaxLength(2).IsRequired();
        builder.Ignore(m => m.Descricao);
        builder.HasIndex(m => new { m.Uf, m.Nome });
    }
}

/// <summary>Empresas: CNPJ e CPF únicos quando preenchidos.</summary>
internal sealed class ConfiguracaoEmpresa : IEntityTypeConfiguration<Empresa>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresas");
        builder.Property(e => e.RazaoSocial).HasMaxLength(60).IsRequired();
        builder.Property(e => e.NomeFantasia).HasMaxLength(60).IsRequired();
        builder.Property(e => e.Cnpj).HasMaxLength(14).IsRequired();
        builder.Property(e => e.Cpf).HasMaxLength(11).IsRequired();
        builder.Property(e => e.Ie).HasMaxLength(14).IsRequired();
        builder.Property(e => e.IeSt).HasMaxLength(14).IsRequired();
        builder.Property(e => e.Rntrc).HasMaxLength(8).IsRequired();
        builder.Property(e => e.Fone).HasMaxLength(14).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(60).IsRequired();
        builder.Ignore(e => e.Documento);
        builder.OwnsOne(e => e.Endereco, ConfiguracaoEndereco.Mapear);
        builder.Navigation(e => e.Endereco).IsRequired();
        builder.HasIndex(e => e.Cnpj).IsUnique().HasFilter("\"Cnpj\" <> ''");
        builder.HasIndex(e => e.Cpf).IsUnique().HasFilter("\"Cpf\" <> ''");
    }
}

/// <summary>Certificados: o mesmo certificado (thumbprint) não se repete na empresa.</summary>
internal sealed class ConfiguracaoCertificado : ConfiguracaoDaEmpresa<CertificadoDigital>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<CertificadoDigital> builder)
    {
        builder.ToTable("Certificados");
        builder.Property(c => c.Thumbprint).HasMaxLength(40).IsRequired();
        builder.Property(c => c.Titular).HasMaxLength(200).IsRequired();
        builder.Property(c => c.CnpjTitular).HasMaxLength(14).IsRequired();
        builder.HasIndex(c => new { c.EmpresaId, c.Thumbprint }).IsUnique();
    }
}

/// <summary>Séries: chave única por empresa, modelo, série e ambiente (§5.3).</summary>
internal sealed class ConfiguracaoSerie : ConfiguracaoDaEmpresa<SerieDocumento>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<SerieDocumento> builder)
    {
        builder.ToTable("Series");
        builder.Property(s => s.Modelo).IsRequired();
        builder.HasIndex(s => new { s.EmpresaId, s.Modelo, s.Serie, s.Ambiente }).IsUnique();
    }
}

/// <summary>
/// Participantes: o CNPJ/CPF não é único, porque o mesmo documento pode ter mais de uma inscrição estadual
/// (ex.: produtor rural com várias propriedades).
/// </summary>
internal sealed class ConfiguracaoParticipante : ConfiguracaoDaEmpresa<Participante>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<Participante> builder)
    {
        builder.ToTable("Participantes");
        builder.Property(p => p.CnpjCpf).HasMaxLength(14).IsRequired();
        builder.Property(p => p.Ie).HasMaxLength(14).IsRequired();
        builder.Property(p => p.RazaoSocial).HasMaxLength(60).IsRequired();
        builder.Property(p => p.NomeFantasia).HasMaxLength(60).IsRequired();
        builder.Property(p => p.Fone).HasMaxLength(14).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(60).IsRequired();
        builder.OwnsOne(p => p.Endereco, ConfiguracaoEndereco.Mapear);
        builder.Navigation(p => p.Endereco).IsRequired();
        builder.HasIndex(p => new { p.EmpresaId, p.CnpjCpf });
    }
}

/// <summary>Proprietários de veículos de terceiros.</summary>
internal sealed class ConfiguracaoProprietario : ConfiguracaoDaEmpresa<ProprietarioVeiculo>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<ProprietarioVeiculo> builder)
    {
        builder.ToTable("ProprietariosVeiculo");
        builder.Property(p => p.CnpjCpf).HasMaxLength(14).IsRequired();
        builder.Property(p => p.Rntrc).HasMaxLength(8).IsRequired();
        builder.Property(p => p.Nome).HasMaxLength(60).IsRequired();
        builder.Property(p => p.TipoProprietario).IsRequired();
        builder.Property(p => p.Ie).HasMaxLength(14).IsRequired();
        builder.Property(p => p.Uf).HasMaxLength(2).IsRequired();
        builder.HasIndex(p => new { p.EmpresaId, p.CnpjCpf }).IsUnique();
    }
}

/// <summary>Veículos: placa única na empresa; proprietário vinculado não pode ser excluído.</summary>
internal sealed class ConfiguracaoVeiculo : ConfiguracaoDaEmpresa<Veiculo>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculos");
        builder.Property(v => v.Tipo).IsRequired();
        builder.Property(v => v.CodigoInterno).HasMaxLength(10).IsRequired();
        builder.Property(v => v.Placa).HasMaxLength(7).IsRequired();
        builder.Property(v => v.Renavam).HasMaxLength(11).IsRequired();
        builder.Property(v => v.TipoCarroceria).IsRequired();
        builder.Property(v => v.Uf).HasMaxLength(2).IsRequired();
        builder.HasOne<ProprietarioVeiculo>().WithMany().HasForeignKey(v => v.ProprietarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(v => new { v.EmpresaId, v.Placa }).IsUnique();
    }
}

/// <summary>Motoristas: CPF único na empresa.</summary>
internal sealed class ConfiguracaoMotorista : ConfiguracaoDaEmpresa<Motorista>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<Motorista> builder)
    {
        builder.ToTable("Motoristas");
        builder.Property(m => m.Nome).HasMaxLength(60).IsRequired();
        builder.Property(m => m.Cpf).HasMaxLength(11).IsRequired();
        builder.Property(m => m.Telefone).HasMaxLength(14).IsRequired();
        builder.HasIndex(m => new { m.EmpresaId, m.Cpf }).IsUnique();
    }
}

/// <summary>Apólices de seguro: número da apólice único por seguradora na empresa.</summary>
internal sealed class ConfiguracaoApolice : ConfiguracaoDaEmpresa<ApoliceSeguro>
{
    /// <inheritdoc />
    protected override void ConfigurarCampos(EntityTypeBuilder<ApoliceSeguro> builder)
    {
        builder.ToTable("Apolices");
        builder.Property(a => a.Responsavel).IsRequired();
        builder.Property(a => a.CnpjCpfResponsavel).HasMaxLength(14).IsRequired();
        builder.Property(a => a.NomeSeguradora).HasMaxLength(30).IsRequired();
        builder.Property(a => a.CnpjSeguradora).HasMaxLength(14).IsRequired();
        builder.Property(a => a.NumeroApolice).HasMaxLength(20).IsRequired();
        builder.HasIndex(a => new { a.EmpresaId, a.CnpjSeguradora, a.NumeroApolice }).IsUnique();
    }
}
