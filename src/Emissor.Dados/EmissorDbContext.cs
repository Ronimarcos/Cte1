using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Contexto do EF Core do emissor (SQLite na v1, PostgreSQL na fase web). As tabelas e regras de cada entidade
/// ficam nas classes da pasta <c>Configuracoes</c>; as datas de inclusão e alteração são preenchidas aqui.
/// </summary>
public class EmissorDbContext : DbContext
{
    /// <summary>Cria o contexto com as opções informadas (provedor e caminho do banco).</summary>
    /// <param name="options">Opções do EF Core.</param>
    public EmissorDbContext(DbContextOptions<EmissorDbContext> options)
        : base(options)
    {
    }

    /// <summary>Municípios (tabela global, código IBGE).</summary>
    public DbSet<Municipio> Municipios => Set<Municipio>();

    /// <summary>Empresas emitentes.</summary>
    public DbSet<Empresa> Empresas => Set<Empresa>();

    /// <summary>Certificados digitais.</summary>
    public DbSet<CertificadoDigital> Certificados => Set<CertificadoDigital>();

    /// <summary>Séries de numeração.</summary>
    public DbSet<SerieDocumento> Series => Set<SerieDocumento>();

    /// <summary>Participantes dos CT-e.</summary>
    public DbSet<Participante> Participantes => Set<Participante>();

    /// <summary>Proprietários de veículos de terceiros.</summary>
    public DbSet<ProprietarioVeiculo> Proprietarios => Set<ProprietarioVeiculo>();

    /// <summary>Veículos (tração e reboque).</summary>
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();

    /// <summary>Motoristas.</summary>
    public DbSet<Motorista> Motoristas => Set<Motorista>();

    /// <summary>Apólices de seguro da carga.</summary>
    public DbSet<ApoliceSeguro> Apolices => Set<ApoliceSeguro>();

    /// <summary>CT-e emitidos.</summary>
    public DbSet<ConhecimentoTransporte> Ctes => Set<ConhecimentoTransporte>();

    /// <summary>MDF-e emitidos.</summary>
    public DbSet<ManifestoEletronico> Mdfes => Set<ManifestoEletronico>();

    /// <summary>Usuários do emissor.</summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>Auditoria (só inclusão; o banco recusa alteração e exclusão).</summary>
    public DbSet<RegistroAuditoria> Auditoria => Set<RegistroAuditoria>();

    /// <summary>Eventos dos documentos (cancelamento, carta de correção, encerramento, inclusão de condutor).</summary>
    public DbSet<EventoDocumento> Eventos => Set<EventoDocumento>();

    /// <summary>
    /// Quem está gravando (informado pelos repositórios). Com usuário logado, toda inclusão, alteração e exclusão vira
    /// linha de auditoria na mesma transação; sem usuário (operações internas), não há auditoria automática.
    /// </summary>
    public IContextoAcesso? Autor { get; set; }

    /// <inheritdoc />
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        PreencherDatas();
        var usuario = Autor?.Usuario;
        var mudancas = usuario is null ? [] : ColetorAuditoria.Coletar(ChangeTracker);
        if (mudancas.Count == 0)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        using var transacao = Database.CurrentTransaction is null ? Database.BeginTransaction() : null;
        var linhas = base.SaveChanges(acceptAllChangesOnSuccess);
        var agora = DateTime.Now;
        Auditoria.AddRange(mudancas.Select(m => m.Criar(usuario!, agora)));
        base.SaveChanges(acceptAllChangesOnSuccess);
        transacao?.Commit();
        return linhas;
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        PreencherDatas();
        var usuario = Autor?.Usuario;
        var mudancas = usuario is null ? [] : ColetorAuditoria.Coletar(ChangeTracker);
        if (mudancas.Count == 0)
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        await using var transacao = Database.CurrentTransaction is null ? await Database.BeginTransactionAsync(cancellationToken) : null;
        var linhas = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        var agora = DateTime.Now;
        Auditoria.AddRange(mudancas.Select(m => m.Criar(usuario!, agora)));
        await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        if (transacao is not null)
        {
            await transacao.CommitAsync(cancellationToken);
        }

        return linhas;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmissorDbContext).Assembly);

    /// <summary>Inclusão preenche as duas datas; alteração atualiza só a de alteração e preserva a de inclusão.</summary>
    private void PreencherDatas()
    {
        var agora = DateTime.Now;
        foreach (var entrada in ChangeTracker.Entries<EntidadeBase>())
        {
            if (entrada.State == EntityState.Added)
            {
                entrada.Entity.CriadoEm = agora;
                entrada.Entity.AlteradoEm = agora;
            }
            else if (entrada.State == EntityState.Modified)
            {
                entrada.Entity.AlteradoEm = agora;
                entrada.Property(e => e.CriadoEm).IsModified = false;
            }
        }
    }
}
