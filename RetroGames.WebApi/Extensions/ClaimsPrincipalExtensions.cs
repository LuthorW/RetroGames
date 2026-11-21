using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

/// <summary>
/// Claims: as informações do usuário guardadas, criptografadas, no cookie de sessão.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Monta o usuário da sessão (id, nome, e-mail e perfil). Nunca o hash da senha.</summary>
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, NomePerfil(usuario.IdPerfil)),
        };

        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identidade);
    }

    /// <summary>O id do usuário logado, ou null se não houver sessão.</summary>
    public static int? ObterIdUsuario(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : null;
    }

    /// <summary>Nome do perfil a partir da chave fixa de tb_perfil.</summary>
    public static string NomePerfil(int idPerfil)
    {
        return idPerfil == PerfilIds.Administrador ? Perfis.Administrador : Perfis.Cliente;
    }
}
