using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = @"
            SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();
        while (await leitor.ReadAsync())
        {
            plataformas.Add(Mapear(leitor));
        }
        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM tb_plataforma
            WHERE no_plataforma = @nome
              AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar)";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorar", idIgnorar);

        var quantidade = Convert.ToInt64(await comando.ExecuteScalarAsync());
        return quantidade > 0;
    }

    public async Task<bool> PossuiJogosAsync(int id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM tb_jogo
            WHERE co_plataforma = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        var quantidade = Convert.ToInt64(await comando.ExecuteScalarAsync());
        return quantidade > 0;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = @"
            INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
            VALUES (@nome, @fabricante, @anoLancamento)";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        const string sql = @"
            UPDATE tb_plataforma
            SET no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @anoLancamento
            WHERE co_seq_plataforma = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);
        AdicionarParametro(comando, "@id", plataforma.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        const string sql = @"
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    /// <summary>Converte a linha atual do leitor numa Plataforma, coluna por coluna.</summary>
    private static Plataforma Mapear(MySqlDataReader leitor)
    {
        return new Plataforma
        {
            Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_plataforma")),
            Nome = leitor.GetString(leitor.GetOrdinal("no_plataforma")),
            Fabricante = leitor.GetString(leitor.GetOrdinal("no_fabricante")),
            AnoLancamento = leitor.GetInt32(leitor.GetOrdinal("nu_ano_lancamento")),
        };
    }
}
