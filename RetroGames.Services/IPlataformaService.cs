using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Regras de negócio de plataformas (RN01 e RN02).</summary>
public interface IPlataformaService
{
    /// <summary>Todas as plataformas, como o Repository entrega.</summary>
    Task<List<Plataforma>> ListarAsync();

    /// <summary>A plataforma pedida. Lança PLATAFORMA_NAO_ENCONTRADA se não existir.</summary>
    Task<Plataforma> BuscarPorIdAsync(int id);

    /// <summary>Grava a plataforma e devolve-a com o id gerado. RN01: nome único.</summary>
    Task<Plataforma> InserirAsync(Plataforma plataforma);

    /// <summary>Altera a plataforma. RN01: nome único, ignorando a própria plataforma.</summary>
    Task AlterarAsync(Plataforma plataforma);

    /// <summary>Exclui a plataforma. RN02: não pode ter jogos.</summary>
    Task ExcluirAsync(int id);
}
