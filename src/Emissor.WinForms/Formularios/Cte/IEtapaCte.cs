using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Etapa do assistente de emissão do CT-e (<see cref="FormEmissaoCte"/>).</summary>
internal interface IEtapaCte
{
    /// <summary>Título exibido na lista de etapas e no cabeçalho.</summary>
    string Titulo { get; }

    /// <summary>Preenche os campos da etapa a partir do CT-e; chamado uma vez, ao abrir o assistente.</summary>
    /// <param name="cte">CT-e em edição.</param>
    /// <param name="empresa">Empresa emitente.</param>
    void Carregar(CtePrototipo cte, EmpresaPrototipo empresa);

    /// <summary>Chamado sempre que a etapa passa a ser exibida (ex.: ajustar campos ao tipo do CT-e).</summary>
    /// <param name="cte">CT-e em edição, com as etapas anteriores já lidas.</param>
    void AoEntrar(CtePrototipo cte);

    /// <summary>Copia os campos da etapa para o CT-e.</summary>
    /// <param name="cte">CT-e em edição.</param>
    void Ler(CtePrototipo cte);

    /// <summary>Pendências que impedem avançar desta etapa.</summary>
    /// <param name="cte">CT-e em edição, já lido.</param>
    IReadOnlyList<string> Validar(CtePrototipo cte);
}
