using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Dados inventados que ainda ficam em memória: os certificados (até a fatia P3) e os documentos CT-e e MDF-e de
/// exemplo (até a fatia P4). Os cadastros estão no banco desde as fatias P2a e P2b. Os documentos apontam para os
/// códigos dos dados de demonstração do banco (participante 1, veículo 1, série 1...).
/// </summary>
internal static class DadosFicticios
{
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
}
