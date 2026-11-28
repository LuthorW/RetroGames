import { Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import Login from './pages/Login/Login'

// Rotas da aplicação (Especificação 8.1 e 8.5). Todas ficam dentro do Layout:
// a página da rota aparece no lugar do <Outlet />.
function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Públicas */}
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />

        {/* Qualquer outro endereço */}
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
