namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Mensagens padronizadas das telas de cadastro.</summary>
internal static class AvisosCadastro
{
    /// <summary>
    /// Se houver campos obrigatórios não preenchidos, avisa o usuário com a lista e retorna verdadeiro.
    /// </summary>
    /// <param name="dono">Janela que exibe o aviso.</param>
    /// <param name="titulo">Título da mensagem.</param>
    /// <param name="faltando">Campos não preenchidos.</param>
    public static bool HaPendencias(IWin32Window dono, string titulo, IReadOnlyList<string> faltando)
    {
        if (faltando.Count == 0)
        {
            return false;
        }

        var lista = string.Join(Environment.NewLine, faltando.Select(campo => $"• {campo}"));
        MessageBox.Show(dono, $"Preencha os campos obrigatórios:{Environment.NewLine}{Environment.NewLine}{lista}",
            titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return true;
    }
}
