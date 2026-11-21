import axios from 'axios'

// Único lugar do projeto que importa o Axios (Especificação 8.2).
// baseURL '/api': o proxy do Vite repassa para a API na porta 5080.
// withCredentials: o cookie de sessão vai junto em toda requisição.
const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

export default api
