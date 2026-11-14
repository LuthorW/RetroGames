using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Leitura de gêneros (tb_genero). Não há regra de negócio.</summary>
public interface IGeneroService
{
    /// <summary>Todos os gêneros, ordenados por nome.</summary>
    Task<List<Genero>> ListarAsync();
}
