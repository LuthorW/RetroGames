import axios from 'axios'

// Único lugar do projeto que importa o Axios (Especificação 8.2).
// baseURL '/api': o proxy do Vite repassa para a API na porta 5080.
// withCredentials: o cookie de sessão vai junto em toda requisição.
const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

// Rotas em que o 401 é resposta esperada, e não sessão expirada:
// o "eu" responde 401 para o visitante e o login, para senha errada.
const ROTAS_SEM_AVISO = ['/Autenticacao/eu', '/Autenticacao/login']

// Um 401 de qualquer outra rota avisa o app que a sessão acabou.
api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const status = erro?.response?.status
    const url = erro?.config?.url ?? ''
    const ehRotaSemAviso = ROTAS_SEM_AVISO.some((rota) => url.startsWith(rota))

    if (status === 401 && !ehRotaSemAviso) {
      window.dispatchEvent(new Event('sessao:expirada'))
    }

    // Repassa o erro para a tela decidir o que mostrar.
    return Promise.reject(erro)
  },
)

export default api
