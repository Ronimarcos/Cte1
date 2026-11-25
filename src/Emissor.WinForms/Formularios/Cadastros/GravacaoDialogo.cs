using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Passo comum dos diálogos de edição gravados no banco: valida pelas regras do domínio, grava pelo delegado e,
/// se o banco recusar, mostra o motivo e mantém o diálogo aberto.
/// </summary>
internal static class GravacaoDialogo
{
    /// <summary>Valida e grava; retorna verdadeiro se gravou.</summary>
    /// <typeparam name="T">Entidade.</typeparam>
    /// <param name="dialogo">Diálogo de edição.</param>
    /// <param name="botaoSalvar">Botão desabilitado durante a gravação.</param>
    /// <param name="registro">Registro já lido dos campos.</param>
    /// <param name="gravar">Inclui ou altera no banco.</param>
    public static async Task<bool> GravarAsync<T>(Form dialogo, Button botaoSalvar, T registro, Func<T, Task> gravar) where T : EntidadeBase
    {
        if (AvisosCadastro.HaPendencias(dialogo, dialogo.Text, registro.Validar()))
        {
            return false;
        }

        botaoSalvar.Enabled = false;
        dialogo.UseWaitCursor = true;
        try
        {
            await gravar(registro);
            return true;
        }
        catch (RegraNegocioException erro)
        {
            AvisosCadastro.HaPendencias(dialogo, dialogo.Text, erro.Pendencias);
            return false;
        }
        finally
        {
            dialogo.UseWaitCursor = false;
            botaoSalvar.Enabled = true;
        }
    }
}
