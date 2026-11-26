using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Emissor.Dados;

/// <summary>
/// Local e criação do banco SQLite. O padrão é <c>%LOCALAPPDATA%\Cte1\emissor.db</c>, na pasta do usuário do Windows
/// (outros usuários da máquina não leem o arquivo); o caminho pode ser trocado pelo <c>appsettings.json</c> do
/// aplicativo. Até a fatia A3 o padrão era <c>C:\ProgramData\Cte1\emissor.db</c>, compartilhado pela máquina: o
/// aplicativo oferece copiar esse banco antigo na primeira abertura.
/// </summary>
public static class ConfiguracaoBanco
{
    /// <summary>Caminho padrão do banco, na pasta local do usuário do Windows.</summary>
    public static string CaminhoPadrao { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cte1", "emissor.db");

    /// <summary>Caminho padrão antigo (até a fatia A3), compartilhado pelos usuários do Windows da máquina.</summary>
    public static string CaminhoAntigo { get; } =
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
        CriarPasta(caminho);
        await using var contexto = new EmissorDbContext(Opcoes(caminho));
        await contexto.Database.MigrateAsync(cancelamento);
    }

    /// <summary>
    /// Verdadeiro quando vale oferecer a cópia do banco antigo: o banco do caminho informado ainda não existe e o
    /// banco antigo (<see cref="CaminhoAntigo"/>, ou o informado) existe.
    /// </summary>
    /// <param name="caminho">Banco que o aplicativo vai abrir.</param>
    /// <param name="antigo">Banco antigo; vazio usa <see cref="CaminhoAntigo"/>.</param>
    public static bool HaBancoAntigoParaCopiar(string caminho, string? antigo = null)
    {
        antigo ??= CaminhoAntigo;
        return !File.Exists(caminho) && File.Exists(antigo)
            && !string.Equals(Path.GetFullPath(caminho), Path.GetFullPath(antigo), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Copia um banco SQLite pela cópia de segurança do próprio SQLite (inclui o que estiver pendente no arquivo
    /// <c>-wal</c>). Grava num arquivo temporário e só renomeia para o destino no fim, para não deixar banco pela
    /// metade. O original não é alterado.
    /// </summary>
    /// <param name="origem">Banco existente.</param>
    /// <param name="destino">Banco novo (não pode existir).</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task CopiarBancoAsync(string origem, string destino, CancellationToken cancelamento = default)
    {
        if (!File.Exists(origem))
        {
            throw new FileNotFoundException("O banco de origem não existe.", origem);
        }

        if (File.Exists(destino))
        {
            throw new IOException($"Já existe um banco em {destino}.");
        }

        CriarPasta(destino);
        var temporario = destino + ".copiando";
        File.Delete(temporario);
        await Task.Run(() =>
        {
            using var conexaoOrigem = new SqliteConnection(Conexao(origem, SqliteOpenMode.ReadWrite));
            using var conexaoDestino = new SqliteConnection(Conexao(temporario, SqliteOpenMode.ReadWriteCreate));
            conexaoOrigem.Open();
            conexaoDestino.Open();
            conexaoOrigem.BackupDatabase(conexaoDestino);
        }, cancelamento);
        File.Move(temporario, destino);
    }

    /// <summary>Exclui o arquivo do banco e os arquivos auxiliares do SQLite (<c>-wal</c> e <c>-shm</c>).</summary>
    /// <param name="caminho">Arquivo do banco.</param>
    public static void ExcluirBanco(string caminho)
    {
        SqliteConnection.ClearAllPools();
        foreach (var arquivo in new[] { caminho, caminho + "-wal", caminho + "-shm" })
        {
            File.Delete(arquivo);
        }
    }

    private static string Conexao(string caminho, SqliteOpenMode modo) =>
        new SqliteConnectionStringBuilder { DataSource = caminho, Mode = modo, Pooling = false }.ToString();

    private static void CriarPasta(string caminho)
    {
        var pasta = Path.GetDirectoryName(Path.GetFullPath(caminho));
        if (!string.IsNullOrEmpty(pasta))
        {
            Directory.CreateDirectory(pasta);
        }
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
