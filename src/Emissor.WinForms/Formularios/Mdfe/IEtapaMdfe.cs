using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>Etapa do assistente de emissão do MDF-e (<see cref="FormEmissaoMdfe"/>).</summary>
internal interface IEtapaMdfe
{
    /// <summary>Título exibido na lista de etapas e no cabeçalho.</summary>
    string Titulo { get; }

    /// <summary>Preenche os campos da etapa a partir do MDF-e; chamado uma vez, ao abrir o assistente.</summary>
    /// <param name="mdfe">MDF-e em edição.</param>
    /// <param name="empresa">Empresa emitente.</param>
    void Carregar(MdfePrototipo mdfe, EmpresaPrototipo empresa);

    /// <summary>Chamado sempre que a etapa passa a ser exibida (ex.: ajustar campos ao tipo do emitente).</summary>
    /// <param name="mdfe">MDF-e em edição, com as etapas anteriores já lidas.</param>
    void AoEntrar(MdfePrototipo mdfe);

    /// <summary>Copia os campos da etapa para o MDF-e.</summary>
    /// <param name="mdfe">MDF-e em edição.</param>
    void Ler(MdfePrototipo mdfe);

    /// <summary>Pendências que impedem avançar desta etapa.</summary>
    /// <param name="mdfe">MDF-e em edição, já lido.</param>
    IReadOnlyList<string> Validar(MdfePrototipo mdfe);
}
