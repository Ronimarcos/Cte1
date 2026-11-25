using Emissor.Dominio.Enums;
using Emissor.Dominio.Entidades;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>Etapa do assistente de emissão do CT-e (<see cref="FormEmissaoCte"/>).</summary>
internal interface IEtapaCte
{
    /// <summary>Título exibido na lista de etapas e no cabeçalho.</summary>
    string Titulo { get; }

    /// <summary>Preenche os campos da etapa a partir do CT-e; chamado uma vez, ao abrir o assistente.</summary>
    /// <param name="cte">CT-e em edição.</param>
    /// <param name="cadastros">Empresa emitente e cadastros lidos do banco.</param>
    void Carregar(ConhecimentoTransporte cte, CadastrosEmissao cadastros);

    /// <summary>Chamado sempre que a etapa passa a ser exibida (ex.: ajustar campos ao tipo do CT-e).</summary>
    /// <param name="cte">CT-e em edição, com as etapas anteriores já lidas.</param>
    void AoEntrar(ConhecimentoTransporte cte);

    /// <summary>Copia os campos da etapa para o CT-e.</summary>
    /// <param name="cte">CT-e em edição.</param>
    void Ler(ConhecimentoTransporte cte);

    /// <summary>Pendências que impedem avançar desta etapa.</summary>
    /// <param name="cte">CT-e em edição, já lido.</param>
    IReadOnlyList<string> Validar(ConhecimentoTransporte cte);
}
