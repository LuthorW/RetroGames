// Os três estados de uma tela que busca dados (Especificação 8.4).

// Enquanto os dados não chegam.
export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="d-flex align-items-center gap-2 py-4" role="status">
      <div className="spinner-border spinner-border-sm" aria-hidden="true" />
      <span>{texto}</span>
    </div>
  )
}

// Quando a busca falha. O botão é opcional: só aparece se aoTentarNovamente vier.
export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger d-flex flex-wrap align-items-center justify-content-between gap-2" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-outline-danger btn-sm" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  )
}

// Quando a busca dá certo, mas não há nada para mostrar.
export function EstadoVazio({ titulo, children }) {
  return (
    <div className="text-center text-muted py-5">
      <p className="fs-5 mb-2">{titulo}</p>
      {children}
    </div>
  )
}
