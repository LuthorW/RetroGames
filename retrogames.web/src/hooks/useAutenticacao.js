import { useContext } from 'react'

import { AutenticacaoContext } from '../contexts/autenticacaoContext'

// Dá às telas acesso à sessão: usuario, carregando, estaAutenticado,
// ehAdministrador, entrar e sair.
export function useAutenticacao() {
  const contexto = useContext(AutenticacaoContext)
  if (!contexto) {
    throw new Error('useAutenticacao precisa estar dentro do AutenticacaoProvider.')
  }
  return contexto
}
