using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Emissor.Dados;

/// <summary>
/// Local e criação do banco SQLite. O padrão é <c>C:\ProgramData\Cte1\emissor.db</c>, compartilhado pelos usuários
/// Windows da máquina; o caminho poderá ser trocado pelo <c>appsettings.json</c> do aplicativo (fatia P2).
/// </summary>
public static class ConfiguracaoBanco
{
    /// <summary>Caminho padrão do banco.</summary>
    public static string CaminhoPadrao { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cte1", "emissor.db");

    /// <summary>Opções do EF Core para o banco SQLite no caminho informado.</summary>
    /// <param name="caminho">Arquivo do banco.</param>
    public static DbContextOptions<EmissorDbContext> Opcoes(string caminho) =>
        new DbContextOptionsBuilder<EmissorDbContext>().UseSqlite($"Data Source={caminho}").Options;

    /// <summary>Cria a pasta, se preciso, e aplica as migrações pendentes.</summary>
    /// <param name="caminho">Arquivo do banco.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task PrepararAsync(string caminho, CancellationToken cancelamento = default)
    {
        var pasta = Path.GetDirectoryName(Path.GetFullPath(caminho));
        if (!string.IsNullOrEmpty(pasta))
        {
            Directory.CreateDirectory(pasta);
        }

        await using var contexto = new EmissorDbContext(Opcoes(caminho));
        await contexto.Database.MigrateAsync(cancelamento);
    }
}

/// <summary>
/// Usado só pela ferramenta <c>dotnet ef</c> para gerar migrações; não abre o banco real.
/// </summary>
internal sealed class FabricaContextoDesign : IDesignTimeDbContextFactory<EmissorDbContext>
{
    /// <inheritdoc />
    public EmissorDbContext CreateDbContext(string[] args) => new(ConfiguracaoBanco.Opcoes("design.db"));
}
