using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>SQL da tabela tb_condicao_jogo (somente leitura).</summary>
public interface ICondicaoJogoRepository
{
    /// <summary>Todas as condições de jogo, pela chave fixa.</summary>
    Task<List<CondicaoJogo>> ListarAsync();
}
