using Emissor.Dominio.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Emissor.DFe.Impressao;

/// <summary>
/// DAMDFE em PDF (A4 retrato), conforme o Anexo II do MOC MDF-e (DAMDFE v3.00b) e o modelo do modal rodoviário
/// (item 2.7.1): emitente, identificação com a folha, QR Code, totais, código de barras CODE-128C da chave, protocolo,
/// veículos, condutores, vale-pedágio e observações. Abaixo do leiaute padrão (o manual permite campos adicionais
/// abaixo dele) vêm os documentos vinculados e o CIOT. Em homologação e em contingência imprime o texto obrigatório
/// no lugar do protocolo (itens 2.4 e 2.5).
/// </summary>
public static class GeradorDamdfe
{
    /// <summary>Texto obrigatório do MDF-e de homologação (item 2.5).</summary>
    public const string TextoHomologacao = "EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO – SEM VALOR FISCAL";

    /// <summary>Texto obrigatório do MDF-e em contingência (item 2.4).</summary>
    public const string TextoContingencia = "EMISSÃO EM CONTINGÊNCIA";

    /// <summary>
    /// Portal nacional do MDF-e, onde ficam os endereços de consulta (MOC MDF-e 3.00b, item 9.2.1; conferido em
    /// 26/11/2026: a consulta pública do portal exige login gov.br).
    /// </summary>
    public const string EnderecoConsulta = "https://dfe-portal.svrs.rs.gov.br/mdfe";

    private const string Azul = "#CFE6F7";
    private const string Cinza = "#ECEEF0";
    private const float Linha = 0.6f;

    // Licença Community do QuestPDF: ver GeradorDacte e o levantamento (P5a).
    static GeradorDamdfe() => QuestPDF.Settings.License = LicenseType.Community;

    /// <summary>Gera o PDF do DAMDFE.</summary>
    /// <param name="dados">Conteúdo lido do XML.</param>
    /// <returns>Bytes do PDF.</returns>
    public static byte[] Gerar(DadosDamdfe dados) => Documento(dados).GeneratePdf();

    /// <summary>Documento do DAMDFE (para gerar o PDF ou, nos testes, as imagens das folhas).</summary>
    /// <param name="dados">Conteúdo lido do XML.</param>
    public static IDocument Documento(DadosDamdfe dados) =>
        Document.Create(documento => documento.Page(pagina =>
        {
            pagina.Size(PageSizes.A4);
            pagina.Margin(10, Unit.Millimetre);
            pagina.DefaultTextStyle(estilo => estilo.FontSize(8));
            pagina.Header().Element(c => Cabecalho(c, dados));
            pagina.Content().PaddingTop(6).Element(c => Corpo(c, dados));
            if (dados.Ambiente == TipoAmbiente.Homologacao)
            {
                pagina.Foreground().AlignMiddle().AlignCenter().Rotate(-30)
                    .Text("SEM VALOR FISCAL").FontSize(60).Bold().FontColor("#22B00000");
            }
        }));

