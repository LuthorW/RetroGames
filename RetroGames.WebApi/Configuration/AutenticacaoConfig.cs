using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

/// <summary>Sessão por cookie (Especificação 6, RS01 e RS02).</summary>
public static class AutenticacaoConfig
{
    public const string NomeCookie = "retrogames.sessao";

    public static IServiceCollection AddAutenticacao(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = NomeCookie;

                // HttpOnly: o JavaScript da página não consegue ler o cookie.
                opcoes.Cookie.HttpOnly = true;

                // SameSite=Strict: o navegador não envia o cookie em requisições vindas
                // de outros sites. É a defesa do projeto contra CSRF (RS02).
                opcoes.Cookie.SameSite = SameSiteMode.Strict;

                // 2 horas, renovadas enquanto houver uso (expiração deslizante).
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                // Uma API não redireciona para uma tela de login: responde 401 ou 403
                // no envelope, e o front-end decide o que mostrar.
                opcoes.Events.OnRedirectToLogin = contexto =>
                    EscreverErroAsync(contexto.Response, StatusCodes.Status401Unauthorized, ResponseCodes.NAO_AUTENTICADO);
                opcoes.Events.OnRedirectToAccessDenied = contexto =>
                    EscreverErroAsync(contexto.Response, StatusCodes.Status403Forbidden, ResponseCodes.ACESSO_NEGADO);
            });

        return services;
    }

    private static Task EscreverErroAsync(HttpResponse resposta, int status, (int Code, string DefaultMessage) erro)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage,
        });
    }
}
