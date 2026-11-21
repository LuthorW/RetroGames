using System.Security.Claims;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Mappers;

/// <summary>Conversões para o ApiModel do usuário logado.</summary>
public static class UsuarioMapper
{
    /// <summary>A partir do usuário lido do banco (no login).</summary>
    public static UsuarioLogadoResponseApiModel ParaUsuarioLogadoResponse(this Usuario usuario)
    {
        return new UsuarioLogadoResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = ClaimsPrincipalExtensions.NomePerfil(usuario.IdPerfil),
        };
    }

    /// <summary>A partir das claims do cookie de sessão (em GET eu).</summary>
    public static UsuarioLogadoResponseApiModel ParaUsuarioLogadoResponse(this ClaimsPrincipal usuario)
    {
        return new UsuarioLogadoResponseApiModel
        {
            Id = usuario.ObterIdUsuario() ?? 0,
            Nome = usuario.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            Email = usuario.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Perfil = usuario.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
        };
    }
}
