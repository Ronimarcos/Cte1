using System.Globalization;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Emissor.Dados;

/// <summary>
/// Monta as linhas de auditoria a partir das mudanças rastreadas pelo EF Core antes da gravação: inclusão, alteração
/// (campos com o valor anterior e o novo) e exclusão de cadastros, documentos, certificados, empresas e usuários.
/// Mudanças nos itens de um documento (tabelas filhas) viram "itens alterados" do documento dono.
/// </summary>
internal static class ColetorAuditoria
{
    /// <summary>Campos cujo valor nunca vai para a auditoria (aparecem só como "alterado").</summary>
    private static readonly HashSet<string> Sensiveis =
    [
        nameof(Usuario.SenhaHash), nameof(CertificadoDigital.ArquivoPfxCriptografado), nameof(CertificadoDigital.SenhaCriptografada),
    ];

    /// <summary>Campos de controle que não interessam na alteração.</summary>
    private static readonly HashSet<string> Ignorados = [nameof(EntidadeBase.CriadoEm), nameof(EntidadeBase.AlteradoEm)];

    /// <summary>Campos que identificam o registro na inclusão e na exclusão, quando existirem.</summary>
    private static readonly string[] Identificadores =
    [
        nameof(Empresa.RazaoSocial), "Nome", nameof(Usuario.Login), nameof(Veiculo.Placa), "CnpjCpf", nameof(Empresa.Cnpj),
        nameof(CertificadoDigital.Titular), nameof(ApoliceSeguro.NumeroApolice), nameof(SerieDocumento.Serie), "Tipo",
        nameof(ConhecimentoTransporte.Cfop), nameof(DocumentoFiscal.Situacao), nameof(DocumentoFiscal.DataEmissao),
    ];

    private static readonly Dictionary<Type, string> Nomes = new()
    {
        [typeof(ConhecimentoTransporte)] = "CT-e",
        [typeof(ManifestoEletronico)] = "MDF-e",
        [typeof(CertificadoDigital)] = "Certificado",
        [typeof(SerieDocumento)] = "Série",
        [typeof(ApoliceSeguro)] = "Apólice",
        [typeof(ProprietarioVeiculo)] = "Proprietário de veículo",
        [typeof(Veiculo)] = "Veículo",
        [typeof(Usuario)] = "Usuário",
    };

    /// <summary>Mudanças pendentes de gravação que viram linhas de auditoria.</summary>
    /// <param name="rastreador">Rastreador de mudanças do contexto.</param>
    public static IReadOnlyList<MudancaAuditada> Coletar(ChangeTracker rastreador)
    {
        var mudancas = new Dictionary<EntidadeBase, MudancaAuditada>(ReferenceEqualityComparer.Instance);
        var entradas = rastreador.Entries().ToList();
        foreach (var entrada in entradas.Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            if (entrada.Entity is EntidadeBase entidade)
            {
                var mudanca = Obter(mudancas, entidade, Acao(entrada.State));
                if (entrada.State == EntityState.Modified)
                {
                    mudanca.Campos.AddRange(CamposAlterados(entrada, string.Empty));
                }
                else
                {
                    mudanca.Campos.AddRange(CamposIdentificadores(entrada));
                }
            }
            else if (entrada.Metadata.IsOwned() && Dono(entradas, entrada) is { Entity: EntidadeBase dono } entradaDono
                     && entradaDono.State is EntityState.Unchanged or EntityState.Modified)
            {
                var mudanca = Obter(mudancas, dono, AcaoAuditoria.Alteracao);
                var navegacao = entrada.Metadata.FindOwnership()?.PrincipalToDependent?.Name ?? entrada.Metadata.ClrType.Name;
                if (entrada.Metadata.FindOwnership()?.IsUnique == true && entrada.State == EntityState.Modified)
                {
                    mudanca.Campos.AddRange(CamposAlterados(entrada, navegacao + "."));
                }
                else
                {
                    mudanca.ItensAlterados.Add(navegacao);
                }
            }
        }

        return mudancas.Values.Where(m => m.Acao != AcaoAuditoria.Alteracao || m.Campos.Count > 0 || m.ItensAlterados.Count > 0).ToList();
    }

    /// <summary>Nome do tipo do registro para a auditoria.</summary>
    /// <param name="tipo">Tipo da entidade.</param>
    public static string NomeEntidade(Type tipo) => Nomes.GetValueOrDefault(tipo, tipo.Name);

    private static MudancaAuditada Obter(Dictionary<EntidadeBase, MudancaAuditada> mudancas, EntidadeBase entidade, AcaoAuditoria acao)
    {
        if (!mudancas.TryGetValue(entidade, out var mudanca))
        {
            mudanca = new MudancaAuditada(entidade, acao);
            mudancas[entidade] = mudanca;
        }

        return mudanca;
    }

