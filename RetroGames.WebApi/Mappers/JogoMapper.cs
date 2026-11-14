using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

/// <summary>Conversões entre o JogoDetalheServiceModel e os ApiModels de jogo.</summary>
public static class JogoMapper
{
    /// <summary>Um jogo na listagem: os gêneros saem só com o nome.</summary>
    public static ListarJogoResponseApiModel ParaListarResponse(this JogoDetalheServiceModel detalhe)
    {
        return new ListarJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicaoJogo = detalhe.DescricaoCondicaoJogo,
            NomesGeneros = detalhe.Generos.Select(genero => genero.Nome).ToList(),
        };
    }

    /// <summary>O detalhe do jogo: os gêneros saem com id e nome.</summary>
    public static ListarPorIdJogoResponseApiModel ParaListarPorIdResponse(this JogoDetalheServiceModel detalhe)
    {
        return new ListarPorIdJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Descricao = detalhe.Jogo.Descricao,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            IdPlataforma = detalhe.Jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
            DescricaoCondicaoJogo = detalhe.DescricaoCondicaoJogo,
            Generos = detalhe.Generos.Select(genero => genero.ParaResponse()).ToList(),
        };
    }

    /// <summary>Monta a página com os jogos e os totais.</summary>
    public static PaginaResponseApiModel<ListarJogoResponseApiModel> ParaPaginaResponse(
        List<JogoDetalheServiceModel> jogos,
        int totalItens,
        int pagina,
        int tamanhoPagina)
    {
        return new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = jogos.Select(jogo => jogo.ParaListarResponse()).ToList(),
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            // Arredonda para cima: 22 jogos de 5 em 5 = 5 páginas.
            TotalPaginas = tamanhoPagina > 0
                ? (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
                : 0,
        };
    }
}
