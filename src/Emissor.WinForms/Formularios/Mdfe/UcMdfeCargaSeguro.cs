using Emissor.Dominio.Entidades;
using System.ComponentModel;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>
/// Etapa 4: produto predominante (<c>prodPred</c>), seguro da carga (<c>seg</c>) e totais (<c>tot</c>).
/// Valor e peso podem ser recalculados pela soma dos CT-e do sistema vinculados na etapa 2.
/// </summary>
public partial class UcMdfeCargaSeguro : UserControl, IEtapaMdfe
{
    /// <summary>Valor da opção "(nenhuma)" na lista de apólices.</summary>
    private const int Nenhuma = 0;

    private ManifestoEletronico? _mdfe;
    private CadastrosEmissao? _cadastros;

    /// <summary>Cria a etapa.</summary>
    public UcMdfeCargaSeguro()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "4. Carga, seguro e totais";

    /// <inheritdoc />
    void IEtapaMdfe.Carregar(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
    {
        _cadastros = cadastros;
        cboTipoCarga.Carregar(OpcoesCadastro.TiposCargaMdfe);
        cboUnidadePeso.Carregar(OpcoesCadastro.UnidadesPesoMdfe);
        var apolices = new List<Opcao<int>> { new(Nenhuma, "(nenhuma)") };
        apolices.AddRange(cadastros.Apolices.OrderBy(a => a.NumeroApolice)
            .Select(a => new Opcao<int>(a.Id,
                $"{a.NumeroApolice} - {a.NomeSeguradora} - {OpcoesCadastro.Texto(OpcoesCadastro.ResponsaveisSeguro, a.Responsavel)}")));
        cboApolice.Carregar(apolices);

        cboTipoCarga.Selecionar(mdfe.TipoCarga);
        txtProduto.Text = mdfe.ProdutoPredominante;
        cboApolice.Selecionar<int>(mdfe.ApoliceId ?? Nenhuma);
        txtAverbacoes.Text = string.Join(Environment.NewLine, mdfe.Averbacoes);
        nudValorCarga.Value = mdfe.ValorCarga ?? 0;
        cboUnidadePeso.Selecionar(mdfe.UnidadePeso);
        nudPeso.Value = mdfe.PesoCarga ?? 0;
    }

    /// <summary>Atualiza as quantidades de documentos e a indicação de seguro obrigatório.</summary>
    void IEtapaMdfe.AoEntrar(ManifestoEletronico mdfe)
    {
        _mdfe = mdfe;
        lblQuantidades.Text = $"CT-e: {mdfe.QuantidadeCte}    NF-e: {mdfe.QuantidadeNfe}";
        lblApolice.Text = mdfe.ExigeSeguro ? "Apólice *" : "Apólice";
        AtualizarDadosApolice();
    }

    /// <inheritdoc />
    void IEtapaMdfe.Ler(ManifestoEletronico mdfe)
    {
        mdfe.TipoCarga = cboTipoCarga.ValorSelecionado<TipoCargaMdfe>();
        mdfe.ProdutoPredominante = txtProduto.Text.Trim();
        var apolice = cboApolice.ValorSelecionado<int>() ?? Nenhuma;
        mdfe.ApoliceId = apolice == Nenhuma ? null : apolice;
        mdfe.Averbacoes = txtAverbacoes.Lines.Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
        mdfe.ValorCarga = nudValorCarga.Value;
        mdfe.UnidadePeso = cboUnidadePeso.ValorSelecionado<UnidadePesoMdfe>();
        mdfe.PesoCarga = nudPeso.Value;
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaMdfe.Validar(ManifestoEletronico mdfe) => mdfe.ValidarCargaSeguro();

    private void cboApolice_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarDadosApolice();

    /// <summary>Soma valor e peso dos CT-e do sistema vinculados; o peso usa as quantidades em KG e TON dos CT-e.</summary>
    private async void btnRecalcular_Click(object? sender, EventArgs e)
    {
        var ids = (_mdfe?.Descargas ?? [])
            .SelectMany(d => d.Documentos)
            .Where(d => d.CteId is not null)
            .Select(d => d.CteId!.Value)
            .ToHashSet();
        IReadOnlyList<ConhecimentoTransporte> ctes = [];
        if (ids.Count > 0 && _cadastros is not null)
        {
            btnRecalcular.Enabled = false;
            try
            {
                ctes = await _cadastros.Ctes.ListarPorIdsAsync(_cadastros.Empresa.Id, ids);
            }
            catch (Exception erro)
            {
                MessageBox.Show(this, $"Não foi possível ler os CT-e vinculados.{Environment.NewLine}{erro.GetBaseException().Message}",
                    Titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                btnRecalcular.Enabled = true;
            }
        }

        if (ctes.Count == 0)
        {
            MessageBox.Show(this, "Nenhum CT-e do sistema foi vinculado na etapa 2. Informe valor e peso manualmente.",
                Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var pesoKg = ctes.SelectMany(c => c.Quantidades).Sum(q => q.Unidade switch
        {
            UnidadeMedidaCarga.Quilograma => q.Quantidade,
            UnidadeMedidaCarga.Tonelada => q.Quantidade * 1000,
            _ => 0,
        });
        var emToneladas = cboUnidadePeso.ValorSelecionado<UnidadePesoMdfe>() == UnidadePesoMdfe.Tonelada;
        nudValorCarga.Value = Math.Min(nudValorCarga.Maximum, ctes.Sum(c => c.ValorCarga ?? 0));
        nudPeso.Value = Math.Min(nudPeso.Maximum, emToneladas ? pesoKg / 1000 : pesoKg);
    }

    private void AtualizarDadosApolice()
    {
        var id = cboApolice.ValorSelecionado<int>() ?? Nenhuma;
        var apolice = id == Nenhuma ? null : _cadastros?.Apolice(id);
        lblDadosApolice.Text = apolice is null
            ? string.Empty
            : $"Seguradora CNPJ {Formatador.Cnpj(apolice.CnpjSeguradora)}{(string.IsNullOrEmpty(apolice.CnpjCpfResponsavel) ? string.Empty : $" - responsável {Formatador.CnpjCpf(apolice.CnpjCpfResponsavel)}")}";
    }
}
