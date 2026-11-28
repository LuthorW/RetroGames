import { useEffect, useState } from 'react'

import { plataformaApi } from '../../api/servicos'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'

const FORMULARIO_VAZIO = { nome: '', fabricante: '', anoLancamento: '' }

// Manutenção de plataformas (Especificação 8.5): lista, inclusão, alteração e
// exclusão. A tela não repete as regras: mostra o que a API respondeu.
function Plataforma() {
  // Lista: mesmo desenho do catálogo, com a "chave" da busca que gerou o resultado.
  // Mudar a versão recarrega a lista (depois de gravar ou em "Tentar novamente").
  const [versao, setVersao] = useState(0)
  const [resultado, setResultado] = useState({ chave: null, plataformas: [], erro: null })
  const primeiraCarga = resultado.chave === null

  // Formulário: sem plataforma em edição, inclui; com, altera.
  const [formulario, setFormulario] = useState(FORMULARIO_VAZIO)
  const [emEdicao, setEmEdicao] = useState(null)
  const [errosCampos, setErrosCampos] = useState({})
  const [erroGravacao, setErroGravacao] = useState('')
  const [sucesso, setSucesso] = useState('')
  const [enviando, setEnviando] = useState(false)

  useEffect(() => {
    let cancelado = false

    plataformaApi
      .listar()
      .then((plataformas) => {
        if (!cancelado) setResultado({ chave: versao, plataformas, erro: null })
      })
      .catch((falha) => {
        if (!cancelado) setResultado({ chave: versao, plataformas: [], erro: obterMensagemErro(falha) })
      })

    return () => {
      cancelado = true
    }
  }, [versao])

  const recarregar = () => setVersao((valor) => valor + 1)

  function alterarCampo(evento) {
    const { name, value } = evento.target
    setFormulario((atual) => ({ ...atual, [name]: value }))
  }

  function limparMensagens() {
    setErrosCampos({})
    setErroGravacao('')
    setSucesso('')
  }

  function iniciarEdicao(plataforma) {
    limparMensagens()
    setEmEdicao(plataforma)
    setFormulario({
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento),
    })
  }

  function cancelarEdicao() {
    limparMensagens()
    setEmEdicao(null)
    setFormulario(FORMULARIO_VAZIO)
  }

  async function salvar(evento) {
    evento.preventDefault()
    limparMensagens()
    setEnviando(true)

    // Os campos devolvem texto: o ano vai como número.
    const dados = {
      nome: formulario.nome.trim(),
      fabricante: formulario.fabricante.trim(),
      anoLancamento: Number(formulario.anoLancamento),
    }

    try {
      if (emEdicao) {
        await plataformaApi.alterar(emEdicao.id, dados)
        setSucesso(`Plataforma "${dados.nome}" alterada.`)
      } else {
        await plataformaApi.inserir(dados)
        setSucesso(`Plataforma "${dados.nome}" incluída.`)
      }
      setEmEdicao(null)
      setFormulario(FORMULARIO_VAZIO)
      recarregar()
    } catch (falha) {
      // 400 (código 3): erros por campo. Demais (ex.: 409, código 101): mensagem.
      const porCampo = obterErrosDosCampos(falha)
      if (Object.keys(porCampo).length > 0) {
        setErrosCampos(porCampo)
      } else {
        setErroGravacao(obterMensagemErro(falha))
      }
    } finally {
      setEnviando(false)
    }
  }

  async function excluir(plataforma) {
    if (!window.confirm(`Excluir a plataforma "${plataforma.nome}"?`)) return

    limparMensagens()
    try {
      await plataformaApi.excluir(plataforma.id)
      if (emEdicao?.id === plataforma.id) cancelarEdicao()
      setSucesso(`Plataforma "${plataforma.nome}" excluída.`)
      recarregar()
    } catch (falha) {
      // Ex.: plataforma com jogos (409, código 102). Ela continua na lista.
      setErroGravacao(obterMensagemErro(falha))
    }
  }

  // Classe do Bootstrap que pinta o campo de vermelho quando há erro nele.
  const classeCampo = (campo) => `form-control${errosCampos[campo] ? ' is-invalid' : ''}`

  return (
    <>
      <h1 className="h3 mb-4">Plataformas</h1>

      <div className="row g-4">
        {/* Formulário */}
        <div className="col-12 col-lg-4">
          <div className="card shadow-sm">
            <div className="card-body">
              <h2 className="h5 mb-3">{emEdicao ? 'Alterar plataforma' : 'Nova plataforma'}</h2>

              {erroGravacao && (
                <div className="alert alert-danger" role="alert">
                  {erroGravacao}
                </div>
              )}
              {sucesso && (
                <div className="alert alert-success" role="status">
                  {sucesso}
                </div>
              )}

              <form onSubmit={salvar} noValidate>
                <div className="mb-3">
                  <label htmlFor="nome" className="form-label">
                    Nome
                  </label>
                  <input id="nome" name="nome" className={classeCampo('nome')} value={formulario.nome} onChange={alterarCampo} />
                  <div className="invalid-feedback">{errosCampos.nome}</div>
                </div>

                <div className="mb-3">
                  <label htmlFor="fabricante" className="form-label">
                    Fabricante
                  </label>
                  <input
                    id="fabricante"
                    name="fabricante"
                    className={classeCampo('fabricante')}
                    value={formulario.fabricante}
                    onChange={alterarCampo}
                  />
                  <div className="invalid-feedback">{errosCampos.fabricante}</div>
                </div>

                <div className="mb-3">
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

                <div className="d-flex gap-2">
                  <button type="submit" className="btn btn-primary" disabled={enviando}>
                    {enviando ? 'Salvando...' : emEdicao ? 'Salvar alteração' : 'Incluir'}
                  </button>
                  {emEdicao && (
                    <button type="button" className="btn btn-outline-secondary" onClick={cancelarEdicao}>
                      Cancelar
                    </button>
                  )}
                </div>
              </form>
            </div>
          </div>
        </div>

        {/* Lista */}
        <div className="col-12 col-lg-8">
          {primeiraCarga ? (
            <Carregando texto="Carregando plataformas..." />
          ) : resultado.erro ? (
            <MensagemErro mensagem={resultado.erro} aoTentarNovamente={recarregar} />
          ) : resultado.plataformas.length === 0 ? (
            <EstadoVazio titulo="Nenhuma plataforma cadastrada.">
              <p className="mb-0">Use o formulário para incluir a primeira.</p>
            </EstadoVazio>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Fabricante</th>
                    <th>Ano</th>
                    <th className="text-end">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {resultado.plataformas.map((plataforma) => (
                    <tr key={plataforma.id} className={emEdicao?.id === plataforma.id ? 'table-active' : undefined}>
                      <td>{plataforma.nome}</td>
                      <td>{plataforma.fabricante}</td>
                      <td>{plataforma.anoLancamento}</td>
                      <td className="text-end text-nowrap">
                        <button
                          type="button"
                          className="btn btn-sm btn-outline-primary me-2"
                          onClick={() => iniciarEdicao(plataforma)}
                        >
                          Editar
                        </button>
                        <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => excluir(plataforma)}>
                          Excluir
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </>
  )
}

export default Plataforma
