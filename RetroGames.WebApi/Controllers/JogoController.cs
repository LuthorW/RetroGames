using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
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

    /// <summary>Lista paginada de jogos, com busca e filtros. Inativos só para o Administrador.</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        // incluirInativos só vale para o Administrador: quem decide é o JogoService (RN04).
        var (jogos, totalItens) = await _jogoService.ListarAsync(
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            request.IncluirInativos,
            request.Pagina,
            request.TamanhoPagina);

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = JogoMapper.ParaPaginaResponse(jogos, totalItens, request.Pagina, request.TamanhoPagina),
        });
    }

    /// <summary>Consulta um jogo pelo id, com os seus gêneros.</summary>
    [AllowAnonymous]
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

    /// <summary>Inclui um jogo com os seus gêneros. Responde 201, com o cabeçalho Location.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(
        InserirJogoRequestApiModel request)
    {
        var criado = await _jogoService.InserirAsync(request.ParaModel(), request.IdsGeneros);

        // Devolve o jogo completo (com nomes da plataforma, condição e gêneros).
        var detalhe = await _jogoService.BuscarPorIdAsync(criado.Id);

        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Id },
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = detalhe.ParaListarPorIdResponse(),
            });
    }

    /// <summary>Altera um jogo (recurso completo, inclusive gêneros e situação). Responde 200.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Alterar(
        int id, AlterarJogoRequestApiModel request)
    {
        await _jogoService.AlterarAsync(request.ParaModel(id), request.IdsGeneros);

        var detalhe = await _jogoService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaListarPorIdResponse(),
        });
    }

    /// <summary>
    /// Desativa um jogo (exclusão lógica, RN03): o registro fica no banco com
    /// st_ativo = FALSE. Responde 200 com o envelope (a API não usa 204).
    /// </summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object>>> Desativar(int id)
    {
        await _jogoService.DesativarAsync(id);

        return Ok(new RetroGamesResponseApiModel<object>
        {
            ResponseCode = ResponseCodes.SUCESSO,
        });
    }
}
