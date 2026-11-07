-- =====================================================================
-- RetroGames — Etapa 4: dados de exemplo
-- Referência: Especificação "RetroGames — Arquitetura e Stack", 4.2
--
-- Executar LOGO DEPOIS do 01_criar_tabelas.sql, num banco recém-criado.
-- As chaves co_seq_ são geradas em ordem (1, 2, 3...) a partir do banco
-- vazio; por isso tb_jogo e tb_jogo_genero usam esses números.
--
-- Não há INSERT em tb_usuario: o administrador será criado pela API,
-- na aula de autenticação.
-- =====================================================================

USE retrogames;

-- ---------------------------------------------------------------------
-- Domínio: chaves fixas (iguais às constantes da etapa 6)
-- ---------------------------------------------------------------------

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

-- ---------------------------------------------------------------------
-- Plataformas (8). A plataforma 8 (Game Boy) fica SEM jogos, para a
-- contagem por plataforma da etapa 5 mostrar uma linha com 0.
-- ---------------------------------------------------------------------

INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
    ('Nintendo Entertainment System', 'Nintendo', 1983),  -- 1
    ('Super Nintendo',                'Nintendo', 1990),  -- 2
    ('Mega Drive',                    'Sega',     1988),  -- 3
    ('Master System',                 'Sega',     1985),  -- 4
    ('Nintendo 64',                   'Nintendo', 1996),  -- 5
    ('PlayStation',                   'Sony',     1994),  -- 6
    ('Atari 2600',                    'Atari',    1977),  -- 7
    ('Game Boy',                      'Nintendo', 1989);  -- 8 (sem jogos)

-- ---------------------------------------------------------------------
-- Gêneros (8)
-- ---------------------------------------------------------------------

INSERT INTO tb_genero (no_genero) VALUES
    ('Plataforma'),  -- 1
    ('Ação'),        -- 2
    ('Aventura'),    -- 3
    ('RPG'),         -- 4
    ('Corrida'),     -- 5
    ('Luta'),        -- 6
    ('Esporte'),     -- 7
    ('Tiro');        -- 8

-- ---------------------------------------------------------------------
-- Jogos (23)
--   Chrono Trigger (7): estoque zero
--   River Raid (23):    inativo (st_ativo = FALSE)
--   Alguns jogos ficam com ds_jogo NULL (única coluna opcional).
-- Condição: 1 Novo, 2 Usado, 3 Recondicionado
-- ---------------------------------------------------------------------

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('Super Mario Bros.', 'Cartucho original, sem caixa.',                         1985, 149.90,  5, TRUE,  UTC_TIMESTAMP(), 1, 2),  -- 1
    ('The Legend of Zelda', 'Cartucho dourado com manual.',                        1986, 289.90,  2, TRUE,  UTC_TIMESTAMP(), 1, 2),  -- 2
    ('Mega Man 2', NULL,                                                           1988, 219.90,  3, TRUE,  UTC_TIMESTAMP(), 1, 3),  -- 3
    ('Contra', 'Clássico de tiro cooperativo.',                                    1987, 199.90,  4, TRUE,  UTC_TIMESTAMP(), 1, 2),  -- 4
    ('Super Mario World', 'Cartucho com caixa e manual.',                          1990, 239.90,  6, TRUE,  UTC_TIMESTAMP(), 2, 2),  -- 5
    ('Super Metroid', NULL,                                                        1994, 349.90,  2, TRUE,  UTC_TIMESTAMP(), 2, 3),  -- 6
    ('Chrono Trigger', 'RPG clássico da Square.',                                  1995, 599.90,  0, TRUE,  UTC_TIMESTAMP(), 2, 2),  -- 7 estoque zero
    ('Street Fighter II', 'Versão original de Super Nintendo.',                    1992, 179.90,  7, TRUE,  UTC_TIMESTAMP(), 2, 2),  -- 8
    ('Super Mario Kart', NULL,                                                     1992, 259.90,  3, TRUE,  UTC_TIMESTAMP(), 2, 3),  -- 9
    ('International Superstar Soccer Deluxe', 'Futebol clássico dos anos 90.',     1995, 129.90,  8, TRUE,  UTC_TIMESTAMP(), 2, 2),  -- 10
    ('Sonic the Hedgehog', 'Cartucho com caixa original.',                         1991, 119.90, 10, TRUE,  UTC_TIMESTAMP(), 3, 2),  -- 11
    ('Streets of Rage 2', NULL,                                                    1992, 189.90,  4, TRUE,  UTC_TIMESTAMP(), 3, 3),  -- 12
    ('Phantasy Star IV', 'RPG completo com mapa.',                                 1993, 449.90,  1, TRUE,  UTC_TIMESTAMP(), 3, 2),  -- 13
    ('Alex Kidd in Miracle World', 'Lacrado de fábrica.',                          1986, 399.90,  1, TRUE,  UTC_TIMESTAMP(), 4, 1),  -- 14
    ('Wonder Boy III: The Dragon''s Trap', NULL,                                   1989, 229.90,  2, TRUE,  UTC_TIMESTAMP(), 4, 2),  -- 15
    ('Super Mario 64', 'Cartucho com caixa e manual.',                             1996, 279.90,  5, TRUE,  UTC_TIMESTAMP(), 5, 2),  -- 16
    ('The Legend of Zelda: Ocarina of Time', 'Edição dourada.',                    1998, 329.90,  3, TRUE,  UTC_TIMESTAMP(), 5, 3),  -- 17
    ('GoldenEye 007', NULL,                                                        1997, 199.90,  4, TRUE,  UTC_TIMESTAMP(), 5, 2),  -- 18
    ('Final Fantasy VII', 'Edição original com os três discos.',                   1997, 249.90,  3, TRUE,  UTC_TIMESTAMP(), 6, 2),  -- 19
    ('Crash Bandicoot', 'Edição Greatest Hits.',                                   1996,  99.90,  6, TRUE,  UTC_TIMESTAMP(), 6, 2),  -- 20
    ('Gran Turismo', NULL,                                                         1997,  89.90,  5, TRUE,  UTC_TIMESTAMP(), 6, 3),  -- 21
    ('Pitfall!', 'Cartucho original da Activision.',                               1982, 159.90,  2, TRUE,  UTC_TIMESTAMP(), 7, 2),  -- 22
    ('River Raid', 'Fora de catálogo.',                                            1982, 139.90,  1, FALSE, UTC_TIMESTAMP(), 7, 2);  -- 23 inativo

