using System.Security.Cryptography;
using System.Text;
using Emissor.Dados;
using Emissor.DFe.Certificados;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Testes;

/// <summary>
/// Proteção local (fatia A3): banco na pasta do usuário com cópia do banco antigo e certificados A1 passados da DPAPI
/// da máquina para a DPAPI do usuário do Windows.
/// </summary>
public sealed class ProtecaoLocalTests : IDisposable
{
    private static readonly byte[] EntropiaAntiga = Encoding.UTF8.GetBytes("Cte1.Emissor.Certificados.v1");

    private readonly string _pasta = Path.Combine(Path.GetTempPath(), "Cte1Testes", Guid.NewGuid().ToString("N"));

    public ProtecaoLocalTests() => Directory.CreateDirectory(_pasta);

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        Directory.Delete(_pasta, recursive: true);
    }

    [Fact]
    public void CaminhoPadrao_FicaNaPastaDoUsuario()
    {
        var pastaUsuario = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        Assert.StartsWith(pastaUsuario, ConfiguracaoBanco.CaminhoPadrao, StringComparison.OrdinalIgnoreCase);
        Assert.NotEqual(ConfiguracaoBanco.CaminhoAntigo, ConfiguracaoBanco.CaminhoPadrao, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CopiaDoBancoAntigo_PreservaOsDadosEOriginal()
    {
        var antigo = Path.Combine(_pasta, "antigo", "emissor.db");
        var novo = Path.Combine(_pasta, "novo", "emissor.db");
        await ConfiguracaoBanco.PrepararAsync(antigo);
        await using (var contexto = new EmissorDbContext(ConfiguracaoBanco.Opcoes(antigo)))
        {
            await CargaMunicipios.CarregarSeVazioAsync(contexto);
            await DadosDemonstracao.CarregarAsync(contexto);
        }

        Assert.True(ConfiguracaoBanco.HaBancoAntigoParaCopiar(novo, antigo));
        await ConfiguracaoBanco.CopiarBancoAsync(antigo, novo);

        Assert.False(ConfiguracaoBanco.HaBancoAntigoParaCopiar(novo, antigo));
        Assert.False(File.Exists(novo + ".copiando"));
        Assert.True(File.Exists(antigo));
        await using (var contexto = new EmissorDbContext(ConfiguracaoBanco.Opcoes(novo)))
        {
            Assert.Equal(3, await contexto.Empresas.CountAsync());
            Assert.True(await contexto.Participantes.AnyAsync());
        }

        await Assert.ThrowsAsync<IOException>(() => ConfiguracaoBanco.CopiarBancoAsync(antigo, novo));

        ConfiguracaoBanco.ExcluirBanco(antigo);
        Assert.False(File.Exists(antigo));
    }

    [Fact]
    public void SemBancoAntigo_NaoOfereceCopia()
    {
        Assert.False(ConfiguracaoBanco.HaBancoAntigoParaCopiar(Path.Combine(_pasta, "novo.db"), Path.Combine(_pasta, "inexistente.db")));
    }

    [Fact]
    public void ProtecaoNova_NaoAbreComAProtecaoDaMaquina()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var dados = Encoding.UTF8.GetBytes("segredo");
        var protegido = new ProtetorDpapi().Proteger(dados);

        Assert.Equal(dados, new ProtetorDpapi().Desproteger(protegido));
        Assert.ThrowsAny<CryptographicException>(() => new ProtetorDpapiMaquina().Desproteger(protegido));
        Assert.Throws<InvalidOperationException>(() => new ProtetorDpapiMaquina().Proteger(dados));
    }

    [Fact]
    public async Task Recriptografia_ConverteOsAntigosUmaVezEListaOsQueNaoAbrem()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var caminho = Path.Combine(_pasta, "emissor.db");
        await ConfiguracaoBanco.PrepararAsync(caminho);
        await using var contexto = new EmissorDbContext(ConfiguracaoBanco.Opcoes(caminho));
        await CargaMunicipios.CarregarSeVazioAsync(contexto);
        await DadosDemonstracao.CarregarAsync(contexto);

        var pfx = Encoding.UTF8.GetBytes("conteudo do pfx");
        var senha = Encoding.UTF8.GetBytes("senha do pfx");
        var atual = new ProtetorDpapi();
        var antigo = Certificado("ANTIGO", Antiga(pfx), Antiga(senha));
        var novo = Certificado("NOVO", atual.Proteger(pfx), atual.Proteger(senha));
        var corrompido = Certificado("CORROMPIDO", [1, 2, 3], [4, 5, 6]);
        contexto.Certificados.AddRange(antigo, novo, corrompido);
        await contexto.SaveChangesAsync();
        var novoAntes = novo.ArquivoPfxCriptografado;

        var resultado = await RecriptografiaCertificados.ExecutarAsync(contexto, atual, new ProtetorDpapiMaquina());

        Assert.Equal(1, resultado.Convertidos);
        Assert.Equal("CORROMPIDO", Assert.Single(resultado.Falhas).Titular);
        Assert.Equal(pfx, atual.Desproteger(antigo.ArquivoPfxCriptografado!));
        Assert.Equal(senha, atual.Desproteger(antigo.SenhaCriptografada!));
        Assert.Same(novoAntes, novo.ArquivoPfxCriptografado);

        var deNovo = await RecriptografiaCertificados.ExecutarAsync(contexto, atual, new ProtetorDpapiMaquina());
        Assert.Equal(0, deNovo.Convertidos);
        Assert.Single(deNovo.Falhas);
        Assert.Equal(0, await contexto.Auditoria.CountAsync());
    }

    private static byte[] Antiga(byte[] dados) =>
        OperatingSystem.IsWindows() ? ProtectedData.Protect(dados, EntropiaAntiga, DataProtectionScope.LocalMachine) : dados;

    private static CertificadoDigital Certificado(string titular, byte[] pfx, byte[] senha) => new()
    {
        EmpresaId = 1,
        Tipo = TipoCertificado.A1,
        Titular = titular,
        CnpjTitular = "11111111000111",
        Thumbprint = Guid.NewGuid().ToString("N"),
        ValidoDe = DateTime.Today.AddDays(-1),
        ValidoAte = DateTime.Today.AddYears(1),
        ArquivoPfxCriptografado = pfx,
        SenhaCriptografada = senha,
    };
}
