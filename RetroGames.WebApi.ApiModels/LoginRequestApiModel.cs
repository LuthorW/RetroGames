namespace RetroGames.WebApi.ApiModels;

/// <summary>Credenciais enviadas no login.</summary>
public class LoginRequestApiModel
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
