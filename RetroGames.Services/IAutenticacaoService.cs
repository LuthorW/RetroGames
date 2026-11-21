using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Conferência de login e criação do administrador inicial.</summary>
public interface IAutenticacaoService
{
    /// <summary>
    /// O usuário, se o e-mail existe, a senha confere e o usuário está ativo.
    /// Em qualquer outro caso, null: quem chama não sabe qual dos três falhou.
    /// </summary>
    Task<Usuario?> AutenticarAsync(string email, string senha);

    /// <summary>Cria o administrador com o e-mail e a senha informados, se ainda não houver nenhum.</summary>
    Task GarantirAdministradorInicialAsync(string email, string senha);
}
