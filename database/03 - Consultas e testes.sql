-- =====================================================================
-- RetroGames — Etapa 5: consultas e testes de integridade
-- =====================================================================

USE retrogames;

-- =====================================================================
-- PARTE 1 — CONSULTAS
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. Catálogo: jogos ativos com o nome da plataforma e a descrição
--    da condição, ordenados por nome.
--    Esperado: 22 jogos (River Raid é inativo e não aparece).
-- ---------------------------------------------------------------------
SELECT
    j.co_seq_jogo,
    j.no_jogo,
    p.no_plataforma,
    c.ds_condicao_jogo,
    j.vl_preco,
    j.qt_estoque
FROM tb_jogo j
INNER JOIN tb_plataforma    p ON p.co_seq_plataforma = j.co_plataforma
INNER JOIN tb_condicao_jogo c ON c.co_condicao_jogo  = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- ---------------------------------------------------------------------
-- 2. Gêneros de um jogo informado.
--    Troque o valor de @co_jogo para consultar outro jogo.
--    Esperado com 15 (Wonder Boy III): Ação, Aventura, Plataforma.
-- ---------------------------------------------------------------------
SET @co_jogo = 15;

SELECT
    g.co_seq_genero,
    g.no_genero
FROM tb_jogo_genero jg
INNER JOIN tb_genero g ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = @co_jogo
ORDER BY g.no_genero;

-- ---------------------------------------------------------------------
-- 3. Jogos de um gênero informado.
--    Troque o valor de @co_genero para consultar outro gênero.
--    Esperado com 4 (RPG): Chrono Trigger, Final Fantasy VII,
--    Phantasy Star IV.
-- ---------------------------------------------------------------------
SET @co_genero = 4;

SELECT
    j.co_seq_jogo,
    j.no_jogo
FROM tb_jogo_genero jg
INNER JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo
WHERE jg.co_genero = @co_genero
ORDER BY j.no_jogo;

-- ---------------------------------------------------------------------
-- 4. Quantidade de jogos por plataforma, incluindo as plataformas
--    sem jogos.
--    LEFT JOIN mantém a plataforma mesmo sem jogo; COUNT(coluna do jogo)
--    conta só as linhas com jogo, então o Game Boy aparece com 0.
-- ---------------------------------------------------------------------
SELECT
    p.co_seq_plataforma,
    p.no_plataforma,
    COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma p
LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- ---------------------------------------------------------------------
-- 5. Busca por parte do nome do jogo, com 5 itens por página.
--    OFFSET = (página - 1) * 5.
--    Com o termo 'Super' há 6 resultados: 5 na página 1 e 1 na página 2
--    (Superstar Soccer também contém 'Super').
-- ---------------------------------------------------------------------
SET @termo = 'Super';

-- Página 1
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', @termo, '%')
ORDER BY no_jogo
LIMIT 5 OFFSET 0;

-- Página 2
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', @termo, '%')
ORDER BY no_jogo
LIMIT 5 OFFSET 5;


-- =====================================================================
-- PARTE 2 — TESTES DE INTEGRIDADE
-- Execute UM POR VEZ (cursor na linha + Ctrl+Enter) e anote, no
-- comentário "Resultado obtido", o código e a mensagem que apareceram
-- na aba Output do Workbench.
-- =====================================================================

-- ---------------------------------------------------------------------
-- Teste 1: inserir jogo com co_plataforma inexistente (999)
-- Resultado esperado: Erro 1452 (chave estrangeira)
-- Resultado obtido:
-- ---------------------------------------------------------------------
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('Teste plataforma inexistente', NULL, 1990, 50.00, 1, TRUE, UTC_TIMESTAMP(), 999, 1);

-- ---------------------------------------------------------------------
-- Teste 2: inserir plataforma com nome já cadastrado
-- Resultado esperado: Erro 1062 (UNIQUE)
-- Resultado obtido:
-- ---------------------------------------------------------------------
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Mega Drive', 'Sega', 1988);

-- ---------------------------------------------------------------------
-- Teste 3: excluir plataforma que possui jogos (1 = NES)
-- Resultado esperado: Erro 1451 (chave estrangeira)
-- Resultado obtido:
-- ---------------------------------------------------------------------
DELETE FROM tb_plataforma WHERE co_seq_plataforma = 1;

-- ---------------------------------------------------------------------
-- Teste 4: inserir jogo com no_jogo nulo
-- Resultado esperado: Erro 1048 (NOT NULL)
-- Resultado obtido:
-- ---------------------------------------------------------------------
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (NULL, NULL, 1990, 50.00, 1, TRUE, UTC_TIMESTAMP(), 1, 1);

-- ---------------------------------------------------------------------
-- Teste 5: inserir jogo com vl_preco igual a 0
-- Resultado esperado: Aceito — a regra fica no Service (RN06)
-- Resultado obtido:
-- ---------------------------------------------------------------------
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('Teste preço zero', NULL, 1990, 0.00, 1, TRUE, UTC_TIMESTAMP(), 1, 1);

-- PARTE 3 — LIMPEZA
-- Só o teste 5 grava dado; os outros quatro são recusados pelo banco.
-- O DELETE usa a chave primária por causa do "safe update mode" do
-- Workbench, que recusa DELETE sem chave no WHERE.

SET @co_jogo_teste = (SELECT co_seq_jogo FROM tb_jogo WHERE no_jogo = 'Teste preço zero');
DELETE FROM tb_jogo WHERE co_seq_jogo = @co_jogo_teste;

-- Conferência: deve voltar 23 jogos, 8 plataformas e nenhum "Teste"
SELECT
    (SELECT COUNT(*) FROM tb_jogo)                            AS qt_jogos,
    (SELECT COUNT(*) FROM tb_plataforma)                      AS qt_plataformas,
    (SELECT COUNT(*) FROM tb_jogo WHERE no_jogo LIKE 'Teste%') AS qt_testes;
