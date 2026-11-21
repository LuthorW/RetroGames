-- =====================================================================
-- RetroGames — Aula 6, Etapa 1: SQL do UsuarioRepository
-- Rode um bloco por vez (cursor no comando + Ctrl+Enter).
-- O usuário de teste é apagado no fim.
-- =====================================================================

USE retrogames;

-- Buscar por e-mail
SET @email = 'teste@retrogames.local';
SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil
FROM tb_usuario
WHERE ds_email = @email;                        -- vazio, por enquanto

-- Existe administrador? (co_perfil 1 = Administrador)
SET @idPerfil = 1;
SELECT COUNT(*) AS qt
FROM tb_usuario
WHERE co_perfil = @idPerfil;                    -- 0, antes de subir a API

-- Inserir (no C#, o hash vem do PasswordHasher; aqui é só um texto de teste)
SET @nome = 'Usuário Teste'; SET @senhaHash = 'HASH-DE-TESTE'; SET @ativo = TRUE;
INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
VALUES (@nome, @email, @senhaHash, @ativo, UTC_TIMESTAMP(), @idPerfil);
SET @id = LAST_INSERT_ID();

-- Buscar por id
SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil
FROM tb_usuario
WHERE co_seq_usuario = @id;                     -- o usuário de teste

-- E-mail repetido: deve dar erro 1062 (UNIQUE em ds_email)
INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
VALUES (@nome, @email, @senhaHash, @ativo, UTC_TIMESTAMP(), @idPerfil);

-- LIMPEZA do teste
DELETE FROM tb_usuario WHERE co_seq_usuario = @id;
SELECT COUNT(*) AS qt_usuarios FROM tb_usuario; -- 0
