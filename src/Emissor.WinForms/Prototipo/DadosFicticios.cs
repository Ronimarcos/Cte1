using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Dados inventados para navegar pelo protótipo das telas. CNPJs, CPFs, placas e RNTRCs são fictícios
/// e não pertencem a empresas ou pessoas reais. As alterações feitas nas telas ficam só em memória.
/// Esta classe será removida quando os cadastros passarem a usar o banco (EF Core).
/// </summary>
internal static class DadosFicticios
{
    /// <summary>
    /// Lista reduzida de municípios para preencher endereços. Os códigos são do IBGE e serão conferidos
    /// na carga oficial da tabela (§5.9) quando ela for implementada.
    /// </summary>
    public static IReadOnlyList<MunicipioPrototipo> Municipios { get; } =
    [
        new(5103403, "Cuiabá", "MT"),
        new(5108402, "Várzea Grande", "MT"),
        new(5107602, "Rondonópolis", "MT"),
        new(5107909, "Sinop", "MT"),
        new(5107925, "Sorriso", "MT"),
        new(5105259, "Lucas do Rio Verde", "MT"),
        new(5107958, "Tangará da Serra", "MT"),
        new(5002704, "Campo Grande", "MS"),
        new(5208707, "Goiânia", "GO"),
        new(3550308, "São Paulo", "SP"),
        new(4314902, "Porto Alegre", "RS"),
        new(1302603, "Manaus", "AM"),
    ];

    /// <summary>Empresas (emitentes) disponíveis na seleção e no cadastro de empresas.</summary>
    public static RepositorioMemoria<EmpresaPrototipo> Empresas { get; } = new(
    [
        new()
        {
            Id = 1, RazaoSocial = "TRANSPORTADORA EXEMPLO ALFA LTDA", NomeFantasia = "ALFA TRANSPORTES",
            Cnpj = "11.111.111/0001-11", Ie = "111111111", Crt = Crt.SimplesNacional, Rntrc = "11111111",
            Logradouro = "AVENIDA EXEMPLO", Numero = "100", Bairro = "CENTRO", MunicipioCodigo = 5103403,
            Cep = "78000-000", Fone = "6533330001", Email = "fiscal@alfa.exemplo", Homologacao = true,
        },
        new()
        {
            Id = 2, RazaoSocial = "TRANSPORTADORA EXEMPLO BETA LTDA", NomeFantasia = "BETA LOG",
            Cnpj = "22.222.222/0001-22", Ie = "222222222", Crt = Crt.SimplesNacional, Rntrc = "22222222",
            Logradouro = "RUA FICTÍCIA", Numero = "200", Bairro = "VILA AURORA", MunicipioCodigo = 5107602,
            Cep = "78700-000", Fone = "6634440002", Email = "fiscal@beta.exemplo", Homologacao = true,
        },
        new()
        {
            Id = 3, RazaoSocial = "LOGISTICA EXEMPLO GAMA LTDA", NomeFantasia = "GAMA",
            Cnpj = "33.333.333/0001-33", Ie = "333333333", Crt = Crt.SimplesNacional, Rntrc = "33333333",
            Logradouro = "RODOVIA EXEMPLO", Numero = "KM 10", Bairro = "DISTRITO INDUSTRIAL", MunicipioCodigo = 5107909,
            Cep = "78550-000", Fone = "6635550003", Email = "fiscal@gama.exemplo", Homologacao = false,
        },
    ]);

