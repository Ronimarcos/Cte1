using Emissor.Dominio.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Emissor.DFe.Impressao;

/// <summary>
/// DACTE em PDF (A4 retrato), conforme o Anexo II do MOC CT-e (DACTE v3.00a) e o modelo do modal rodoviário (item
/// 5.1). Folha n/m em todas as folhas; documentos originários que não cabem seguem nas folhas seguintes, com o
/// cabeçalho repetido. Em homologação imprime "EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL" (item 2.19).
/// </summary>
public static class GeradorDacte
{
    /// <summary>Texto obrigatório do CT-e de homologação (item 2.19).</summary>
    public const string TextoHomologacao = "EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL";

    /// <summary>Endereço de consulta pela chave indicado no MOC CT-e 4.00 (item 10.1): o Portal do CT-e da SVRS.</summary>
    public const string EnderecoConsulta = "https://dfe-portal.svrs.rs.gov.br/cte";

    private const float Borda = 0.6f;

    // Licença Community do QuestPDF: empresa com faturamento bruto anual abaixo de US$ 1 milhão (decisão registrada
    // no levantamento, P5a). Se isso mudar, é preciso adquirir a licença Professional.
    static GeradorDacte() => QuestPDF.Settings.License = LicenseType.Community;

    /// <summary>Gera o PDF do DACTE.</summary>
    /// <param name="dados">Conteúdo lido do XML.</param>
    /// <returns>Bytes do PDF.</returns>
    public static byte[] Gerar(DadosDacte dados) => Documento(dados).GeneratePdf();

    /// <summary>Documento do DACTE (para gerar o PDF ou, nos testes, as imagens das folhas).</summary>
    /// <param name="dados">Conteúdo lido do XML.</param>
    public static IDocument Documento(DadosDacte dados) =>
        Document.Create(documento => documento.Page(pagina =>
        {
            pagina.Size(PageSizes.A4);
            pagina.Margin(8, Unit.Millimetre);
            pagina.DefaultTextStyle(estilo => estilo.FontSize(7));
            pagina.Header().Element(c => Cabecalho(c, dados));
            pagina.Content().PaddingTop(2).Element(c => Corpo(c, dados));
            if (dados.Ambiente == TipoAmbiente.Homologacao)
            {
                pagina.Foreground().AlignMiddle().AlignCenter().Rotate(-30)
                    .Text("SEM VALOR FISCAL").FontSize(60).Bold().FontColor("#22B00000");
            }
        }));

    private static void Cabecalho(IContainer container, DadosDacte dados) => container.Column(coluna =>
    {
        coluna.Item().ShowOnce().Element(c => Canhoto(c, dados));
        coluna.Item().Row(linha =>
        {
            linha.RelativeItem(4).Border(Borda).Padding(3).Column(emitente =>
            {
                emitente.Item().Text(dados.Emitente.Nome).FontSize(10).Bold();
                emitente.Item().Text(dados.Emitente.Endereco);
                emitente.Item().Text($"{dados.Emitente.Municipio} - {dados.Emitente.Uf}   CEP {dados.Emitente.Cep}");
                emitente.Item().Text($"CNPJ/CPF: {dados.Emitente.Documento}   IE: {dados.Emitente.Ie}");
                if (dados.Emitente.Fone.Length > 0)
                {
                    emitente.Item().Text($"Fone: {dados.Emitente.Fone}");
                }
            });
            linha.RelativeItem(6).Border(Borda).Column(identificacao =>
            {
                identificacao.Item().Padding(2).AlignCenter().Text(t =>
                {
                    t.Span("DACTE").FontSize(11).Bold();
                    t.Span("  Documento Auxiliar do Conhecimento de Transporte Eletrônico").FontSize(7);
                });
                identificacao.Item().Row(campos =>
                {
                    Campo(campos.RelativeItem(3), "MODAL", dados.Modal);
                    Campo(campos.RelativeItem(2), "MODELO", dados.Modelo);
                    Campo(campos.RelativeItem(2), "SÉRIE", dados.Serie);
                    Campo(campos.RelativeItem(3), "NÚMERO", dados.Numero);
                    campos.RelativeItem(3).Border(Borda).Padding(2).Column(folha =>
                    {
                        folha.Item().Text("FOLHA").FontSize(5.5f);
                        folha.Item().Text(t =>
                        {
                            t.CurrentPageNumber().Bold();
                            t.Span("/").Bold();
                            t.TotalPages().Bold();
                        });
                    });
                    Campo(campos.RelativeItem(5), "DATA E HORA DE EMISSÃO", dados.DataHoraEmissao);
                });
                identificacao.Item().Padding(3).Column(barras =>
                {
                    if (dados.Chave.Length == 44 && dados.Chave.All(char.IsAsciiDigit))
                    {
                        barras.Item().AlignCenter().Width(9, Unit.Centimetre).Height(1.6f, Unit.Centimetre)
                            .Svg(CodigoBarras128C.Svg(dados.Chave));
                    }

                    barras.Item().AlignCenter().Text("CHAVE DE ACESSO").FontSize(5.5f);
                    barras.Item().AlignCenter().Text(dados.ChaveFormatada).FontSize(8.5f).Bold();
                });
            });
        });
        coluna.Item().Row(variaveis =>
        {
            variaveis.RelativeItem().Border(Borda).Padding(3).Column(campos =>
            {
                if (dados.Ambiente == TipoAmbiente.Homologacao)
                {
                    campos.Item().AlignCenter().Text(TextoHomologacao).FontSize(9).Bold();
                }

                campos.Item().Text("Consulta de autenticidade pela chave de acesso no Portal do CT-e:").FontSize(6);
                campos.Item().Text(EnderecoConsulta).Bold();
                campos.Item().PaddingTop(3).Text("PROTOCOLO DE AUTORIZAÇÃO DE USO").FontSize(5.5f);
                campos.Item().Text(dados.Protocolo.Length > 0 ? dados.Protocolo : "SEM PROTOCOLO DE AUTORIZAÇÃO (DOCUMENTO NÃO AUTORIZADO)").Bold();
            });
            if (ImpressaoComum.QrCodePng(dados.QrCode) is { } qrCode)
            {
                variaveis.ConstantItem(27, Unit.Millimetre).Border(Borda).Padding(1).Width(25, Unit.Millimetre).Height(25, Unit.Millimetre)
                    .Image(qrCode).FitArea();
            }
        });
    });

