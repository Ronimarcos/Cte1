using Emissor.WinForms.Formularios;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms;

static class Program
{
    /// <summary>
    ///  Ponto de entrada: pede a empresa com que o usuário vai trabalhar e abre a janela principal.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Configurações de DPI e fonte padrão: ver https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        EmpresaPrototipo? empresa;
        using (var selecao = new FormSelecaoEmpresa(DadosFicticios.Empresas.Listar()))
        {
            if (selecao.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            empresa = selecao.EmpresaSelecionada;
        }

        if (empresa is null)
        {
            return;
        }

        Application.Run(new FormPrincipal(empresa));
    }
}
