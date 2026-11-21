import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

// Cartão de um jogo no catálogo (Especificação 8.4). Só recebe dados por props.
function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body d-flex flex-column">
        <p className="text-muted small mb-1">{jogo.nomePlataforma}</p>
        <h2 className="h5 card-title">{jogo.nome}</h2>
        <p className="small mb-2">
          {jogo.anoLancamento} · {jogo.descricaoCondicaoJogo}
        </p>

        <div className="d-flex flex-wrap gap-1 mb-3">
          {jogo.nomesGeneros.map((genero) => (
            <span key={genero} className="badge text-bg-light border">
              {genero}
            </span>
          ))}
        </div>

        <div className="mt-auto d-flex align-items-center justify-content-between">
          <span className="fs-5 fw-semibold">{formatarPreco(jogo.preco)}</span>
          {esgotado && <span className="badge text-bg-secondary">Esgotado</span>}
        </div>
      </div>

      <div className="card-footer bg-transparent border-top-0 pt-0 pb-3">
        {/* stretched-link: o cartão inteiro vira clicável. */}
        <Link className="btn btn-outline-primary btn-sm stretched-link" to={`/jogos/${jogo.id}`}>
          Ver detalhes
        </Link>
      </div>
    </div>
  )
}

export default CardJogo
