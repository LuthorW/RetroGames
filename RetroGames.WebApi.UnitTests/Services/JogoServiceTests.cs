using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

/// <summary>
/// Testes do JogoService. Cada repositório usado pelo Service é um mock, e o
/// IRepositorySet simulado devolve todos eles. Nenhum teste usa o banco.
/// </summary>
public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IJogoGeneroRepository> _jogoGeneros = new();
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IUsuarioAtualService> _usuarioAtual = new();

    public JogoServiceTests()
    {
        _repositorio.Setup(r => r.Jogo).Returns(_jogos.Object);
        _repositorio.Setup(r => r.Plataforma).Returns(_plataformas.Object);
        _repositorio.Setup(r => r.CondicaoJogo).Returns(_condicoes.Object);
        _repositorio.Setup(r => r.Genero).Returns(_generos.Object);
        _repositorio.Setup(r => r.JogoGenero).Returns(_jogoGeneros.Object);

        // Dados de apoio: plataforma 1, condição 2 (Usado) e gêneros 1 e 2.
        _plataformas.Setup(p => p.BuscarPorIdAsync(1))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 });
        _plataformas.Setup(p => p.ListarAsync())
            .ReturnsAsync([new Plataforma { Id = 1, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 }]);
        _condicoes.Setup(c => c.ListarAsync())
            .ReturnsAsync([new CondicaoJogo { Id = 2, Descricao = "Usado" }]);
        _generos.Setup(g => g.ListarAsync())
            .ReturnsAsync([new Genero { Id = 1, Nome = "Aventura" }, new Genero { Id = 2, Nome = "Plataforma" }]);
    }

    private JogoService CriarService() => new(_repositorio.Object, _usuarioAtual.Object);

    private static Jogo NovoJogo(int idPlataforma = 1, int idCondicao = 2) => new()
    {
        Nome = "Super Mario World",
        AnoLancamento = 1990,
        Preco = 149.90m,
        Estoque = 3,
        IdPlataforma = idPlataforma,
        IdCondicaoJogo = idCondicao,
    };

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaNaoEncontrada()
    {
        // Arrange: a plataforma 99 não existe (o mock devolve null).
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(NovoJogo(idPlataforma: 99), [1]));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
        _jogos.Verify(j => j.InserirAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaNaoEncontrada()
    {
        // Arrange: a condição 99 não está na lista de condições.
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(NovoJogo(idCondicao: 99), [1]));

        // Assert
        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA, erro.Erro);
        _jogos.Verify(j => j.InserirAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_LancaNaoEncontrado()
    {
        // Arrange: o gênero 99 não está na lista de gêneros.
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(NovoJogo(), [1, 99]));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, erro.Erro);
        _jogos.Verify(j => j.InserirAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task Listar_ComGenero_EnviaSomenteIdsDosJogosDoGenero()
    {
        // Arrange: os jogos 10 e 30 são do gênero 1; o 20 é só do gênero 2.
        _jogoGeneros.Setup(v => v.ListarTodosAsync()).ReturnsAsync(
        [
            new JogoGenero { IdJogo = 10, IdGenero = 1 },
            new JogoGenero { IdJogo = 20, IdGenero = 2 },
            new JogoGenero { IdJogo = 30, IdGenero = 1 },
            new JogoGenero { IdJogo = 30, IdGenero = 2 },
        ]);
        _jogos.Setup(j => j.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Jogo>(), 0));
        var service = CriarService();

        // Act
        await service.ListarAsync(null, null, idGenero: 1, false, 1, 12);

        // Assert: o filtro que chegou ao Repository tem só os ids 10 e 30.
        _jogos.Verify(j => j.ListarAsync(null, null,
            It.Is<IReadOnlyCollection<int>?>(ids => ids != null && ids.OrderBy(id => id).SequenceEqual(new[] { 10, 30 })),
            false, 1, 12), Times.Once);
    }

    [Fact]
    public async Task Listar_SemGenero_NaoConsultaVinculosNemRestringeIds()
    {
        // Arrange
        _jogos.Setup(j => j.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Jogo>(), 0));
        var service = CriarService();

        // Act
        await service.ListarAsync(null, null, idGenero: null, false, 1, 12);

        // Assert: sem gênero, os vínculos não são consultados e a lista de ids é null.
        _jogoGeneros.Verify(v => v.ListarTodosAsync(), Times.Never);
        _jogos.Verify(j => j.ListarAsync(null, null, null, false, 1, 12), Times.Once);
    }

    [Fact]
    public async Task Listar_PaginaComJogos_MontaPlataformaECondicao()
    {
        // Arrange: uma página com um jogo da plataforma 1, condição 2, gênero 1.
        var jogo = NovoJogo();
        jogo.Id = 10;
        jogo.Ativo = true;
        _jogos.Setup(j => j.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Jogo> { jogo }, 1));
        _jogoGeneros.Setup(v => v.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>()))
            .ReturnsAsync([new JogoGenero { IdJogo = 10, IdGenero = 1 }]);
        var service = CriarService();

        // Act
        var (jogos, total) = await service.ListarAsync(null, null, null, false, 1, 12);

        // Assert: o LINQ montou o nome da plataforma e a descrição da condição.
        Assert.Equal(1, total);
        var detalhe = Assert.Single(jogos);
        Assert.Equal("Super Nintendo", detalhe.NomePlataforma);
        Assert.Equal("Usado", detalhe.DescricaoCondicaoJogo);
        Assert.Equal("Aventura", Assert.Single(detalhe.Generos).Nome);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrado()
    {
        // Arrange: o jogo 99 não existe.
        _jogos.Setup(j => j.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
    }

    [Fact]
    public async Task Desativar_IdInexistente_LancaNaoEncontradoENaoDesativa()
    {
        // Arrange: o jogo 99 não existe.
        _jogos.Setup(j => j.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.DesativarAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
        _jogos.Verify(j => j.DesativarAsync(It.IsAny<int>()), Times.Never);
    }
}
