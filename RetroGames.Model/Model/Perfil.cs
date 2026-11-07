namespace RetroGames.Model.Model;

/// <summary>Tabela tb_perfil (domínio, chave fixa).</summary>
public class Perfil
{
    /// <summary>co_perfil</summary>
    public int Id { get; set; }

    /// <summary>ds_perfil</summary>
    public string Descricao { get; set; } = string.Empty;
}
