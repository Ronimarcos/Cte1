namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Dados inventados para navegar pelo protótipo das telas. Os CNPJs são fictícios e não pertencem a empresas reais.
/// Esta classe será removida quando os cadastros passarem a usar o banco (EF Core).
/// </summary>
internal static class DadosFicticios
{
    /// <summary>Empresas disponíveis na tela de seleção.</summary>
    public static IReadOnlyList<EmpresaPrototipo> Empresas { get; } =
    [
        new(1, "TRANSPORTADORA EXEMPLO ALFA LTDA", "11.111.111/0001-11", "MT", Homologacao: true),
        new(2, "TRANSPORTADORA EXEMPLO BETA LTDA", "22.222.222/0001-22", "MT", Homologacao: true),
        new(3, "LOGISTICA EXEMPLO GAMA LTDA", "33.333.333/0001-33", "MT", Homologacao: false),
    ];
}
