using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>
/// Jogo com os dados das tabelas relacionadas, combinados com LINQ no JogoService:
/// nome da plataforma, descrição da condição e lista de gêneros.
/// </summary>
public class JogoDetalheServiceModel
{
    /// <summary>O jogo, como o JogoRepository entrega (tb_jogo).</summary>
    public Jogo Jogo { get; set; } = new();

    /// <summary>no_plataforma da plataforma do jogo.</summary>
    public string NomePlataforma { get; set; } = string.Empty;

    /// <summary>ds_condicao_jogo da condição do jogo.</summary>
    public string DescricaoCondicaoJogo { get; set; } = string.Empty;

    /// <summary>Gêneros do jogo, ordenados por nome.</summary>
    public List<Genero> Generos { get; set; } = new();
}
