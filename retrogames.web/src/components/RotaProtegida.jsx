import { Navigate, Outlet, useLocation } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'
import { Carregando } from './MensagensEstado'

// Restringe rotas no navegador (Especificação 8.4). Só melhora a navegação:
// quem protege de verdade é a API (RS03), que responde 401 ou 403.
function RotaProtegida({ somenteAdministrador = false }) {
  const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao()
  const localizacao = useLocation()

  // Enquanto o "eu" não responde: assim o F5 numa tela /admin não manda ao login.
  if (carregando) {
    return <Carregando texto="Verificando sessão..." />
  }

  // Sem login: vai ao login e guarda o destino para voltar depois.
  if (!estaAutenticado) {
    const voltarPara = localizacao.pathname + localizacao.search
    return <Navigate to={`/login?voltarPara=${encodeURIComponent(voltarPara)}`} replace />
  }

  // Logado, mas sem o perfil exigido: volta ao catálogo.
  if (somenteAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}

export default RotaProtegida
