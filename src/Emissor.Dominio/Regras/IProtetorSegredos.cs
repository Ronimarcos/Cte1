namespace Emissor.Dominio.Regras;

/// <summary>
/// Protege segredos antes de gravá-los no banco (arquivo e senha do certificado A1). Na versão desktop a
/// implementação usa a proteção de dados do Windows (DPAPI); a versão web poderá usar outra, sem mudar o resto.
/// </summary>
public interface IProtetorSegredos
{
    /// <summary>Criptografa os dados.</summary>
    /// <param name="dados">Dados abertos.</param>
    byte[] Proteger(byte[] dados);

    /// <summary>Descriptografa dados protegidos por <see cref="Proteger"/>.</summary>
    /// <param name="protegidos">Dados criptografados.</param>
    byte[] Desproteger(byte[] protegidos);
}
