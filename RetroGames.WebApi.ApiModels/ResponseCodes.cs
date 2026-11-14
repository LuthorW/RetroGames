namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// Catálogo dos códigos que nascem na própria API (0 a 99), Especificação 5.3.
/// Os erros de negócio (100 em diante) ficam no BusinessErrors, no projeto Services,
/// e não se repetem aqui.
/// </summary>
public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO = 1;
}
