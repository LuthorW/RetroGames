using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    /// <summary>Confere e-mail e senha e cria a sessão (cookie retrogames.sessao).</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        LoginRequestApiModel request)
    {
        var usuario = await _autenticacaoService.AutenticarAsync(request.Email, request.Senha);

        // E-mail inexistente, senha errada ou usuário inativo: a mesma resposta,
        // para não revelar qual dos três falhou.
        if (usuario is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage,
            });
        }

        // Grava as claims no cookie de sessão, criptografadas.
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuario.ParaClaimsPrincipal());

        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = usuario.ParaUsuarioLogadoResponse(),
        });
    }

    /// <summary>Encerra a sessão (apaga o cookie). Exige login: vale a política padrão.</summary>
    [HttpPost("logout")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object>>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok(new RetroGamesResponseApiModel<object>
        {
            ResponseCode = ResponseCodes.SUCESSO,
        });
    }

    /// <summary>Dados do usuário logado, lidos do cookie de sessão. Exige login: vale a política padrão.</summary>
    [HttpGet("eu")]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
    {
        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = User.ParaUsuarioLogadoResponse(),
        });
    }
}
