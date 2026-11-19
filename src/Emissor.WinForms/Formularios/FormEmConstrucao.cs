namespace Emissor.WinForms.Formularios;

/// <summary>
/// Tela provisória do protótipo: ocupa o lugar das telas que ainda serão desenhadas.
/// </summary>
public partial class FormEmConstrucao : Form
{
    /// <summary>Cria a tela provisória com o título da opção de menu que a abriu.</summary>
    /// <param name="titulo">Título exibido na janela e no cabeçalho.</param>
    public FormEmConstrucao(string titulo)
    {
        InitializeComponent();
        Titulo = titulo;
        Text = titulo;
        lblTitulo.Text = titulo;
    }

    /// <summary>Título da tela; usado pela janela principal para não abrir a mesma tela duas vezes.</summary>
    public string Titulo { get; }
}
