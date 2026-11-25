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
    /// Lista reduzida de municípios usada pelos cadastros que ainda estão em memória. Os códigos foram conferidos
    /// com a lista oficial do IBGE carregada no banco (fatia P2a).
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

    /// <summary>
    /// Certificados de todas as empresas. As datas são relativas a hoje para mostrar as três situações
    /// (válido, vencendo em breve e vencido). Não há arquivo .pfx real: o caminho é só ilustrativo.
    /// </summary>
    public static RepositorioMemoria<CertificadoPrototipo> Certificados { get; } = new(
    [
        new()
        {
            EmpresaId = 1, Tipo = TipoCertificado.A1, CaminhoArquivo = @"C:\Certificados\alfa-exemplo.pfx",
            Titular = "TRANSPORTADORA EXEMPLO ALFA LTDA", CnpjTitular = "11.111.111/0001-11",
            ValidoDe = DateTime.Today.AddDays(-65), ValidoAte = DateTime.Today.AddDays(300),
        },
        new()
        {
            EmpresaId = 1, Tipo = TipoCertificado.A3, Thumbprint = "0123456789ABCDEF0123456789ABCDEF01234567",
            Titular = "TRANSPORTADORA EXEMPLO ALFA LTDA", CnpjTitular = "11.111.111/0001-11",
            ValidoDe = DateTime.Today.AddDays(-715), ValidoAte = DateTime.Today.AddDays(15),
        },
        new()
        {
            EmpresaId = 2, Tipo = TipoCertificado.A1, CaminhoArquivo = @"C:\Certificados\beta-exemplo.pfx",
            Titular = "TRANSPORTADORA EXEMPLO BETA LTDA", CnpjTitular = "22.222.222/0001-22",
            ValidoDe = DateTime.Today.AddDays(-375), ValidoAte = DateTime.Today.AddDays(-10),
        },
    ]);

    /// <summary>Séries de numeração de todas as empresas.</summary>
    public static RepositorioMemoria<SeriePrototipo> Series { get; } = new(
    [
        new() { EmpresaId = 1, Modelo = ModeloDocumento.Cte, Serie = 1, Homologacao = true, ProximoNumero = 15 },
        new() { EmpresaId = 1, Modelo = ModeloDocumento.Mdfe, Serie = 1, Homologacao = true, ProximoNumero = 4 },
        new() { EmpresaId = 3, Modelo = ModeloDocumento.Cte, Serie = 1, Homologacao = false, ProximoNumero = 1 },
    ]);

    /// <summary>Motoristas de todas as empresas.</summary>
    public static RepositorioMemoria<MotoristaPrototipo> Motoristas { get; } = new(
    [
        new() { EmpresaId = 1, Nome = "JOSE EXEMPLO DA SILVA", Cpf = "333.333.333-33", Telefone = "65999990001" },
        new() { EmpresaId = 1, Nome = "MARIA EXEMPLO SOUZA", Cpf = "444.444.444-44" },
        new() { EmpresaId = 2, Nome = "JOAO EXEMPLO PEREIRA", Cpf = "555.555.555-55", Telefone = "66999990002" },
    ]);

    /// <summary>Apólices de seguro de todas as empresas.</summary>
    public static RepositorioMemoria<ApolicePrototipo> Apolices { get; } = new(
    [
        new()
        {
            EmpresaId = 1, Responsavel = ResponsavelSeguro.Emitente, NomeSeguradora = "SEGURADORA EXEMPLO S/A",
            CnpjSeguradora = "88.888.888/0001-88", NumeroApolice = "APOL-0001",
        },
        new()
        {
            EmpresaId = 1, Responsavel = ResponsavelSeguro.Contratante, CnpjCpfResponsavel = "55.555.555/0001-55",
            NomeSeguradora = "SEGUROS FICTICIOS S/A", CnpjSeguradora = "99.999.999/0001-99", NumeroApolice = "998877",
        },
    ]);

    /// <summary>
    /// CT-e da empresa 1 para a consulta. Os participantes e a série são os da lista acima, pelo <c>Id</c> gerado
    /// na ordem de inclusão. As chaves são fictícias (dígito verificador não calculado).
    /// </summary>
    public static RepositorioMemoria<CtePrototipo> Ctes { get; } = new(
    [
        new()
        {
            EmpresaId = 1, Situacao = SituacaoCte.ProntoParaTransmitir, Tipo = TipoCte.Normal, TipoServico = TipoServicoCte.Normal,
            SerieId = 1, Cfop = "6352", NaturezaOperacao = "PRESTACAO DE SERVICO DE TRANSPORTE",
            DataEmissao = DateTime.Today.AddDays(-2), RemetenteId = 1, DestinatarioId = 2, Tomador = TomadorServico.Remetente,
            MunicipioInicioCodigo = 5107925, MunicipioFimCodigo = 3550308,
            ValorCarga = 150_000m, ProdutoPredominante = "SOJA EM GRAOS",
            Quantidades = [new() { Unidade = UnidadeMedidaCarga.Quilograma, TipoMedida = "PESO BRUTO", Quantidade = 32_000m }],
            DocumentosNfe = [new() { Chave = ChaveFicticia(55, "44444444000144", 1501) }],
            Componentes = [new() { Nome = "FRETE PESO", Valor = 8_500m }, new() { Nome = "PEDAGIO", Valor = 320m }],
            ValorReceber = 8_820m,
        },
        new()
        {
            EmpresaId = 1, Situacao = SituacaoCte.EmDigitacao, Tipo = TipoCte.Normal, TipoServico = TipoServicoCte.Subcontratacao,
            SerieId = 1, Cfop = "5352", NaturezaOperacao = "PRESTACAO DE SERVICO DE TRANSPORTE",
            DataEmissao = DateTime.Today.AddDays(-1), RemetenteId = 2, DestinatarioId = 3, Tomador = TomadorServico.Destinatario,
            MunicipioInicioCodigo = 5103403, MunicipioFimCodigo = 5108402,
            ValorCarga = 12_000m, ProdutoPredominante = "ALIMENTOS",
            Quantidades = [new() { Unidade = UnidadeMedidaCarga.Quilograma, TipoMedida = "PESO BRUTO", Quantidade = 1_800m }],
            DocumentosNfe = [new() { Chave = ChaveFicticia(55, "55555555000155", 9020) }],
            Componentes = [new() { Nome = "FRETE VALOR", Valor = 450m }],
            ValorReceber = 450m,
        },
        new()
        {
            EmpresaId = 1, Situacao = SituacaoCte.ProntoParaTransmitir, Tipo = TipoCte.Complemento, TipoServico = TipoServicoCte.Normal,
            SerieId = 1, Cfop = "6352", NaturezaOperacao = "COMPLEMENTO DE FRETE",
            DataEmissao = DateTime.Today, RemetenteId = 1, DestinatarioId = 2, Tomador = TomadorServico.Remetente,
            MunicipioInicioCodigo = 5107925, MunicipioFimCodigo = 3550308,
            ChaveCteComplementado = ChaveFicticia(57, "11111111000111", 14),
            Componentes = [new() { Nome = "DIF. FRETE", Valor = 380m }],
            ValorReceber = 380m,
        },
    ]);

    /// <summary>
    /// MDF-e da empresa 1 para a consulta. Veículos, motoristas, apólice, série e CT-e são os das listas acima,
    /// pelo <c>Id</c> gerado na ordem de inclusão. O CIOT é fictício.
    /// </summary>
    public static RepositorioMemoria<MdfePrototipo> Mdfes { get; } = new(
    [
        new()
        {
            EmpresaId = 1, Situacao = SituacaoMdfe.ProntoParaTransmitir, TipoEmitente = TipoEmitenteMdfe.PrestadorServico,
            TipoTransportador = TipoTransportador.Etc, SerieId = 2, DataEmissao = DateTime.Today.AddDays(-2),
            InicioViagem = DateTime.Today.AddDays(-2).AddHours(8), UfInicio = "MT", UfFim = "SP", Percurso = ["MS"],
            MunicipiosCarregamento = [5107925],
            Descargas = [new() { MunicipioCodigo = 3550308, Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, CteId = 1 }] }],
            VeiculoTracaoId = 1, ReboquesIds = [2], CondutoresIds = [1],
            Ciots = [new() { Ciot = "123456789012", CpfCnpjResponsavel = "11.111.111/0001-11" }],
            TipoCarga = TipoCargaMdfe.GranelSolido, ProdutoPredominante = "SOJA EM GRAOS", ApoliceId = 1, Averbacoes = ["AVB-000123"],
            ValorCarga = 150_000m, UnidadePeso = UnidadePesoMdfe.Quilograma, PesoCarga = 32_000m,
        },
        new()
        {
            EmpresaId = 1, Situacao = SituacaoMdfe.EmDigitacao, TipoEmitente = TipoEmitenteMdfe.PrestadorServico,
            SerieId = 2, DataEmissao = DateTime.Today, UfInicio = "MT", UfFim = "MT",
            MunicipiosCarregamento = [5103403],
            Descargas = [new() { MunicipioCodigo = 5108402, Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, Chave = ChaveFicticia(57, "11111111000111", 16) }] }],
            VeiculoTracaoId = 3, CondutoresIds = [2],
            TipoCarga = TipoCargaMdfe.CargaGeral, ProdutoPredominante = "ALIMENTOS", ApoliceId = 1,
            ValorCarga = 12_000m, UnidadePeso = UnidadePesoMdfe.Quilograma, PesoCarga = 1_800m,
        },
    ]);

    /// <summary>Monta uma chave de acesso fictícia de 44 dígitos (MT, mês atual, série 1), sem dígito verificador real.</summary>
    /// <param name="modelo">55 para NF-e, 57 para CT-e.</param>
    /// <param name="cnpj">CNPJ do emitente, só dígitos.</param>
    /// <param name="numero">Número do documento.</param>
    private static string ChaveFicticia(int modelo, string cnpj, int numero) =>
        $"51{DateTime.Today:yyMM}{cnpj}{modelo:00}001{numero:000000000}1{numero:00000000}0";

    /// <summary>Procura o município pelo código IBGE.</summary>
    /// <param name="codigo">Código IBGE, ou <c>null</c>.</param>
    public static MunicipioPrototipo? ObterMunicipio(int? codigo) =>
        codigo is null ? null : Municipios.FirstOrDefault(m => m.Codigo == codigo);
}
