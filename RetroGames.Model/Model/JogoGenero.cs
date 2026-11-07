namespace RetroGames.Model.Model;

/// <summary>Tabela tb_jogo_genero (associativa, chave primária composta).</summary>
public class JogoGenero
{
    /// <summary>co_jogo</summary>
    public int IdJogo { get; set; }

    /// <summary>co_genero</summary>
    public int IdGenero { get; set; }
}
