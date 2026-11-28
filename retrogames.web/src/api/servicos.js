import api from './api'

// Uma função por endpoint. Cada uma já devolve o "data" do envelope
// { responseCode, data, errorMessage }, para as páginas não lidarem com ele.

export const autenticacaoApi = {
  // Confere e-mail e senha; a API devolve o cookie de sessão e o usuário.
  entrar: async (email, senha) => {
    const resposta = await api.post('/Autenticacao/login', { email, senha })
    return resposta.data.data
  },
  // Encerra a sessão (a API apaga o cookie).
  sair: async () => {
    await api.post('/Autenticacao/logout')
  },
  // Quem está logado: { id, nome, email, perfil }. Visitante recebe 401.
  usuarioAtual: async () => {
    const resposta = await api.get('/Autenticacao/eu')
    return resposta.data.data
  },
}

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
