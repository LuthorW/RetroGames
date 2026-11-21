namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// Catálogo dos códigos que nascem na própria API (0 a 99), Especificação 5.3.
/// SUCESSO e ERRO são números simples; a partir de 2, cada código é uma tupla
/// (Code, DefaultMessage). Os erros de negócio (100 em diante) ficam no
/// BusinessErrors, no projeto Services, e não se repetem aqui.
/// </summary>
public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO = 1;

    public static readonly (int Code, string DefaultMessage) MODEL_NULL =
        (2, "O modelo veio nulo.");

    public static readonly (int Code, string DefaultMessage) MODEL_STATE_INVALIDO =
        (3, "Há um erro com o modelo enviado.");

    public static readonly (int Code, string DefaultMessage) NAO_AUTENTICADO =
        (4, "Faça login para continuar.");

    public static readonly (int Code, string DefaultMessage) ACESSO_NEGADO =
        (5, "Você não tem permissão para esta operação.");

    public static readonly (int Code, string DefaultMessage) CREDENCIAIS_INVALIDAS =
        (6, "E-mail ou senha inválidos.");
}
