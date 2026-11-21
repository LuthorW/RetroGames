import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Porta do front-end (Especificação 1.3). strictPort: se a 5173 estiver
    // ocupada, o Vite para com erro em vez de escolher outra porta sozinho.
    port: 5173,
    strictPort: true,
    // Proxy: tudo que começa com /api é repassado para a API na porta 5080.
    // Para o navegador, front-end e API têm a mesma origem: sem CORS, e o
    // cookie de sessão SameSite=Strict funciona.
    proxy: {
      '/api': 'http://localhost:5080',
    },
  },
})
