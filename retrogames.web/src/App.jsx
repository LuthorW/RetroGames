import { Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import Login from './pages/Login/Login'
import RotaProtegida from './components/RotaProtegida'
import Jogos from './pages/Administracao/Jogos'
import FormularioJogo from './pages/Administracao/FormularioJogo'
import Plataforma from './pages/Auxiliares/Plataforma'

// Rotas da aplicação (Especificação 8.1 e 8.5), agrupadas em públicas e
// restritas ao Administrador. Todas ficam dentro do Layout:
// a página da rota aparece no lugar do <Outlet />.
function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Públicas */}
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />

        {/* Restritas ao Administrador */}
        <Route path="admin" element={<RotaProtegida somenteAdministrador />}>
          <Route path="jogos" element={<Jogos />} />
          <Route path="jogos/novo" element={<FormularioJogo />} />
          <Route path="jogos/:id/editar" element={<FormularioJogo />} />
          <Route path="plataformas" element={<Plataforma />} />
        </Route>

        {/* Qualquer outro endereço */}
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