    private static void Corpo(IContainer container, DadosDacte dados) => container.Column(coluna =>
    {
        coluna.Item().Row(linha =>
        {
            Campo(linha.RelativeItem(), "TIPO DO CT-E", dados.TipoCte);
            Campo(linha.RelativeItem(), "TIPO DO SERVIÇO", dados.TipoServico);
            Campo(linha.RelativeItem(), "CT-E GLOBALIZADO", dados.Globalizado);
            Campo(linha.RelativeItem(), "TOMADOR DO SERVIÇO", dados.TomadorPapel);
        });
        coluna.Item().Element(c => Campo(c, "CFOP - NATUREZA DA OPERAÇÃO", dados.CfopNatureza));
        coluna.Item().Row(linha =>
        {
            Campo(linha.RelativeItem(), "INÍCIO DA PRESTAÇÃO", dados.Inicio);
            Campo(linha.RelativeItem(), "TÉRMINO DA PRESTAÇÃO", dados.Termino);
        });
        coluna.Item().Row(linha =>
        {
            Pessoa(linha.RelativeItem(), "REMETENTE", dados.Remetente);
            Pessoa(linha.RelativeItem(), "DESTINATÁRIO", dados.Destinatario);
        });
        coluna.Item().Row(linha =>
        {
            Pessoa(linha.RelativeItem(), "EXPEDIDOR", dados.Expedidor);
            Pessoa(linha.RelativeItem(), "RECEBEDOR", dados.Recebedor);
        });
        coluna.Item().Element(c => Pessoa(c, "TOMADOR DO SERVIÇO", dados.Tomador));

        coluna.Item().Row(linha =>
        {
            Campo(linha.RelativeItem(5), "PRODUTO PREDOMINANTE", dados.ProdutoPredominante);
            Campo(linha.RelativeItem(3), "OUTRAS CARACTERÍSTICAS DA CARGA", dados.OutrasCaracteristicas);
            Campo(linha.RelativeItem(2), "VALOR TOTAL DA CARGA", dados.ValorCarga);
        });
        if (dados.Quantidades.Count > 0)
        {
            coluna.Item().Row(linha =>
            {
                foreach (var quantidade in dados.Quantidades.Take(5))
                {
                    Campo(linha.RelativeItem(), quantidade.Descricao.ToUpperInvariant(), quantidade.Valor);
                }
            });
        }

        coluna.Item().Element(c => Titulo(c, "COMPONENTES DO VALOR DA PRESTAÇÃO DE SERVIÇO"));
        coluna.Item().Row(linha =>
        {
            linha.RelativeItem(7).Border(Borda).Padding(2).Table(tabela =>
            {
                tabela.ColumnsDefinition(colunas =>
                {
                    colunas.RelativeColumn(3);
                    colunas.RelativeColumn(2);
                    colunas.RelativeColumn(3);
                    colunas.RelativeColumn(2);
                });
                foreach (var (nome, valor) in dados.Componentes)
                {
                    tabela.Cell().Text(nome);
                    tabela.Cell().AlignRight().PaddingRight(6).Text(valor);
                }
            });
            linha.RelativeItem(3).Column(totais =>
            {
                totais.Item().Element(c => Campo(c, "VALOR TOTAL DA PRESTAÇÃO DO SERVIÇO", dados.ValorTotalPrestacao));
                totais.Item().Element(c => Campo(c, "VALOR A RECEBER", dados.ValorReceber));
            });
        });

        coluna.Item().Element(c => Titulo(c, "INFORMAÇÕES RELATIVAS AO IMPOSTO"));
        coluna.Item().Row(linha =>
        {
            Campo(linha.RelativeItem(4), "SITUAÇÃO TRIBUTÁRIA", dados.SituacaoTributaria);
            Campo(linha.RelativeItem(2), "BASE DE CÁLCULO", dados.BaseCalculo);
            Campo(linha.RelativeItem(1), "ALÍQ. ICMS", dados.Aliquota);
            Campo(linha.RelativeItem(2), "VALOR DO ICMS", dados.ValorIcms);
            Campo(linha.RelativeItem(1), "% RED. BC", dados.ReducaoBase);
        });
        if (dados.IbsCbsSimples.Length > 0)
        {
            coluna.Item().Element(c => Campo(c, "IBS/CBS - SIMPLES NACIONAL", dados.IbsCbsSimples));
        }

        coluna.Item().Element(c => Titulo(c, "DOCUMENTOS ORIGINÁRIOS"));
        coluna.Item().Border(Borda).Padding(2).Table(tabela =>
        {
            tabela.ColumnsDefinition(colunas =>
            {
                colunas.ConstantColumn(22, Unit.Millimetre);
                colunas.RelativeColumn();
                colunas.ConstantColumn(22, Unit.Millimetre);
                colunas.RelativeColumn();
            });
            tabela.Header(cabecalho =>
            {
                for (var i = 0; i < 2; i++)
                {
                    cabecalho.Cell().Text("TIPO DOC.").FontSize(5.5f);
                    cabecalho.Cell().Text("CHAVE / SÉRIE E NÚMERO").FontSize(5.5f);
                }
            });
            foreach (var documento in dados.Documentos)
            {
                tabela.Cell().Text(documento.Tipo);
                tabela.Cell().Text(documento.Identificacao);
            }
        });

        coluna.Item().Element(c => Titulo(c, "OBSERVAÇÕES GERAIS"));
        coluna.Item().Border(Borda).Padding(2).MinHeight(12, Unit.Millimetre).Text(dados.Observacoes);

        coluna.Item().Element(c => Titulo(c, $"INFORMAÇÕES ESPECÍFICAS DO MODAL {dados.Modal}"));
        coluna.Item().Element(c => Campo(c, "RNTRC DA EMPRESA", dados.Rntrc));

        coluna.Item().Element(c => Titulo(c, "RESERVADO AO FISCO"));
        coluna.Item().Border(Borda).Height(15, Unit.Millimetre);
    });

