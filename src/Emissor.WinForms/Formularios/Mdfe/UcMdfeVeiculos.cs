using System.ComponentModel;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.WinForms.Formularios.Cadastros;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Mdfe;

/// <summary>
/// Etapa 3: veículo de tração, até 3 reboques, condutores (1 a 10) e CIOT (<c>rodo/infANTT/infCIOT</c>).
/// O CIOT é obrigatório conforme o tipo do emitente (NT MDF-e 2026.001, rejeição 684; §6.1 do levantamento).
/// </summary>
public partial class UcMdfeVeiculos : UserControl, IEtapaMdfe
{
    /// <summary>Valor da opção "(nenhum)" nas listas de reboque.</summary>
    private const int Nenhum = 0;

    private BindingList<CiotMdfe> _ciots = [];
    private List<Opcao<int>> _motoristas = [];

    /// <summary>Cria a etapa.</summary>
    public UcMdfeVeiculos()
    {
        InitializeComponent();
        CriarColunas();
    }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Titulo => "3. Veículos, condutores e CIOT";

    /// <inheritdoc />
    void IEtapaMdfe.Carregar(ManifestoEletronico mdfe, CadastrosEmissao cadastros)
    {
        var empresa = cadastros.Empresa;
        var veiculos = cadastros.Veiculos.OrderBy(v => v.Placa).ToList();
        cboTracao.Carregar(veiculos.Where(v => v.Tipo == TipoVeiculo.Tracao).Select(Descrever));

        var reboques = new List<Opcao<int>> { new(Nenhum, "(nenhum)") };
        reboques.AddRange(veiculos.Where(v => v.Tipo == TipoVeiculo.Reboque).Select(Descrever));
        foreach (var combo in Reboques)
        {
            combo.Carregar(reboques);
            combo.SelectedIndex = 0;
        }

        _motoristas = cadastros.Motoristas.OrderBy(m => m.Nome)
            .Select(m => new Opcao<int>(m.Id, $"{m.Nome} - CPF {Formatador.Cpf(m.Cpf)}")).ToList();
        clbCondutores.Items.Clear();
        foreach (var motorista in _motoristas)
        {
            clbCondutores.Items.Add(motorista, mdfe.CondutoresIds.Contains(motorista.Valor));
        }

        cboTracao.Selecionar(mdfe.VeiculoTracaoId);
        for (var i = 0; i < mdfe.ReboquesIds.Count && i < Reboques.Length; i++)
        {
            Reboques[i].Selecionar<int>(mdfe.ReboquesIds[i]);
        }

        lblRntrc.Text = $"RNTRC da empresa: {(string.IsNullOrWhiteSpace(empresa.Rntrc) ? "não cadastrado" : empresa.Rntrc)}";
        _ciots = new(mdfe.Ciots.Select(c => c.Copiar()).ToList());
        dgvCiot.DataSource = _ciots;
    }

    /// <summary>Explica se o CIOT é obrigatório para o tipo de emitente escolhido na etapa 1.</summary>
    void IEtapaMdfe.AoEntrar(ManifestoEletronico mdfe)
    {
        grpCiot.Text = mdfe.ExigeCiot ? "CIOT * (obrigatório)" : "CIOT (opcional)";
        lblAvisoCiot.Text = mdfe.ExigeCiot
            ? "Obrigatório para este tipo de emitente (NT MDF-e 2026.001, rejeição 684). Gere o CIOT no portal da IPEF e informe aqui."
            : "Carga própria sem tipo de transportador: o CIOT não é exigido.";
    }

    /// <inheritdoc />
    void IEtapaMdfe.Ler(ManifestoEletronico mdfe)
    {
        dgvCiot.EndEdit();
        Validate();
        mdfe.VeiculoTracaoId = cboTracao.ValorSelecionado<int>();
        mdfe.ReboquesIds = Reboques
            .Select(c => c.ValorSelecionado<int>() ?? Nenhum)
            .Where(id => id != Nenhum)
            .ToList();
        mdfe.CondutoresIds = clbCondutores.CheckedItems.OfType<Opcao<int>>().Select(o => o.Valor).ToList();
        mdfe.Ciots = _ciots
            .Where(c => !string.IsNullOrWhiteSpace(c.Ciot) || !string.IsNullOrWhiteSpace(c.CpfCnpjResponsavel))
            .Select(c => new CiotMdfe
            {
                Ciot = new string(c.Ciot.Where(char.IsDigit).ToArray()),
                CpfCnpjResponsavel = c.CpfCnpjResponsavel.Trim(),
            })
            .ToList();
    }

    /// <inheritdoc />
    IReadOnlyList<string> IEtapaMdfe.Validar(ManifestoEletronico mdfe) => mdfe.ValidarVeiculos();

    private void dgvCiot_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    private ComboBox[] Reboques => [cboReboque1, cboReboque2, cboReboque3];

    private static Opcao<int> Descrever(Veiculo veiculo) =>
        new(veiculo.Id, $"{veiculo.Placa} - {(veiculo.Tipo == TipoVeiculo.Tracao
            ? OpcoesCadastro.Texto(OpcoesCadastro.TiposRodado, veiculo.TipoRodado)
            : OpcoesCadastro.Texto(OpcoesCadastro.TiposCarroceria, veiculo.TipoCarroceria))}"
            + $" - tara {veiculo.TaraKg:N0} kg{(veiculo.Proprio ? string.Empty : " - terceiro")}");

    private void CriarColunas()
    {
        dgvCiot.AutoGenerateColumns = false;
        dgvCiot.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CiotMdfe.Ciot),
            HeaderText = "CIOT (12 dígitos)",
            MaxInputLength = 12,
            FillWeight = 45,
        });
        dgvCiot.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CiotMdfe.CpfCnpjResponsavel),
            HeaderText = "CPF/CNPJ do responsável pelo CIOT",
            MaxInputLength = 18,
            FillWeight = 55,
        });
    }
}
