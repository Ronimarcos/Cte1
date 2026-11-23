namespace Emissor.WinForms.Prototipo;

/// <summary>Motorista (condutor do MDF-e) fictício, com os campos do §5.7.</summary>
public sealed class MotoristaPrototipo : RegistroPrototipo
{
    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }

    /// <summary>Nome do condutor (<c>condutor/xNome</c>, até 60 caracteres).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>CPF do condutor, formatado (<c>condutor/CPF</c>).</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>Telefone para contato (uso interno, não vai para o XML).</summary>
    public string Telefone { get; set; } = string.Empty;

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (string.IsNullOrWhiteSpace(Nome)) faltando.Add("Nome");
        if (string.IsNullOrWhiteSpace(Cpf)) faltando.Add("CPF");
        return faltando;
    }
}
