using Emissor.Dominio.Enums;

namespace Emissor.Dominio.Entidades;

/// <summary>
/// Base do CT-e e do MDF-e emitidos pela empresa. O rascunho ("Em digitação") pode ser gravado incompleto;
/// "Pronto para transmitir" só é gravado sem pendências. O número sai da série só na transmissão (P4c).
/// </summary>
public abstract class DocumentoFiscal : EntidadeDaEmpresa
{
    /// <summary>Situação do documento.</summary>
    public SituacaoDocumento Situacao { get; set; } = SituacaoDocumento.EmDigitacao;

    /// <summary>Série escolhida no cadastro de séries (<c>ide/serie</c>).</summary>
    public int? SerieId { get; set; }

    /// <summary>Número do documento (<c>ide/nCT</c> ou <c>ide/nMDF</c>); definido só na transmissão.</summary>
    public int? Numero { get; set; }

    /// <summary>Data de emissão (<c>ide/dhEmi</c>).</summary>
    public DateTime DataEmissao { get; set; } = DateTime.Today;

    /// <summary>Chave de acesso (44 posições); preenchida na transmissão (P4c2 e P4c3).</summary>
    public string Chave { get; set; } = string.Empty;

    /// <summary>Protocolo de autorização de uso (15 dígitos); preenchido na autorização.</summary>
    public string Protocolo { get; set; } = string.Empty;

    /// <summary>Data e hora da autorização (<c>dhRecbto</c>); base dos prazos de cancelamento.</summary>
    public DateTime? AutorizadoEm { get; set; }

    /// <summary>Só o rascunho pode ser alterado e excluído livremente.</summary>
    public bool EmDigitacao => Situacao == SituacaoDocumento.EmDigitacao;

    /// <summary>Pendências que impedem o documento de ficar "Pronto para transmitir".</summary>
    public abstract IReadOnlyList<string> ValidarObrigatorios();

    /// <summary>O rascunho grava sempre; o documento pronto só grava sem pendências.</summary>
    public override IReadOnlyList<string> Validar() =>
        Situacao == SituacaoDocumento.EmDigitacao ? [] : ValidarObrigatorios();

    /// <summary>
    /// Substitui as listas de itens (quantidades, documentos, componentes...) pelas de outro documento do mesmo
    /// tipo. Usado na alteração, para o banco trocar os itens gravados pelos editados.
    /// </summary>
    /// <param name="origem">Documento editado.</param>
    public abstract void CopiarItensDe(DocumentoFiscal origem);
}
