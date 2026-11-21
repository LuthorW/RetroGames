using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.WebApi.Services;

namespace RetroGames.WebApi.Configuration;

/// <summary>
/// Registro das dependências na injeção de dependência do ASP.NET Core.
/// Quem precisar de um repositório ou Service recebe pronto no construtor.
/// </summary>
public static class InjecaoDependenciaConfig
{
    /// <summary>
    /// Registra a configuração da conexão, cada Repository e o RepositorySet.
    /// </summary>
    public static IServiceCollection AddRepositorios(this IServiceCollection services, string connectionString)
    {
        // Uma única instância para a aplicação inteira: a connection string não muda.
        services.AddSingleton(new RepositoryConfig { ConnectionString = connectionString });

        // Uma instância por requisição (scoped).
        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    /// <summary>
    /// Registra cada Service com a sua interface, uma instância por requisição.
    /// </summary>
    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        // Usuário atual: a WebApi lê o usuário da requisição e entrega aos Services pela interface.
        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

        // Gera e confere o hash das senhas (PBKDF2 com salt). Não guarda estado: uma instância basta.
        services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        return services;
    }
}
