import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import Paginacao from '../../components/Paginacao'
import { formatarPreco } from '../../lib/formato'

const TAMANHO_PAGINA = 10

// Administração de jogos (Especificação 8.5). Mostra também os inativos: o
// administrador envia incluirInativos=true (RN04).
function Jogos() {
  const [textoBusca, setTextoBusca] = useState('')
  const [busca, setBusca] = useState('')
  const [pagina, setPagina] = useState(1)
  const [versao, setVersao] = useState(0)

  // O resultado guarda a "chave" da busca que o gerou (mesmo desenho do catálogo).
  const chave = `${busca}#${pagina}#${versao}`
  const [resultado, setResultado] = useState({ chave: null, dados: null, erro: null })
  const carregando = resultado.chave !== chave

  const [erroAcao, setErroAcao] = useState('')
  const [sucesso, setSucesso] = useState('')

  useEffect(() => {
    let cancelado = false

    jogoApi
      .listar({ busca: busca || undefined, pagina, tamanhoPagina: TAMANHO_PAGINA, incluirInativos: true })
      .then((dados) => {
        if (!cancelado) setResultado({ chave, dados, erro: null })
      })
      .catch((falha) => {
        if (!cancelado) setResultado({ chave, dados: null, erro: obterMensagemErro(falha) })
      })

    return () => {
      cancelado = true
    }
  }, [chave, busca, pagina])

  function buscar(evento) {
    evento.preventDefault()
    setBusca(textoBusca.trim())
    setPagina(1)
  }

  async function desativar(jogo) {
    if (!window.confirm(`Desativar o jogo "${jogo.nome}"? Ele deixa de aparecer no catálogo.`)) return

    setErroAcao('')
    setSucesso('')
    try {
      await jogoApi.desativar(jogo.id)
      setSucesso(`Jogo "${jogo.nome}" desativado.`)
      setVersao((valor) => valor + 1)
    } catch (falha) {
      setErroAcao(obterMensagemErro(falha))
    }
  }

  // Durante uma nova busca, a tabela anterior continua na tela.
  const dados = resultado.dados

  return (
    <>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
        <h1 className="h3 mb-0">Jogos</h1>
        <Link to="/admin/jogos/novo" className="btn btn-primary">
          Novo jogo
        </Link>
      </div>

      <form className="row g-2 mb-3" onSubmit={buscar} role="search">
        <div className="col-12 col-md-8 col-lg-6">
          <input
            type="search"
            className="form-control"
            placeholder="Buscar pelo nome"
            aria-label="Buscar pelo nome"
            value={textoBusca}
            onChange={(e) => setTextoBusca(e.target.value)}
          />
        </div>
        <div className="col-auto">
          <button type="submit" className="btn btn-primary">
            Buscar
          </button>
        </div>
      </form>

      {erroAcao && (
        <div className="alert alert-danger" role="alert">
          {erroAcao}
        </div>
      )}
      {sucesso && (
        <div className="alert alert-success" role="status">
          {sucesso}
        </div>
      )}

      {carregando && !dados ? (
        <Carregando texto="Carregando jogos..." />
      ) : resultado.erro ? (
        <MensagemErro mensagem={resultado.erro} aoTentarNovamente={() => setVersao((valor) => valor + 1)} />
      ) : dados.itens.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado.">
          <p className="mb-0">Tente outra busca ou inclua um novo jogo.</p>
        </EstadoVazio>
      ) : (
        <>
          <div className="table-responsive">
            <table className="table table-hover align-middle">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Plataforma</th>
                  <th>Condição</th>
                  <th className="text-end">Preço</th>
                  <th className="text-end">Estoque</th>
                  <th>Situação</th>
                  <th className="text-end">Ações</th>
                </tr>
              </thead>
              <tbody>
                {dados.itens.map((jogo) => (
                  <tr key={jogo.id} className={jogo.ativo ? undefined : 'text-muted'}>
                    <td>{jogo.nome}</td>
                    <td>{jogo.nomePlataforma}</td>
                    <td>{jogo.descricaoCondicaoJogo}</td>
                    <td className="text-end">{formatarPreco(jogo.preco)}</td>
                    <td className="text-end">{jogo.estoque}</td>
                    <td>
                      {jogo.ativo ? (
                        <span className="badge text-bg-success">Ativo</span>
                      ) : (
                        <span className="badge text-bg-secondary">Inativo</span>
                      )}
                    </td>
                    <td className="text-end text-nowrap">
                      <Link to={`/admin/jogos/${jogo.id}/editar`} className="btn btn-sm btn-outline-primary me-2">
                        Editar
                      </Link>
                      {jogo.ativo && (
                        <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => desativar(jogo)}>
                          Desativar
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <Paginacao pagina={dados.pagina} totalPaginas={dados.totalPaginas} aoMudar={setPagina} />
        </>
      )}
    </>
  )
}

export default Jogos
