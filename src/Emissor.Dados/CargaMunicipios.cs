using System.Reflection;
using Emissor.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Carga da tabela de municípios (§5.9) a partir da lista oficial do IBGE embutida no assembly
/// (<c>Recursos/municipios-ibge.csv</c>, baixada em 25/11/2026 da API <c>servicodados.ibge.gov.br</c>).
/// Não precisa de internet: o arquivo vai junto com o sistema.
/// </summary>
public static class CargaMunicipios
{
    /// <summary>Nome do recurso embutido.</summary>
    internal const string Recurso = "Emissor.Dados.municipios-ibge.csv";

    /// <summary>Lê a lista oficial embutida (código IBGE, nome e UF).</summary>
    public static IReadOnlyList<Municipio> LerListaOficial()
    {
        using var fluxo = Assembly.GetExecutingAssembly().GetManifestResourceStream(Recurso)
            ?? throw new InvalidOperationException($"Recurso {Recurso} não encontrado.");
        using var leitor = new StreamReader(fluxo);

        var municipios = new List<Municipio>();
        leitor.ReadLine(); // cabeçalho: codigo;nome;uf
        while (leitor.ReadLine() is { } linha)
        {
            if (linha.Length == 0)
            {
                continue;
            }

            var campos = linha.Split(';');
            municipios.Add(new Municipio { Codigo = int.Parse(campos[0]), Nome = campos[1], Uf = campos[2] });
        }

        return municipios;
    }

    /// <summary>Carrega os municípios se a tabela estiver vazia; retorna quantos foram incluídos.</summary>
    /// <param name="contexto">Contexto do banco.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task<int> CarregarSeVazioAsync(EmissorDbContext contexto, CancellationToken cancelamento = default)
    {
        if (await contexto.Municipios.AnyAsync(cancelamento))
        {
            return 0;
        }

        var municipios = LerListaOficial();
        contexto.Municipios.AddRange(municipios);
        await contexto.SaveChangesAsync(cancelamento);
        contexto.ChangeTracker.Clear();
        return municipios.Count;
    }
}
