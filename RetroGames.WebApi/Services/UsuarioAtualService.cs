using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

/// <summary>Implementa IUsuarioAtualService lendo o usuário da requisição atual.</summary>
public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool EstaAutenticado =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public int? IdUsuario =>
        _httpContextAccessor.HttpContext?.User.ObterIdUsuario();

    public bool EhAdministrador =>
        _httpContextAccessor.HttpContext?.User.IsInRole(Perfis.Administrador) ?? false;
}