    private static void Cabecalho(IContainer container, DadosDamdfe dados) => container.Row(linha =>
    {
        linha.RelativeItem().Column(coluna =>
        {
            coluna.Item().Text(dados.EmitenteNome).FontSize(11).Bold();
            coluna.Item().Text(dados.EmitenteEndereco);
            coluna.Item().Text(dados.EmitenteMunicipio);
            coluna.Item().Text(t =>
            {
                t.Span("CNPJ/CPF: ").Bold();
                t.Span(dados.EmitenteDocumento + "   ");
                t.Span("IE: ").Bold();
                t.Span(dados.EmitenteIe + "   ");
                t.Span("RNTRC: ").Bold();
                t.Span(dados.EmitenteRntrc);
            });
            coluna.Item().PaddingTop(6).Text(t =>
            {
                t.Span("DAMDFE").FontSize(10).Bold();
                t.Span(" - Documento Auxiliar de Manifesto Eletrônico de Documentos Fiscais").FontSize(10);
            });
            coluna.Item().PaddingTop(4).Row(campos =>
            {
                campos.Spacing(3);
                campos.RelativeItem(4).Background(Azul).Padding(3).Row(grupo =>
                {
                    Campo(grupo.RelativeItem(), "Modelo", dados.Modelo);
                    Campo(grupo.RelativeItem(), "Série", dados.Serie);
                    Campo(grupo.RelativeItem(), "Número", dados.Numero);
                });
                campos.RelativeItem(1).Background(Azul).Padding(3).Column(folha =>
                {
                    folha.Item().Text("FL").FontSize(6.5f);
                    folha.Item().Text(t =>
                    {
                        t.CurrentPageNumber().FontSize(10);
                        t.Span("/").FontSize(10);
                        t.TotalPages().FontSize(10);
                    });
                });
                Campo(campos.RelativeItem(4).Background(Azul).Padding(3), "Data e hora de emissão", dados.DataHoraEmissao, negrito: true);
                Campo(campos.RelativeItem(2).Background(Azul).Padding(3), "UF Carreg.", dados.UfCarregamento, negrito: true);
                Campo(campos.RelativeItem(2).Background(Azul).Padding(3), "UF Descarreg.", dados.UfDescarregamento, negrito: true);
            });
        });

        if (ImpressaoComum.QrCodePng(dados.QrCode) is { } qrCode)
        {
            linha.ConstantItem(32, Unit.Millimetre).AlignRight().Width(30, Unit.Millimetre).Height(30, Unit.Millimetre).Image(qrCode).FitArea();
        }
    });

