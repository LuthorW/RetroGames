-- =====================================================================
-- RetroGames — Aula 2, Etapa 5: SQL do GeneroRepository e do
-- CondicaoJogoRepository (somente leitura)
-- =====================================================================

USE retrogames;

-- Gêneros, ordenados por nome -> 8 linhas, começando por Ação
SELECT co_seq_genero, no_genero
FROM tb_genero
ORDER BY no_genero;

-- Condições de jogo, pela chave fixa -> 1 Novo, 2 Usado, 3 Recondicionado
SELECT co_condicao_jogo, ds_condicao_jogo
FROM tb_condicao_jogo
ORDER BY co_condicao_jogo;
