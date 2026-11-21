using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

/// <summary>Formato dos parâmetros da listagem de jogos (query string).</summary>
public class ListarJogoRequestValidator : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThanOrEqualTo(1).WithMessage("O tamanho da página deve ser maior ou igual a 1.");
    }
}
