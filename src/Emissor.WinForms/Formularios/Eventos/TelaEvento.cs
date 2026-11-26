using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Eventos;

/// <summary>
/// Fluxo comum dos eventos nas consultas de CT-e e MDF-e: calcula a sequência, abre a tela do evento, gera o XML de
/// teste (assinado e validado, sem enviar à SEFAZ) e oferece salvar o arquivo.
/// </summary>
internal static class TelaEvento
{
    /// <summary>Aviso mostrado nas telas enquanto a transmissão dos eventos não existe.</summary>
    public const string AvisoTeste =
        "Por enquanto o evento só gera o XML de teste (assinado e validado contra o XSD), sem enviar à SEFAZ e sem " +
        "gravar. Enquanto o documento não for autorizado, a chave e o protocolo do XML são de teste.";

    /// <summary>Confere os dados do evento e mostra as pendências; verdadeiro se pode seguir.</summary>
    /// <param name="dono">Tela do evento.</param>
    /// <param name="evento">Evento preenchido.</param>
    public static bool Conferir(IWin32Window dono, EventoDocumento evento) =>
        !AvisosCadastro.HaPendencias(dono, evento.Tipo.Descricao(), RegrasEvento.ValidarDados(evento));

    /// <summary>Abre a tela do evento para o documento e gera o XML de teste.</summary>
    /// <param name="dono">Consulta que chamou.</param>
    /// <param name="cadastros">Cadastros da empresa, recém-lidos do banco.</param>
    /// <param name="documento">CT-e ou MDF-e lido do banco.</param>
    /// <param name="tipo">Tipo do evento.</param>
    /// <param name="descricaoDocumento">Documento para o título da tela (ex.: "CT-e 12 de 25/11/2026").</param>
    public static async Task ExecutarAsync(Form dono, CadastrosEmissao cadastros, DocumentoFiscal documento, TipoEvento tipo, string descricaoDocumento)
    {
        var sequencia = await cadastros.Eventos.ProximaSequenciaAsync(tipo, documento.Id);
        if (sequencia > tipo.SequenciaMaxima())
        {
            MessageBox.Show(dono, tipo.SequenciaMaxima() == 1
                    ? $"{tipo.Descricao()} já foi registrado para este documento."
                    : $"O documento já tem o máximo de {tipo.SequenciaMaxima()} eventos de {tipo.Descricao().ToLowerInvariant()}.",
                tipo.Descricao(), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var evento = new EventoDocumento
        {
            EmpresaId = documento.EmpresaId,
            Tipo = tipo,
            DocumentoId = documento.Id,
            Sequencia = sequencia,
            DataHora = DateTime.Now,
        };

        using Form tela = tipo switch
        {
            TipoEvento.CartaCorrecaoCte => new FormCartaCorrecao(evento, descricaoDocumento),
            TipoEvento.EncerramentoMdfe => new FormEncerramentoMdfe(evento, descricaoDocumento, cadastros.Municipios,
                documento as ManifestoEletronico),
            TipoEvento.InclusaoCondutorMdfe => new FormInclusaoCondutor(evento, descricaoDocumento, cadastros.Motoristas),
            _ => new FormEventoCancelamento(evento, descricaoDocumento),
        };
        if (tela.ShowDialog(dono) != DialogResult.OK)
        {
            return;
        }

        dono.UseWaitCursor = true;
        XmlEventoTeste gerado;
        try
        {
            gerado = await cadastros.XmlTeste.GerarEventoAsync(evento, documento, cadastros);
        }
        finally
        {
            dono.UseWaitCursor = false;
        }

        await MostrarAsync(dono, evento, gerado);
    }

    private static async Task MostrarAsync(Form dono, EventoDocumento evento, XmlEventoTeste gerado)
    {
        var titulo = evento.Tipo.Descricao();
        var resultado = gerado.Resultado;
        if (resultado.Xml.Length == 0)
        {
            AvisosCadastro.HaPendencias(dono, titulo, resultado.Pendencias);
            return;
        }

        var linha = Environment.NewLine;
        var aviso = gerado.DadosFicticios
            ? $"O documento ainda não foi autorizado: a chave e o protocolo são de teste.{linha}"
            : string.Empty;
        var mensagem = resultado.Valido
            ? $"XML do evento assinado e válido contra o XSD.{linha}{linha}Evento: {evento.Tipo.Codigo()} — sequência {evento.Sequencia}{linha}" +
              $"Chave do documento: {resultado.Chave}{linha}Ambiente: homologação{linha}{linha}{aviso}" +
              $"Nada foi enviado à SEFAZ nem gravado.{linha}{linha}Deseja salvar o arquivo?"
            : $"O XML do evento foi gerado, mas não passou na validação do XSD:{linha}{linha}" +
              string.Join(linha, resultado.Pendencias.Take(10).Select(p => $"• {p}")) +
              (resultado.Pendencias.Count > 10 ? $"{linha}• ... e mais {resultado.Pendencias.Count - 10}" : string.Empty) +
              $"{linha}{linha}Deseja salvar o arquivo para análise?";
        if (MessageBox.Show(dono, mensagem, titulo, MessageBoxButtons.YesNo,
                resultado.Valido ? MessageBoxIcon.Information : MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        using var dialogo = new SaveFileDialog
        {
            Title = titulo,
            Filter = "Arquivo XML (*.xml)|*.xml",
            FileName = $"{resultado.Chave}-{evento.Tipo.Codigo()}-{evento.Sequencia:000}-teste-evento.xml",
        };
        if (dialogo.ShowDialog(dono) == DialogResult.OK)
        {
            await File.WriteAllTextAsync(dialogo.FileName, resultado.Xml, new System.Text.UTF8Encoding(false));
        }
    }
}
