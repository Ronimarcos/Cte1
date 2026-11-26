using System.Security.Cryptography;
using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Passa os certificados A1 gravados com a proteção antiga (fatia A3: DPAPI da máquina) para a proteção atual (DPAPI
/// do usuário do Windows). Roda na preparação do banco, sem usuário logado (não gera auditoria). Rodar de novo não
/// muda nada: certificado que já abre com a proteção atual fica como está.
/// </summary>
public static class RecriptografiaCertificados
{
    /// <summary>Resultado: quantos foram convertidos e quais não abriram com nenhuma das proteções.</summary>
    /// <param name="Convertidos">Certificados regravados com a proteção atual.</param>
    /// <param name="Falhas">Certificados que precisam ser cadastrados de novo (arquivo e senha do PFX).</param>
    public sealed record Resultado(int Convertidos, IReadOnlyList<CertificadoDigital> Falhas);

    /// <summary>Converte os certificados A1 de todas as empresas.</summary>
    /// <param name="contexto">Banco (sem <see cref="EmissorDbContext.Autor"/>: operação interna).</param>
    /// <param name="atual">Proteção usada daqui em diante.</param>
    /// <param name="antiga">Proteção anterior, só para leitura.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public static async Task<Resultado> ExecutarAsync(EmissorDbContext contexto, IProtetorSegredos atual, IProtetorSegredos antiga,
        CancellationToken cancelamento = default)
    {
        var certificados = await contexto.Certificados
            .Where(c => c.Tipo == TipoCertificado.A1 && c.ArquivoPfxCriptografado != null && c.SenhaCriptografada != null)
            .ToListAsync(cancelamento);
        var falhas = new List<CertificadoDigital>();
        var convertidos = 0;
        foreach (var certificado in certificados)
        {
            if (Abre(atual, certificado.ArquivoPfxCriptografado!) && Abre(atual, certificado.SenhaCriptografada!))
            {
                continue;
            }

            try
            {
                var pfx = atual.Proteger(antiga.Desproteger(certificado.ArquivoPfxCriptografado!));
                var senha = atual.Proteger(antiga.Desproteger(certificado.SenhaCriptografada!));
                certificado.ArquivoPfxCriptografado = pfx;
                certificado.SenhaCriptografada = senha;
                convertidos++;
            }
            catch (CryptographicException)
            {
                falhas.Add(certificado);
            }
        }

        if (convertidos > 0)
        {
            await contexto.SaveChangesAsync(cancelamento);
        }

        return new Resultado(convertidos, falhas);
    }

    private static bool Abre(IProtetorSegredos protetor, byte[] protegidos)
    {
        try
        {
            protetor.Desproteger(protegidos);
            return true;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}
