using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Emissor.Dominio.Regras;

namespace Emissor.DFe.Certificados;

/// <summary>
/// Proteção de segredos pela DPAPI do Windows, no escopo da máquina: qualquer usuário do Windows desta máquina
/// consegue usar o certificado A1 cadastrado (o banco também é compartilhado, em <c>C:\ProgramData</c>), mas o
/// conteúdo gravado não pode ser aberto em outra máquina. Uma entropia fixa do aplicativo impede que outros
/// programas descriptografem os dados só com a DPAPI.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class ProtetorDpapi : IProtetorSegredos
{
    private static readonly byte[] Entropia = Encoding.UTF8.GetBytes("Cte1.Emissor.Certificados.v1");

    /// <inheritdoc />
    public byte[] Proteger(byte[] dados) => ProtectedData.Protect(dados, Entropia, DataProtectionScope.LocalMachine);

    /// <inheritdoc />
    public byte[] Desproteger(byte[] protegidos) => ProtectedData.Unprotect(protegidos, Entropia, DataProtectionScope.LocalMachine);
}
