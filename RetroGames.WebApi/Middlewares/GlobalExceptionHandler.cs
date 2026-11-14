using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

/// <summary>
/// Ponto único que transforma exceções não tratadas em resposta HTTP, sempre no
/// formato do envelope (Especificação 3.3 e 5.3). Assim, nenhum controller precisa
/// de try/catch.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private const string MensagemErroGenerica =
        "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int status;
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException erroNegocio)
        {
            // Regra de negócio violada: status e código vêm do erro do catálogo.
            status = (int)erroNegocio.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = erroNegocio.Erro.Code,
                ErrorMessage = erroNegocio.Message,
            };
        }
        else
        {
            // Erro inesperado: os detalhes vão só para o log (terminal da API).
            // O cliente recebe uma mensagem genérica, sem SQL, tabela ou caminho.
            _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}",
                httpContext.Request.Method, httpContext.Request.Path);

            status = StatusCodes.Status500InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = MensagemErroGenerica,
            };
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);

        // true: a exceção foi tratada aqui, e nenhum outro handler precisa agir.
        return true;
    }
}
