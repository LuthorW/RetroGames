using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = @"
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        return await LerVinculosAsync(comando);
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos)
    {
        // Lista vazia: nenhum vínculo a buscar ("IN ()" é SQL inválido).
        if (idsJogos.Count == 0)
        {
            return new List<JogoGenero>();
        }

        // Um parâmetro por id: IN (@id0, @id1, ...). Os valores entram como parâmetro.
        var nomes = idsJogos.Select((_, indice) => $"@id{indice}").ToList();
        var sql = $@"
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", nomes)})";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        var indiceId = 0;
        foreach (var id in idsJogos)
        {
            AdicionarParametro(comando, nomes[indiceId], id);
            indiceId++;
        }

        return await LerVinculosAsync(comando);
    }

    public async Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros)
    {
        // Sem gêneros, nada a gravar. (A regra "ao menos um gênero", RN05, é do Service.)
        if (idsGeneros.Count == 0)
        {
            return;
        }

        // Um único INSERT com uma linha por gênero: (@idJogo, @genero0), (@idJogo, @genero1), ...
        var nomes = idsGeneros.Select((_, indice) => $"@genero{indice}").ToList();
        var linhas = nomes.Select(nome => $"(@idJogo, {nome})");
        var sql = $@"
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES {string.Join(", ", linhas)}";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);

        var indiceGenero = 0;
        foreach (var idGenero in idsGeneros)
        {
            AdicionarParametro(comando, nomes[indiceGenero], idGenero);
            indiceGenero++;
        }

        // A chave primária composta não aceita o mesmo gênero duas vezes (erro 1062).
        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = @"
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }

    /// <summary>Executa a consulta e converte cada linha num JogoGenero.</summary>
    private static async Task<List<JogoGenero>> LerVinculosAsync(MySqlCommand comando)
    {
        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();
        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32(leitor.GetOrdinal("co_jogo")),
                IdGenero = leitor.GetInt32(leitor.GetOrdinal("co_genero")),
            });
        }
        return vinculos;
    }
}
