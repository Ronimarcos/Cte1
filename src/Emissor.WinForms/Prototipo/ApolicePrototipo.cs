using Emissor.Dominio.Enums;

namespace Emissor.WinForms.Prototipo;

/// <summary>
/// Apólice de seguro da carga fictícia (grupo <c>seg</c> do MDF-e), com os campos do §5.8.
/// O número de averbação é por documento e fica no MDF-e, não na apólice.
/// </summary>
public sealed class ApolicePrototipo : RegistroPrototipo
{
    /// <summary>Empresa dona do cadastro.</summary>
    public int EmpresaId { get; set; }

    /// <summary>Responsável pelo seguro (<c>infResp/respSeg</c>).</summary>
    public ResponsavelSeguro? Responsavel { get; set; }

    /// <summary>CNPJ ou CPF do responsável (<c>infResp/CNPJ</c> ou <c>infResp/CPF</c>); exigido quando é o contratante.</summary>
    public string CnpjCpfResponsavel { get; set; } = string.Empty;

    /// <summary>Nome da seguradora (<c>infSeg/xSeg</c>, até 30 caracteres).</summary>
    public string NomeSeguradora { get; set; } = string.Empty;

    /// <summary>CNPJ da seguradora, formatado (<c>infSeg/CNPJ</c>).</summary>
    public string CnpjSeguradora { get; set; } = string.Empty;

    /// <summary>Número da apólice (<c>nApol</c>, até 20 caracteres).</summary>
    public string NumeroApolice { get; set; } = string.Empty;

    /// <summary>Descrição do responsável para exibição.</summary>
    public string ResponsavelDescricao => OpcoesCadastro.Texto(OpcoesCadastro.ResponsaveisSeguro, Responsavel);

    /// <inheritdoc />
    public override IReadOnlyList<string> ValidarObrigatorios()
    {
        var faltando = new List<string>();
        if (Responsavel is null) faltando.Add("Responsável pelo seguro");
        if (Responsavel == ResponsavelSeguro.Contratante && string.IsNullOrWhiteSpace(CnpjCpfResponsavel))
        {
            faltando.Add("CNPJ/CPF do contratante");
        }

        if (string.IsNullOrWhiteSpace(NomeSeguradora)) faltando.Add("Seguradora");
        if (string.IsNullOrWhiteSpace(CnpjSeguradora)) faltando.Add("CNPJ da seguradora");
        if (string.IsNullOrWhiteSpace(NumeroApolice)) faltando.Add("Número da apólice");
        return faltando;
    }
}
