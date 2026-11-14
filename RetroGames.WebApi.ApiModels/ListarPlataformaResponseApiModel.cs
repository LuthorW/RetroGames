namespace RetroGames.WebApi.ApiModels;

/// <summary>Dados de uma plataforma que saem no JSON.</summary>
public class ListarPlataformaResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
