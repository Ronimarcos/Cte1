using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Certificados;

/// <summary>
/// Proteção de segredos pela DPAPI do Windows, no escopo do usuário do Windows (fatia A3): só o mesmo usuário, na
/// mesma máquina, consegue usar o certificado A1 cadastrado (o banco também fica na pasta do usuário). Uma entropia
/// fixa do aplicativo impede que outros programas do mesmo usuário descriptografem os dados só com a DPAPI.
/// Se a senha do Windows for redefinida pela TI (e não trocada pelo próprio usuário), a chave da DPAPI pode se perder
/// e os certificados A1 precisam ser cadastrados de novo.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class ProtetorDpapi : IProtetorSegredos
{
    private static readonly byte[] Entropia = Encoding.UTF8.GetBytes("Cte1.Emissor.Certificados.v2");

    /// <inheritdoc />
    public byte[] Proteger(byte[] dados) => ProtectedData.Protect(dados, Entropia, DataProtectionScope.CurrentUser);

    /// <inheritdoc />
    public byte[] Desproteger(byte[] protegidos) => ProtectedData.Unprotect(protegidos, Entropia, DataProtectionScope.CurrentUser);
}

/// <summary>
/// Proteção anterior à fatia A3 (DPAPI no escopo da máquina), só para ler os certificados antigos e regravá-los com
/// <see cref="ProtetorDpapi"/>. Não grava nada novo.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class ProtetorDpapiMaquina : IProtetorSegredos
{
    private static readonly byte[] Entropia = Encoding.UTF8.GetBytes("Cte1.Emissor.Certificados.v1");

    /// <summary>Não usado: a proteção da máquina só é lida.</summary>
    /// <param name="dados">Dados abertos.</param>
    /// <exception cref="InvalidOperationException">Sempre.</exception>
    public byte[] Proteger(byte[] dados) =>
        throw new InvalidOperationException("A proteção da máquina é só para leitura dos certificados antigos.");

    /// <inheritdoc />
    public byte[] Desproteger(byte[] protegidos) => ProtectedData.Unprotect(protegidos, Entropia, DataProtectionScope.LocalMachine);
}
