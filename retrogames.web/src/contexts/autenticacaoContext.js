import { createContext } from 'react'

// Contexto da sessão, compartilhado por todas as telas (Especificação 8.3).
// O valor é fornecido pelo AutenticacaoProvider.
export const AutenticacaoContext = createContext(null)
