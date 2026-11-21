namespace RetroGames.Repository;

/// <summary>
/// Recebe os repositórios prontos no construtor. Assim, na aula 4, a
/// injeção de dependência da API monta cada um, e nos testes unitários cada
/// repositório pode ser trocado por um simulado (Moq).
/// </summary>
public class RepositorySet : IRepositorySet
{
    public RepositorySet(
        IPlataformaRepository plataforma,
        IJogoRepository jogo,
        IJogoGeneroRepository jogoGenero,
        IGeneroRepository genero,
        ICondicaoJogoRepository condicaoJogo,
        IUsuarioRepository usuario)
    {
        Plataforma = plataforma;
        Jogo = jogo;
        JogoGenero = jogoGenero;
        Genero = genero;
        CondicaoJogo = condicaoJogo;
        Usuario = usuario;
    }

    public IPlataformaRepository Plataforma { get; }
    public IJogoRepository Jogo { get; }
    public IJogoGeneroRepository JogoGenero { get; }
    public IGeneroRepository Genero { get; }
    public ICondicaoJogoRepository CondicaoJogo { get; }
    public IUsuarioRepository Usuario { get; }
}