    /// <summary>Canhoto (opcional pelo item 2.20.5; leiaute livre), só na primeira folha.</summary>
    private static void Canhoto(IContainer container, DadosDacte dados) => container.PaddingBottom(3).Column(coluna =>
    {
        coluna.Item().Border(Borda).Padding(2)
            .Text("RECEBEMOS OS VOLUMES DESTE CONHECIMENTO EM PERFEITO ESTADO, CONFORME AS INFORMAÇÕES ABAIXO DESCRITAS.").FontSize(6);
        coluna.Item().Row(linha =>
        {
            Campo(linha.RelativeItem(4), "NOME", string.Empty);
            Campo(linha.RelativeItem(2), "RG", string.Empty);
            Campo(linha.RelativeItem(4), "ASSINATURA / CARIMBO", string.Empty);
            Campo(linha.RelativeItem(3), "CHEGADA DATA/HORA", string.Empty);
            Campo(linha.RelativeItem(3), "SAÍDA DATA/HORA", string.Empty);
            Campo(linha.RelativeItem(3), "CT-E", $"Nº {dados.Numero}  SÉRIE {dados.Serie}");
        });
        coluna.Item().PaddingTop(2).LineHorizontal(0.5f).LineDashPattern([2f, 2f]);
    });

    private static void Titulo(IContainer container, string titulo) =>
        container.PaddingTop(2).Background(Colors.Grey.Lighten3).Border(Borda).PaddingHorizontal(2).Text(titulo).FontSize(6.5f).Bold();

    private static void Campo(IContainer container, string rotulo, string valor) =>
        container.Border(Borda).Padding(2).Column(coluna =>
        {
            coluna.Item().Text(rotulo).FontSize(5.5f);
            coluna.Item().MinHeight(8).Text(valor).Bold();
        });

    private static void Pessoa(IContainer container, string papel, PessoaDacte pessoa) =>
        container.Border(Borda).Padding(2).Column(coluna =>
        {
            coluna.Item().Text(t =>
            {
                t.Span(papel + ": ").FontSize(5.5f);
                t.Span(pessoa.Nome).Bold();
            });
            coluna.Item().Text($"ENDEREÇO: {pessoa.Endereco}");
            coluna.Item().Text($"MUNICÍPIO: {pessoa.Municipio}   UF: {pessoa.Uf}   CEP: {pessoa.Cep}");
            coluna.Item().Text($"CNPJ/CPF: {pessoa.Documento}   IE: {pessoa.Ie}");
            coluna.Item().Text($"PAÍS: {pessoa.Pais}   FONE: {pessoa.Fone}");
        });
}
