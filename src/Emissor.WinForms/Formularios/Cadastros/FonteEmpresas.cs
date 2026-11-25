using System.Collections;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Regras;
using Emissor.WinForms.Prototipo;
using Emissor.WinForms.Servicos;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>
/// Cadastro de empresas (emitentes), gravado no banco. Não é filtrado pela empresa ativa.
/// A empresa em uso não pode ser excluída; a empresa com cadastros vinculados é recusada pelo banco.
/// </summary>
internal sealed class FonteEmpresas : IFonteCadastro
{
    private readonly ServicoEmpresas _empresas;
    private readonly CatalogoMunicipios _municipios;
    private readonly int _empresaAtivaId;
    private readonly Func<Task> _aoAlterar;

    /// <summary>Cria o cadastro de empresas.</summary>
    /// <param name="empresas">Operações de empresa no banco.</param>
    /// <param name="municipios">Municípios do IBGE.</param>
    /// <param name="empresaAtivaId">Empresa em uso, que não pode ser excluída.</param>
    /// <param name="aoAlterar">Avisa a janela principal para atualizar a barra de status.</param>
    public FonteEmpresas(ServicoEmpresas empresas, CatalogoMunicipios municipios, int empresaAtivaId, Func<Task> aoAlterar)
    {
        _empresas = empresas;
        _municipios = municipios;
        _empresaAtivaId = empresaAtivaId;
        _aoAlterar = aoAlterar;
    }

    /// <inheritdoc />
    public string Titulo => "Empresas";

    /// <inheritdoc />
    public IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(EmpresaResumo.RazaoSocial), "Razão social", 40),
        new(nameof(EmpresaResumo.Cnpj), "CNPJ/CPF", 18),
        new(nameof(EmpresaResumo.MunicipioDescricao), "Município", 18),
        new(nameof(EmpresaResumo.CrtDescricao), "CRT", 14),
        new(nameof(EmpresaResumo.AmbienteDescricao), "Ambiente", 10),
    ];

    /// <inheritdoc />
    public async Task<IList> ListarAsync(string pesquisa)
    {
        var termo = pesquisa.Trim();
        var empresas = await _empresas.ListarResumosAsync();
        return empresas
            .Where(e => termo.Length == 0
                || $"{e.RazaoSocial} {e.NomeFantasia} {e.Cnpj} {e.MunicipioDescricao}".Contains(termo, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    /// <inheritdoc />
    public int IdDe(object registro) => ((EmpresaResumo)registro).Id;

    /// <inheritdoc />
    public async Task<int?> NovoAsync(IWin32Window dono)
    {
        var empresa = new Empresa();
        using var tela = new FormEdicaoEmpresa(empresa, novo: true, _municipios, _empresas.IncluirAsync);
        if (tela.ShowDialog(dono) != DialogResult.OK)
        {
            return null;
        }

        await _aoAlterar();
        return empresa.Id;
    }

    /// <inheritdoc />
    public async Task<bool> EditarAsync(IWin32Window dono, object registro)
    {
        if (await _empresas.ObterAsync(IdDe(registro)) is not { } empresa)
        {
            MessageBox.Show(dono, "A empresa não existe mais. A lista será atualizada.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        using var tela = new FormEdicaoEmpresa(empresa, novo: false, _municipios, _empresas.AlterarAsync);
        if (tela.ShowDialog(dono) != DialogResult.OK)
        {
            return false;
        }

        await _aoAlterar();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ExcluirAsync(IWin32Window dono, object registro)
    {
        var empresa = (EmpresaResumo)registro;
        if (empresa.Id == _empresaAtivaId)
        {
            MessageBox.Show(dono, "A empresa em uso não pode ser excluída. Troque de empresa antes.", Titulo,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        var resposta = MessageBox.Show(dono, $"Excluir a empresa \"{empresa.RazaoSocial}\"?", Titulo,
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (resposta != DialogResult.Yes)
        {
            return false;
        }

        try
        {
            await _empresas.ExcluirAsync(empresa.Id);
        }
        catch (RegraNegocioException erro)
        {
            MessageBox.Show(dono, string.Join(Environment.NewLine, erro.Pendencias), Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        await _aoAlterar();
        return true;
    }

    /// <inheritdoc />
    public Color? CorDaLinha(object registro) => null;

}
