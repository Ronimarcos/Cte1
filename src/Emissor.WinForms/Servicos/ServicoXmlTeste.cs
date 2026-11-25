using Emissor.DFe.Certificados;
using Emissor.DFe;
using Emissor.DFe.Cte;
using Emissor.DFe.Mdfe;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;

namespace Emissor.WinForms.Servicos;

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
        var certificado = await CertificadoValidoAsync(cadastros.Empresa);
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
        return await Task.Run(() =>
        {
            using var x509 = ProvedoresCertificado.Criar(certificado, _servicos.Protetor).ObterCertificado();
            return GeradorXmlCte.Gerar(dados, x509);
        });
    }

    /// <summary>Gera o XML do MDF-e com o próximo número da série, sem reservá-lo.</summary>
    /// <param name="mdfe">MDF-e lido do banco.</param>
    /// <param name="cadastros">Empresa e cadastros lidos do banco.</param>
    /// <exception cref="RegraNegocioException">Série ou certificado ausente, ou falha ao assinar.</exception>
    public async Task<XmlGerado> GerarMdfeAsync(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
    {
        var serie = cadastros.Serie(mdfe.SerieId)
            ?? throw new RegraNegocioException("O MDF-e não tem série, ou a série não existe mais.");
        var certificado = await CertificadoValidoAsync(cadastros.Empresa);
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
        return await Task.Run(() =>
        {
            using var x509 = ProvedoresCertificado.Criar(certificado, _servicos.Protetor).ObterCertificado();
            return GeradorXmlMdfe.Gerar(dados, x509);
        });
    }

    /// <summary>Certificado da empresa dentro da validade; o de vencimento mais distante, se houver mais de um.</summary>
    private async Task<CertificadoDigital> CertificadoValidoAsync(Empresa empresa)
    {
        var certificados = await _servicos.Certificados.ListarAsync(empresa.Id);
        return certificados
            .Where(c => c.SituacaoEm(DateTime.Today) is SituacaoCertificado.Valido or SituacaoCertificado.VencendoEmBreve)
            .OrderByDescending(c => c.ValidoAte)
            .FirstOrDefault()
            ?? throw new RegraNegocioException(
                "A empresa não tem certificado digital dentro da validade. Cadastre um em Cadastros → Certificados digitais.");
    }
}
