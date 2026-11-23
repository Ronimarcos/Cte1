using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Atalhos para carregar e ler listas de escolha (<see cref="Opcao{T}"/>) em <see cref="ComboBox"/>.</summary>
internal static class ComboBoxExtensoes
{
    /// <summary>Preenche a lista com as opções; o combo fica sem seleção.</summary>
    /// <param name="combo">Lista de escolha.</param>
    /// <param name="opcoes">Opções exibidas.</param>
    public static void Carregar<T>(this ComboBox combo, IEnumerable<Opcao<T>> opcoes)
    {
        combo.BeginUpdate();
        combo.Items.Clear();
        foreach (var opcao in opcoes)
        {
            combo.Items.Add(opcao);
        }

        combo.SelectedIndex = -1;
        combo.EndUpdate();
    }

    /// <summary>Seleciona a opção com o valor informado; sem valor, deixa o combo sem seleção.</summary>
    /// <param name="combo">Lista de escolha.</param>
    /// <param name="valor">Valor a selecionar.</param>
    public static void Selecionar<T>(this ComboBox combo, T? valor) where T : struct
    {
        combo.SelectedIndex = -1;
        if (valor is null)
        {
            return;
        }

        for (var i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is Opcao<T> opcao && opcao.Valor.Equals(valor.Value))
            {
                combo.SelectedIndex = i;
                return;
            }
        }
    }

    /// <summary>Valor da opção selecionada, ou <c>null</c> se não houver seleção.</summary>
    /// <param name="combo">Lista de escolha.</param>
    public static T? ValorSelecionado<T>(this ComboBox combo) where T : struct =>
        combo.SelectedItem is Opcao<T> opcao ? opcao.Valor : null;
}
