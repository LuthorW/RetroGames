namespace RetroGames.Repository;

/// <summary>
/// Conjunto de repositórios: um único objeto que dá acesso a todos.
/// Na aula 3, o Service recebe este objeto e usa
/// repositorio.Plataforma, repositorio.Jogo, repositorio.JogoGenero,
/// repositorio.Genero, repositorio.CondicaoJogo e repositorio.Usuario.
/// </summary>
public interface IRepositorySet
{
    IPlataformaRepository Plataforma { get; }
    IJogoRepository Jogo { get; }
    IJogoGeneroRepository JogoGenero { get; }
    IGeneroRepository Genero { get; }
    ICondicaoJogoRepository CondicaoJogo { get; }
    IUsuarioRepository Usuario { get; }
}
