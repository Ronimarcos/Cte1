using Emissor.Dados;
using Emissor.Dominio.Entidades;
using Emissor.WinForms.Formularios;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Emissor.WinForms;

static class Program
{
    /// <summary>
    /// Ponto de entrada: lê a configuração, prepara o banco (migrações e municípios), oferece os dados de
    /// demonstração quando não há empresas, pede a empresa de trabalho e abre a janela principal.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Configurações de DPI e fonte padrão: ver https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var caminhoBanco = LerCaminhoBanco();
        using var provedor = ConfigurarServicos(caminhoBanco);
        var empresas = provedor.GetRequiredService<ServicoEmpresas>();
        var municipios = provedor.GetRequiredService<CatalogoMunicipios>();

        var preparado = FormAguarde.Executar("Preparando o banco de dados...", async progresso =>
        {
            await ConfiguracaoBanco.PrepararAsync(caminhoBanco);
            progresso.Report("Carregando os municípios do IBGE...");
            await using (var contexto = await provedor.GetRequiredService<IDbContextFactory<EmissorDbContext>>().CreateDbContextAsync())
            {
                await CargaMunicipios.CarregarSeVazioAsync(contexto);
            }

            await municipios.CarregarAsync();
        });
        if (!preparado || !GarantirEmpresa(empresas, municipios))
        {
            return;
        }

        var empresa = EscolherEmpresa(empresas);
        if (empresa is null)
        {
            return;
        }

        Application.Run(new FormPrincipal(empresa, empresas, municipios));
    }

    /// <summary>Caminho do banco no <c>appsettings.json</c>; vazio usa o padrão (<c>C:\ProgramData\Cte1\emissor.db</c>).</summary>
    private static string LerCaminhoBanco()
    {
        var configuracao = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        var caminho = configuracao["Banco:Caminho"];
        return string.IsNullOrWhiteSpace(caminho) ? ConfiguracaoBanco.CaminhoPadrao : Environment.ExpandEnvironmentVariables(caminho);
    }

    /// <summary>Serviços do aplicativo: fábrica de contextos do banco (um contexto por operação) e serviços das telas.</summary>
    private static ServiceProvider ConfigurarServicos(string caminhoBanco)
    {
        var servicos = new ServiceCollection();
        servicos.AddDbContextFactory<EmissorDbContext>(opcoes => opcoes.UseSqlite($"Data Source={caminhoBanco}"));
        servicos.AddSingleton<CatalogoMunicipios>();
        servicos.AddSingleton<ServicoEmpresas>();
        return servicos.BuildServiceProvider();
    }

    /// <summary>
    /// Com o banco sem empresas, oferece os dados de demonstração; se o usuário recusar, abre o cadastro da
    /// primeira empresa. Retorna falso se nenhuma empresa existir ao final.
    /// </summary>
    private static bool GarantirEmpresa(ServicoEmpresas empresas, CatalogoMunicipios municipios)
    {
        var semEmpresas = FormAguarde.Executar("Verificando as empresas...", async () => (object)await empresas.SemEmpresasAsync());
        if (semEmpresas is not true)
        {
            return semEmpresas is false;
        }

        var resposta = MessageBox.Show(
            "Ainda não há empresas cadastradas." + Environment.NewLine + Environment.NewLine +
            "Deseja carregar os dados de demonstração (empresas fictícias ALFA, BETA e GAMA)?" + Environment.NewLine +
            "Escolha \"Não\" para cadastrar a sua empresa agora.",
            "Emissor CT-e / MDF-e", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

        if (resposta == DialogResult.Yes)
        {
            return FormAguarde.Executar("Carregando os dados de demonstração...", _ => empresas.CarregarDemonstracaoAsync());
        }

        if (resposta == DialogResult.No)
        {
            using var tela = new FormEdicaoEmpresa(new Empresa(), novo: true, municipios, empresas.IncluirAsync);
            return tela.ShowDialog() == DialogResult.OK;
        }

        return false;
    }

    /// <summary>Mostra a seleção de empresa e devolve a escolhida, lida do banco; <c>null</c> se o usuário desistir.</summary>
    private static Empresa? EscolherEmpresa(ServicoEmpresas empresas)
    {
        var lista = FormAguarde.Executar("Lendo as empresas...", async () => await empresas.ListarResumosAsync());
        if (lista is null)
        {
            return null;
        }

        int? escolhida;
        using (var selecao = new FormSelecaoEmpresa(lista))
        {
            if (selecao.ShowDialog() != DialogResult.OK || selecao.EmpresaSelecionada is null)
            {
                return null;
            }

            escolhida = selecao.EmpresaSelecionada.Id;
        }

        return FormAguarde.Executar("Abrindo a empresa...", async () =>
            await empresas.ObterAsync(escolhida.Value) ?? throw new InvalidOperationException("A empresa escolhida não existe mais."));
    }
}
