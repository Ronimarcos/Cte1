using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Dados de demonstração, oferecidos quando o banco não tem empresas (ou tem só as empresas de demonstração,
/// sem os demais cadastros). Documentos, placas e RNTRCs são fictícios. Os registros recebem os mesmos códigos
/// dos dados que os assistentes de CT-e e MDF-e ainda leem da memória (até a fatia P2c), para que continuem
/// batendo com os cadastros do banco.
/// </summary>
public static class DadosDemonstracao
{
    /// <summary>Indica se o banco ainda não tem nenhuma empresa.</summary>
    /// <param name="contexto">Contexto do banco.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task<bool> BancoSemEmpresasAsync(EmissorDbContext contexto, CancellationToken cancelamento = default) =>
        !await contexto.Empresas.AnyAsync(cancelamento);

    /// <summary>
    /// Indica se o banco tem só as empresas de demonstração (mesmos códigos e CNPJs) e nenhum dos demais
    /// cadastros — caso de quem carregou a demonstração na fatia P2a.
    /// </summary>
    /// <param name="contexto">Contexto do banco.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task<bool> CadastrosDemonstracaoPendentesAsync(EmissorDbContext contexto, CancellationToken cancelamento = default)
    {
        var esperadas = Empresas().Select(e => (e.Id, e.Cnpj)).ToList();
        var existentes = await contexto.Empresas.AsNoTracking().Select(e => new { e.Id, e.Cnpj }).ToListAsync(cancelamento);
        var soDemonstracao = existentes.Count == esperadas.Count && existentes.All(e => esperadas.Contains((e.Id, e.Cnpj)));
        return soDemonstracao
            && !await contexto.Participantes.AnyAsync(cancelamento)
            && !await contexto.Proprietarios.AnyAsync(cancelamento)
            && !await contexto.Veiculos.AnyAsync(cancelamento)
            && !await contexto.Motoristas.AnyAsync(cancelamento)
            && !await contexto.Apolices.AnyAsync(cancelamento)
            && !await contexto.Series.AnyAsync(cancelamento);
    }

    /// <summary>Empresas fictícias de demonstração (ALFA e BETA em homologação, GAMA em produção).</summary>
    public static IReadOnlyList<Empresa> Empresas() =>
    [
        new()
        {
            Id = 1, RazaoSocial = "TRANSPORTADORA EXEMPLO ALFA LTDA", NomeFantasia = "ALFA TRANSPORTES", Cnpj = "11111111000111",
            Ie = "111111111", Crt = Crt.SimplesNacional, Rntrc = "11111111", Fone = "6533330001", Email = "fiscal@alfa.exemplo",
            Endereco = new() { Logradouro = "AVENIDA EXEMPLO", Numero = "100", Bairro = "CENTRO", MunicipioCodigo = 5103403, Cep = "78000000" },
        },
        new()
        {
            Id = 2, RazaoSocial = "TRANSPORTADORA EXEMPLO BETA LTDA", NomeFantasia = "BETA LOG", Cnpj = "22222222000122",
            Ie = "222222222", Crt = Crt.SimplesNacional, Rntrc = "22222222", Fone = "6634440002", Email = "fiscal@beta.exemplo",
            Endereco = new() { Logradouro = "RUA FICTICIA", Numero = "200", Bairro = "VILA AURORA", MunicipioCodigo = 5107602, Cep = "78700000" },
        },
        new()
        {
            Id = 3, RazaoSocial = "LOGISTICA EXEMPLO GAMA LTDA", NomeFantasia = "GAMA", Cnpj = "33333333000133",
            Ie = "333333333", Crt = Crt.SimplesNacional, Rntrc = "33333333", Fone = "6635550003", Email = "fiscal@gama.exemplo",
            AmbientePadrao = TipoAmbiente.Producao,
            Endereco = new() { Logradouro = "RODOVIA EXEMPLO", Numero = "KM 10", Bairro = "DISTRITO INDUSTRIAL", MunicipioCodigo = 5107909, Cep = "78550000" },
        },
    ];

    /// <summary>Participantes de demonstração (códigos 1 a 4).</summary>
    public static IReadOnlyList<Participante> Participantes() =>
    [
        new()
        {
            Id = 1, EmpresaId = 1, CnpjCpf = "44444444000144", IndicadorIe = IndicadorIe.Contribuinte, Ie = "444444444",
            RazaoSocial = "COMERCIO DE GRAOS EXEMPLO LTDA", Fone = "6636660004",
            Endereco = new() { Logradouro = "RUA DOS GRÃOS", Numero = "45", Bairro = "CENTRO", MunicipioCodigo = 5107925, Cep = "78890000" },
        },
        new()
        {
            Id = 2, EmpresaId = 1, CnpjCpf = "55555555000155", IndicadorIe = IndicadorIe.Contribuinte, Ie = "555555555",
            RazaoSocial = "INDUSTRIA DE ALIMENTOS EXEMPLO S/A", NomeFantasia = "ALIMENTOS EXEMPLO", Email = "nfe@alimentos.exemplo",
            Endereco = new() { Logradouro = "AVENIDA INDUSTRIAL", Numero = "1000", Bairro = "DISTRITO", MunicipioCodigo = 3550308, Cep = "01000000" },
        },
        new()
        {
            Id = 3, EmpresaId = 1, CnpjCpf = "11111111111", IndicadorIe = IndicadorIe.NaoContribuinte,
            RazaoSocial = "FULANO DE TAL EXEMPLO",
            Endereco = new() { Logradouro = "RUA SEM NOME", Numero = "S/N", Bairro = "JARDIM EXEMPLO", MunicipioCodigo = 5108402 },
        },
        new()
        {
            Id = 4, EmpresaId = 2, CnpjCpf = "66666666000166", IndicadorIe = IndicadorIe.Isento,
            RazaoSocial = "COOPERATIVA EXEMPLO",
            Endereco = new() { Logradouro = "ESTRADA RURAL", Numero = "5", Bairro = "ZONA RURAL", MunicipioCodigo = 5107602 },
        },
    ];

    /// <summary>Proprietários de veículos de terceiros de demonstração (códigos 1 e 2).</summary>
    public static IReadOnlyList<ProprietarioVeiculo> Proprietarios() =>
    [
        new() { Id = 1, EmpresaId = 1, CnpjCpf = "22222222222", Rntrc = "44444444", Nome = "MOTORISTA AGREGADO EXEMPLO", TipoProprietario = TipoProprietario.TacAgregado },
        new() { Id = 2, EmpresaId = 1, CnpjCpf = "77777777000177", Rntrc = "55555555", Nome = "FROTAS EXEMPLO LTDA", TipoProprietario = TipoProprietario.Outros },
    ];

    /// <summary>Veículos de demonstração (códigos 1 a 3; o 3 é de terceiro).</summary>
    public static IReadOnlyList<Veiculo> Veiculos() =>
    [
        new()
        {
            Id = 1, EmpresaId = 1, Tipo = TipoVeiculo.Tracao, CodigoInterno = "C01", Placa = "AAA1A11", Renavam = "00000000001",
            TaraKg = 8500, TipoRodado = TipoRodado.CavaloMecanico, TipoCarroceria = TipoCarroceria.NaoAplicavel, Uf = "MT",
        },
        new()
        {
            Id = 2, EmpresaId = 1, Tipo = TipoVeiculo.Reboque, CodigoInterno = "R01", Placa = "AAA2B22", Renavam = "00000000002",
            TaraKg = 7000, CapacidadeKg = 32000, CapacidadeM3 = 90, TipoCarroceria = TipoCarroceria.Sider, Uf = "MT",
        },
        new()
        {
            Id = 3, EmpresaId = 1, Tipo = TipoVeiculo.Tracao, Placa = "AAA3C33", TaraKg = 6000, CapacidadeKg = 14000,
            TipoRodado = TipoRodado.Truck, TipoCarroceria = TipoCarroceria.FechadaBau, Uf = "MT", Proprio = false, ProprietarioId = 1,
        },
    ];

    /// <summary>Motoristas de demonstração (códigos 1 a 3).</summary>
    public static IReadOnlyList<Motorista> Motoristas() =>
    [
        new() { Id = 1, EmpresaId = 1, Nome = "JOSE EXEMPLO DA SILVA", Cpf = "33333333333", Telefone = "65999990001" },
        new() { Id = 2, EmpresaId = 1, Nome = "MARIA EXEMPLO SOUZA", Cpf = "44444444444" },
        new() { Id = 3, EmpresaId = 2, Nome = "JOAO EXEMPLO PEREIRA", Cpf = "55555555555", Telefone = "66999990002" },
    ];

    /// <summary>Apólices de demonstração (códigos 1 e 2).</summary>
    public static IReadOnlyList<ApoliceSeguro> Apolices() =>
    [
        new()
        {
            Id = 1, EmpresaId = 1, Responsavel = ResponsavelSeguro.Emitente, NomeSeguradora = "SEGURADORA EXEMPLO S/A",
            CnpjSeguradora = "88888888000188", NumeroApolice = "APOL-0001",
        },
        new()
        {
            Id = 2, EmpresaId = 1, Responsavel = ResponsavelSeguro.Contratante, CnpjCpfResponsavel = "55555555000155",
            NomeSeguradora = "SEGUROS FICTICIOS S/A", CnpjSeguradora = "99999999000199", NumeroApolice = "998877",
        },
    ];

    /// <summary>Séries de demonstração (códigos 1 a 3).</summary>
    public static IReadOnlyList<SerieDocumento> Series() =>
    [
        new() { Id = 1, EmpresaId = 1, Modelo = ModeloDocumento.Cte, Serie = 1, Ambiente = TipoAmbiente.Homologacao, ProximoNumero = 15 },
        new() { Id = 2, EmpresaId = 1, Modelo = ModeloDocumento.Mdfe, Serie = 1, Ambiente = TipoAmbiente.Homologacao, ProximoNumero = 4 },
        new() { Id = 3, EmpresaId = 3, Modelo = ModeloDocumento.Cte, Serie = 1, Ambiente = TipoAmbiente.Producao, ProximoNumero = 1 },
    ];

    /// <summary>
    /// Grava a demonstração: as empresas, se o banco não tiver nenhuma, e os demais cadastros, se o banco tiver só as
    /// empresas de demonstração e nenhum outro cadastro. Em qualquer outro caso não faz nada.
    /// </summary>
    /// <param name="contexto">Contexto do banco (com os municípios já carregados).</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task CarregarAsync(EmissorDbContext contexto, CancellationToken cancelamento = default)
    {
        if (await BancoSemEmpresasAsync(contexto, cancelamento))
        {
            contexto.Empresas.AddRange(Empresas());
            await contexto.SaveChangesAsync(cancelamento);
            contexto.ChangeTracker.Clear();
        }

        if (!await CadastrosDemonstracaoPendentesAsync(contexto, cancelamento))
        {
            return;
        }

        // Proprietários antes dos veículos, por causa do vínculo.
        contexto.Proprietarios.AddRange(Proprietarios());
        contexto.Participantes.AddRange(Participantes());
        contexto.Motoristas.AddRange(Motoristas());
        contexto.Apolices.AddRange(Apolices());
        contexto.Series.AddRange(Series());
        await contexto.SaveChangesAsync(cancelamento);
        contexto.Veiculos.AddRange(Veiculos());
        await contexto.SaveChangesAsync(cancelamento);
        contexto.ChangeTracker.Clear();
    }
}
