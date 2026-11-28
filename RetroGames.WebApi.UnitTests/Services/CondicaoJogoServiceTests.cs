using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

/// <summary>Teste do CondicaoJogoService, que só lê dados. O Repository é um mock.</summary>
public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_RepositoryComDados_DevolveOsMesmosItens()
    {
        // Arrange: o Repository entrega duas condições de jogo.
        var itens = new List<CondicaoJogo>
        {
            new CondicaoJogo { Id = 1, Descricao = "Novo" },
            new CondicaoJogo { Id = 2, Descricao = "Usado" },
        };
        var repositorioCondicaoJogo = new Mock<ICondicaoJogoRepository>();
        repositorioCondicaoJogo.Setup(r => r.ListarAsync()).ReturnsAsync(itens);
        var repositorio = new Mock<IRepositorySet>();
        repositorio.Setup(r => r.CondicaoJogo).Returns(repositorioCondicaoJogo.Object);
        var service = new CondicaoJogoService(repositorio.Object);

        // Act
        var resultado = await service.ListarAsync();

        // Assert: o Service devolve exatamente o que o Repository entregou.
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Novo", resultado[0].Descricao);
        Assert.Equal("Usado", resultado[1].Descricao);
    }
}
