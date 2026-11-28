import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'

import { condicaoJogoApi, generoApi, jogoApi, plataformaApi } from '../../api/servicos'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'

const FORMULARIO_VAZIO = {
  nome: '',
  descricao: '',
  anoLancamento: '',
  preco: '',
  estoque: '',
  idPlataforma: '',
  idCondicaoJogo: '',
  idsGeneros: [],
  ativo: true,
}

// Inclusão (/admin/jogos/novo) e alteração (/admin/jogos/:id/editar) de jogo
// (Especificação 8.5). Os gêneros são N:N, enviados como a lista idsGeneros.
function FormularioJogo() {
  const { id } = useParams()
  const ehEdicao = id !== undefined
  const idValido = !ehEdicao || /^\d+$/.test(id)
  const navegar = useNavigate()

  // Carrega as listas (e, na edição, o jogo). A "chave" diz de qual carga é o resultado.
  const [tentativa, setTentativa] = useState(0)
  const chave = `${id ?? 'novo'}#${tentativa}`
  const [carga, setCarga] = useState({ chave: null, listas: null, erro: null })
  const carregando = carga.chave !== chave

  const [formulario, setFormulario] = useState(FORMULARIO_VAZIO)
  const [errosCampos, setErrosCampos] = useState({})
  const [erroGravacao, setErroGravacao] = useState('')
  const [enviando, setEnviando] = useState(false)

  useEffect(() => {
    if (!idValido) return

    let cancelado = false

    Promise.all([
      plataformaApi.listar(),
      generoApi.listar(),
      condicaoJogoApi.listar(),
      ehEdicao ? jogoApi.buscarPorId(id) : Promise.resolve(null),
    ])
      .then(([plataformas, generos, condicoes, jogo]) => {
        if (cancelado) return
        // Os campos do formulário trabalham com texto. Na inclusão, começa vazio.
        setFormulario(
          jogo
            ? {
                nome: jogo.nome,
                descricao: jogo.descricao ?? '',
                anoLancamento: String(jogo.anoLancamento),
                preco: String(jogo.preco),
                estoque: String(jogo.estoque),
                idPlataforma: String(jogo.idPlataforma),
                idCondicaoJogo: String(jogo.idCondicaoJogo),
                idsGeneros: jogo.generos.map((genero) => genero.id),
                ativo: jogo.ativo,
              }
            : FORMULARIO_VAZIO,
        )
        setCarga({ chave, listas: { plataformas, generos, condicoes }, erro: null })
      })
      .catch((falha) => {
        if (!cancelado) setCarga({ chave, listas: null, erro: obterMensagemErro(falha) })
      })

    return () => {
      cancelado = true
    }
  }, [chave, id, ehEdicao, idValido])

  function alterarCampo(evento) {
    const { name, value, type, checked } = evento.target
    setFormulario((atual) => ({ ...atual, [name]: type === 'checkbox' ? checked : value }))
  }

  function alternarGenero(idGenero) {
    setFormulario((atual) => ({
      ...atual,
      idsGeneros: atual.idsGeneros.includes(idGenero)
        ? atual.idsGeneros.filter((valor) => valor !== idGenero)
        : [...atual.idsGeneros, idGenero],
    }))
  }

  async function salvar(evento) {
    evento.preventDefault()
    setErrosCampos({})
    setErroGravacao('')
    setEnviando(true)

    // Os campos devolvem texto: os números são convertidos com Number().
    const dados = {
      nome: formulario.nome.trim(),
      descricao: formulario.descricao.trim() || null,
      anoLancamento: Number(formulario.anoLancamento),
      preco: Number(formulario.preco),
      estoque: Number(formulario.estoque),
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros,
    }

    try {
      if (ehEdicao) {
        await jogoApi.alterar(id, { ...dados, ativo: formulario.ativo })
      } else {
        await jogoApi.inserir(dados)
      }
      navegar('/admin/jogos')
    } catch (falha) {
      const porCampo = obterErrosDosCampos(falha)
      if (Object.keys(porCampo).length > 0) {
        setErrosCampos(porCampo)
      } else {
        // Ex.: plataforma, condição ou gênero inexistente (404, códigos 100, 104, 105).
        setErroGravacao(obterMensagemErro(falha))
      }
      setEnviando(false)
    }
  }

  const titulo = ehEdicao ? 'Alterar jogo' : 'Novo jogo'
  const linkVoltar = (
    <Link to="/admin/jogos" className="d-inline-block mb-3">
      ← Voltar à lista de jogos
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
        <Carregando texto="Carregando formulário..." />
      </>
    )
  }

  if (carga.erro) {
    return (
      <>
        {linkVoltar}
        <MensagemErro mensagem={carga.erro} aoTentarNovamente={() => setTentativa((valor) => valor + 1)} />
      </>
    )
  }

  const { plataformas, generos, condicoes } = carga.listas
  const classeCampo = (campo, base = 'form-control') => `${base}${errosCampos[campo] ? ' is-invalid' : ''}`
  // Erros da lista de gêneros podem vir como "idsGeneros" ou "idsGeneros[0]".
  const erroGeneros =
    errosCampos.idsGeneros ??
    Object.entries(errosCampos).find(([campo]) => campo.startsWith('idsGeneros'))?.[1]

  return (
    <>
      {linkVoltar}
      <h1 className="h3 titulo-pagina">{titulo}</h1>

      {erroGravacao && (
        <div className="alert alert-danger" role="alert">
          {erroGravacao}
        </div>
      )}

      <form onSubmit={salvar} noValidate className="card">
        <div className="card-body row g-3">
          <div className="col-12 col-md-8">
            <label htmlFor="nome" className="form-label">
              Nome
            </label>
            <input id="nome" name="nome" className={classeCampo('nome')} value={formulario.nome} onChange={alterarCampo} />
            <div className="invalid-feedback">{errosCampos.nome}</div>
          </div>

          <div className="col-12 col-md-4">
            <label htmlFor="anoLancamento" className="form-label">
              Ano de lançamento
            </label>
            <input
              id="anoLancamento"
              name="anoLancamento"
              type="number"
              className={classeCampo('anoLancamento')}
              value={formulario.anoLancamento}
              onChange={alterarCampo}
            />
            <div className="invalid-feedback">{errosCampos.anoLancamento}</div>
          </div>

          <div className="col-12">
            <label htmlFor="descricao" className="form-label">
              Descrição <span className="text-muted">(opcional)</span>
            </label>
            <textarea
              id="descricao"
              name="descricao"
              rows={3}
              className={classeCampo('descricao')}
              value={formulario.descricao}
              onChange={alterarCampo}
            />
            <div className="invalid-feedback">{errosCampos.descricao}</div>
          </div>

          <div className="col-12 col-sm-6 col-md-3">
            <label htmlFor="preco" className="form-label">
              Preço (R$)
            </label>
            <input
              id="preco"
              name="preco"
              type="number"
              step="0.01"
              min="0"
              className={classeCampo('preco')}
              value={formulario.preco}
              onChange={alterarCampo}
            />
            <div className="invalid-feedback">{errosCampos.preco}</div>
          </div>

          <div className="col-12 col-sm-6 col-md-3">
            <label htmlFor="estoque" className="form-label">
              Estoque
            </label>
            <input
              id="estoque"
              name="estoque"
              type="number"
              min="0"
              className={classeCampo('estoque')}
              value={formulario.estoque}
              onChange={alterarCampo}
            />
            <div className="invalid-feedback">{errosCampos.estoque}</div>
          </div>

          <div className="col-12 col-sm-6 col-md-3">
            <label htmlFor="idPlataforma" className="form-label">
              Plataforma
            </label>
            <select
              id="idPlataforma"
              name="idPlataforma"
              className={classeCampo('idPlataforma', 'form-select')}
              value={formulario.idPlataforma}
              onChange={alterarCampo}
            >
              <option value="">Selecione...</option>
              {plataformas.map((plataforma) => (
                <option key={plataforma.id} value={plataforma.id}>
                  {plataforma.nome}
                </option>
              ))}
            </select>
            <div className="invalid-feedback">{errosCampos.idPlataforma}</div>
          </div>

          <div className="col-12 col-sm-6 col-md-3">
            <label htmlFor="idCondicaoJogo" className="form-label">
              Condição
            </label>
            <select
              id="idCondicaoJogo"
              name="idCondicaoJogo"
              className={classeCampo('idCondicaoJogo', 'form-select')}
              value={formulario.idCondicaoJogo}
              onChange={alterarCampo}
            >
              <option value="">Selecione...</option>
              {condicoes.map((condicao) => (
                <option key={condicao.id} value={condicao.id}>
                  {condicao.descricao}
                </option>
              ))}
            </select>
            <div className="invalid-feedback">{errosCampos.idCondicaoJogo}</div>
          </div>

          <fieldset className="col-12">
            <legend className="form-label fs-6">Gêneros</legend>
            <div className={`d-flex flex-wrap gap-3${erroGeneros ? ' is-invalid' : ''}`}>
              {generos.map((genero) => (
                <div className="form-check" key={genero.id}>
                  <input
                    id={`genero-${genero.id}`}
                    type="checkbox"
                    className={`form-check-input${erroGeneros ? ' is-invalid' : ''}`}
                    checked={formulario.idsGeneros.includes(genero.id)}
                    onChange={() => alternarGenero(genero.id)}
                  />
                  <label htmlFor={`genero-${genero.id}`} className="form-check-label">
                    {genero.nome}
                  </label>
                </div>
              ))}
            </div>
            <div className="invalid-feedback">{erroGeneros}</div>
          </fieldset>

          {/* Só na edição: desmarcar desativa; marcar reativa um jogo inativo. */}
          {ehEdicao && (
            <div className="col-12">
              <div className="form-check form-switch">
                <input
                  id="ativo"
                  name="ativo"
                  type="checkbox"
                  role="switch"
                  className="form-check-input"
                  checked={formulario.ativo}
                  onChange={alterarCampo}
                />
                <label htmlFor="ativo" className="form-check-label">
                  Ativo (aparece no catálogo)
                </label>
              </div>
            </div>
          )}

          <div className="col-12 d-flex gap-2">
            <button type="submit" className="btn btn-primary" disabled={enviando}>
              {enviando ? 'Salvando...' : 'Salvar'}
            </button>
            <Link to="/admin/jogos" className="btn btn-outline-secondary">
              Cancelar
            </Link>
          </div>
        </div>
      </form>
    </>
  )
}

export default FormularioJogo
