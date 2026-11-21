using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

/// <summary>Conversões entre a entidade Plataforma e os ApiModels.</summary>
public static class PlataformaMapper
{
    public static ListarPlataformaResponseApiModel ParaResponse(this Plataforma plataforma)
    {
        return new ListarPlataformaResponseApiModel
        {
            Id = plataforma.Id,
            Nome = plataforma.Nome,
            Fabricante = plataforma.Fabricante,
            AnoLancamento = plataforma.AnoLancamento,
        };
    }

    /// <summary>Inclusão: sem id, que é gerado pelo banco.</summary>
    public static Plataforma ParaModel(this InserirPlataformaRequestApiModel request)
    {
        return new Plataforma
        {
            Nome = request.Nome,
            Fabricante = request.Fabricante,
            AnoLancamento = request.AnoLancamento,
        };
    }

    /// <summary>Alteração: o id vem da rota.</summary>
    public static Plataforma ParaModel(this AlterarPlataformaRequestApiModel request, int id)
    {
        return new Plataforma
        {
            Id = id,
            Nome = request.Nome,
            Fabricante = request.Fabricante,
            AnoLancamento = request.AnoLancamento,
        };
    }
}
