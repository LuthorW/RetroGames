namespace RetroGames.WebApi.Configuration;

/// <summary>Nomes das políticas de autorização (Especificação 6, RS03).</summary>
public static class Politicas
{
    /// <summary>Exige usuário logado com o perfil Administrador.</summary>
    public const string Administrador = "SomenteAdministrador";
}
