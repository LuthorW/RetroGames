import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'

import { obterMensagemErro } from '../../api/erros'
import { useAutenticacao } from '../../hooks/useAutenticacao'

// Aceita só caminhos internos do site ("/admin/jogos"), nunca outro endereço
// ("https://..." ou "//site.com"), para o voltarPara não levar a um site externo.
function destinoSeguro(voltarPara) {
  if (voltarPara && voltarPara.startsWith('/') && !voltarPara.startsWith('//')) {
    return voltarPara
  }
  return '/'
}

// Tela de login (Especificação 8.5). Com sucesso, volta para o voltarPara
// (a página protegida de onde o usuário veio) ou para o catálogo.
function Login() {
  const { entrar } = useAutenticacao()
  const navegar = useNavigate()
  const [parametros] = useSearchParams()

  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')

  async function enviar(evento) {
    evento.preventDefault()
    setErro('')
    setEnviando(true)

    try {
      await entrar(email, senha)
      navegar(destinoSeguro(parametros.get('voltarPara')), { replace: true })
    } catch (falha) {
      // Ex.: "E-mail ou senha inválidos." (401, código 6). Continua na tela.
      setErro(obterMensagemErro(falha))
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-sm-10 col-md-7 col-lg-5">
        <div className="card">
          <div className="card-body p-4">
            <h1 className="h3 mb-4 text-center">Entrar</h1>

            {erro && (
              <div className="alert alert-danger" role="alert">
                {erro}
              </div>
            )}

            <form onSubmit={enviar} noValidate>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  E-mail
                </label>
                <input
                  id="email"
                  type="email"
                  className="form-control"
                  autoComplete="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  autoFocus
                />
              </div>

              <div className="mb-4">
                <label htmlFor="senha" className="form-label">
                  Senha
                </label>
                <input
                  id="senha"
                  type="password"
                  className="form-control"
                  autoComplete="current-password"
                  value={senha}
                  onChange={(e) => setSenha(e.target.value)}
                />
              </div>

              {/* Desabilitado durante o envio: evita dois cliques. */}
              <button type="submit" className="btn btn-primary w-100" disabled={enviando}>
                {enviando ? 'Entrando...' : 'Entrar'}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Login
