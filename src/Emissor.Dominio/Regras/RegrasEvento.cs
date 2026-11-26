using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;

namespace Emissor.Dominio.Regras;

/// <summary>
/// Regras dos eventos conferidas pelo emissor antes da SEFAZ, conforme o MOC CT-e 4.00 (itens 6.3, 6.4 e 13) e o MOC
/// MDF-e 3.00b (eventos 110111, 110112 e 110114). A SEFAZ continua sendo quem decide: aqui só se evita enviar o que
/// certamente seria rejeitado.
/// </summary>
public static class RegrasEvento
{
    /// <summary>Tamanho mínimo da justificativa (<c>TJust</c>).</summary>
    public const int JustificativaMinima = 15;

    /// <summary>Tamanho máximo da justificativa (<c>TJust</c>).</summary>
    public const int JustificativaMaxima = 255;

    /// <summary>Prazo de cancelamento do CT-e, contado da autorização (rejeição 220).</summary>
    public static readonly TimeSpan PrazoCancelamentoCte = TimeSpan.FromHours(168);

    /// <summary>Prazo de cancelamento do MDF-e, contado da autorização (rejeição 220).</summary>
    public static readonly TimeSpan PrazoCancelamentoMdfe = TimeSpan.FromHours(24);

    /// <summary>
    /// Campos que a carta de correção do CT-e modelo 57 não pode alterar (MOC CT-e 4.00, item 13.1; rejeição 681),
    /// por grupo do leiaute. A comparação ignora maiúsculas e minúsculas.
    /// </summary>
    public static IReadOnlyDictionary<string, IReadOnlySet<string>> CamposProibidosCce { get; } =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["infCte"] = Conjunto("versao", "Id"),
            ["ide"] = Conjunto("cUF", "cCT", "mod", "serie", "nCT", "tpEmis", "cDV", "tpAmb", "dhEmi", "modal"),
            ["toma3"] = Conjunto("toma"),
            ["toma4"] = Conjunto("CNPJ", "CPF", "IE"),
            ["emit"] = Conjunto("CNPJ", "IE"),
            ["rem"] = Conjunto("CNPJ", "CPF", "IE"),
            ["dest"] = Conjunto("CNPJ", "CPF", "IE"),
            ["vPrest"] = Conjunto("vTPrest"),
            ["Comp"] = Conjunto("vComp"),
            ["vPresComp"] = Conjunto("vTPrest"),
            ["compComp"] = Conjunto("vComp"),
            ["infNFe"] = Conjunto("chave"),
            ["ICMS00"] = Conjunto("CST", "vBC", "pICMS", "vICMS"),
            ["ICMS20"] = Conjunto("CST", "pRedBC", "vBC", "pICMS", "vICMS"),
            ["ICMS45"] = Conjunto("CST"),
            ["ICMS60"] = Conjunto("CST", "vBCSTRet", "vICMSSTRet", "pICMSSTRet", "vCred"),
            ["ICMS90"] = Conjunto("CST", "pRedBC", "vBC", "pICMS", "vICMS", "vCred"),
            ["ICMSOutraUF"] = Conjunto("CST", "pRedBCOutraUF", "vBCOutraUF", "pICMSOutraUF", "vICMSOutraUF"),
            ["ICMSSN"] = Conjunto("indSN", "CST"),
        };

    /// <summary>Texto sem espaços nas pontas nem repetidos, como vai no XML.</summary>
    /// <param name="texto">Texto digitado.</param>
    public static string Normalizar(string? texto) =>
        string.Join(' ', (texto ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    /// <summary>Indica se a carta de correção pode alterar o campo do grupo.</summary>
    /// <param name="grupo">Grupo do leiaute.</param>
    /// <param name="campo">Campo do leiaute.</param>
    public static bool CampoPermitidoNaCce(string grupo, string campo) =>
        !(CamposProibidosCce.TryGetValue(grupo.Trim(), out var campos) && campos.Contains(campo.Trim()));

    /// <summary>Confere os dados informados no evento (independente da situação do documento).</summary>
    /// <param name="evento">Evento a conferir.</param>
    /// <returns>Pendências; vazia quando o evento pode ser montado.</returns>
    public static IReadOnlyList<string> ValidarDados(EventoDocumento evento)
    {
        var pendencias = new List<string>();
        var maxima = evento.Tipo.SequenciaMaxima();
        if (evento.Sequencia < 1 || evento.Sequencia > maxima)
        {
            pendencias.Add(maxima == 1
                ? $"{evento.Tipo.Descricao()} só pode ser feito uma vez por documento."
                : $"O número sequencial do evento deve ficar entre 1 e {maxima} (rejeição 636).");
        }

        if (evento.DocumentoId <= 0)
        {
            pendencias.Add("Informe o documento do evento.");
        }

        switch (evento.Tipo)
        {
            case TipoEvento.CancelamentoCte or TipoEvento.CancelamentoMdfe:
                ValidarJustificativa(evento.Justificativa, pendencias);
                break;
            case TipoEvento.CartaCorrecaoCte:
                ValidarCorrecoes(evento.Correcoes, pendencias);
                break;
            case TipoEvento.EncerramentoMdfe:
                if (evento.DataEncerramento is null)
                {
                    pendencias.Add("Informe a data do encerramento.");
                }
                else if (evento.DataEncerramento.Value.Date > DateTime.Today)
                {
                    pendencias.Add("A data do encerramento não pode ser futura.");
                }

                if (evento.MunicipioEncerramentoCodigo is not (>= 1_000_000 and <= 9_999_999))
                {
                    pendencias.Add("Informe o município do encerramento.");
                }

                break;
            case TipoEvento.InclusaoCondutorMdfe:
                var nome = Normalizar(evento.NomeCondutor);
                if (nome.Length is < 2 or > 60)
                {
                    pendencias.Add("Informe o nome do condutor (2 a 60 caracteres).");
                }

                if (!Formatos.CpfValido(evento.CpfCondutor))
                {
                    pendencias.Add("Informe o CPF do condutor com 11 dígitos.");
                }

                break;
        }

        return pendencias;
    }

    /// <summary>
    /// Confere se o documento está numa situação que aceita o evento e se o prazo não venceu. Usado antes da
    /// transmissão (P4d2); o XML de teste não passa por aqui, porque o documento ainda não foi autorizado.
    /// </summary>
    /// <param name="tipo">Tipo do evento.</param>
    /// <param name="documento">CT-e ou MDF-e.</param>
    /// <param name="registrados">Eventos já registrados do documento.</param>
    /// <param name="agora">Data e hora atuais.</param>
    /// <returns>Pendências; vazia quando o evento pode ser transmitido.</returns>
    public static IReadOnlyList<string> ValidarDocumento(TipoEvento tipo, DocumentoFiscal documento, IEnumerable<EventoDocumento> registrados,
        DateTime agora)
    {
        var pendencias = new List<string>();
        if (documento.Situacao != SituacaoDocumento.Autorizado)
        {
            pendencias.Add(documento.Situacao switch
            {
                SituacaoDocumento.Cancelado => "O documento já está cancelado.",
                SituacaoDocumento.Encerrado => "O MDF-e já está encerrado.",
                _ => "O evento exige documento autorizado pela SEFAZ.",
            });
            return pendencias;
        }

        if (documento.Chave.Length != 44 || documento.Protocolo.Length != 15)
        {
            pendencias.Add("O documento não tem chave de acesso ou protocolo de autorização gravados.");
        }

        var registradosDoDocumento = registrados.Where(e => e.Situacao == SituacaoEvento.Registrado).ToList();
        switch (tipo)
        {
            case TipoEvento.CancelamentoCte:
                VerificarPrazo(documento, agora, PrazoCancelamentoCte, "7 dias (168 horas)", pendencias);
                if (registradosDoDocumento.Any(e => e.Tipo == TipoEvento.CartaCorrecaoCte))
                {
                    pendencias.Add("CT-e com carta de correção registrada não pode ser cancelado (rejeição 523).");
                }

                break;
            case TipoEvento.CancelamentoMdfe:
                VerificarPrazo(documento, agora, PrazoCancelamentoMdfe, "24 horas", pendencias);
                break;
            case TipoEvento.CartaCorrecaoCte:
                if (registradosDoDocumento.Count(e => e.Tipo == TipoEvento.CartaCorrecaoCte) >= tipo.SequenciaMaxima())
                {
                    pendencias.Add("O CT-e já tem o máximo de 20 cartas de correção (rejeição 636).");
                }

                break;
        }

        return pendencias;
    }

    /// <summary>Próximo número sequencial do tipo de evento para o documento.</summary>
    /// <param name="tipo">Tipo do evento.</param>
    /// <param name="existentes">Eventos já gravados do documento.</param>
    public static int ProximaSequencia(TipoEvento tipo, IEnumerable<EventoDocumento> existentes) =>
        existentes.Where(e => e.Tipo == tipo && e.Situacao != SituacaoEvento.Rejeitado).Select(e => e.Sequencia).DefaultIfEmpty(0).Max() + 1;

    private static void ValidarJustificativa(string justificativa, List<string> pendencias)
    {
        var texto = Normalizar(justificativa);
        if (texto.Length is < JustificativaMinima or > JustificativaMaxima)
        {
            pendencias.Add($"A justificativa deve ter de {JustificativaMinima} a {JustificativaMaxima} caracteres (tem {texto.Length}).");
        }
    }

    private static void ValidarCorrecoes(IReadOnlyList<CorrecaoCte> correcoes, List<string> pendencias)
    {
        if (correcoes.Count == 0)
        {
            pendencias.Add("Informe ao menos uma correção.");
            return;
        }

        for (var i = 0; i < correcoes.Count; i++)
        {
            var correcao = correcoes[i];
            var linha = $"Correção {i + 1}";
            var grupo = correcao.Grupo.Trim();
            var campo = correcao.Campo.Trim();
            if (grupo.Length is < 1 or > 20 || campo.Length is < 1 or > 20)
            {
                pendencias.Add($"{linha}: informe o grupo e o campo do leiaute (até 20 caracteres cada).");
            }
            else if (!CampoPermitidoNaCce(grupo, campo))
            {
                pendencias.Add($"{linha}: o campo {grupo}/{campo} não pode ser alterado por carta de correção (rejeição 681).");
            }

            var valor = Normalizar(correcao.Valor);
            if (valor.Length is < 1 or > 500)
            {
                pendencias.Add($"{linha}: informe o valor correto (até 500 caracteres).");
            }

            if (correcao.NumeroItem is < 1 or > 99)
            {
                pendencias.Add($"{linha}: o número do item deve ficar entre 1 e 99 (rejeição 522).");
            }
        }
    }

    private static void VerificarPrazo(DocumentoFiscal documento, DateTime agora, TimeSpan prazo, string descricao, List<string> pendencias)
    {
        if (documento.AutorizadoEm is { } autorizado && agora - autorizado > prazo)
        {
            pendencias.Add($"O prazo de cancelamento ({descricao} da autorização) venceu (rejeição 220).");
        }
    }

    private static IReadOnlySet<string> Conjunto(params string[] campos) => new HashSet<string>(campos, StringComparer.OrdinalIgnoreCase);
}
