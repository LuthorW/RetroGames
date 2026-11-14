namespace RetroGames.WebApi.ApiModels;

/// <summary>Dados de uma condição de jogo que saem no JSON.</summary>
public class ListarCondicaoJogoResponseApiModel
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
