import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { generoApi, jogoApi, plataformaApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import CardJogo from '../../components/CardJogo'
import Paginacao from '../../components/Paginacao'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'

const TAMANHO_PAGINA = 12

// Catálogo público (Especificação 8.5): busca, filtros e paginação, todos guardados
// na URL (?busca=mario&idPlataforma=2&pagina=2). Assim, o link pode ser compartilhado
// e o botão Voltar do navegador restaura o filtro anterior.
function Catalogo() {
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const idPlataforma = parametros.get('idPlataforma') ?? ''
  const idGenero = parametros.get('idGenero') ?? ''
  const pagina = Number(parametros.get('pagina')) || 1

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])

  // Resultado da última busca, guardado junto com a "chave" da busca que o gerou.
  // Se a chave guardada não é a da URL atual, a busca nova ainda está carregando.
  const [tentativa, setTentativa] = useState(0)
  const chave = `${parametros.toString()}#${tentativa}`
  const [resultado, setResultado] = useState({ chave: null, dados: null, erro: null })
  const carregando = resultado.chave !== chave

  // Listas dos filtros: carregam uma vez só.
  useEffect(() => {
    plataformaApi.listar().then(setPlataformas).catch(() => setPlataformas([]))
    generoApi.listar().then(setGeneros).catch(() => setGeneros([]))
  }, [])

  // Jogos: recarregam sempre que um filtro da URL muda (ou em "Tentar novamente").
  useEffect(() => {
    let cancelado = false

    jogoApi
      .listar({
        busca: busca || undefined,
        idPlataforma: idPlataforma || undefined,
        idGenero: idGenero || undefined,
        pagina,
        tamanhoPagina: TAMANHO_PAGINA,
      })
      .then((dados) => {
        if (!cancelado) setResultado({ chave, dados, erro: null })
      })
      .catch((falha) => {
        if (!cancelado) setResultado({ chave, dados: null, erro: obterMensagemErro(falha) })
      })

    // Se o filtro mudar antes da resposta chegar, a resposta antiga é ignorada.
    return () => {
      cancelado = true
    }
  }, [chave, busca, idPlataforma, idGenero, pagina])

  // Troca filtros na URL. Trocar um filtro volta à página 1.
  function atualizarFiltros(novosFiltros) {
    const novosParametros = new URLSearchParams(parametros)
    for (const [nome, valor] of Object.entries(novosFiltros)) {
      if (valor) {
        novosParametros.set(nome, valor)
      } else {
        novosParametros.delete(nome)
      }
    }
    novosParametros.delete('pagina')
    setParametros(novosParametros)
  }

  function mudarPagina(novaPagina) {
    const novosParametros = new URLSearchParams(parametros)
    if (novaPagina > 1) {
      novosParametros.set('pagina', novaPagina)
    } else {
      novosParametros.delete('pagina')
    }
    setParametros(novosParametros)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  function aoBuscar(evento) {
    evento.preventDefault()
    const texto = new FormData(evento.currentTarget).get('busca').toString().trim()
    atualizarFiltros({ busca: texto })
  }

  function limparFiltros() {
    setParametros(new URLSearchParams())
  }

  const temFiltro = Boolean(busca || idPlataforma || idGenero)

  return (
    <>
      {/* Destaque do topo, com órbitas animadas (só decoração). */}
      <section className="destaque">
        <span className="orbita orbita-1" aria-hidden="true" />
        <span className="orbita orbita-2" aria-hidden="true" />
        <p className="sobretitulo">Loja de jogos retrô usados</p>
        <h1 className="titulo-brilho mb-3">Catálogo</h1>
        <p className="mb-0">Clássicos de todas as gerações, conferidos e prontos para jogar de novo.</p>
      </section>

      <div className="row g-2 mb-4">
        {/* key={busca}: quando a URL muda (Voltar, Limpar), o campo mostra o texto dela. */}
        <form key={busca} className="col-12 col-lg-6" role="search" onSubmit={aoBuscar}>
          <div className="input-group">
            <input
              type="search"
              name="busca"
              className="form-control"
              placeholder="Buscar pelo nome do jogo"
              aria-label="Buscar pelo nome do jogo"
              defaultValue={busca}
            />
            <button type="submit" className="btn btn-primary">
              Buscar
            </button>
          </div>
        </form>

        <div className="col-6 col-lg-3">
          <select
            className="form-select"
            aria-label="Filtrar por plataforma"
            value={idPlataforma}
            onChange={(evento) => atualizarFiltros({ idPlataforma: evento.target.value })}
          >
            <option value="">Todas as plataformas</option>
            {plataformas.map((plataforma) => (
              <option key={plataforma.id} value={plataforma.id}>
                {plataforma.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-6 col-lg-3">
          <select
            className="form-select"
            aria-label="Filtrar por gênero"
            value={idGenero}
            onChange={(evento) => atualizarFiltros({ idGenero: evento.target.value })}
          >
            <option value="">Todos os gêneros</option>
            {generos.map((genero) => (
              <option key={genero.id} value={genero.id}>
                {genero.nome}
              </option>
            ))}
          </select>
        </div>
      </div>

      {carregando && <Carregando texto="Carregando jogos..." />}

      {!carregando && resultado.erro && (
        <MensagemErro mensagem={resultado.erro} aoTentarNovamente={() => setTentativa((valor) => valor + 1)} />
      )}

      {!carregando && resultado.dados && resultado.dados.totalItens === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado.">
          {temFiltro && (
            <button type="button" className="btn btn-link" onClick={limparFiltros}>
              Limpar filtros
            </button>
          )}
        </EstadoVazio>
      )}

      {!carregando && resultado.dados && resultado.dados.totalItens > 0 && (
        <>
          <div className="d-flex justify-content-between align-items-center mb-3">
            <p className="text-muted mb-0">
              {resultado.dados.totalItens} {resultado.dados.totalItens === 1 ? 'jogo encontrado' : 'jogos encontrados'}
            </p>
            {temFiltro && (
              <button type="button" className="btn btn-link btn-sm" onClick={limparFiltros}>
                Limpar filtros
              </button>
            )}
          </div>

          <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4 g-3 mb-4">
            {resultado.dados.itens.map((jogo) => (
              <div key={jogo.id} className="col">
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>

          <Paginacao pagina={pagina} totalPaginas={resultado.dados.totalPaginas} aoMudar={mudarPagina} />
        </>
      )}
    </>
  )
}

export default Catalogo
