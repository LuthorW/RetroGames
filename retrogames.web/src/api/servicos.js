import api from './api'

// Uma função por endpoint. Cada uma já devolve o "data" do envelope
// { responseCode, data, errorMessage }, para as páginas não lidarem com ele.
// Por enquanto, só leituras: gravação e login entram na aula 8.

export const jogoApi = {
  // filtros: { busca, idPlataforma, idGenero, pagina, tamanhoPagina }
  listar: async (filtros) => {
    const resposta = await api.get('/Jogo', { params: filtros })
    return resposta.data.data
  },
  buscarPorId: async (id) => {
    const resposta = await api.get(`/Jogo/${id}`)
    return resposta.data.data
  },
}

export const plataformaApi = {
  listar: async () => {
    const resposta = await api.get('/Plataforma')
    return resposta.data.data
  },
}

export const generoApi = {
  listar: async () => {
    const resposta = await api.get('/Genero')
    return resposta.data.data
  },
}

export const condicaoJogoApi = {
  listar: async () => {
    const resposta = await api.get('/CondicaoJogo')
    return resposta.data.data
  },
}
