import { useCallback, useEffect, useMemo, useState } from 'react'

import { autenticacaoApi } from '../api/servicos'
import { AutenticacaoContext } from './autenticacaoContext'

// Guarda quem está logado. O cookie é HttpOnly (o JavaScript não lê), então a
// fonte da verdade é a API: o GET /Autenticacao/eu ao abrir o site e a cada F5.
// Nada fica no localStorage nem no sessionStorage.
function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  // Ao iniciar: pergunta à API quem está logado. 401 = visitante (normal).
  useEffect(() => {
    let cancelado = false

    autenticacaoApi
      .usuarioAtual()
      .then((dados) => {
        if (!cancelado) setUsuario(dados)
      })
      .catch(() => {
        if (!cancelado) setUsuario(null)
      })
      .finally(() => {
        if (!cancelado) setCarregando(false)
      })

    return () => {
      cancelado = true
    }
  }, [])

  // Um 401 em outra rota (aviso do api.js): a sessão expirou.
  useEffect(() => {
    const limparSessao = () => setUsuario(null)
    window.addEventListener('sessao:expirada', limparSessao)
    return () => window.removeEventListener('sessao:expirada', limparSessao)
  }, [])

  const entrar = useCallback(async (email, senha) => {
    const dados = await autenticacaoApi.entrar(email, senha)
    setUsuario(dados)
    return dados
  }, [])

  const sair = useCallback(async () => {
    try {
      await autenticacaoApi.sair()
    } finally {
      // Mesmo se a API falhar, o site volta a tratar o usuário como visitante.
      setUsuario(null)
    }
  }, [])

  const valor = useMemo(
    () => ({
      usuario,
      carregando,
      estaAutenticado: usuario !== null,
      ehAdministrador: usuario?.perfil === 'Administrador',
      entrar,
      sair,
    }),
    [usuario, carregando, entrar, sair],
  )

  return (
    <AutenticacaoContext.Provider value={valor}>
      {children}
    </AutenticacaoContext.Provider>
  )
}

export default AutenticacaoProvider
