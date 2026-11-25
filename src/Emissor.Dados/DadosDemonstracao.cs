using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Dados de demonstração, oferecidos quando o banco não tem empresas (ou tem só as empresas de demonstração,
/// sem os demais cadastros ou sem os documentos de exemplo). Documentos, placas, chaves e RNTRCs são fictícios. Os
/// registros recebem códigos fixos, usados pelos CT-e e MDF-e de exemplo para apontar para os cadastros.
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

    /// <summary>
    /// Indica se o banco tem só as empresas e os cadastros de demonstração e ainda nenhum CT-e ou MDF-e — caso de
    /// quem carregou a demonstração antes da fatia P4a, quando os documentos de exemplo ficavam em memória.
    /// </summary>
    /// <param name="contexto">Contexto do banco.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task<bool> DocumentosDemonstracaoPendentesAsync(EmissorDbContext contexto, CancellationToken cancelamento = default)
    {
        var esperadas = Empresas().Select(e => (e.Id, e.Cnpj)).ToList();
        var existentes = await contexto.Empresas.AsNoTracking().Select(e => new { e.Id, e.Cnpj }).ToListAsync(cancelamento);
        if (existentes.Count != esperadas.Count || !existentes.All(e => esperadas.Contains((e.Id, e.Cnpj))))
        {
            return false;
        }

        // Os documentos apontam para estes códigos: só carrega se os cadastros de demonstração estiverem lá.
        var cadastrosPresentes =
            await contexto.Participantes.CountAsync(p => p.EmpresaId == 1 && p.Id <= 3, cancelamento) == 3
            && await contexto.Series.CountAsync(s => s.EmpresaId == 1 && s.Id <= 2, cancelamento) == 2
            && await contexto.Veiculos.CountAsync(v => v.EmpresaId == 1 && v.Id <= 3, cancelamento) == 3
            && await contexto.Motoristas.CountAsync(m => m.EmpresaId == 1 && m.Id <= 2, cancelamento) == 2
            && await contexto.Apolices.AnyAsync(a => a.EmpresaId == 1 && a.Id == 1, cancelamento);
        return cadastrosPresentes
            && !await contexto.Ctes.AnyAsync(cancelamento)
            && !await contexto.Mdfes.AnyAsync(cancelamento);
    }

    /// <summary>Indica se falta carregar alguma parte da demonstração (cadastros ou documentos de exemplo).</summary>
    /// <param name="contexto">Contexto do banco.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task<bool> DemonstracaoIncompletaAsync(EmissorDbContext contexto, CancellationToken cancelamento = default) =>
        await CadastrosDemonstracaoPendentesAsync(contexto, cancelamento)
        || await DocumentosDemonstracaoPendentesAsync(contexto, cancelamento);

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
        new() { Id = 1, EmpresaId = 1, CnpjCpf = "22222222222", Rntrc = "44444444", Nome = "MOTORISTA AGREGADO EXEMPLO", TipoProprietario = TipoProprietario.TacAgregado, Ie = "ISENTO", Uf = "MT" },
        new() { Id = 2, EmpresaId = 1, CnpjCpf = "77777777000177", Rntrc = "55555555", Nome = "FROTAS EXEMPLO LTDA", TipoProprietario = TipoProprietario.Outros, Ie = "777777777", Uf = "MT" },
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

        if (await CadastrosDemonstracaoPendentesAsync(contexto, cancelamento))
        {
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

        if (await DocumentosDemonstracaoPendentesAsync(contexto, cancelamento))
        {
            // CT-e antes dos MDF-e, que vinculam o primeiro CT-e.
            contexto.Ctes.AddRange(Ctes());
            await contexto.SaveChangesAsync(cancelamento);
            contexto.Mdfes.AddRange(Mdfes());
            await contexto.SaveChangesAsync(cancelamento);
            contexto.ChangeTracker.Clear();
        }
    }

    /// <summary>
    /// CT-e de exemplo da empresa 1 (dois prontos para transmitir e um rascunho), com participantes e série da
    /// demonstração. As chaves são fictícias (dígito verificador não calculado).
    /// </summary>
    public static IReadOnlyList<ConhecimentoTransporte> Ctes() =>
    [
        new()
        {
            Id = 1, EmpresaId = 1, Situacao = SituacaoDocumento.ProntoParaTransmitir, Tipo = TipoCte.Normal, TipoServico = TipoServicoCte.Normal,
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
            Id = 2, EmpresaId = 1, Situacao = SituacaoDocumento.EmDigitacao, Tipo = TipoCte.Normal, TipoServico = TipoServicoCte.Subcontratacao,
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
            Id = 3, EmpresaId = 1, Situacao = SituacaoDocumento.ProntoParaTransmitir, Tipo = TipoCte.Complemento, TipoServico = TipoServicoCte.Normal,
            SerieId = 1, Cfop = "6352", NaturezaOperacao = "COMPLEMENTO DE FRETE",
            DataEmissao = DateTime.Today, RemetenteId = 1, DestinatarioId = 2, Tomador = TomadorServico.Remetente,
            MunicipioInicioCodigo = 5107925, MunicipioFimCodigo = 3550308,
            ChaveCteComplementado = ChaveFicticia(57, "11111111000111", 14),
            Componentes = [new() { Nome = "DIF. FRETE", Valor = 380m }],
            ValorReceber = 380m,
        },
    ];

    /// <summary>
    /// MDF-e de exemplo da empresa 1 (um pronto, vinculando o CT-e 1, e um rascunho), com veículos, motoristas,
    /// apólice e série da demonstração. O CIOT é fictício.
    /// </summary>
    public static IReadOnlyList<ManifestoEletronico> Mdfes() =>
    [
        new()
        {
            Id = 1, EmpresaId = 1, Situacao = SituacaoDocumento.ProntoParaTransmitir, TipoEmitente = TipoEmitenteMdfe.PrestadorServico,
            TipoTransportador = TipoTransportador.Etc, SerieId = 2, DataEmissao = DateTime.Today.AddDays(-2),
            InicioViagem = DateTime.Today.AddDays(-2).AddHours(8), UfInicio = "MT", UfFim = "SP", Percurso = ["MS"],
            MunicipiosCarregamento = [5107925],
            Descargas = [new() { MunicipioCodigo = 3550308, Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, CteId = 1 }] }],
            VeiculoTracaoId = 1, ReboquesIds = [2], CondutoresIds = [1],
            Ciots = [new() { Ciot = "123456789012", CpfCnpjResponsavel = "11111111000111" }],
            TipoCarga = TipoCargaMdfe.GranelSolido, ProdutoPredominante = "SOJA EM GRAOS", ApoliceId = 1, Averbacoes = ["AVB-000123"],
            ValorCarga = 150_000m, UnidadePeso = UnidadePesoMdfe.Quilograma, PesoCarga = 32_000m,
        },
        new()
        {
            Id = 2, EmpresaId = 1, Situacao = SituacaoDocumento.EmDigitacao, TipoEmitente = TipoEmitenteMdfe.PrestadorServico,
            SerieId = 2, DataEmissao = DateTime.Today, UfInicio = "MT", UfFim = "MT",
            MunicipiosCarregamento = [5103403],
            Descargas = [new() { MunicipioCodigo = 5108402, Documentos = [new() { Tipo = TipoDocumentoMdfe.Cte, Chave = ChaveFicticia(57, "11111111000111", 16) }] }],
            VeiculoTracaoId = 3, CondutoresIds = [2],
            TipoCarga = TipoCargaMdfe.CargaGeral, ProdutoPredominante = "ALIMENTOS", ApoliceId = 1,
            ValorCarga = 12_000m, UnidadePeso = UnidadePesoMdfe.Quilograma, PesoCarga = 1_800m,
        },
    ];

    /// <summary>Monta uma chave de acesso fictícia de 44 dígitos (MT, mês atual, série 1), sem dígito verificador real.</summary>
    /// <param name="modelo">55 para NF-e, 57 para CT-e.</param>
    /// <param name="cnpj">CNPJ do emitente, só dígitos.</param>
    /// <param name="numero">Número do documento.</param>
    private static string ChaveFicticia(int modelo, string cnpj, int numero) =>
        $"51{DateTime.Today:yyMM}{cnpj}{modelo:00}001{numero:000000000}1{numero:00000000}0";
}
