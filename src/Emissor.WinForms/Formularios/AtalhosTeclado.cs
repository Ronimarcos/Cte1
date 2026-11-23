namespace Emissor.WinForms.Formularios;

/// <summary>
/// Teclas de atalho do sistema: o texto de ajuda exibido com F1 e a configuração comum dos diálogos
/// (Ctrl+S salva, F1 mostra a ajuda). Os atalhos dos menus ficam no <see cref="FormPrincipal"/>.
/// </summary>
internal static class AtalhosTeclado
{
    /// <summary>Lista de atalhos exibida pelo F1.</summary>
    public const string TextoAjuda =
        "MENUS (em qualquer tela da janela principal)\n" +
        "  Ctrl+T\tEmitir CT-e\n" +
        "  Ctrl+Shift+T\tConsultar CT-e\n" +
        "  Ctrl+M\tEmitir MDF-e\n" +
        "  Ctrl+Shift+M\tConsultar MDF-e\n" +
        "  Ctrl+Shift+P\tParticipantes\n" +
        "  Ctrl+Shift+V\tVeículos\n" +
        "  Ctrl+Shift+O\tMotoristas\n" +
        "  Ctrl+Shift+E\tTrocar empresa\n" +
        "  Alt+F4\tSair\n" +
        "  Alt + letra sublinhada\tAbre o menu ou aciona o botão\n" +
        "\n" +
        "LISTAS (cadastros e consulta de CT-e)\n" +
        "  Insert ou F2\tNovo\n" +
        "  Enter\tEditar / abrir\n" +
        "  Delete\tExcluir\n" +
        "  Ctrl+F ou F3\tIr para a pesquisa\n" +
        "  Esc\tFechar a janela\n" +
        "\n" +
        "TELAS DE EDIÇÃO\n" +
        "  Enter ou Ctrl+S\tSalvar\n" +
        "  Esc\tCancelar\n" +
        "\n" +
        "ASSISTENTE DE CT-e\n" +
        "  Ctrl+→ / Ctrl+←\tAvançar / voltar etapa\n" +
        "  Ctrl+S\tSalvar rascunho\n" +
        "  Ctrl+Enter\tConcluir (última etapa)\n" +
        "  Esc\tCancelar\n" +
        "\n" +
        "  F1\tEsta ajuda";

    /// <summary>Mostra a lista de atalhos.</summary>
    /// <param name="dono">Janela que abre a ajuda.</param>
    public static void MostrarAjuda(IWin32Window? dono) =>
        MessageBox.Show(dono, TextoAjuda, "Teclas de atalho", MessageBoxButtons.OK, MessageBoxIcon.Information);

    /// <summary>
    /// Liga F1 (ajuda) e, quando houver botão de salvar, Ctrl+S, para funcionar com o foco em qualquer campo,
    /// inclusive nos de várias linhas, onde o Enter quebra a linha.
    /// </summary>
    /// <param name="dialogo">Diálogo a configurar.</param>
    /// <param name="salvar">Botão acionado pelo Ctrl+S, ou <c>null</c> para só a ajuda.</param>
    public static void ConfigurarDialogo(Form dialogo, Button? salvar)
    {
        dialogo.KeyPreview = true;
        dialogo.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.F1 && e.Modifiers == Keys.None)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                MostrarAjuda(dialogo);
            }
            else if (salvar is not null && e.KeyData == (Keys.Control | Keys.S))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                salvar.PerformClick();
            }
        };
    }
}
