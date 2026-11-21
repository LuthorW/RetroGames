const MENSAGEM_SEM_CONEXAO = 'Não foi possível conectar ao servidor.'
const MENSAGEM_GENERICA = 'Ocorreu um erro inesperado. Tente novamente.'

// Transforma uma falha do Axios em mensagem para o usuário.
export function obterMensagemErro(erro) {
  // Com resposta da API: a mensagem do envelope (ex.: "Jogo não encontrado.").
  const mensagem = erro?.response?.data?.errorMessage
  if (mensagem) {
    return mensagem
  }

  // Sem resposta, ou o proxy do Vite sem conseguir falar com a API (502, 503, 504).
  const status = erro?.response?.status
  if (!erro?.response || status === 502 || status === 503 || status === 504) {
    return MENSAGEM_SEM_CONEXAO
  }

  return MENSAGEM_GENERICA
}
