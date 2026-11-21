import { Link } from 'react-router-dom'

// Página para endereços que não existem (rota *).
function NaoEncontrada() {
  return (
    <div className="text-center py-5">
      <h1 className="display-6">Página não encontrada</h1>
      <p className="text-muted">O endereço que você abriu não existe.</p>
      <Link className="btn btn-primary" to="/">
        Ir para o catálogo
      </Link>
    </div>
  )
}

export default NaoEncontrada
