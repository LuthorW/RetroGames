using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

/// <summary>Formato das credenciais. Se o usuário existe e a senha confere é com o Service.</summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .EmailAddress().WithMessage("Informe um e-mail válido.")
            .MaximumLength(255).WithMessage("O e-mail deve ter no máximo 255 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Informe a senha.");
    }
}
