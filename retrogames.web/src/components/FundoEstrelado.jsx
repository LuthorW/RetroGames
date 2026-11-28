import { useEffect, useRef } from 'react'

// Fundo de estrelas em movimento lento (mesma ambientação do site Luthor
// Corporation). Fica atrás de tudo e não recebe cliques. Quem prefere menos
// movimento (configuração do sistema) não vê a animação.
function FundoEstrelado() {
  const canvasRef = useRef(null)

  useEffect(() => {
    const canvas = canvasRef.current
    if (!canvas || window.matchMedia('(prefers-reduced-motion: reduce)').matches) return

    const contexto = canvas.getContext('2d')
    let largura = 0
    let altura = 0
    let estrelas = []
    let quadro = 0
    let ultimoQuadro = 0

    function criarEstrelas() {
      // Densidade limitada: em telas de alta resolução, desenha menos pixels.
      const densidade = Math.min(window.devicePixelRatio || 1, 1.5)
      largura = window.innerWidth
      altura = window.innerHeight
      canvas.width = largura * densidade
      canvas.height = altura * densidade
      canvas.style.width = `${largura}px`
      canvas.style.height = `${altura}px`
      contexto.setTransform(densidade, 0, 0, densidade, 0, 0)

      const quantidade = Math.min(70, Math.floor((largura * altura) / 20000))
      estrelas = Array.from({ length: quantidade }, () => ({
        x: Math.random() * largura,
        y: Math.random() * altura,
        raio: Math.random() * 1.3 + 0.2,
        velocidade: Math.random() * 0.15 + 0.03,
        fase: Math.random() * Math.PI * 2,
        roxa: Math.random() < 0.25,
      }))
    }

    function desenhar(tempo) {
      quadro = requestAnimationFrame(desenhar)
      // No máximo 30 quadros por segundo: o movimento é lento, não precisa de mais.
      if (tempo - ultimoQuadro < 33) return
      ultimoQuadro = tempo

      contexto.clearRect(0, 0, largura, altura)
      for (const estrela of estrelas) {
        estrela.y -= estrela.velocidade * 2
        if (estrela.y < -2) {
          estrela.y = altura + 2
          estrela.x = Math.random() * largura
        }
        // Brilho que pisca devagar.
        const brilho = 0.35 + 0.35 * Math.sin(tempo / 900 + estrela.fase)
        contexto.beginPath()
        contexto.arc(estrela.x, estrela.y, estrela.raio, 0, Math.PI * 2)
        contexto.fillStyle = estrela.roxa ? `rgba(184, 76, 255, ${brilho + 0.2})` : `rgba(246, 240, 255, ${brilho})`
        contexto.fill()
      }
    }

    // Com a aba escondida, a animação para.
    function aoMudarVisibilidade() {
      cancelAnimationFrame(quadro)
      if (!document.hidden) quadro = requestAnimationFrame(desenhar)
    }

    criarEstrelas()
    quadro = requestAnimationFrame(desenhar)
    window.addEventListener('resize', criarEstrelas)
    document.addEventListener('visibilitychange', aoMudarVisibilidade)

    return () => {
      cancelAnimationFrame(quadro)
      window.removeEventListener('resize', criarEstrelas)
      document.removeEventListener('visibilitychange', aoMudarVisibilidade)
    }
  }, [])

  return <canvas ref={canvasRef} className="fundo-estrelado" aria-hidden="true" />
}

export default FundoEstrelado
