import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'

// Moldura comum a todas as páginas: menu, área da página e rodapé.
// O menu muda conforme quem está logado: visitante vê Entrar; quem está logado
// vê "Olá, nome" e Sair; o administrador vê também o menu Administração.
function Layout() {
  const { usuario, carregando, estaAutenticado, ehAdministrador, sair } = useAutenticacao()
  const navegar = useNavigate()

  async function aoSair() {
    await sair()
    navegar('/')
  }

  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-md navbar-dark navbar-retro">
        <div className="container">
          {/* Link (e não <a href>): troca a página sem recarregar o site. */}
          <Link className="navbar-brand fw-bold" to="/">
            RetroGames
          </Link>

          {/* Em tela estreita, o menu vira este botão (Bootstrap collapse). */}
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuPrincipal"
            aria-controls="menuPrincipal"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon" />
          </button>

          <div className="collapse navbar-collapse" id="menuPrincipal">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                {/* NavLink marca o link da página atual com a classe "active". */}
                <NavLink className="nav-link" to="/" end>
                  Catálogo
                </NavLink>
              </li>

              {/* Só para o administrador. A API também protege (RS03). */}
              {ehAdministrador && (
                <li className="nav-item dropdown">
                  <button
                    className="nav-link dropdown-toggle"
                    type="button"
                    data-bs-toggle="dropdown"
                    aria-expanded="false"
                  >
                    Administração
                  </button>
                  <ul className="dropdown-menu">
                    <li>
                      <NavLink className="dropdown-item" to="/admin/jogos">
                        Jogos
                      </NavLink>
                    </li>
                    <li>
                      <NavLink className="dropdown-item" to="/admin/plataformas">
                        Plataformas
                      </NavLink>
                    </li>
                  </ul>
                </li>
              )}
            </ul>

            {/* Enquanto o "eu" não responde, não mostra Entrar nem Sair. */}
            {!carregando && (
              <ul className="navbar-nav align-items-md-center gap-md-2">
                {estaAutenticado ? (
                  <>
                    <li className="nav-item">
                      <span className="navbar-text">Olá, {usuario.nome}</span>
                    </li>
                    <li className="nav-item">
                      <button type="button" className="btn btn-outline-light btn-sm" onClick={aoSair}>
                        Sair
                      </button>
                    </li>
                  </>
                ) : (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/login">
                      Entrar
                    </NavLink>
                  </li>
                )}
              </ul>
            )}
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        <Outlet />
      </main>

      <footer className="bg-light border-top py-3">
        <div className="container text-center text-muted small">
          RetroGames · Loja de jogos retrô usados
        </div>
      </footer>
    </div>
  )
}

export default Layout
