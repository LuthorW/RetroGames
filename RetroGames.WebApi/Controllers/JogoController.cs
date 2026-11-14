using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    /// <summary>Lista paginada de jogos ativos, com busca e filtros.</summary>
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        // Só jogos ativos: incluirInativos (somente Administrador) entra na aula 6.
        var (jogos, totalItens) = await _jogoService.ListarAsync(
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            incluirInativos: false,
            request.Pagina,
            request.TamanhoPagina);

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = JogoMapper.ParaPaginaResponse(jogos, totalItens, request.Pagina, request.TamanhoPagina),
        });
    }

    /// <summary>Consulta um jogo pelo id, com os seus gêneros.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var detalhe = await _jogoService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaListarPorIdResponse(),
        });
    }
}
