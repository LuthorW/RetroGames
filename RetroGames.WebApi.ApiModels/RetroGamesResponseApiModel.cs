using System.Text.Json.Serialization;

namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// Envelope de resposta sem dados: usado nas respostas de erro, em que
/// "data" é omitido (Especificação 5.3).
/// </summary>
public class RetroGamesResponseApiModel
{
    [JsonPropertyOrder(0)]
    public int ResponseCode { get; set; }

    [JsonPropertyOrder(2)]
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Envelope de resposta com dados: { "responseCode", "data", "errorMessage" }.
/// </summary>
public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    [JsonPropertyOrder(1)]
    public T? Data { get; set; }
}
