namespace RetroGames.Model.Model;

/// <summary>Tabela tb_plataforma.</summary>
public class Plataforma
{
    /// <summary>co_seq_plataforma</summary>
    public int Id { get; set; }

    /// <summary>no_plataforma</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>no_fabricante</summary>
    public string Fabricante { get; set; } = string.Empty;

    /// <summary>nu_ano_lancamento</summary>
    public int AnoLancamento { get; set; }
}
