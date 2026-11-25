namespace Emissor.WinForms.Formularios;

/// <summary>
/// Janela de espera usada antes da janela principal existir (preparar o banco, carregar municípios, ler empresas).
/// Executa o trabalho assíncrono sem travar a tela e mostra o passo atual.
/// </summary>
public partial class FormAguarde : Form
{
    private readonly Func<IProgress<string>, Task> _trabalho;

    private FormAguarde(string mensagem, Func<IProgress<string>, Task> trabalho)
    {
        InitializeComponent();
        _trabalho = trabalho;
        lblMensagem.Text = mensagem;
    }

    /// <summary>Erro ocorrido no trabalho, se houver.</summary>
    private Exception? Erro { get; set; }

    /// <summary>
    /// Executa o trabalho mostrando a janela de espera. Em caso de erro, avisa o usuário e retorna falso.
    /// </summary>
    /// <param name="mensagem">Texto inicial.</param>
    /// <param name="trabalho">Trabalho a executar; pode informar o passo atual pelo <see cref="IProgress{T}"/>.</param>
    public static bool Executar(string mensagem, Func<IProgress<string>, Task> trabalho)
    {
        using var tela = new FormAguarde(mensagem, trabalho);
        tela.ShowDialog();
        if (tela.Erro is null)
        {
            return true;
        }

        MessageBox.Show(
            $"Não foi possível concluir: {mensagem}{Environment.NewLine}{Environment.NewLine}{tela.Erro.GetBaseException().Message}",
            "Emissor CT-e / MDF-e", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }

    /// <summary>Executa o trabalho e devolve o resultado; <c>null</c> se houve erro.</summary>
    /// <typeparam name="T">Tipo do resultado.</typeparam>
    /// <param name="mensagem">Texto inicial.</param>
    /// <param name="trabalho">Trabalho a executar.</param>
    public static T? Executar<T>(string mensagem, Func<Task<T>> trabalho) where T : class
    {
        T? resultado = null;
        return Executar(mensagem, async _ => resultado = await trabalho()) ? resultado : null;
    }

    /// <summary>Começa o trabalho quando a janela aparece e fecha ao terminar (com ou sem erro).</summary>
    private async void FormAguarde_Shown(object? sender, EventArgs e)
    {
        var progresso = new Progress<string>(passo => lblMensagem.Text = passo);
        try
        {
            await _trabalho(progresso);
        }
        catch (Exception erro)
        {
            Erro = erro;
        }

        Close();
    }
}
