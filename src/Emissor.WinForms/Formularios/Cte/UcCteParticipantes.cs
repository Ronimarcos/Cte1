using System.ComponentModel;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cte;

/// <summary>
/// Etapa 2: remetente, destinatário, expedidor, recebedor, tomador e percurso, com os participantes e os
/// municípios lidos do banco. Os botões Incluir e Editar gravam o participante no banco sem sair da emissão.
/// </summary>
public partial class UcCteParticipantes : UserControl, IEtapaCte
{
    /// <summary>Valor da opção "(nenhum)" nas listas de participantes.</summary>
    private const int Nenhum = 0;

    private readonly Dictionary<Button, ComboBox> _comboDoBotao;
    private ComboBox[] _combosParticipante = [];
    private CadastrosEmissao? _cadastros;

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
    void IEtapaCte.Carregar(ConhecimentoTransporte cte, CadastrosEmissao cadastros)
    {
        _cadastros = cadastros;
        _combosParticipante = [cboRemetente, cboDestinatario, cboExpedidor, cboRecebedor, cboTomadorOutros];

        // Os 5.571 municípios do IBGE: o campo aceita digitação com sugestões ("Cuia" → "Cuiabá/MT").
        cboMunicipioInicio.Carregar(cadastros.Municipios.Opcoes);
        cboMunicipioFim.Carregar(cadastros.Municipios.Opcoes);
        cboMunicipioInicio.ConfigurarBuscaPorTexto();
        cboMunicipioFim.ConfigurarBuscaPorTexto();
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
    void IEtapaCte.AoEntrar(ConhecimentoTransporte cte)
    {
    }

    /// <inheritdoc />
    void IEtapaCte.Ler(ConhecimentoTransporte cte)
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
    IReadOnlyList<string> IEtapaCte.Validar(ConhecimentoTransporte cte) => cte.ValidarParticipantes();

    private async void btnIncluir_Click(object? sender, EventArgs e)
    {
        if (sender is Button botao && _comboDoBotao.TryGetValue(botao, out var combo))
        {
            await IncluirParticipanteAsync(combo);
        }
    }

    private async void btnEditar_Click(object? sender, EventArgs e)
    {
        if (sender is Button botao && _comboDoBotao.TryGetValue(botao, out var combo))
        {
            await EditarParticipanteAsync(combo);
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

    private void SugerirMunicipio(ComboBox participante, ComboBox municipio)
    {
        if (municipio.SelectedIndex >= 0 || IdSelecionado(participante) is not { } id)
        {
            return;
        }

        municipio.Selecionar(_cadastros?.Participante(id)?.Endereco.MunicipioCodigo);
    }

    /// <summary>Recarrega as listas de participantes preservando o que estava escolhido em cada uma.</summary>
    private void CarregarParticipantes()
    {
        if (_cadastros is null)
        {
            return;
        }

        var opcoes = new List<Opcao<int>> { new(Nenhum, "(nenhum)") };
        opcoes.AddRange(_cadastros.Participantes
            .OrderBy(p => p.RazaoSocial)
            .Select(p => new Opcao<int>(p.Id,
                $"{p.RazaoSocial} - {Formatador.CnpjCpf(p.CnpjCpf)} - {_cadastros.Municipios.Descricao(p.Endereco.MunicipioCodigo)}")));

        foreach (var combo in _combosParticipante)
        {
            var atual = combo.ValorSelecionado<int>() ?? Nenhum;
            combo.Carregar(opcoes);
            combo.Selecionar<int>(atual);
        }
    }

    /// <summary>Inclui o participante no banco e já o escolhe na lista de onde o botão foi clicado.</summary>
    private async Task IncluirParticipanteAsync(ComboBox combo)
    {
        if (_cadastros is null)
        {
            return;
        }

        var participante = new Participante { EmpresaId = _cadastros.Empresa.Id };
        using (var tela = new FormEdicaoParticipante(participante, novo: true, _cadastros.Municipios.Opcoes, _cadastros.ServicoParticipantes.IncluirAsync))
        {
            if (tela.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
        }

        await AtualizarListaAsync();
        combo.Selecionar<int>(participante.Id);
    }

    /// <summary>Edita no banco o participante escolhido na lista.</summary>
    private async Task EditarParticipanteAsync(ComboBox combo)
    {
        if (_cadastros is null)
        {
            return;
        }

        if (IdSelecionado(combo) is not { } id)
        {
            MessageBox.Show(this, "Escolha o participante que deseja editar.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (await _cadastros.ServicoParticipantes.ObterAsync(id) is not { } copia)
        {
            MessageBox.Show(this, "O participante não existe mais. A lista será atualizada.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            await AtualizarListaAsync();
            return;
        }

        using (var tela = new FormEdicaoParticipante(copia, novo: false, _cadastros.Municipios.Opcoes, _cadastros.ServicoParticipantes.AlterarAsync))
        {
            if (tela.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
        }

        await AtualizarListaAsync();
    }

    private async Task AtualizarListaAsync()
    {
        try
        {
            await _cadastros!.RecarregarParticipantesAsync();
            CarregarParticipantes();
        }
        catch (RegraNegocioException erro)
        {
            AvisosCadastro.HaPendencias(this, Titulo, erro.Pendencias);
        }
    }

    private static int? IdSelecionado(ComboBox combo) =>
        combo.ValorSelecionado<int>() is { } id && id != Nenhum ? id : null;
}
