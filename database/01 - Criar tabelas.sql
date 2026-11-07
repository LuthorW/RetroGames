-- =====================================================================
-- RetroGames — Etapa 3: criação do banco e das tabelas
-- =====================================================================

DROP DATABASE IF EXISTS retrogames;

CREATE DATABASE retrogames
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE retrogames;

-- ---------------------------------------------------------------------
-- Tabelas de domínio: chave fixa (co_), sem AUTO_INCREMENT
-- ---------------------------------------------------------------------

CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo INT          NOT NULL,
    ds_condicao_jogo VARCHAR(50)  NOT NULL,
    CONSTRAINT pk_condicao_jogo PRIMARY KEY (co_condicao_jogo),
    CONSTRAINT uk_condicao_jogo_ds UNIQUE (ds_condicao_jogo)
);

CREATE TABLE tb_perfil (
    co_perfil INT         NOT NULL,
    ds_perfil VARCHAR(50) NOT NULL,
    CONSTRAINT pk_perfil PRIMARY KEY (co_perfil),
    CONSTRAINT uk_perfil_ds UNIQUE (ds_perfil)
);

-- ---------------------------------------------------------------------
-- Tabelas de cadastro: chave gerada pelo banco (co_seq_)
-- ---------------------------------------------------------------------

CREATE TABLE tb_plataforma (
    co_seq_plataforma INT          NOT NULL AUTO_INCREMENT,
    no_plataforma     VARCHAR(100) NOT NULL,
    no_fabricante     VARCHAR(100) NOT NULL,
    nu_ano_lancamento INT          NOT NULL,
    CONSTRAINT pk_plataforma PRIMARY KEY (co_seq_plataforma),
    CONSTRAINT uk_plataforma_no UNIQUE (no_plataforma)
);

CREATE TABLE tb_genero (
    co_seq_genero INT         NOT NULL AUTO_INCREMENT,
    no_genero     VARCHAR(50) NOT NULL,
    CONSTRAINT pk_genero PRIMARY KEY (co_seq_genero),
    CONSTRAINT uk_genero_no UNIQUE (no_genero)
);

-- ---------------------------------------------------------------------
-- Tabela principal
-- ds_jogo é a única coluna do modelo que aceita NULL.
-- ---------------------------------------------------------------------

CREATE TABLE tb_jogo (
    co_seq_jogo       INT            NOT NULL AUTO_INCREMENT,
    no_jogo           VARCHAR(150)   NOT NULL,
    ds_jogo           VARCHAR(1000)  NULL,
    nu_ano_lancamento INT            NOT NULL,
    vl_preco          DECIMAL(10,2)  NOT NULL,
    qt_estoque        INT            NOT NULL,
    st_ativo          BOOLEAN        NOT NULL DEFAULT TRUE,
    dt_cadastro       DATETIME       NOT NULL,  -- data e hora em UTC, informada pela aplicação
    co_plataforma     INT            NOT NULL,
    co_condicao_jogo  INT            NOT NULL,
    CONSTRAINT pk_jogo PRIMARY KEY (co_seq_jogo),
    CONSTRAINT fk_jogo_plataforma
        FOREIGN KEY (co_plataforma) REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao_jogo
        FOREIGN KEY (co_condicao_jogo) REFERENCES tb_condicao_jogo (co_condicao_jogo),
    INDEX ix_jogo_no (no_jogo)
);

-- ---------------------------------------------------------------------
-- Tabela associativa (N:N jogo x gênero): chave primária composta
-- ---------------------------------------------------------------------

CREATE TABLE tb_jogo_genero (
    co_jogo   INT NOT NULL,
    co_genero INT NOT NULL,
    CONSTRAINT pk_jogo_genero PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo
        FOREIGN KEY (co_jogo) REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero
        FOREIGN KEY (co_genero) REFERENCES tb_genero (co_seq_genero)
);

-- ---------------------------------------------------------------------
-- Usuários: e-mail único e apenas o hash da senha
-- ---------------------------------------------------------------------

CREATE TABLE tb_usuario (
    co_seq_usuario INT          NOT NULL AUTO_INCREMENT,
    no_usuario     VARCHAR(150) NOT NULL,
    ds_email       VARCHAR(255) NOT NULL,
    ds_senha_hash  VARCHAR(255) NOT NULL,
    st_ativo       BOOLEAN      NOT NULL DEFAULT TRUE,
    dt_cadastro    DATETIME     NOT NULL,  -- data e hora em UTC, informada pela aplicação
    co_perfil      INT          NOT NULL,
    CONSTRAINT pk_usuario PRIMARY KEY (co_seq_usuario),
    CONSTRAINT uk_usuario_email UNIQUE (ds_email),
    CONSTRAINT fk_usuario_perfil
        FOREIGN KEY (co_perfil) REFERENCES tb_perfil (co_perfil)
);

-- Conferência rápida
SHOW TABLES;
