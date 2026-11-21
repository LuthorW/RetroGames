using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>Todo o SQL da tabela tb_usuario.</summary>
public interface IUsuarioRepository
{
    /// <summary>O usuário com o e-mail informado, ou null se não existir.</summary>
    Task<Usuario?> BuscarPorEmailAsync(string email);

    /// <summary>O usuário com o id informado, ou null se não existir.</summary>
    Task<Usuario?> BuscarPorIdAsync(int id);

    /// <summary>Se já existe algum usuário com o perfil Administrador.</summary>
    Task<bool> ExisteAdministradorAsync();

    /// <summary>Grava um novo usuário e devolve o id gerado. dt_cadastro é gerado pelo banco (UTC).</summary>
    Task<int> InserirAsync(Usuario usuario);
}
