using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

/// <summary>
/// API segura por padrão (Especificação 6, RS03): toda rota exige login, exceto as
/// marcadas com [AllowAnonymous]. As de manutenção exigem ainda o perfil Administrador.
/// </summary>
public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            // Política padrão: vale para toda rota sem [AllowAnonymous] nem [Authorize(...)].
            // Sem login → 401 (código 4).
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Logado, mas sem o perfil Administrador → 403 (código 5).
            opcoes.AddPolicy(Politicas.Administrador, politica => politica
                .RequireAuthenticatedUser()
                .RequireRole(Perfis.Administrador));
        });

        return services;
    }
}
