using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        int? idGenero,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina)
    {
        // Filtro por gênero (depende de outra tabela): seleciona com LINQ os ids dos
        // jogos do gênero e entrega a lista ao Repository, que filtra e pagina no SQL.
        List<int>? idsJogos = null;
        if (idGenero is not null)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();
            idsJogos = vinculos
                .Where(vinculo => vinculo.IdGenero == idGenero)
                .Select(vinculo => vinculo.IdJogo)
                .Distinct()
                .ToList();
        }

        // incluirInativos é repassado como veio. A regra de só o Administrador
        // ver jogos inativos (RN04) entra na aula 6.
        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            busca, idPlataforma, idsJogos, incluirInativos, pagina, tamanhoPagina);

        var detalhes = await MontarDetalhesAsync(jogos);
        return (detalhes, total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await BuscarJogoExistenteAsync(id);
        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });
        return detalhes.Single();
    }

    public async Task<Jogo> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;

        var generosDoJogo = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generosDoJogo);

        // Duas gravações (jogo e vínculos): ou as duas acontecem, ou nenhuma.
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, generosDoJogo);

            // Só depois da última gravação. Sem Complete(), tudo é desfeito.
            transactionScope.Complete();
        }

        return jogo;
    }

    public async Task AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var jogoGravado = await BuscarJogoExistenteAsync(jogo.Id);

        jogo.Nome = jogo.Nome.Trim();

        // A situação não muda na alteração: desativar é uma operação própria (RN03).
        jogo.Ativo = jogoGravado.Ativo;

        var generosDoJogo = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generosDoJogo);

        // Três gravações (jogo, remoção e inclusão de vínculos): ou todas, ou nenhuma.
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorio.Jogo.AlterarAsync(jogo);
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, generosDoJogo);

            transactionScope.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        await BuscarJogoExistenteAsync(id);

        // RN03: exclusão lógica (st_ativo = FALSE). O jogo nunca é apagado.
        await _repositorio.Jogo.DesativarAsync(id);
    }

    /// <summary>O jogo gravado, ou BusinessRuleException com JOGO_NAO_ENCONTRADO.</summary>
    private async Task<Jogo> BuscarJogoExistenteAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);
        if (jogo is null)
        {
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);
        }
        return jogo;
    }

    /// <summary>
    /// RN05: plataforma, condição e gêneros informados precisam existir.
    /// Lança PLATAFORMA_NAO_ENCONTRADA (100), CONDICAO_NAO_ENCONTRADA (104)
    /// ou GENERO_NAO_ENCONTRADO (105).
    /// </summary>
    private async Task ValidarReferenciasAsync(Jogo jogo, List<int> idsGeneros)
    {
        if (await _repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
        {
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(condicao => condicao.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CONDICAO_NAO_ENCONTRADA);
        }

        var idsGenerosExistentes = (await _repositorio.Genero.ListarAsync())
            .Select(genero => genero.Id)
            .ToHashSet();
        if (idsGeneros.Any(idGenero => !idsGenerosExistentes.Contains(idGenero)))
        {
            throw new BusinessRuleException(BusinessErrors.GENERO_NAO_ENCONTRADO);
        }
    }

    /// <summary>
    /// Monta, com LINQ, o detalhe de uma lista de jogos. Faz sempre as mesmas
    /// 4 consultas (plataformas, condições, gêneros e vínculos), qualquer que seja
    /// o número de jogos: nunca uma consulta por jogo.
    /// </summary>
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var plataformas = (await _repositorio.Plataforma.ListarAsync())
            .ToDictionary(plataforma => plataforma.Id);
        var condicoes = (await _repositorio.CondicaoJogo.ListarAsync())
            .ToDictionary(condicao => condicao.Id);
        var generos = (await _repositorio.Genero.ListarAsync())
            .ToDictionary(genero => genero.Id);
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(
            jogos.Select(jogo => jogo.Id).ToList());

        return jogos
            .Select(jogo => new JogoDetalheServiceModel
            {
                Jogo = jogo,
                NomePlataforma = plataformas[jogo.IdPlataforma].Nome,
                DescricaoCondicaoJogo = condicoes[jogo.IdCondicaoJogo].Descricao,
                Generos = vinculos
                    .Where(vinculo => vinculo.IdJogo == jogo.Id)
                    .Select(vinculo => generos[vinculo.IdGenero])
                    .OrderBy(genero => genero.Nome)
                    .ToList(),
            })
            .ToList();
    }
}
