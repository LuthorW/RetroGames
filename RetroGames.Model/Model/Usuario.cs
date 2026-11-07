namespace RetroGames.Model.Model;

/// <summary>Tabela tb_usuario.</summary>
public class Usuario
{
    /// <summary>co_seq_usuario</summary>
    public int Id { get; set; }

    /// <summary>no_usuario</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>ds_email</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>ds_senha_hash</summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>st_ativo</summary>
    public bool Ativo { get; set; }

    /// <summary>dt_cadastro (UTC)</summary>
    public DateTime DataCadastro { get; set; }

    /// <summary>co_perfil</summary>
    public int IdPerfil { get; set; }
}
