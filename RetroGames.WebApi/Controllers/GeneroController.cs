using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GeneroController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    /// <summary>Lista os gêneros, ordenados por nome.</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>>> Listar()
    {
        var generos = await _generoService.ListarAsync();

        return Ok(new RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = generos.Select(genero => genero.ParaResponse()).ToList(),
        });
    }
}
