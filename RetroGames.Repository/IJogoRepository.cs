using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>Todo o SQL da tabela tb_jogo.</summary>
public interface IJogoRepository
{
    /// <summary>
    /// Os jogos de uma página e o total encontrado com o mesmo filtro.
    /// Cada filtro só vale quando informado (null = sem filtro):
    ///   busca: parte do nome; idPlataforma: plataforma;
    ///   ids: lista de ids de jogos (o filtro por gênero chega assim, montado pelo Service).
    /// incluirInativos: false lista só jogos ativos.
    /// </summary>
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? ids,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    /// <summary>O jogo com o id informado (ativo ou não), ou null se não existir.</summary>
    Task<Jogo?> BuscarPorIdAsync(int id);

    /// <summary>Grava um novo jogo e devolve o id gerado. dt_cadastro é gerado pelo banco (UTC).</summary>
    Task<int> InserirAsync(Jogo jogo);

    /// <summary>Atualiza os dados do jogo (não altera dt_cadastro).</summary>
    Task AlterarAsync(Jogo jogo);

    /// <summary>Marca o jogo como inativo (st_ativo = FALSE). Jogo nunca é apagado (RN03).</summary>
    Task DesativarAsync(int id);
}
