// Controles de paginação (Especificação 8.4). Só recebe dados por props:
// a página atual, o total de páginas e a função chamada ao trocar de página.
function Paginacao({ pagina, totalPaginas, aoMudar }) {
  // Com uma página só, não há o que paginar.
  if (totalPaginas <= 1) {
    return null
  }

  const numeros = Array.from({ length: totalPaginas }, (_, indice) => indice + 1)

  return (
    <nav aria-label="Paginação do catálogo">
      <ul className="pagination justify-content-center flex-wrap">
        <li className={`page-item ${pagina === 1 ? 'disabled' : ''}`}>
          <button type="button" className="page-link" onClick={() => aoMudar(pagina - 1)} disabled={pagina === 1}>
            Anterior
          </button>
        </li>

        {numeros.map((numero) => (
          <li key={numero} className={`page-item ${numero === pagina ? 'active' : ''}`}>
            <button
              type="button"
              className="page-link"
              onClick={() => aoMudar(numero)}
              aria-current={numero === pagina ? 'page' : undefined}
            >
              {numero}
            </button>
          </li>
        ))}

        <li className={`page-item ${pagina === totalPaginas ? 'disabled' : ''}`}>
          <button
            type="button"
            className="page-link"
            onClick={() => aoMudar(pagina + 1)}
            disabled={pagina === totalPaginas}
          >
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}

export default Paginacao
