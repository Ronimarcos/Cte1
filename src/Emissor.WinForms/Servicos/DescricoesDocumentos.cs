using System.Globalization;
using System.Text;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Servicos;

/// <summary>
/// Textos de tela do CT-e e do MDF-e (lista, título e conferência), que dependem das opções das telas e dos nomes
/// dos cadastros; por isso ficam aqui e não nas entidades do domínio.
/// </summary>
internal static class DescricoesDocumentos
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");

    extension(DocumentoFiscal documento)
    {
        /// <summary>Número para exibição ("—" até a transmissão).</summary>
        public string NumeroDescricao => documento.Numero?.ToString(Brasil) ?? "—";

        /// <summary>Situação para exibição.</summary>
        public string SituacaoDescricao => DescreverSituacao(documento.Situacao);
    }

    extension(ConhecimentoTransporte cte)
    {
        /// <summary>Tipo do CT-e para exibição.</summary>
        public string TipoDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposCte, cte.Tipo);

        /// <summary>Tipo do serviço para exibição.</summary>
        public string TipoServicoDescricao => OpcoesCadastro.Texto(OpcoesCadastro.TiposServicoCte, cte.TipoServico);

        /// <summary>Texto da conferência (etapa 5), com os nomes lidos dos cadastros do banco.</summary>
        /// <param name="cadastros">Cadastros da empresa.</param>
        public string MontarResumo(CadastrosEmissao cadastros) => ResumoCte(cte, cadastros);
    }

    extension(ManifestoEletronico mdfe)
    {
        /// <summary>Texto da conferência (etapa 5), com os nomes lidos dos cadastros do banco.</summary>
        /// <param name="cadastros">Cadastros da empresa.</param>
        public string MontarResumo(CadastrosEmissao cadastros) => ResumoMdfe(mdfe, cadastros);
    }

    extension(DocumentoMdfe documento)
    {
        /// <summary>Texto da chave para exibição (CT-e deste sistema ainda sem chave mostram o código).</summary>
        public string ChaveDescricao => documento.CteId is null
            ? documento.Chave
            : $"nº {documento.CteId} emitido neste sistema (chave após a autorização)";
    }

    /// <summary>Situação para exibição.</summary>
    /// <param name="situacao">Situação do documento.</param>
    public static string DescreverSituacao(SituacaoDocumento situacao) => situacao switch
    {
        SituacaoDocumento.ProntoParaTransmitir => "Pronto para transmitir",
        _ => "Em digitação",
    };

    private static string ResumoCte(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
    {
        var texto = new StringBuilder();
        texto.AppendLine("IDENTIFICAÇÃO");
        texto.AppendLine($"  Tipo: {cte.TipoDescricao}    Serviço: {cte.TipoServicoDescricao}");
        texto.AppendLine($"  Série: {cadastros.NumeroSerie(cte.SerieId)}    Número: {cte.Numero?.ToString(Brasil) ?? "definido na transmissão"}    Emissão: {cte.DataEmissao:dd/MM/yyyy}");
        texto.AppendLine($"  CFOP: {cte.Cfop} - {cte.NaturezaOperacao}");
        texto.AppendLine();
        texto.AppendLine("PARTICIPANTES");
        texto.AppendLine($"  Remetente: {cadastros.NomeParticipante(cte.RemetenteId)}");
        texto.AppendLine($"  Destinatário: {cadastros.NomeParticipante(cte.DestinatarioId)}");
        if (cte.ExpedidorId is not null) texto.AppendLine($"  Expedidor: {cadastros.NomeParticipante(cte.ExpedidorId)}");
        if (cte.RecebedorId is not null) texto.AppendLine($"  Recebedor: {cadastros.NomeParticipante(cte.RecebedorId)}");
        texto.AppendLine($"  Tomador: {OpcoesCadastro.Texto(OpcoesCadastro.Tomadores, cte.Tomador)} - {cadastros.NomeParticipante(cte.TomadorId)}");
        texto.AppendLine($"  Percurso: {cadastros.Municipio(cte.MunicipioInicioCodigo)} → {cadastros.Municipio(cte.MunicipioFimCodigo)}");
        texto.AppendLine();

        if (cte.EhComplemento)
        {
            texto.AppendLine("CT-E COMPLEMENTADO");
            texto.AppendLine($"  Chave: {cte.ChaveCteComplementado}");
        }
        else
        {
            texto.AppendLine("CARGA");
            texto.AppendLine($"  Valor: {Moeda(cte.ValorCarga ?? 0)}    Produto predominante: {cte.ProdutoPredominante}");
            if (!string.IsNullOrWhiteSpace(cte.OutrasCaracteristicas)) texto.AppendLine($"  Outras características: {cte.OutrasCaracteristicas}");
            foreach (var quantidade in cte.Quantidades)
            {
                var unidade = OpcoesCadastro.Texto(OpcoesCadastro.UnidadesMedidaCarga, quantidade.Unidade);
                texto.AppendLine($"  {quantidade.TipoMedida}: {quantidade.Quantidade.ToString("N4", Brasil)} ({unidade})");
            }

            texto.AppendLine();
            texto.AppendLine($"DOCUMENTOS ({cte.DocumentosNfe.Count} NF-e)");
            foreach (var documento in cte.DocumentosNfe) texto.AppendLine($"  NF-e {documento.Chave}");
            foreach (var anterior in cte.DocumentosAnteriores)
            {
                texto.AppendLine($"  Doc. anterior: {anterior.NomeEmitente} ({anterior.CnpjCpfEmitente}) CT-e {anterior.ChaveCte}");
            }
        }

        texto.AppendLine();
        texto.AppendLine("VALORES");
        foreach (var componente in cte.Componentes) texto.AppendLine($"  {componente.Nome}: {Moeda(componente.Valor)}");
        texto.AppendLine($"  Total da prestação: {Moeda(cte.ValorTotalPrestacao)}    A receber: {Moeda(cte.ValorReceber ?? 0)}");
        texto.AppendLine("  ICMS: Simples Nacional (grupo ICMSSN, CST 90, indSN 1)");
        texto.AppendLine("  IBS/CBS: a calcular na implementação (NT 2026.005)");
        return texto.ToString();
    }

    private static string ResumoMdfe(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
    {
        var texto = new StringBuilder();
        texto.AppendLine("IDENTIFICAÇÃO");
        texto.AppendLine($"  Emitente: {OpcoesCadastro.Texto(OpcoesCadastro.TiposEmitenteMdfe, mdfe.TipoEmitente)}");
        texto.AppendLine($"  Transportador: {(mdfe.TipoTransportador is null ? "não informado" : OpcoesCadastro.Texto(OpcoesCadastro.TiposTransportador, mdfe.TipoTransportador))}");
        texto.AppendLine($"  Série: {cadastros.NumeroSerie(mdfe.SerieId)}    Número: {mdfe.Numero?.ToString(Brasil) ?? "definido na transmissão"}    Emissão: {mdfe.DataEmissao:dd/MM/yyyy}");
        texto.AppendLine($"  Início da viagem: {(mdfe.InicioViagem is null ? "não informado" : mdfe.InicioViagem.Value.ToString("dd/MM/yyyy HH:mm", Brasil))}");
        texto.AppendLine($"  Percurso: {mdfe.PercursoDescricao}");
        texto.AppendLine();
        texto.AppendLine("CARREGAMENTO");
        foreach (var codigo in mdfe.MunicipiosCarregamento) texto.AppendLine($"  {cadastros.Municipio(codigo)}");
        texto.AppendLine("DESCARGA");
        foreach (var descarga in mdfe.Descargas)
        {
            texto.AppendLine($"  {cadastros.Municipio(descarga.MunicipioCodigo)}");
            foreach (var documento in descarga.Documentos) texto.AppendLine($"    {(documento.Tipo == TipoDocumentoMdfe.Cte ? "CT-e" : "NF-e")} {documento.ChaveDescricao}");
        }

        texto.AppendLine();
        texto.AppendLine("VEÍCULOS E CONDUTORES");
        texto.AppendLine($"  Tração: {cadastros.Placa(mdfe.VeiculoTracaoId)}");
        foreach (var id in mdfe.ReboquesIds) texto.AppendLine($"  Reboque: {cadastros.Placa(id)}");
        foreach (var id in mdfe.CondutoresIds)
        {
            var motorista = cadastros.Motorista(id);
            texto.AppendLine(motorista is null ? $"  Condutor: {CadastrosEmissao.NaoEncontrado}" : $"  Condutor: {motorista.Nome} ({Formatador.Cpf(motorista.Cpf)})");
        }

        foreach (var ciot in mdfe.Ciots) texto.AppendLine($"  CIOT: {ciot.Ciot} (responsável {ciot.CpfCnpjResponsavel})");
        if (mdfe.Ciots.Count == 0) texto.AppendLine($"  CIOT: não informado{(mdfe.ExigeCiot ? " (OBRIGATÓRIO)" : string.Empty)}");
        texto.AppendLine();
        texto.AppendLine("CARGA, SEGURO E TOTAIS");
        texto.AppendLine($"  Produto: {OpcoesCadastro.Texto(OpcoesCadastro.TiposCargaMdfe, mdfe.TipoCarga)} - {mdfe.ProdutoPredominante}");
        var apolice = cadastros.Apolice(mdfe.ApoliceId);
        var seguro = mdfe.ApoliceId is null ? "não informado"
            : apolice is null ? CadastrosEmissao.NaoEncontrado
            : $"{apolice.NomeSeguradora}, apólice {apolice.NumeroApolice} ({OpcoesCadastro.Texto(OpcoesCadastro.ResponsaveisSeguro, apolice.Responsavel)})";
        texto.AppendLine($"  Seguro: {seguro}");
        foreach (var averbacao in mdfe.Averbacoes) texto.AppendLine($"  Averbação: {averbacao}");
        texto.AppendLine($"  CT-e: {mdfe.QuantidadeCte}    NF-e: {mdfe.QuantidadeNfe}");
        texto.AppendLine($"  Valor da carga: {(mdfe.ValorCarga ?? 0).ToString("C2", Brasil)}    Peso: {(mdfe.PesoCarga ?? 0).ToString("N4", Brasil)} {OpcoesCadastro.Texto(OpcoesCadastro.UnidadesPesoMdfe, mdfe.UnidadePeso)}");
        return texto.ToString();
    }

    private static string Moeda(decimal valor) => valor.ToString("C2", Brasil);
}
