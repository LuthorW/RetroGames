using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Leitura de condições de jogo (tb_condicao_jogo). Não há regra de negócio.</summary>
public interface ICondicaoJogoService
{
    /// <summary>Todas as condições de jogo, pela chave fixa.</summary>
    Task<List<CondicaoJogo>> ListarAsync();
}
