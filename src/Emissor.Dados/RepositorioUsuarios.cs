using Emissor.Dominio.Entidades;
using Emissor.Dominio.Enums;
using Emissor.Dominio.Regras;
using Microsoft.EntityFrameworkCore;

namespace Emissor.Dados;

/// <summary>
/// Repositório do cadastro de usuários (só administrador, pela alçada do <see cref="RepositorioCadastro{T}"/>).
/// Troca as empresas liberadas na alteração e impede ficar sem administrador ativo.
/// </summary>
public sealed class RepositorioUsuarios : RepositorioCadastro<Usuario>
{
    /// <summary>Cria o repositório aplicando a alçada do usuário.</summary>
    /// <param name="contexto">Contexto do EF Core.</param>
    /// <param name="acesso">Quem acessa.</param>
    public RepositorioUsuarios(EmissorDbContext contexto, IContextoAcesso acesso)
        : base(contexto, acesso)
    {
    }

    /// <summary>Altera o usuário, substituindo as empresas liberadas.</summary>
    /// <param name="registro">Usuário editado.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    /// <exception cref="RegraNegocioException">Fora da alçada, pendências ou último administrador ativo.</exception>
    public override async Task AlterarAsync(Usuario registro, CancellationToken cancelamento = default)
    {
        GarantirEscrita(registro);
        registro.Login = Usuario.NormalizarLogin(registro.Login);
        Validar(registro);
        var gravado = await Consulta().FirstOrDefaultAsync(u => u.Id == registro.Id, cancelamento)
            ?? throw new RegraNegocioException("O usuário não existe mais. Atualize a lista.");

        var deixaDeSerAdministradorAtivo = gravado.Administrador && gravado.Ativo && (!registro.Administrador || !registro.Ativo);
        if (deixaDeSerAdministradorAtivo && !await OutroAdministradorAtivoAsync(registro.Id, cancelamento))
        {
            Contexto.ChangeTracker.Clear();
            throw new RegraNegocioException("É preciso manter ao menos um administrador ativo.");
        }

        Contexto.Entry(gravado).CurrentValues.SetValues(registro);
        gravado.Empresas = registro.Administrador ? [] : registro.Empresas.Select(e => new UsuarioEmpresa { EmpresaId = e.EmpresaId }).ToList();
        await GravarAsync(cancelamento, "alterar");
    }

    /// <summary>Exclui o usuário; recusa excluir o último administrador ativo.</summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelamento">Cancelamento da operação.</param>
    public override async Task ExcluirAsync(int id, CancellationToken cancelamento = default)
    {
        var gravado = await Consulta().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancelamento);
        if (gravado is null)
        {
            return;
        }

        if (Acesso.Usuario?.Id == id)
        {
            throw new RegraNegocioException("Você não pode excluir o seu próprio usuário.");
        }

        if (gravado.Administrador && gravado.Ativo && !await OutroAdministradorAtivoAsync(id, cancelamento))
        {
            throw new RegraNegocioException("É preciso manter ao menos um administrador ativo.");
        }

        await base.ExcluirAsync(id, cancelamento);
    }

    private Task<bool> OutroAdministradorAtivoAsync(int id, CancellationToken cancelamento) =>
        Contexto.Usuarios.AnyAsync(u => u.Id != id && u.Ativo && u.Perfil == PerfilUsuario.Administrador, cancelamento);
}
