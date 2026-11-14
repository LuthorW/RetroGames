using System.Net;

namespace RetroGames.Services;

/// <summary>
/// Lançada pelo Service quando uma regra de negócio é violada.
/// Uso: throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
/// Na aula 4, o GlobalExceptionHandler lê o Erro e responde com o Status dele.
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException((int Code, string DefaultMessage, HttpStatusCode Status) erro)
        : base(erro.DefaultMessage)
    {
        Erro = erro;
    }

    /// <summary>O erro do catálogo BusinessErrors que foi violado.</summary>
    public (int Code, string DefaultMessage, HttpStatusCode Status) Erro { get; }
}
