using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>Todo o SQL da tabela tb_jogo_genero (vínculos entre jogos e gêneros).</summary>
public interface IJogoGeneroRepository
{
    /// <summary>Todos os vínculos entre jogos e gêneros.</summary>
    Task<List<JogoGenero>> ListarTodosAsync();

    /// <summary>Os vínculos de uma lista de jogos, numa única consulta.</summary>
    Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos);

    /// <summary>Grava os vínculos de um jogo com os seus gêneros.</summary>
    Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros);

    /// <summary>Apaga os vínculos de um jogo, para regravá-los na alteração.</summary>
    Task RemoverPorJogoAsync(int idJogo);
}
