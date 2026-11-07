-- =====================================================================
-- RetroGames — Aula 2, Etapa 4: SQL do JogoRepository
-- Rode um bloco por vez (cursor no comando + Ctrl+Enter).
--
-- No C#, LIMIT e OFFSET recebem parâmetros (@limite, @deslocamento).
-- No Workbench, o MySQL não aceita variável @ no LIMIT de um SELECT
-- comum, por isso aqui eles aparecem como números.
-- OFFSET = (pagina - 1) * tamanhoPagina.
-- =====================================================================

USE retrogames;

-- ---------------------------------------------------------------------
-- Listar — o filtro é sempre o mesmo na contagem e na página.
-- Cada filtro só vale quando informado (NULL = sem filtro).
-- ---------------------------------------------------------------------

-- Caso 1: sem filtro, só ativos, página 1 de 5
-- Esperado: total 22; página 1 = Alex Kidd in Miracle World, Chrono Trigger,
-- Contra, Crash Bandicoot, Final Fantasy VII
SET @busca = NULL; SET @idPlataforma = NULL; SET @incluirInativos = FALSE;

SELECT COUNT(*) AS total
FROM tb_jogo
WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
  AND (@incluirInativos = TRUE OR st_ativo = TRUE);

SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
       st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
FROM tb_jogo
WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
  AND (@incluirInativos = TRUE OR st_ativo = TRUE)
ORDER BY no_jogo
LIMIT 5 OFFSET 0;

-- Caso 2: mesma listagem, SEGUNDA página (OFFSET 5)
-- Esperado: GoldenEye 007, Gran Turismo, International Superstar Soccer Deluxe,
-- Mega Man 2, Phantasy Star IV
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
  AND (@incluirInativos = TRUE OR st_ativo = TRUE)
ORDER BY no_jogo
LIMIT 5 OFFSET 5;

-- Caso 3: filtro por parte do nome ('Mario') -> total 4
SET @busca = 'Mario'; SET @idPlataforma = NULL; SET @incluirInativos = FALSE;
SELECT COUNT(*) AS total
FROM tb_jogo
WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
  AND (@incluirInativos = TRUE OR st_ativo = TRUE);

-- Caso 4: filtro por plataforma (7 = Atari 2600)
--   sem inativos -> total 1 (Pitfall!); com inativos -> total 2 (+ River Raid)
SET @busca = NULL; SET @idPlataforma = 7; SET @incluirInativos = FALSE;
SELECT COUNT(*) AS total
FROM tb_jogo
WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
  AND (@incluirInativos = TRUE OR st_ativo = TRUE);

SET @incluirInativos = TRUE;
SELECT COUNT(*) AS total
FROM tb_jogo
WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
  AND (@incluirInativos = TRUE OR st_ativo = TRUE);

-- Caso 5: filtro por lista de ids (é assim que o filtro por gênero chega:
-- o Service descobre os ids dos jogos do gênero e manda a lista).
-- Ids dos RPGs: 7, 13, 19 -> total 3
SET @busca = NULL; SET @idPlataforma = NULL; SET @incluirInativos = FALSE;
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
  AND (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND co_seq_jogo IN (7, 13, 19)
ORDER BY no_jogo
LIMIT 5 OFFSET 0;

-- ---------------------------------------------------------------------
-- Buscar por id (não filtra st_ativo: quem decide é o Service, RN04)
-- Esperado com 23: River Raid. Com 999: nenhuma linha.
-- ---------------------------------------------------------------------
SET @id = 23;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
       st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
FROM tb_jogo
WHERE co_seq_jogo = @id;

-- ---------------------------------------------------------------------
-- Inserir (dt_cadastro gerado pelo banco em UTC)
-- ---------------------------------------------------------------------
SET @nome = 'Jogo Teste'; SET @descricao = NULL; SET @anoLancamento = 1990;
SET @preco = 99.90; SET @estoque = 1; SET @ativo = TRUE;
SET @idPlataforma = 1; SET @idCondicaoJogo = 2;
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (@nome, @descricao, @anoLancamento, @preco, @estoque, @ativo,
     UTC_TIMESTAMP(), @idPlataforma, @idCondicaoJogo);
SET @id = LAST_INSERT_ID();
SELECT @id AS id_gerado;

-- ---------------------------------------------------------------------
-- Alterar (não mexe em dt_cadastro)
-- ---------------------------------------------------------------------
SET @nome = 'Jogo Teste Alterado'; SET @descricao = 'Descrição de teste';
SET @preco = 109.90; SET @estoque = 3;
UPDATE tb_jogo
SET no_jogo = @nome,
    ds_jogo = @descricao,
    nu_ano_lancamento = @anoLancamento,
    vl_preco = @preco,
    qt_estoque = @estoque,
    st_ativo = @ativo,
    co_plataforma = @idPlataforma,
    co_condicao_jogo = @idCondicaoJogo
WHERE co_seq_jogo = @id;

-- ---------------------------------------------------------------------
-- Desativar (exclusão lógica, RN03: jogo nunca é apagado)
-- ---------------------------------------------------------------------
UPDATE tb_jogo
SET st_ativo = FALSE
WHERE co_seq_jogo = @id;

SELECT co_seq_jogo, no_jogo, ds_jogo, vl_preco, qt_estoque, st_ativo
FROM tb_jogo
WHERE co_seq_jogo = @id;

-- ---------------------------------------------------------------------
-- LIMPEZA do teste (só aqui no Workbench; o Repository não tem DELETE de jogo)
-- ---------------------------------------------------------------------
DELETE FROM tb_jogo WHERE co_seq_jogo = @id;
SELECT COUNT(*) AS qt_jogos FROM tb_jogo;  -- deve voltar 23
