import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'

// Detalhe de um jogo (Especificação 8.5). O id vem da rota /jogos/:id.
function DetalheJogo() {
  const { id } = useParams()

  // Só números são ids válidos. Com /jogos/abc, a página nem chama a API.
  const idValido = /^\d+$/.test(id)

  // Mesmo desenho do catálogo: o resultado guarda a "chave" da busca que o gerou.
  const [tentativa, setTentativa] = useState(0)
  const chave = `${id}#${tentativa}`
  const [resultado, setResultado] = useState({ chave: null, jogo: null, erro: null, naoEncontrado: false })
  const carregando = resultado.chave !== chave

  useEffect(() => {
    if (!idValido) return

    let cancelado = false

    jogoApi
      .buscarPorId(id)
      .then((jogo) => {
        if (!cancelado) setResultado({ chave, jogo, erro: null, naoEncontrado: false })
      })
      .catch((falha) => {
        if (cancelado) return
        // 404: jogo inexistente ou inativo (a API devolve "Jogo não encontrado.").
        const naoEncontrado = falha?.response?.status === 404
        setResultado({ chave, jogo: null, erro: obterMensagemErro(falha), naoEncontrado })
      })

    return () => {
      cancelado = true
    }
  }, [chave, id, idValido])

  const linkVoltar = (
    <Link to="/" className="d-inline-block mb-3">
      ← Voltar ao catálogo
    </Link>
  )

  if (!idValido) {
    return (
      <>
        {linkVoltar}
        <MensagemErro mensagem="Jogo não encontrado." />
      </>
    )
  }

  if (carregando) {
    return (
      <>
        {linkVoltar}
        <Carregando texto="Carregando jogo..." />
      </>
    )
  }

  if (resultado.erro) {
    return (
      <>
        {linkVoltar}
        {/* Jogo inexistente: tentar de novo não adianta, então não há botão. */}
        <MensagemErro
          mensagem={resultado.erro}
          aoTentarNovamente={resultado.naoEncontrado ? undefined : () => setTentativa((valor) => valor + 1)}
        />
      </>
    )
  }

  const { jogo } = resultado
  const esgotado = jogo.estoque === 0

  return (
    <>
      {linkVoltar}

      <div className="row g-4">
        <div className="col-12 col-md-8">
          <p className="sobretitulo mb-2">{jogo.nomePlataforma}</p>
          <h1 className="titulo-brilho h1">{jogo.nome}</h1>
          <p className="mb-3">
            {jogo.anoLancamento} · {jogo.descricaoCondicaoJogo}
          </p>

          <div className="d-flex flex-wrap gap-1 mb-4">
            {jogo.generos.map((genero) => (
              <span key={genero.id} className="badge badge-genero">
                {genero.nome}
              </span>
            ))}
          </div>

          {/* Descrição é opcional: sem ela, o parágrafo não aparece (nada de "null"). */}
          {jogo.descricao && <p className="lead">{jogo.descricao}</p>}
        </div>

        <div className="col-12 col-md-4">
          <div className="card">
            <div className="card-body">
              <p className="display-6 preco mb-2">{formatarPreco(jogo.preco)}</p>
              {esgotado ? (
                <span className="badge text-bg-secondary">Esgotado</span>
              ) : (
                <p className="text-muted mb-0">
                  {jogo.estoque} {jogo.estoque === 1 ? 'unidade em estoque' : 'unidades em estoque'}
                </p>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  )
}

export default DetalheJogo
