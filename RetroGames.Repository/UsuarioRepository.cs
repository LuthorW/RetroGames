using MySqlConnector;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    private const string Colunas = @"
        co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil";

    public UsuarioRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        var sql = $@"
            SELECT {Colunas}
            FROM tb_usuario
            WHERE ds_email = @email";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@email", email);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        var sql = $@"
            SELECT {Colunas}
            FROM tb_usuario
            WHERE co_seq_usuario = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM tb_usuario
            WHERE co_perfil = @idPerfil";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idPerfil", PerfilIds.Administrador);

        var quantidade = Convert.ToInt64(await comando.ExecuteScalarAsync());
        return quantidade > 0;
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO tb_usuario
                (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
            VALUES
                (@nome, @email, @senhaHash, @ativo, UTC_TIMESTAMP(), @idPerfil)";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);

        // ds_email é UNIQUE: e-mail repetido vira RegistroDuplicadoException.
        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    /// <summary>Converte a linha atual do leitor num Usuario, coluna por coluna.</summary>
    private static Usuario Mapear(MySqlDataReader leitor)
    {
        return new Usuario
        {
            Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_usuario")),
            Nome = leitor.GetString(leitor.GetOrdinal("no_usuario")),
            Email = leitor.GetString(leitor.GetOrdinal("ds_email")),
            SenhaHash = leitor.GetString(leitor.GetOrdinal("ds_senha_hash")),
            Ativo = leitor.GetBoolean(leitor.GetOrdinal("st_ativo")),
            DataCadastro = leitor.GetDateTime(leitor.GetOrdinal("dt_cadastro")),
            IdPerfil = leitor.GetInt32(leitor.GetOrdinal("co_perfil")),
        };
    }
}
