using Emissor.WinForms.Prototipo;

namespace Emissor.WinForms.Formularios.Cadastros;

/// <summary>Cadastro de empresas (emitentes). Não é filtrado pela empresa ativa.</summary>
internal sealed class FonteEmpresas : FonteCadastro<EmpresaPrototipo>
{
    private readonly int _empresaAtivaId;
    private readonly Action _aoAlterar;

    /// <summary>Cria o cadastro de empresas.</summary>
    /// <param name="empresaAtivaId">Empresa em uso, que não pode ser excluída.</param>
    /// <param name="aoAlterar">Avisa a janela principal para atualizar a barra de status.</param>
    public FonteEmpresas(int empresaAtivaId, Action aoAlterar)
        : base(DadosFicticios.Empresas)
    {
        _empresaAtivaId = empresaAtivaId;
        _aoAlterar = aoAlterar;
    }

    /// <inheritdoc />
    public override string Titulo => "Empresas";

    /// <inheritdoc />
    public override IReadOnlyList<ColunaCadastro> Colunas { get; } =
    [
        new(nameof(EmpresaPrototipo.RazaoSocial), "Razão social", 40),
        new(nameof(EmpresaPrototipo.Cnpj), "CNPJ", 18),
        new(nameof(EmpresaPrototipo.MunicipioDescricao), "Município", 18),
        new(nameof(EmpresaPrototipo.CrtDescricao), "CRT", 14),
        new(nameof(EmpresaPrototipo.AmbienteDescricao), "Ambiente", 10),
    ];

    /// <inheritdoc />
    protected override string NomeRegistro => "a empresa";

    /// <inheritdoc />
    protected override string TextoPesquisa(EmpresaPrototipo item) =>
        $"{item.RazaoSocial} {item.NomeFantasia} {item.Cnpj} {item.MunicipioDescricao}";

    /// <inheritdoc />
    protected override string Descrever(EmpresaPrototipo item) => item.RazaoSocial;

    /// <inheritdoc />
    protected override EmpresaPrototipo CriarNovo() => new();

    /// <inheritdoc />
    protected override bool AbrirEdicao(IWin32Window dono, EmpresaPrototipo item, bool novo)
    {
        using var tela = new FormEdicaoEmpresa(item, novo);
        return tela.ShowDialog(dono) == DialogResult.OK;
    }

    /// <inheritdoc />
    protected override string? MotivoParaNaoExcluir(EmpresaPrototipo item)
    {
        if (item.Id == _empresaAtivaId)
        {
            return "A empresa em uso não pode ser excluída. Troque de empresa antes.";
        }

        var temCadastros = DadosFicticios.Participantes.Existe(p => p.EmpresaId == item.Id)
            || DadosFicticios.Veiculos.Existe(v => v.EmpresaId == item.Id)
            || DadosFicticios.Proprietarios.Existe(p => p.EmpresaId == item.Id);
        return temCadastros
            ? "A empresa tem participantes, veículos ou proprietários cadastrados e não pode ser excluída."
            : null;
    }

    /// <inheritdoc />
    protected override void AoAlterar() => _aoAlterar();
}
