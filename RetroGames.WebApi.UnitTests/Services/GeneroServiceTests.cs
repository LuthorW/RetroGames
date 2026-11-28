using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

/// <summary>Teste do GeneroService, que só lê dados. O Repository é um mock.</summary>
public class GeneroServiceTests
{
    [Fact]
    public async Task Listar_RepositoryComDados_DevolveOsMesmosItens()
    {
        // Arrange: o Repository entrega duas gêneros.
        var itens = new List<Genero>
        {
            new Genero { Id = 1, Nome = "Aventura" },
            new Genero { Id = 2, Nome = "Plataforma" },
        };
        var repositorioGenero = new Mock<IGeneroRepository>();
        repositorioGenero.Setup(r => r.ListarAsync()).ReturnsAsync(itens);
        var repositorio = new Mock<IRepositorySet>();
        repositorio.Setup(r => r.Genero).Returns(repositorioGenero.Object);
        var service = new GeneroService(repositorio.Object);

        // Act
        var resultado = await service.ListarAsync();

        // Assert: o Service devolve exatamente o que o Repository entregou.
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Aventura", resultado[0].Nome);
        Assert.Equal("Plataforma", resultado[1].Nome);
    }
}
