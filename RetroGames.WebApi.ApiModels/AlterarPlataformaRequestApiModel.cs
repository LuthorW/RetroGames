namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// O que o cliente envia para alterar uma plataforma: o recurso completo (PUT).
/// O id vem da rota, nunca do corpo.
/// </summary>
public class AlterarPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
