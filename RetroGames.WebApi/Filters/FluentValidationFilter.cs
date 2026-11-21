using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

/// <summary>
/// Filtro de ação: roda depois do model binding e antes da action. Para cada
/// argumento da action, procura um IValidator registrado para o tipo dele e valida.
/// Com erro, responde 400 com o código 3 e os erros por campo; a action não roda.
/// </summary>
public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate proximo)
    {
        var erros = new Dictionary<string, string[]>();

        foreach (var (nomeParametro, argumento) in contexto.ActionArguments)
        {
            if (argumento is null)
            {
                contexto.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.MODEL_NULL.Code,
                    ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage,
                });
                return;
            }

            // Ex.: para InserirPlataformaRequestApiModel, procura IValidator<InserirPlataformaRequestApiModel>.
            // Tipos sem validator (como o int id da rota) são ignorados.
            var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (contexto.HttpContext.RequestServices.GetService(tipoValidator) is not IValidator validator)
            {
                continue;
            }

            var resultado = await validator.ValidateAsync(new ValidationContext<object>(argumento));
            foreach (var grupo in resultado.Errors.GroupBy(erro => erro.PropertyName))
            {
                erros[ParaCamelCase(grupo.Key)] = grupo.Select(erro => erro.ErrorMessage).ToArray();
            }
        }

        if (erros.Count > 0)
        {
            contexto.Result = CriarRespostaErrosDeCampo(erros);
            return;
        }

        await proximo();
    }

    /// <summary>
    /// Resposta 400 com o código 3 e os erros por campo em data (Especificação 5.4).
    /// Também usada no Program.cs para JSON malformado ou tipo errado.
    /// </summary>
    public static BadRequestObjectResult CriarRespostaErrosDeCampo(Dictionary<string, string[]> erros)
    {
        return new BadRequestObjectResult(new RetroGamesResponseApiModel<Dictionary<string, string[]>>
        {
            ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
            Data = erros,
            ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
        });
    }

    /// <summary>
    /// Ajusta o nome do campo ao JSON: "AnoLancamento" vira "anoLancamento" e
    /// "$.anoLancamento" (erro de leitura do JSON) também vira "anoLancamento".
    /// </summary>
    public static string ParaCamelCase(string nomeCampo)
    {
        var nome = nomeCampo.StartsWith("$.") ? nomeCampo[2..] : nomeCampo;
        if (nome.Length == 0 || nome == "$")
        {
            return "corpo";
        }
        return char.ToLowerInvariant(nome[0]) + nome[1..];
    }
}