    private static void Corpo(IContainer container, DadosDamdfe dados) => container.Column(coluna =>
    {
        coluna.Spacing(8);
        coluna.Item().Row(linha =>
        {
            linha.Spacing(12);
            linha.RelativeItem().Column(esquerda =>
            {
                esquerda.Item().Text("Modelo Rodoviário de Carga").FontSize(11).Bold();
                esquerda.Item().PaddingTop(4).Row(totais =>
                {
                    totais.Spacing(3);
                    Campo(totais.RelativeItem().Background(Cinza).Padding(3), "Qtd. CTe", dados.QuantidadeCte, negrito: true);
                    Campo(totais.RelativeItem().Background(Cinza).Padding(3), "Qtd. NFe", dados.QuantidadeNfe, negrito: true);
                    Campo(totais.RelativeItem().Background(Cinza).Padding(3), "Peso total (Kg)", dados.PesoTotalKg, negrito: true);
                });
                esquerda.Item().PaddingTop(10).Text("Protocolo de autorização").Bold();
                if (dados.Ambiente == TipoAmbiente.Homologacao)
                {
                    esquerda.Item().AlignCenter().Text(GeradorDamdfe.TextoHomologacao).Bold();
                }

                if (dados.Contingencia)
                {
                    esquerda.Item().AlignCenter().Text(TextoContingencia).FontSize(11).Bold();
                }

                esquerda.Item().Text(dados.Protocolo.Length > 0 ? dados.Protocolo : "Sem protocolo de autorização (documento não autorizado)");
            });
            linha.RelativeItem().Column(direita =>
            {
                direita.Item().Text("CONTROLE DO FISCO").FontSize(6.5f);
                if (dados.Chave.Length == 44 && dados.Chave.All(char.IsAsciiDigit))
                {
                    direita.Item().Width(9, Unit.Centimetre).Height(1.6f, Unit.Centimetre).Svg(CodigoBarras128C.Svg(dados.Chave));
                }

                direita.Item().PaddingTop(4).Text("Chave de Acesso").Bold();
                direita.Item().Text(dados.ChaveFormatada);
                direita.Item().Text(t =>
                {
                    t.Span("Consulte em ").FontSize(7);
                    t.Span(EnderecoConsulta).Bold();
                });
            });
        });

        coluna.Item().Row(linha =>
        {
            linha.Spacing(12);
            linha.RelativeItem().Column(esquerda =>
            {
                esquerda.Item().Element(c => Titulo(c, "Veículo"));
                esquerda.Item().Element(c => Tabela(c, ["Placa", "RNTRC"], [1, 1], dados.Veiculos.Select(v => new[] { v.Placa, v.Rntrc })));
                if (dados.ValesPedagio.Count > 0)
                {
                    esquerda.Item().PaddingTop(8).Element(c => Titulo(c, "Vale Pedágio"));
                    esquerda.Item().Element(c => Tabela(c, ["Responsável CNPJ", "Fornecedor CNPJ", "Nº Comprovante"], [3, 3, 4],
                        dados.ValesPedagio.Select(v => new[] { v.Responsavel, v.Fornecedor, v.Comprovante }), pequeno: true));
                }
            });
            linha.RelativeItem().Column(direita =>
            {
                direita.Item().Element(c => Titulo(c, "Condutor"));
                direita.Item().Element(c => Tabela(c, ["CPF", "Nome"], [3, 5], dados.Condutores.Select(d => new[] { d.Cpf, d.Nome })));
            });
        });

        coluna.Item().Column(observacoes =>
        {
            observacoes.Item().Text("Observações").Bold();
            observacoes.Item().LineHorizontal(Linha).LineColor(Colors.Grey.Medium);
            observacoes.Item().PaddingTop(2).Text(dados.Observacoes);
        });

        // Campos adicionais, abaixo do leiaute padrão (Anexo II, item 2.1).
        if (dados.Ciots.Count > 0)
        {
            coluna.Item().Column(ciot =>
            {
                ciot.Item().Element(c => Titulo(c, "CIOT"));
                ciot.Item().Element(c => Tabela(c, ["CIOT", "Responsável (CPF/CNPJ)"], [1, 1], dados.Ciots.Select(x => new[] { x.Ciot, x.Responsavel })));
            });
        }

        if (dados.Documentos.Count > 0)
        {
            coluna.Item().Column(documentos =>
            {
                documentos.Item().Element(c => Titulo(c, $"Documentos vinculados ({dados.Documentos.Count})"));
                documentos.Item().Element(c => Tabela(c, ["Tipo", "Chave de acesso", "Município de descarregamento"], [1, 6, 3],
                    dados.Documentos.Select(d => new[] { d.Tipo, d.Chave, d.MunicipioDescarga }), pequeno: true));
            });
        }
    });

    private static void Titulo(IContainer container, string titulo) => container.PaddingBottom(2).Text(titulo).FontSize(11).Bold();

    private static void Campo(IContainer container, string rotulo, string valor, bool negrito = false) => container.Column(coluna =>
    {
        coluna.Item().Text(rotulo).FontSize(6.5f);
        var texto = coluna.Item().Text(valor).FontSize(10);
        if (negrito)
        {
            texto.Bold();
        }
    });

    private static void Tabela(IContainer container, string[] cabecalhos, float[] pesos, IEnumerable<string[]> linhas, bool pequeno = false) =>
        container.Table(tabela =>
        {
            tabela.ColumnsDefinition(colunas =>
            {
                foreach (var peso in pesos)
                {
                    colunas.RelativeColumn(peso);
                }
            });
            tabela.Header(cabecalho =>
            {
                foreach (var titulo in cabecalhos)
                {
                    cabecalho.Cell().BorderBottom(Linha).BorderColor(Colors.Grey.Medium).PaddingBottom(1).Text(titulo).FontSize(7);
                }
            });
            foreach (var linha in linhas)
            {
                foreach (var celula in linha)
                {
                    var texto = tabela.Cell().BorderBottom(0.4f).BorderColor(Colors.Grey.Lighten1).PaddingVertical(2).Text(celula);
                    if (pequeno)
                    {
                        texto.FontSize(7.5f);
                    }
                    else
                    {
                        texto.FontSize(10).Bold();
                    }
                }
            }
        });
}
