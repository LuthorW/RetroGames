namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// O que o cliente pode enviar para incluir uma plataforma. Não tem Id:
/// quem gera é o banco (proteção contra overposting).
/// </summary>
public class InserirPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
