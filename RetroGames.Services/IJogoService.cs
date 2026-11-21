using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Regras de negócio de jogos (RN03 e RN05) e montagem do catálogo.</summary>
public interface IJogoService
{
    /// <summary>
    /// Uma página do catálogo, com o detalhe de cada jogo, e o total encontrado.
    /// O filtro por gênero é resolvido aqui, com LINQ; a paginação fica no SQL.
    /// </summary>
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        int? idGenero,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    /// <summary>O detalhe do jogo. Lança JOGO_NAO_ENCONTRADO se não existir.</summary>
    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    /// <summary>Grava o jogo (ativo) e os seus gêneros numa transação e devolve o jogo com o id gerado.</summary>
    Task<Jogo> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    /// <summary>
    /// Grava o jogo e substitui os seus gêneros numa transação. A situação (Ativo)
    /// vem do jogo recebido: assim, um jogo desativado pode ser reativado.
    /// </summary>
    Task AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    /// <summary>Marca o jogo como inativo. Jogo nunca é apagado (RN03).</summary>
    Task DesativarAsync(int id);
}
