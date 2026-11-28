import { Link, NavLink, Outlet } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'

// Moldura comum a todas as páginas: menu, área da página e rodapé.
function Layout() {
  // TESTE PROVISÓRIO da etapa 1: remover antes do commit.
  const { usuario, carregando } = useAutenticacao()

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
            </ul>

            {/* TESTE PROVISÓRIO da etapa 1: remover antes do commit. */}
            <span className="navbar-text">
              {carregando ? '...' : usuario ? usuario.nome : 'visitante'}
            </span>
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
