namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// Parâmetros da listagem de jogos, lidos da query string
/// (ex.: ?busca=zelda&amp;idPlataforma=2&amp;pagina=1). Cada filtro é opcional.
/// IncluirInativos só tem efeito para o Administrador (RN04).
/// </summary>
public class ListarJogoRequestApiModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
    public bool IncluirInativos { get; set; }
}
