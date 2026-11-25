namespace Emissor.Dominio.Enums;

/// <summary>Perfil de acesso do usuário do emissor.</summary>
public enum PerfilUsuario
{
    /// <summary>Vê e configura tudo: empresas, usuários, certificados e todos os cadastros e documentos.</summary>
    Administrador = 1,

    /// <summary>Trabalha só nas empresas liberadas para ele (emitir, consultar e cadastros do dia a dia).</summary>
    Operador = 2,
}