-- ---------------------------------------------------------------------
-- Gêneros de cada jogo (1 a 3 por jogo)
-- Gêneros: 1 Plataforma, 2 Ação, 3 Aventura, 4 RPG,
--          5 Corrida, 6 Luta, 7 Esporte, 8 Tiro
-- ---------------------------------------------------------------------

INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (1, 1),                  -- Super Mario Bros.
    (2, 2), (2, 3),          -- The Legend of Zelda
    (3, 1), (3, 2),          -- Mega Man 2
    (4, 2), (4, 8),          -- Contra
    (5, 1),                  -- Super Mario World
    (6, 2), (6, 3),          -- Super Metroid
    (7, 3), (7, 4),          -- Chrono Trigger
    (8, 6),                  -- Street Fighter II
    (9, 5),                  -- Super Mario Kart
    (10, 7),                 -- International Superstar Soccer Deluxe
    (11, 1),                 -- Sonic the Hedgehog
    (12, 2), (12, 6),        -- Streets of Rage 2
    (13, 4),                 -- Phantasy Star IV
    (14, 1), (14, 3),        -- Alex Kidd in Miracle World
    (15, 1), (15, 2), (15, 3), -- Wonder Boy III
    (16, 1), (16, 3),        -- Super Mario 64
    (17, 2), (17, 3),        -- Ocarina of Time
    (18, 8),                 -- GoldenEye 007
    (19, 4),                 -- Final Fantasy VII
    (20, 1),                 -- Crash Bandicoot
    (21, 5),                 -- Gran Turismo
    (22, 1), (22, 3),        -- Pitfall!
    (23, 8);                 -- River Raid

-- =====================================================================
-- Conferência (itens "Confira" da etapa 4)
-- =====================================================================

-- Quantidades: pelo menos 6 plataformas, 8 gêneros e 20 jogos
SELECT
    (SELECT COUNT(*) FROM tb_plataforma) AS qt_plataformas,
    (SELECT COUNT(*) FROM tb_genero)     AS qt_generos,
    (SELECT COUNT(*) FROM tb_jogo)       AS qt_jogos;

-- Jogos sem gênero: deve voltar VAZIO
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo j
LEFT JOIN tb_jogo_genero jg ON jg.co_jogo = j.co_seq_jogo
WHERE jg.co_jogo IS NULL;

-- Jogo inativo e jogo com estoque zero
SELECT co_seq_jogo, no_jogo, st_ativo, qt_estoque
FROM tb_jogo
WHERE st_ativo = FALSE OR qt_estoque = 0;

-- tb_usuario deve estar vazia (resultado 0)
SELECT COUNT(*) AS qt_usuarios FROM tb_usuario;