    private static AcaoAuditoria Acao(EntityState estado) => estado switch
    {
        EntityState.Added => AcaoAuditoria.Inclusao,
        EntityState.Deleted => AcaoAuditoria.Exclusao,
        _ => AcaoAuditoria.Alteracao,
    };

    /// <summary>Entrada da entidade dona de um item (sobe pelas tabelas filhas aninhadas até a entidade principal).</summary>
    private static EntityEntry? Dono(IReadOnlyList<EntityEntry> entradas, EntityEntry entrada)
    {
        var atual = entrada;
        for (var nivel = 0; nivel < 5 && atual is not null && atual.Entity is not EntidadeBase; nivel++)
        {
            var posse = atual.Metadata.FindOwnership();
            if (posse is null)
            {
                return null;
            }

            var valores = posse.Properties.Select(p => atual.Property(p.Name).CurrentValue ?? atual.Property(p.Name).OriginalValue).ToArray();
            atual = entradas.FirstOrDefault(e => e.Metadata == posse.PrincipalEntityType
                && posse.PrincipalKey.Properties.Select(p => e.Property(p.Name).CurrentValue).SequenceEqual(valores));
        }

        return atual;
    }

    private static IEnumerable<string> CamposAlterados(EntityEntry entrada, string prefixo)
    {
        foreach (var propriedade in entrada.Properties)
        {
            var nome = propriedade.Metadata.Name;
            if (!propriedade.IsModified || Ignorados.Contains(nome) || propriedade.Metadata.IsShadowProperty() || propriedade.Metadata.IsPrimaryKey())
            {
                continue;
            }

            var anterior = propriedade.OriginalValue;
            var atual = propriedade.CurrentValue;
            if (Iguais(anterior, atual))
            {
                continue;
            }

            yield return Sensiveis.Contains(nome)
                ? $"{prefixo}{nome}: alterado"
                : $"{prefixo}{nome}: {Texto(anterior)} → {Texto(atual)}";
        }
    }

    private static IEnumerable<string> CamposIdentificadores(EntityEntry entrada)
    {
        foreach (var nome in Identificadores)
        {
            var propriedade = entrada.Properties.FirstOrDefault(p => p.Metadata.Name == nome);
            var valor = propriedade is null ? null : entrada.State == EntityState.Deleted ? propriedade.OriginalValue : propriedade.CurrentValue;
            if (valor is not null && !(valor is string texto && texto.Length == 0))
            {
                yield return $"{nome}: {Texto(valor)}";
            }
        }
    }

    private static bool Iguais(object? a, object? b) => a switch
    {
        null => b is null,
        byte[] bytes => b is byte[] outros && bytes.AsSpan().SequenceEqual(outros),
        _ => a.Equals(b),
    };

    private static string Texto(object? valor)
    {
        var texto = valor switch
        {
            null => "(vazio)",
            string s => s.Length == 0 ? "(vazio)" : $"\"{s}\"",
            DateTime d => d.ToString(d.TimeOfDay == TimeSpan.Zero ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
            byte[] => "(binário)",
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => valor.ToString() ?? string.Empty,
        };
        return texto.Length > 80 ? texto[..77] + "..." : texto;
    }
}

/// <summary>Mudança de um registro, antes da gravação (o código do registro incluído só existe depois).</summary>
/// <param name="Entidade">Registro afetado.</param>
/// <param name="Acao">Inclusão, alteração ou exclusão.</param>
internal sealed record MudancaAuditada(EntidadeBase Entidade, AcaoAuditoria Acao)
{
    /// <summary>Campos alterados (ou identificadores, na inclusão e na exclusão).</summary>
    public List<string> Campos { get; } = [];

    /// <summary>Listas de itens alteradas (tabelas filhas).</summary>
    public SortedSet<string> ItensAlterados { get; } = [];

    /// <summary>Linha de auditoria, depois da gravação.</summary>
    /// <param name="usuario">Autor.</param>
    /// <param name="agora">Data e hora.</param>
    public RegistroAuditoria Criar(Usuario usuario, DateTime agora)
    {
        var partes = new List<string>(Campos);
        if (ItensAlterados.Count > 0)
        {
            partes.Add("Itens alterados: " + string.Join(", ", ItensAlterados));
        }

        var descricao = string.Join("; ", partes);
        return new RegistroAuditoria
        {
            DataHora = agora,
            UsuarioId = usuario.Id,
            UsuarioLogin = usuario.Login,
            EmpresaId = Entidade switch
            {
                Empresa empresa => empresa.Id,
                EntidadeDaEmpresa daEmpresa => daEmpresa.EmpresaId,
                _ => null,
            },
            Acao = Acao,
            Entidade = ColetorAuditoria.NomeEntidade(Entidade.GetType()),
            RegistroId = Entidade.Id,
            Descricao = descricao.Length > RegistroAuditoria.TamanhoDescricao ? descricao[..(RegistroAuditoria.TamanhoDescricao - 3)] + "..." : descricao,
        };
    }
}
