using Emissor.DFe.Certificados;
using Emissor.DFe.Webservices;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.WinForms.Servicos;

/// <summary>Resultado da consulta de um serviço: o status ou o motivo da falha.</summary>
/// <param name="Documento">"CT-e" ou "MDF-e".</param>
/// <param name="Status">Status devolvido pela SEFAZ, ou <c>null</c> se a consulta falhou.</param>
/// <param name="Erro">Motivo da falha, ou <c>null</c>.</param>
public sealed record ResultadoStatusSefaz(string Documento, StatusServico? Status, string? Erro)
{
    /// <summary>Texto para a mensagem da tela.</summary>
    public string Descricao => Status is { } status
        ? $"{Documento}: {status.Codigo} - {status.Motivo}{Environment.NewLine}" +
          $"  Servidor: {status.Servidor} (UF {status.UfAtendida}, aplicativo {status.VersaoAplicativo}){Environment.NewLine}" +
          $"  Recebido em: {status.Recebimento}" +
          (status.TempoMedio is null ? string.Empty : $"    Tempo médio: {status.TempoMedio} s") +
          (status.Observacao is null ? string.Empty : $"{Environment.NewLine}  Observação: {status.Observacao}")
        : $"{Documento}: não consultado.{Environment.NewLine}  {Erro}";
}

/// <summary>
/// Comunicação com a SEFAZ a partir das telas (P4c1): status do serviço do CT-e e do MDF-e, sempre em homologação,
/// com o certificado válido da empresa. A chamada roda fora da thread da tela (o A3 pode pedir o PIN).
/// </summary>
public sealed class ServicoSefaz
{
    private readonly ServicosCadastro _servicos;
    private readonly CatalogoMunicipios _municipios;

    /// <summary>Cria o serviço.</summary>
    /// <param name="servicos">Cadastros, certificados e criptografia.</param>
    /// <param name="municipios">Municípios do IBGE (para a UF da empresa).</param>
    public ServicoSefaz(ServicosCadastro servicos, CatalogoMunicipios municipios)
    {
        _servicos = servicos;
        _municipios = municipios;
    }

    /// <summary>Consulta o status do CT-e (autorizador da UF da empresa) e do MDF-e (SVRS) em homologação.</summary>
    /// <param name="empresa">Empresa ativa.</param>
    /// <exception cref="RegraNegocioException">Empresa sem município ou sem certificado válido.</exception>
    public async Task<IReadOnlyList<ResultadoStatusSefaz>> ConsultarStatusAsync(Empresa empresa)
    {
        var municipio = _municipios.Obter(empresa.Endereco.MunicipioCodigo)
            ?? throw new RegraNegocioException("A empresa não tem município cadastrado (a UF define o autorizador do CT-e).");
        var certificado = await ServicoXmlTeste.CertificadoValidoAsync(_servicos, empresa);

        var resultados = await Task.Run(async () =>
        {
            using var x509 = ProvedoresCertificado.Criar(certificado, _servicos.Protetor).ObterCertificado();
            using var cliente = ClienteHttpSefaz.Criar(x509, TimeSpan.FromSeconds(30));
            return (IReadOnlyList<ResultadoStatusSefaz>)
            [
                await Consultar("CT-e", () => ServicoStatus.ConsultarCteAsync(cliente, municipio.Uf, municipio.Codigo / 100_000, TipoAmbiente.Homologacao)),
                await Consultar("MDF-e", () => ServicoStatus.ConsultarMdfeAsync(cliente, municipio.Uf, TipoAmbiente.Homologacao)),
            ];
        });
        await _servicos.Auditoria.RegistrarAsync(AcaoAuditoria.ConsultaSefaz, empresa.Id,
            "Status da SEFAZ (homologação): " + string.Join("; ", resultados.Select(r => r.Status is { } s ? $"{r.Documento} {s.Codigo}" : $"{r.Documento} sem resposta")));
        return resultados;
    }

    private static async Task<ResultadoStatusSefaz> Consultar(string documento, Func<Task<StatusServico>> consulta)
    {
        try
        {
            return new ResultadoStatusSefaz(documento, await consulta(), null);
        }
        catch (RegraNegocioException erro)
        {
            return new ResultadoStatusSefaz(documento, null, string.Join(" ", erro.Pendencias));
        }
    }
}
