using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>SQL da tabela tb_genero (somente leitura).</summary>
public interface IGeneroRepository
{
    /// <summary>Todos os gêneros, ordenados por nome.</summary>
    Task<List<Genero>> ListarAsync();
}
