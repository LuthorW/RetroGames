-- =====================================================================
-- RetroGames — Aula 2, Etapa 3: SQL do PlataformaRepository
-- =====================================================================

USE retrogames;

-- ---------------------------------------------------------------------
-- Listar: todas as plataformas, ordenadas por nome
-- Esperado: 8 linhas, começando por Atari 2600
-- ---------------------------------------------------------------------
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
ORDER BY no_plataforma;

-- ---------------------------------------------------------------------
-- Buscar por id
-- Esperado com 3: Mega Drive. Com 999: nenhuma linha (vira null no C#)
-- ---------------------------------------------------------------------
SET @id = 3;
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- ---------------------------------------------------------------------
-- Existe com nome (com id opcional a ignorar)
-- Inclusão: @idIgnorar = NULL  -> 'Mega Drive' dá 1 (existe)
-- Alteração da própria Mega Drive: @idIgnorar = 3 -> dá 0 (não acusa ela mesma)
-- ---------------------------------------------------------------------
SET @nome = 'Mega Drive';
SET @idIgnorar = NULL;
SELECT COUNT(*) AS qt
FROM tb_plataforma
WHERE no_plataforma = @nome
  AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar);

SET @idIgnorar = 3;
SELECT COUNT(*) AS qt
FROM tb_plataforma
WHERE no_plataforma = @nome
  AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar);

-- ---------------------------------------------------------------------
-- Possui jogos: consulta tb_jogo
-- Esperado com 1 (NES): maior que 0. Com 8 (Game Boy): 0
-- ---------------------------------------------------------------------
SET @id = 8;
SELECT COUNT(*) AS qt
FROM tb_jogo
WHERE co_plataforma = @id;

-- ---------------------------------------------------------------------
-- Inserir (o C# lê o id gerado em LastInsertedId; aqui, LAST_INSERT_ID())
-- ---------------------------------------------------------------------
SET @nome = 'Plataforma Teste';
SET @fabricante = 'Fabricante Teste';
SET @anoLancamento = 2000;
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES (@nome, @fabricante, @anoLancamento);
SET @id = LAST_INSERT_ID();
SELECT @id AS id_gerado;

-- ---------------------------------------------------------------------
-- Alterar
-- ---------------------------------------------------------------------
SET @nome = 'Plataforma Teste Alterada';
SET @fabricante = 'Outro Fabricante';
SET @anoLancamento = 2001;
UPDATE tb_plataforma
SET no_plataforma = @nome,
    no_fabricante = @fabricante,
    nu_ano_lancamento = @anoLancamento
WHERE co_seq_plataforma = @id;

SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- ---------------------------------------------------------------------
-- Excluir (também serve de limpeza do teste)
-- ---------------------------------------------------------------------
DELETE FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- Conferência: deve voltar 8
SELECT COUNT(*) AS qt_plataformas FROM tb_plataforma;
