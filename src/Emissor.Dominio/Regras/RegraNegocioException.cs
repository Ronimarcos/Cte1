namespace Emissor.Dominio.Regras;

/// <summary>
/// Indica que o registro não pode ser gravado. Traz a lista de pendências para a tela exibir ao usuário.
/// </summary>
public class RegraNegocioException : Exception
{
    /// <summary>Cria a exceção com as pendências encontradas.</summary>
    /// <param name="pendencias">O que precisa ser corrigido.</param>
    public RegraNegocioException(IReadOnlyList<string> pendencias)
        : base("Há pendências no registro: " + string.Join("; ", pendencias)) => Pendencias = pendencias;

    /// <summary>Cria a exceção com uma única mensagem.</summary>
    /// <param name="mensagem">O que precisa ser corrigido.</param>
    public RegraNegocioException(string mensagem)
        : this([mensagem])
    {
    }

    /// <summary>O que precisa ser corrigido.</summary>
    public IReadOnlyList<string> Pendencias { get; }
}
