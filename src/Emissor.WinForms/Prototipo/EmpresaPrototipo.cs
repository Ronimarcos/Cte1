namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Empresa fictícia usada apenas no protótipo das telas.
/// Será substituída pela entidade <c>Empresa</c> do domínio quando a persistência for implementada.
/// </summary>
/// <param name="Id">Identificador interno.</param>
/// <param name="RazaoSocial">Razão social do emitente.</param>
/// <param name="Cnpj">CNPJ formatado para exibição.</param>
/// <param name="Uf">UF do emitente.</param>
/// <param name="Homologacao">Verdadeiro quando o ambiente padrão é homologação (<c>tpAmb</c> 2).</param>
public sealed record EmpresaPrototipo(int Id, string RazaoSocial, string Cnpj, string Uf, bool Homologacao)
{
    /// <summary>Descrição do ambiente para exibição em grades e na barra de status.</summary>
    public string AmbienteDescricao => Homologacao ? "Homologação" : "Produção";
}
