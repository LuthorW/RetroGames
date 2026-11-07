using MySqlConnector;

namespace RetroGames.Repository;

/// <summary>
/// Base de todos os repositórios: abre conexão, cria comando, adiciona parâmetro
/// e executa gravação. Não contém SQL de nenhuma tabela.
/// </summary>
public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Abre uma conexão com o banco. Quem chama deve usar "await using",
    /// para a conexão ser fechada mesmo em caso de erro.
    /// Se houver um TransactionScope aberto pelo Service, a conexão participa dele.
    /// </summary>
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);
        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            // Se a abertura falhar, a conexão não chega a quem chamou: fecha aqui.
            await conexao.DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Cria um comando com o texto SQL. Quem chama deve usar "await using".
    /// </summary>
    protected MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    /// <summary>
    /// Adiciona um parâmetro (@nome) ao comando. Valor nulo vira NULL no banco.
    /// Todo valor vindo de fora entra por aqui, nunca concatenado no SQL (RS06).
    /// </summary>
    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    /// <summary>
    /// Executa um INSERT ou UPDATE e devolve o número de linhas afetadas.
    /// Converte o erro 1062 (UNIQUE) em RegistroDuplicadoException.
    /// Depois de um INSERT, o id gerado fica em comando.LastInsertedId.
    /// </summary>
    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new RegistroDuplicadoException("Registro duplicado em coluna UNIQUE.", ex);
        }
    }
}
