using Emissor.Dominio.Regras;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// Endereço do emitente e dos participantes (<c>enderEmit</c>, <c>enderReme</c> etc.).
/// Gravado nas mesmas tabelas das entidades (tipo próprio do EF Core).
/// </summary>
public class Endereco
{
    /// <summary>Logradouro (<c>xLgr</c>).</summary>
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>Número (<c>nro</c>).</summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>Complemento (<c>xCpl</c>).</summary>
    public string Complemento { get; set; } = string.Empty;

    /// <summary>Bairro (<c>xBairro</c>).</summary>
    public string Bairro { get; set; } = string.Empty;

    /// <summary>Código IBGE do município (<c>cMun</c>).</summary>
    public int? MunicipioCodigo { get; set; }

    /// <summary>CEP só com dígitos (<c>CEP</c>), opcional.</summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>Acrescenta as pendências do endereço à lista da entidade.</summary>
    /// <param name="faltando">Lista onde as pendências são acrescentadas.</param>
    public void Validar(List<string> faltando)
    {
        if (string.IsNullOrWhiteSpace(Logradouro)) faltando.Add("Logradouro");
        if (string.IsNullOrWhiteSpace(Numero)) faltando.Add("Número");
        if (string.IsNullOrWhiteSpace(Bairro)) faltando.Add("Bairro");
        if (MunicipioCodigo is null) faltando.Add("Município");
        if (Cep.Length > 0 && !Formatos.CepValido(Cep)) faltando.Add("CEP com 8 dígitos");
    }
}
