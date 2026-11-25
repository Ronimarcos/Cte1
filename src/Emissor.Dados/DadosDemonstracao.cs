using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Dados de demonstração, oferecidos só quando o banco não tem nenhuma empresa. CNPJs, IEs e RNTRCs são
/// fictícios. As empresas recebem os códigos 1, 2 e 3, os mesmos dos cadastros que ainda estão em memória
/// no protótipo (até a fatia P2b), para que continuem batendo com a empresa escolhida.
/// </summary>
public static class DadosDemonstracao
{
    /// <summary>Indica se o banco ainda não tem nenhuma empresa.</summary>
    /// <param name="contexto">Contexto do banco.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task<bool> BancoSemEmpresasAsync(EmissorDbContext contexto, CancellationToken cancelamento = default) =>
        !await contexto.Empresas.AnyAsync(cancelamento);

    /// <summary>Empresas fictícias de demonstração (ALFA e BETA em homologação, GAMA em produção).</summary>
    public static IReadOnlyList<Empresa> Empresas() =>
    [
        new()
        {
            Id = 1, RazaoSocial = "TRANSPORTADORA EXEMPLO ALFA LTDA", NomeFantasia = "ALFA TRANSPORTES", Cnpj = "11111111000111",
            Ie = "111111111", Crt = Crt.SimplesNacional, Rntrc = "11111111", Fone = "6533330001", Email = "fiscal@alfa.exemplo",
            Endereco = new() { Logradouro = "AVENIDA EXEMPLO", Numero = "100", Bairro = "CENTRO", MunicipioCodigo = 5103403, Cep = "78000000" },
        },
        new()
        {
            Id = 2, RazaoSocial = "TRANSPORTADORA EXEMPLO BETA LTDA", NomeFantasia = "BETA LOG", Cnpj = "22222222000122",
            Ie = "222222222", Crt = Crt.SimplesNacional, Rntrc = "22222222", Fone = "6634440002", Email = "fiscal@beta.exemplo",
            Endereco = new() { Logradouro = "RUA FICTICIA", Numero = "200", Bairro = "VILA AURORA", MunicipioCodigo = 5107602, Cep = "78700000" },
        },
        new()
        {
            Id = 3, RazaoSocial = "LOGISTICA EXEMPLO GAMA LTDA", NomeFantasia = "GAMA", Cnpj = "33333333000133",
            Ie = "333333333", Crt = Crt.SimplesNacional, Rntrc = "33333333", Fone = "6635550003", Email = "fiscal@gama.exemplo",
            AmbientePadrao = TipoAmbiente.Producao,
            Endereco = new() { Logradouro = "RODOVIA EXEMPLO", Numero = "KM 10", Bairro = "DISTRITO INDUSTRIAL", MunicipioCodigo = 5107909, Cep = "78550000" },
        },
    ];

    /// <summary>Grava as empresas de demonstração; não faz nada se já houver empresas.</summary>
    /// <param name="contexto">Contexto do banco (com os municípios já carregados).</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task CarregarAsync(EmissorDbContext contexto, CancellationToken cancelamento = default)
    {
        if (!await BancoSemEmpresasAsync(contexto, cancelamento))
        {
            return;
        }

        contexto.Empresas.AddRange(Empresas());
        await contexto.SaveChangesAsync(cancelamento);
        contexto.ChangeTracker.Clear();
    }
}
