using Emissor.DFe.Certificados;
using Emissor.DFe;
using Emissor.DFe.Cte;
using Emissor.DFe.Eventos;
using Emissor.DFe.Impressao;
using Emissor.DFe.Mdfe;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.WinForms.Servicos;

/// <summary>Resultado do XML de teste de um evento.</summary>
/// <param name="Resultado">XML assinado e pendências.</param>
/// <param name="DadosFicticios">A chave e o protocolo usados são de teste (documento ainda não autorizado).</param>
public sealed record XmlEventoTeste(XmlGerado Resultado, bool DadosFicticios);

/// <summary>
/// Gera o XML de teste do CT-e e do MDF-e (P4b): lê o certificado da empresa, monta, assina e valida contra os XSD, sem
/// transmitir e sem reservar número. O XML é sempre de homologação (<c>tpAmb</c> 2), mesmo em empresa de produção.
/// </summary>
public sealed class ServicoXmlTeste
{
    private readonly ServicosCadastro _servicos;

    /// <summary>Cria o serviço sobre os serviços de cadastro.</summary>
    /// <param name="servicos">Cadastros, certificados e criptografia.</param>
    public ServicoXmlTeste(ServicosCadastro servicos) => _servicos = servicos;

    /// <summary>Gera o XML do CT-e com o próximo número da série, sem reservá-lo.</summary>
    /// <param name="cte">CT-e lido do banco.</param>
    /// <param name="cadastros">Empresa e cadastros lidos do banco.</param>
    /// <exception cref="RegraNegocioException">Série ou certificado ausente, ou falha ao assinar.</exception>
    public async Task<XmlGerado> GerarCteAsync(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
    {
        var serie = cadastros.Serie(cte.SerieId)
            ?? throw new RegraNegocioException("O CT-e não tem série, ou a série não existe mais.");
        var certificado = await CertificadoValidoAsync(_servicos, cadastros.Empresa);
        var dados = new DadosEmissaoCte(
            cte,
            cadastros.Empresa,
            serie,
            serie.ProximoNumero,
            cadastros.Participantes.ToDictionary(p => p.Id),
            codigo => cadastros.Municipios.Obter(codigo),
            TipoAmbiente.Homologacao,
            new DateTimeOffset(cte.DataEmissao.Date + DateTime.Now.TimeOfDay),
            $"Cte1 {Application.ProductVersion.Split('+')[0]}");

        // Leitura dos XSD e assinatura fora da tela; o A3 pode pedir o PIN.
        var resultado = await Task.Run(() =>
        {
            using var x509 = ProvedoresCertificado.Criar(certificado, _servicos.Protetor).ObterCertificado();
            return GeradorXmlCte.Gerar(dados, x509);
        });
        await _servicos.Auditoria.RegistrarAsync(AcaoAuditoria.XmlTeste, cadastros.Empresa.Id, Resumo("CT-e", cte.Id, resultado));
        return resultado;
    }

    /// <summary>Gera o XML do MDF-e com o próximo número da série, sem reservá-lo.</summary>
    /// <param name="mdfe">MDF-e lido do banco.</param>
    /// <param name="cadastros">Empresa e cadastros lidos do banco.</param>
    /// <exception cref="RegraNegocioException">Série ou certificado ausente, ou falha ao assinar.</exception>
    public async Task<XmlGerado> GerarMdfeAsync(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
    {
        var serie = cadastros.Serie(mdfe.SerieId)
            ?? throw new RegraNegocioException("O MDF-e não tem série, ou a série não existe mais.");
        var certificado = await CertificadoValidoAsync(_servicos, cadastros.Empresa);
        var dados = new DadosEmissaoMdfe(
            mdfe,
            cadastros.Empresa,
            serie,
            serie.ProximoNumero,
            cadastros.Veiculos.ToDictionary(v => v.Id),
            cadastros.Proprietarios.ToDictionary(p => p.Id),
            cadastros.Motoristas.ToDictionary(m => m.Id),
            cadastros.Apolices.ToDictionary(a => a.Id),
            codigo => cadastros.Municipios.Obter(codigo),
            TipoAmbiente.Homologacao,
            new DateTimeOffset(mdfe.DataEmissao.Date + DateTime.Now.TimeOfDay),
            $"Cte1 {Application.ProductVersion.Split('+')[0]}");

        // Leitura dos XSD e assinatura fora da tela; o A3 pode pedir o PIN.
        var resultado = await Task.Run(() =>
        {
            using var x509 = ProvedoresCertificado.Criar(certificado, _servicos.Protetor).ObterCertificado();
            return GeradorXmlMdfe.Gerar(dados, x509);
        });
        await _servicos.Auditoria.RegistrarAsync(AcaoAuditoria.XmlTeste, cadastros.Empresa.Id, Resumo("MDF-e", mdfe.Id, resultado));
        return resultado;
    }

    /// <summary>
    /// Gera o XML de teste de um evento (cancelamento, CC-e, encerramento, inclusão de condutor), sem transmitir e
    /// sem gravar o evento. Enquanto o documento não tem chave e protocolo de autorização, usa uma chave de teste
    /// (série e próximo número, código numérico zero) e o protocolo zerado, só para conferir a estrutura.
    /// </summary>
    /// <param name="evento">Evento com os dados informados na tela.</param>
    /// <param name="documento">CT-e ou MDF-e lido do banco.</param>
    /// <param name="cadastros">Empresa e cadastros lidos do banco.</param>
    /// <exception cref="RegraNegocioException">Série, município ou certificado ausente, ou falha ao assinar.</exception>
    public async Task<XmlEventoTeste> GerarEventoAsync(EventoDocumento evento, DocumentoFiscal documento, CadastrosEmissao cadastros)
    {
        var certificado = await CertificadoValidoAsync(_servicos, cadastros.Empresa);
        var autorizado = ChaveAcesso.Valida(documento.Chave) && documento.Protocolo.Length == 15;
        var chave = autorizado ? documento.Chave : ChaveDeTeste(evento.Tipo.Modelo(), documento, cadastros);
        var protocolo = autorizado ? documento.Protocolo : new string('0', 15);
        var dados = new DadosEvento(evento, cadastros.Empresa, chave, protocolo, TipoAmbiente.Homologacao, DateTimeOffset.Now);

        // Leitura dos XSD e assinatura fora da tela; o A3 pode pedir o PIN.
        var resultado = await Task.Run(() =>
        {
            using var x509 = ProvedoresCertificado.Criar(certificado, _servicos.Protetor).ObterCertificado();
            return GeradorXmlEvento.Gerar(dados, x509);
        });
        var sigla = evento.Tipo.Modelo() == ModeloDocumento.Cte ? "CT-e" : "MDF-e";
        await _servicos.Auditoria.RegistrarAsync(AcaoAuditoria.XmlTeste, cadastros.Empresa.Id,
            resultado.Valido
                ? $"XML de teste do evento {evento.Tipo.Descricao()} ({evento.Tipo.Codigo()}, sequência {evento.Sequencia}) do {sigla} {documento.Id}: válido"
                : $"XML de teste do evento {evento.Tipo.Descricao()} do {sigla} {documento.Id}: {resultado.Pendencias.Count} pendência(s)");
        return new XmlEventoTeste(resultado, !autorizado);
    }

    /// <summary>Chave de teste do documento ainda não autorizado (série e número previstos, código numérico zero).</summary>
    private static string ChaveDeTeste(ModeloDocumento modelo, DocumentoFiscal documento, CadastrosEmissao cadastros)
    {
        var empresa = cadastros.Empresa;
        var serie = cadastros.Serie(documento.SerieId)
            ?? throw new RegraNegocioException("O documento não tem série, ou a série não existe mais.");
        var municipio = cadastros.Municipios.Obter(empresa.Endereco.MunicipioCodigo)
            ?? throw new RegraNegocioException("Informe o município no cadastro da empresa.");
        if (!Formatos.CnpjValido(empresa.Cnpj))
        {
            throw new RegraNegocioException("O XML de teste do evento exige empresa com CNPJ.");
        }

        return ChaveAcesso.Montar(municipio.Codigo / 100_000, documento.DataEmissao, empresa.Cnpj, (int)modelo, serie.Serie,
            documento.Numero ?? serie.ProximoNumero, 1, 0);
    }

    /// <summary>
    /// Gera o DACTE de teste: o XML de teste do CT-e (montado, assinado e validado) e o PDF a partir dele, sempre em
    /// homologação e sem protocolo. O DACTE do CT-e autorizado sai do XML autorizado (P4c2).
    /// </summary>
    /// <param name="cte">CT-e lido do banco.</param>
    /// <param name="cadastros">Empresa e cadastros lidos do banco.</param>
    /// <returns>O XML gerado e o PDF; o PDF é nulo se o XML não passou na validação.</returns>
    /// <exception cref="RegraNegocioException">Série ou certificado ausente, ou falha ao assinar.</exception>
    public async Task<(XmlGerado Xml, byte[]? Pdf)> GerarDacteAsync(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
    {
        var xml = await GerarCteAsync(cte, cadastros);
        if (!xml.Valido)
        {
            return (xml, null);
        }

        var pdf = await Task.Run(() => GeradorDacte.Gerar(DadosDacte.Ler(xml.Xml)));
        await _servicos.Auditoria.RegistrarAsync(AcaoAuditoria.XmlTeste, cadastros.Empresa.Id, $"DACTE de teste do CT-e {cte.Id} (chave {xml.Chave})");
        return (xml, pdf);
    }

    /// <summary>Texto da auditoria do XML de teste.</summary>
    private static string Resumo(string documento, int id, XmlGerado resultado) =>
        resultado.Valido
            ? $"XML de teste do {documento} {id}: válido (chave {resultado.Chave})"
            : $"XML de teste do {documento} {id}: {resultado.Pendencias.Count} pendência(s)";

    /// <summary>Certificado da empresa dentro da validade; o de vencimento mais distante, se houver mais de um.</summary>
    /// <param name="servicos">Cadastros e certificados.</param>
    /// <param name="empresa">Empresa ativa.</param>
    internal static async Task<CertificadoDigital> CertificadoValidoAsync(ServicosCadastro servicos, Empresa empresa)
    {
        var certificados = await servicos.Certificados.ListarAsync(empresa.Id);
        return certificados
            .Where(c => c.SituacaoEm(DateTime.Today) is SituacaoCertificado.Valido or SituacaoCertificado.VencendoEmBreve)
            .OrderByDescending(c => c.ValidoAte)
            .FirstOrDefault()
            ?? throw new RegraNegocioException(
                "A empresa não tem certificado digital dentro da validade. Cadastre um em Cadastros → Certificados digitais.");
    }
}
