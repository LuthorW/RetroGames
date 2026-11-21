namespace RetroGames.Services;

/// <summary>
/// Quem está fazendo a requisição. O Service não conhece HTTP: ele usa esta
/// interface, e a WebApi a implementa lendo o cookie de sessão.
/// </summary>
public interface IUsuarioAtualService
{
    /// <summary>Se há usuário logado.</summary>
    bool EstaAutenticado { get; }

    /// <summary>O id do usuário logado, ou null se não houver sessão.</summary>
    int? IdUsuario { get; }

    /// <summary>Se o usuário logado tem o perfil Administrador.</summary>
    bool EhAdministrador { get; }
}
