namespace RetroGames.Model.Model;

/// <summary>Tabela tb_condicao_jogo (domínio, chave fixa).</summary>
public class CondicaoJogo
{
    /// <summary>co_condicao_jogo</summary>
    public int Id { get; set; }

    /// <summary>ds_condicao_jogo</summary>
    public string Descricao { get; set; } = string.Empty;
}