    /// <summary>Participantes de todas as empresas.</summary>
    public static RepositorioMemoria<ParticipantePrototipo> Participantes { get; } = new(
    [
        new()
        {
            EmpresaId = 1, CnpjCpf = "44.444.444/0001-44", IndicadorIe = IndicadorIe.Contribuinte, Ie = "444444444",
            RazaoSocial = "COMERCIO DE GRAOS EXEMPLO LTDA", Logradouro = "RUA DOS GRÃOS", Numero = "45",
            Bairro = "CENTRO", MunicipioCodigo = 5107925, Cep = "78890-000", Fone = "6636660004",
        },
        new()
        {
            EmpresaId = 1, CnpjCpf = "55.555.555/0001-55", IndicadorIe = IndicadorIe.Contribuinte, Ie = "555555555",
            RazaoSocial = "INDUSTRIA DE ALIMENTOS EXEMPLO S/A", NomeFantasia = "ALIMENTOS EXEMPLO",
            Logradouro = "AVENIDA INDUSTRIAL", Numero = "1000", Bairro = "DISTRITO", MunicipioCodigo = 3550308,
            Cep = "01000-000", Email = "nfe@alimentos.exemplo",
        },
        new()
        {
            EmpresaId = 1, CnpjCpf = "111.111.111-11", IndicadorIe = IndicadorIe.NaoContribuinte,
            RazaoSocial = "FULANO DE TAL EXEMPLO", Logradouro = "RUA SEM NOME", Numero = "S/N",
            Bairro = "JARDIM EXEMPLO", MunicipioCodigo = 5108402,
        },
        new()
        {
            EmpresaId = 2, CnpjCpf = "66.666.666/0001-66", IndicadorIe = IndicadorIe.Isento,
            RazaoSocial = "COOPERATIVA EXEMPLO", Logradouro = "ESTRADA RURAL", Numero = "5",
            Bairro = "ZONA RURAL", MunicipioCodigo = 5107602,
        },
    ]);

    /// <summary>Proprietários de veículos de terceiros de todas as empresas.</summary>
    public static RepositorioMemoria<ProprietarioVeiculoPrototipo> Proprietarios { get; } = new(
    [
        new()
        {
            Id = 1, EmpresaId = 1, CnpjCpf = "222.222.222-22", Rntrc = "44444444",
            Nome = "MOTORISTA AGREGADO EXEMPLO", TipoProprietario = TipoProprietario.TacAgregado,
        },
        new()
        {
            Id = 2, EmpresaId = 1, CnpjCpf = "77.777.777/0001-77", Rntrc = "55555555",
            Nome = "FROTAS EXEMPLO LTDA", TipoProprietario = TipoProprietario.Outros,
        },
    ]);

    /// <summary>Veículos de todas as empresas.</summary>
    public static RepositorioMemoria<VeiculoPrototipo> Veiculos { get; } = new(
    [
        new()
        {
            EmpresaId = 1, Tipo = TipoVeiculo.Tracao, CodigoInterno = "C01", Placa = "AAA1A11", Renavam = "00000000001",
            TaraKg = 8500, TipoRodado = TipoRodado.CavaloMecanico, TipoCarroceria = TipoCarroceria.NaoAplicavel,
            Uf = "MT", Proprio = true,
        },
        new()
        {
            EmpresaId = 1, Tipo = TipoVeiculo.Reboque, CodigoInterno = "R01", Placa = "AAA2B22", Renavam = "00000000002",
            TaraKg = 7000, CapacidadeKg = 32000, CapacidadeM3 = 90, TipoCarroceria = TipoCarroceria.Sider,
            Uf = "MT", Proprio = true,
        },
        new()
        {
            EmpresaId = 1, Tipo = TipoVeiculo.Tracao, Placa = "AAA3C33", TaraKg = 6000, CapacidadeKg = 14000,
            TipoRodado = TipoRodado.Truck, TipoCarroceria = TipoCarroceria.FechadaBau, Uf = "MT",
            Proprio = false, ProprietarioId = 1,
        },
    ]);

    /// <summary>Procura o município pelo código IBGE.</summary>
    /// <param name="codigo">Código IBGE, ou <c>null</c>.</param>
    public static MunicipioPrototipo? ObterMunicipio(int? codigo) =>
        codigo is null ? null : Municipios.FirstOrDefault(m => m.Codigo == codigo);
}
