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
  // jogo: { nome, descricao, anoLancamento, preco, estoque, idPlataforma, idCondicaoJogo, idsGeneros }
  inserir: async (jogo) => {
    const resposta = await api.post('/Jogo', jogo)
    return resposta.data.data
  },
  // Na alteração, o jogo também leva "ativo".
  alterar: async (id, jogo) => {
    const resposta = await api.put(`/Jogo/${id}`, jogo)
    return resposta.data.data
  },
  // O DELETE do jogo não apaga: marca como inativo (RN03).
  desativar: async (id) => {
    await api.delete(`/Jogo/${id}`)
  },
}

export const plataformaApi = {
  listar: async () => {
    const resposta = await api.get('/Plataforma')
    return resposta.data.data
  },
  // plataforma: { nome, fabricante, anoLancamento }
  inserir: async (plataforma) => {
    const resposta = await api.post('/Plataforma', plataforma)
    return resposta.data.data
  },
  alterar: async (id, plataforma) => {
    const resposta = await api.put(`/Plataforma/${id}`, plataforma)
    return resposta.data.data
  },
  excluir: async (id) => {
    await api.delete(`/Plataforma/${id}`)
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
