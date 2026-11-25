using Emissor.DFe.Certificados;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Linha da lista de certificados instalados no Windows.</summary>
internal sealed class LinhaCertificadoWindows
{
    /// <summary>Monta a linha.</summary>
    /// <param name="dados">Dados lidos do certificado.</param>
    public LinhaCertificadoWindows(DadosCertificado dados)
    {
        Dados = dados;
        Titular = dados.Titular;
        Documento = Formatador.CnpjCpf(dados.Documento);
        Validade = $"{dados.ValidoDe:dd/MM/yyyy} a {dados.ValidoAte:dd/MM/yyyy}";
        Emissor = dados.Emissor;
    }

    /// <summary>Dados completos do certificado.</summary>
    public DadosCertificado Dados { get; }

    /// <summary>Titular.</summary>
    public string Titular { get; }

    /// <summary>CNPJ/CPF formatado.</summary>
    public string Documento { get; }

    /// <summary>Validade.</summary>
    public string Validade { get; }

    /// <summary>Autoridade certificadora.</summary>
    public string Emissor { get; }
}

/// <summary>
/// Escolha de um certificado instalado no repositório pessoal do Windows (A3 em token ou cartão, ou A1 instalado).
/// Mostrar a lista não pede o PIN.
/// </summary>
public partial class FormSelecaoCertificadoWindows : Form
{
    /// <summary>Cria a lista com os certificados encontrados.</summary>
    /// <param name="certificados">Certificados com chave privada do Windows.</param>
    internal FormSelecaoCertificadoWindows(IReadOnlyList<DadosCertificado> certificados)
    {
        InitializeComponent();
        AtalhosTeclado.ConfigurarDialogo(this, btnOk);
        dgvCertificados.AutoGenerateColumns = false;
        Coluna(nameof(LinhaCertificadoWindows.Titular), "Titular", 40);
        Coluna(nameof(LinhaCertificadoWindows.Documento), "CNPJ/CPF", 18);
        Coluna(nameof(LinhaCertificadoWindows.Validade), "Validade", 22);
        Coluna(nameof(LinhaCertificadoWindows.Emissor), "Emitido por", 20);
        dgvCertificados.DataSource = certificados.Select(c => new LinhaCertificadoWindows(c)).ToList();
        lblVazio.Visible = certificados.Count == 0;
    }

    /// <summary>Certificado escolhido; preenchido quando o diálogo retorna OK.</summary>
    internal DadosCertificado? Selecionado { get; private set; }

    private void btnOk_Click(object? sender, EventArgs e) => Confirmar();

    private void dgvCertificados_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            Confirmar();
        }
    }

    private void Confirmar()
    {
        if (dgvCertificados.CurrentRow?.DataBoundItem is not LinhaCertificadoWindows linha)
        {
            MessageBox.Show(this, "Selecione um certificado na lista.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Selecionado = linha.Dados;
        DialogResult = DialogResult.OK;
    }

    private void Coluna(string propriedade, string titulo, float peso) =>
        dgvCertificados.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propriedade,
            HeaderText = titulo,
            FillWeight = peso,
            ReadOnly = true,
        });
}
