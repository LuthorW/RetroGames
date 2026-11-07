using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas = @"
        co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
        st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo";

    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? ids,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina)
    {
        // Lista de ids informada mas vazia (ex.: gênero sem jogos): nenhum jogo atende.
        // "IN ()" é SQL inválido, então nem consultamos o banco.
        if (ids is not null && ids.Count == 0)
        {
            return (new List<Jogo>(), 0);
        }

        // O mesmo WHERE serve para a contagem e para a página.
        var where = @"
            WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
              AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
              AND (@incluirInativos = TRUE OR st_ativo = TRUE)";

        // Para a lista de ids, o SQL ganha um parâmetro por id: IN (@id0, @id1, ...).
        // Os VALORES continuam entrando como parâmetro; só os nomes são montados aqui.
        List<string> nomesIds = new();
        if (ids is not null)
        {
            nomesIds = ids.Select((_, indice) => $"@id{indice}").ToList();
            where += $" AND co_seq_jogo IN ({string.Join(", ", nomesIds)})";
        }

        var sqlTotal = $"SELECT COUNT(*) FROM tb_jogo {where}";
        var sqlPagina = $@"
            SELECT {Colunas}
            FROM tb_jogo
            {where}
            ORDER BY no_jogo
            LIMIT @limite OFFSET @deslocamento";

        await using var conexao = await AbrirConexaoAsync();

        // 1) Total encontrado
        int total;
        await using (var comandoTotal = CriarComando(conexao, sqlTotal))
        {
            AdicionarFiltros(comandoTotal, busca, idPlataforma, ids, nomesIds, incluirInativos);
            total = Convert.ToInt32(await comandoTotal.ExecuteScalarAsync());
        }

        // 2) Jogos da página
        var jogos = new List<Jogo>();
        await using (var comandoPagina = CriarComando(conexao, sqlPagina))
        {
            AdicionarFiltros(comandoPagina, busca, idPlataforma, ids, nomesIds, incluirInativos);
            AdicionarParametro(comandoPagina, "@limite", tamanhoPagina);
            AdicionarParametro(comandoPagina, "@deslocamento", (pagina - 1) * tamanhoPagina);

            await using var leitor = await comandoPagina.ExecuteReaderAsync();
            while (await leitor.ReadAsync())
            {
                jogos.Add(Mapear(leitor));
            }
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        var sql = $@"
            SELECT {Colunas}
            FROM tb_jogo
            WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = @"
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
                 dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES
                (@nome, @descricao, @anoLancamento, @preco, @estoque, @ativo,
                 UTC_TIMESTAMP(), @idPlataforma, @idCondicaoJogo)";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarDados(comando, jogo);

        // tb_jogo não tem coluna UNIQUE: não passa por ExecutarGravacaoAsync.
        await comando.ExecuteNonQueryAsync();
        return (int)comando.LastInsertedId;
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                st_ativo = @ativo,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarDados(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await comando.ExecuteNonQueryAsync();
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET st_ativo = FALSE
            WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    /// <summary>Parâmetros do filtro, iguais na contagem e na página.</summary>
    private void AdicionarFiltros(
        MySqlCommand comando,
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? ids,
        List<string> nomesIds,
        bool incluirInativos)
    {
        AdicionarParametro(comando, "@busca", string.IsNullOrWhiteSpace(busca) ? null : busca);
        AdicionarParametro(comando, "@idPlataforma", idPlataforma);
        AdicionarParametro(comando, "@incluirInativos", incluirInativos);

        if (ids is not null)
        {
            var indice = 0;
            foreach (var id in ids)
            {
                AdicionarParametro(comando, nomesIds[indice], id);
                indice++;
            }
        }
    }

    /// <summary>Parâmetros com os dados do jogo, usados em inserir e alterar.</summary>
    private void AdicionarDados(MySqlCommand comando, Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
    }

    /// <summary>Converte a linha atual do leitor num Jogo, coluna por coluna.</summary>
    private static Jogo Mapear(MySqlDataReader leitor)
    {
        var ordinalDescricao = leitor.GetOrdinal("ds_jogo");

        return new Jogo
        {
            Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_jogo")),
            Nome = leitor.GetString(leitor.GetOrdinal("no_jogo")),
            Descricao = leitor.IsDBNull(ordinalDescricao) ? null : leitor.GetString(ordinalDescricao),
            AnoLancamento = leitor.GetInt32(leitor.GetOrdinal("nu_ano_lancamento")),
            Preco = leitor.GetDecimal(leitor.GetOrdinal("vl_preco")),
            Estoque = leitor.GetInt32(leitor.GetOrdinal("qt_estoque")),
            Ativo = leitor.GetBoolean(leitor.GetOrdinal("st_ativo")),
            DataCadastro = leitor.GetDateTime(leitor.GetOrdinal("dt_cadastro")),
            IdPlataforma = leitor.GetInt32(leitor.GetOrdinal("co_plataforma")),
            IdCondicaoJogo = leitor.GetInt32(leitor.GetOrdinal("co_condicao_jogo")),
        };
    }
}
