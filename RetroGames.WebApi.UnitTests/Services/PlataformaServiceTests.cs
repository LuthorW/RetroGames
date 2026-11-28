using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

/// <summary>
/// Testes das regras do PlataformaService. O Repository é um mock (Moq):
/// nenhum teste usa o banco nem a connection string.
/// </summary>
public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public PlataformaServiceTests()
    {
        // O conjunto de repositórios simulado devolve o mock de plataformas.
        _repositorio.Setup(r => r.Plataforma).Returns(_plataformas.Object);
    }

    private PlataformaService CriarService() => new(_repositorio.Object);

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange: o Repository não encontra a plataforma 99.
        _plataformas.Setup(p => p.BuscarPorIdAsync(99)).ReturnsAsync((Plataforma?)null);
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(99));

        // Assert: confere o erro do catálogo, não a mensagem em texto.
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
    }
}
