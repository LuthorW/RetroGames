using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = @"
            SELECT co_seq_genero, no_genero
            FROM tb_genero
            ORDER BY no_genero";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var generos = new List<Genero>();
        while (await leitor.ReadAsync())
        {
            generos.Add(new Genero
            {
                Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_genero")),
                Nome = leitor.GetString(leitor.GetOrdinal("no_genero")),
            });
        }
        return generos;
    }
}
