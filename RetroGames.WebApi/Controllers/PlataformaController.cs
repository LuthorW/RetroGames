using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    /// <summary>Lista as plataformas.</summary>
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>>> Listar()
    {
        var plataformas = await _plataformaService.ListarAsync();

        return Ok(new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataformas.Select(plataforma => plataforma.ParaResponse()).ToList(),
        });
    }

    /// <summary>Consulta uma plataforma pelo id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> ListarPorId(int id)
    {
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse(),
        });
    }

    /// <summary>Inclui uma plataforma. Responde 201, com o cabeçalho Location.</summary>
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Inserir(
        InserirPlataformaRequestApiModel request)
    {
        var criada = await _plataformaService.InserirAsync(request.ParaModel());

        // 201 Created + Location: /api/Plataforma/{id}, montado a partir da action ListarPorId.
        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id },
            new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = criada.ParaResponse(),
            });
    }

    /// <summary>Altera uma plataforma (recurso completo). Responde 200 com o registro alterado.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Alterar(
        int id, AlterarPlataformaRequestApiModel request)
    {
        var plataforma = request.ParaModel(id);
        await _plataformaService.AlterarAsync(plataforma);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse(),
        });
    }

    /// <summary>Exclui uma plataforma sem jogos. Responde 200 com o envelope (a API não usa 204).</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object>>> Excluir(int id)
    {
        await _plataformaService.ExcluirAsync(id);

        return Ok(new RetroGamesResponseApiModel<object>
        {
            ResponseCode = ResponseCodes.SUCESSO,
        });
    }
}
