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

    [Fact]
    public async Task Inserir_NomeJaCadastrado_LancaNomeDuplicado()
    {
        // Arrange: RN01, já existe uma plataforma com este nome.
        _plataformas.Setup(p => p.ExisteComNomeAsync("Mega Drive", null)).ReturnsAsync(true);
        var service = CriarService();
        var plataforma = new Plataforma { Nome = "Mega Drive", Fabricante = "Sega", AnoLancamento = 1988 };

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(plataforma));

        // Assert: o erro da RN01, e nada foi gravado.
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
        _plataformas.Verify(p => p.InserirAsync(It.IsAny<Plataforma>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_RepositoryDetectaDuplicidade_LancaNomeDuplicado()
    {
        // Arrange: a conferência passa, mas o UNIQUE do banco recusa o INSERT.
        _plataformas.Setup(p => p.ExisteComNomeAsync("Mega Drive", null)).ReturnsAsync(false);
        _plataformas.Setup(p => p.InserirAsync(It.IsAny<Plataforma>())).ThrowsAsync(new RegistroDuplicadoException("Nome duplicado.", new Exception()));
        var service = CriarService();
        var plataforma = new Plataforma { Nome = "Mega Drive", Fabricante = "Sega", AnoLancamento = 1988 };

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
    }

    [Fact]
    public async Task Inserir_NomeLivre_DevolvePlataformaComIdGerado()
    {
        // Arrange: nome livre; o Repository grava e devolve o id 7.
        _plataformas.Setup(p => p.ExisteComNomeAsync("Mega Drive", null)).ReturnsAsync(false);
        _plataformas.Setup(p => p.InserirAsync(It.IsAny<Plataforma>())).ReturnsAsync(7);
        var service = CriarService();
        var plataforma = new Plataforma { Nome = "  Mega Drive  ", Fabricante = "Sega", AnoLancamento = 1988 };

        // Act
        var gravada = await service.InserirAsync(plataforma);

        // Assert: o caminho que dá certo, com o id gerado e o nome sem espaços.
        Assert.Equal(7, gravada.Id);
        Assert.Equal("Mega Drive", gravada.Nome);
        _plataformas.Verify(p => p.InserirAsync(It.IsAny<Plataforma>()), Times.Once);
    }

    [Fact]
    public async Task Alterar_MantemProprioNome_GravaAlteracao()
    {
        // Arrange: a plataforma 1 existe; ignorando ela mesma, o nome não está em uso.
        _plataformas.Setup(p => p.BuscarPorIdAsync(1))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 });
        _plataformas.Setup(p => p.ExisteComNomeAsync("Super Nintendo", 1)).ReturnsAsync(false);
        var service = CriarService();
        var alterada = new Plataforma { Id = 1, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1991 };

        // Act
        await service.AlterarAsync(alterada);

        // Assert: manter o próprio nome não é duplicidade; o Repository grava.
        _plataformas.Verify(p => p.AlterarAsync(alterada), Times.Once);
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogos()
    {
        // Arrange: RN02, a plataforma 1 existe e tem jogos.
        _plataformas.Setup(p => p.BuscarPorIdAsync(1))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 });
        _plataformas.Setup(p => p.PossuiJogosAsync(1)).ReturnsAsync(true);
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.ExcluirAsync(1));

        // Assert: o erro da RN02, e nada foi excluído.
        Assert.Equal(BusinessErrors.PLATAFORMA_POSSUI_JOGOS, erro.Erro);
        _plataformas.Verify(p => p.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
