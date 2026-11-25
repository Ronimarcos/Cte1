using System.ComponentModel;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>
/// Etapa 2: remetente, destinatário, expedidor, recebedor, tomador e percurso.
/// Os botões Incluir e Editar abrem o cadastro de participante sem sair da emissão.
/// </summary>
public partial class UcCteParticipantes : UserControl, IEtapaCte
{
    /// <summary>Valor da opção "(nenhum)" nas listas de participantes.</summary>
    private const int Nenhum = 0;

    private readonly Dictionary<Button, ComboBox> _comboDoBotao;
    private ComboBox[] _combosParticipante = [];
    private int _empresaId;

    /// <summary>Cria a etapa.</summary>
    public UcCteParticipantes()
    {
        InitializeComponent();
        _comboDoBotao = new()
        {
            [btnIncluirRemetente] = cboRemetente, [btnEditarRemetente] = cboRemetente,
            [btnIncluirDestinatario] = cboDestinatario, [btnEditarDestinatario] = cboDestinatario,
            [btnIncluirExpedidor] = cboExpedidor, [btnEditarExpedidor] = cboExpedidor,
            [btnIncluirRecebedor] = cboRecebedor, [btnEditarRecebedor] = cboRecebedor,
            [btnIncluirTomadorOutros] = cboTomadorOutros, [btnEditarTomadorOutros] = cboTomadorOutros,
        };
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "2. Participantes";

    /// <inheritdoc />
    void IEtapaCte.Carregar(CtePrototipo cte, EmpresaPrototipo empresa)
    {
        _empresaId = empresa.Id;
        _combosParticipante = [cboRemetente, cboDestinatario, cboExpedidor, cboRecebedor, cboTomadorOutros];

        var municipios = DadosFicticios.Municipios.Select(m => new Opcao<int>(m.Codigo, m.Descricao)).ToList();
        cboMunicipioInicio.Carregar(municipios);
        cboMunicipioFim.Carregar(municipios);
        cboTomador.Carregar(OpcoesCadastro.Tomadores);

        CarregarParticipantes();
        cboRemetente.Selecionar<int>(cte.RemetenteId ?? Nenhum);
        cboDestinatario.Selecionar<int>(cte.DestinatarioId ?? Nenhum);
        cboExpedidor.Selecionar<int>(cte.ExpedidorId ?? Nenhum);
        cboRecebedor.Selecionar<int>(cte.RecebedorId ?? Nenhum);
        cboTomadorOutros.Selecionar<int>(cte.TomadorOutrosId ?? Nenhum);
        cboTomador.Selecionar(cte.Tomador);
        cboMunicipioInicio.Selecionar(cte.MunicipioInicioCodigo);
        cboMunicipioFim.Selecionar(cte.MunicipioFimCodigo);
        AtualizarTomadorOutros();
    }

    /// <inheritdoc />
    void IEtapaCte.AoEntrar(CtePrototipo cte)
    {
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(CtePrototipo cte)
    {
        cte.RemetenteId = IdSelecionado(cboRemetente);
        cte.DestinatarioId = IdSelecionado(cboDestinatario);
        cte.ExpedidorId = IdSelecionado(cboExpedidor);
        cte.RecebedorId = IdSelecionado(cboRecebedor);
        cte.Tomador = cboTomador.ValorSelecionado<TomadorServico>();
        cte.TomadorOutrosId = cte.Tomador == TomadorServico.Outros ? IdSelecionado(cboTomadorOutros) : null;
        cte.MunicipioInicioCodigo = cboMunicipioInicio.ValorSelecionado<int>();
        cte.MunicipioFimCodigo = cboMunicipioFim.ValorSelecionado<int>();
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaCte.Validar(CtePrototipo cte) => cte.ValidarParticipantes();

    private void btnIncluir_Click(object? sender, EventArgs e)
    {
        if (sender is Button botao && _comboDoBotao.TryGetValue(botao, out var combo))
        {
            IncluirParticipante(combo);
        }
    }

    private void btnEditar_Click(object? sender, EventArgs e)
    {
        if (sender is Button botao && _comboDoBotao.TryGetValue(botao, out var combo))
        {
            EditarParticipante(combo);
        }
    }

    private void cboTomador_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarTomadorOutros();

    /// <summary>Sugere o início da prestação pelo município do remetente, se ainda não informado.</summary>
    private void cboRemetente_SelectedIndexChanged(object? sender, EventArgs e) => SugerirMunicipio(cboRemetente, cboMunicipioInicio);

    /// <summary>Sugere o término da prestação pelo município do destinatário, se ainda não informado.</summary>
    private void cboDestinatario_SelectedIndexChanged(object? sender, EventArgs e) => SugerirMunicipio(cboDestinatario, cboMunicipioFim);

    /// <summary>O tomador "4 - Outros" é escolhido numa lista própria.</summary>
    private void AtualizarTomadorOutros()
    {
        var outros = cboTomador.ValorSelecionado<TomadorServico>() == TomadorServico.Outros;
        lblTomadorOutros.Enabled = outros;
        cboTomadorOutros.Enabled = outros;
        btnIncluirTomadorOutros.Enabled = outros;
        btnEditarTomadorOutros.Enabled = outros;
    }

    private static void SugerirMunicipio(ComboBox participante, ComboBox municipio)
    {
        if (municipio.SelectedIndex >= 0 || IdSelecionado(participante) is not { } id)
        {
            return;
        }

        municipio.Selecionar(DadosFicticios.Participantes.ObterPorId(id)?.MunicipioCodigo);
    }

    /// <summary>Recarrega as listas de participantes preservando o que estava escolhido em cada uma.</summary>
    private void CarregarParticipantes()
    {
        var opcoes = new List<Opcao<int>> { new(Nenhum, "(nenhum)") };
        opcoes.AddRange(DadosFicticios.Participantes
            .Listar(p => p.EmpresaId == _empresaId)
            .OrderBy(p => p.RazaoSocial)
            .Select(p => new Opcao<int>(p.Id, $"{p.RazaoSocial} - {p.CnpjCpf} - {p.MunicipioDescricao}")));

        foreach (var combo in _combosParticipante)
        {
            var atual = combo.ValorSelecionado<int>() ?? Nenhum;
            combo.Carregar(opcoes);
            combo.Selecionar<int>(atual);
        }
    }

    /// <summary>
    /// Inclui participante sem sair da emissão. O diálogo trabalha com a entidade do domínio; até a fatia P2c
    /// o assistente ainda usa a lista em memória, então o participante é convertido na ida e na volta.
    /// </summary>
    private void IncluirParticipante(ComboBox combo)
    {
        var participante = new ParticipantePrototipo { EmpresaId = _empresaId };
        using var tela = new FormEdicaoParticipante(participante.ParaEntidade(), novo: true, municipios: null, gravar: entidade =>
        {
            participante.CopiarDe(entidade);
            DadosFicticios.Participantes.Incluir(participante);
            return Task.CompletedTask;
        });
        if (tela.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        CarregarParticipantes();
        combo.Selecionar<int>(participante.Id);
    }

    private void EditarParticipante(ComboBox combo)
    {
        if (IdSelecionado(combo) is not { } id || DadosFicticios.Participantes.ObterPorId(id) is not { } copia)
        {
            MessageBox.Show(this, "Escolha o participante que deseja editar.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var tela = new FormEdicaoParticipante(copia.ParaEntidade(), novo: false, municipios: null, gravar: entidade =>
        {
            copia.CopiarDe(entidade);
            DadosFicticios.Participantes.Alterar(copia);
            return Task.CompletedTask;
        });
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            CarregarParticipantes();
        }
    }

    private static int? IdSelecionado(ComboBox combo) =>
        combo.ValorSelecionado<int>() is { } id && id != Nenhum ? id : null;
}
