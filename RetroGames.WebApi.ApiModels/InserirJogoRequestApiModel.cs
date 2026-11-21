namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// O que o cliente pode enviar para incluir um jogo. Não tem Id nem Ativo:
/// o id é gerado pelo banco e todo jogo novo nasce ativo (proteção contra overposting).
/// </summary>
public class InserirJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGeneros { get; set; } = new();
}
