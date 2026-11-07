-- =====================================================================
-- RetroGames — Aula 2, Etapa 4: SQL do JogoGeneroRepository
-- Rode um bloco por vez (cursor no comando + Ctrl+Enter).
-- =====================================================================

USE retrogames;

-- Listar todos os vínculos -> 35 linhas
SELECT co_jogo, co_genero
FROM tb_jogo_genero;

-- Listar por jogos (uma única consulta para vários jogos)
-- No C#, cada id vira um parâmetro: IN (@id0, @id1, @id2)
-- Jogos 1, 7 e 15 -> 6 linhas
SELECT co_jogo, co_genero
FROM tb_jogo_genero
WHERE co_jogo IN (1, 7, 15);

-- ---------------------------------------------------------------------
-- Inserir e remover: teste com um jogo temporário
-- ---------------------------------------------------------------------
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Jogo Teste Gêneros', NULL, 1990, 10.00, 1, TRUE, UTC_TIMESTAMP(), 1, 1);
SET @idJogo = LAST_INSERT_ID();

-- Inserir os vínculos do jogo (um único INSERT com várias linhas)
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@idJogo, 1),
    (@idJogo, 2);

SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo = @idJogo;  -- 2 linhas

-- Remover por jogo (usado na alteração, antes de regravar os gêneros)
DELETE FROM tb_jogo_genero
WHERE co_jogo = @idJogo;

SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo = @idJogo;  -- vazio

-- LIMPEZA do teste
DELETE FROM tb_jogo WHERE co_seq_jogo = @idJogo;
SELECT COUNT(*) AS qt_jogos FROM tb_jogo;               -- 23
SELECT COUNT(*) AS qt_vinculos FROM tb_jogo_genero;     -- 35
