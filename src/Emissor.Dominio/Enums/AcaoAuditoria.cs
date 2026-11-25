namespace Emissor.Dominio.Enums;

/// <summary>Ação registrada na auditoria.</summary>
public enum AcaoAuditoria
{
    /// <summary>Inclusão de registro.</summary>
    Inclusao = 1,

    /// <summary>Alteração de registro.</summary>
    Alteracao = 2,

    /// <summary>Exclusão de registro.</summary>
    Exclusao = 3,

    /// <summary>Login aceito.</summary>
    LoginAceito = 10,

    /// <summary>Login recusado (login ou senha errados).</summary>
    LoginRecusado = 11,

    /// <summary>Login bloqueado por excesso de tentativas.</summary>
    LoginBloqueado = 12,

    /// <summary>Tentativa de login de usuário inativo.</summary>
    LoginInativo = 13,

    /// <summary>Criação do primeiro administrador.</summary>
    PrimeiroAdministrador = 14,

    /// <summary>Troca de senha pelo próprio usuário.</summary>
    TrocaSenha = 15,

    /// <summary>Entrada numa empresa (seleção inicial ou troca de empresa).</summary>
    EntradaEmpresa = 20,

    /// <summary>Troca de usuário (saída do sistema para outro login).</summary>
    TrocaUsuario = 21,

    /// <summary>Geração de XML de teste (sem transmissão).</summary>
    XmlTeste = 30,

    /// <summary>Consulta à SEFAZ (status do serviço).</summary>
    ConsultaSefaz = 31,
}
