namespace RetroGames.Repository;

/// <summary>
/// Configuração da camada Repository. Guarda apenas a connection string,
/// que a API vai ler do arquivo .env e entregar aqui (aula 4).
/// </summary>
public class RepositoryConfig
{
    public string ConnectionString { get; set; } = string.Empty;
}
