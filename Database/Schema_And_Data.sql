/****** Criação do Banco (simplificada) ******/
CREATE DATABASE [Biblioteca]
GO

USE [Biblioteca]
GO

/****** Tabela Usuarios ******/
CREATE TABLE [dbo].[Usuarios](
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Nome] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Telefone] NVARCHAR(20) NOT NULL,
    [Senha] NVARCHAR(255) NOT NULL,
    [TipoPerfil] INT NOT NULL, -- 0=Usuário, 1=Admin
    [TokenRedefinicao] NVARCHAR(255) NULL,
    [TokenExpiracao] DATETIME NULL
);

/****** Tabela Livros ******/
CREATE TABLE [dbo].[Livros](
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Titulo] NVARCHAR(100) NOT NULL,
    [Autor] NVARCHAR(100) NOT NULL,
    [ISBN] NVARCHAR(13) UNIQUE NOT NULL,
    [Disponivel] BIT DEFAULT 1
);

/****** Tabela Locacoes ******/
CREATE TABLE [dbo].[Locacoes](
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [LivroId] INT NOT NULL FOREIGN KEY REFERENCES Livros(Id),
    [UsuarioId] INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    [DataRetirada] DATETIME NOT NULL DEFAULT GETDATE(),
    [DataDevolucaoPrevista] DATETIME NOT NULL,
    [DataDevolucaoReal] DATETIME NULL,
    [Status] INT NOT NULL, -- 0=Pendente, 1=Concluído, 2=Atrasado
    [Multa] DECIMAL(10,2) DEFAULT 0.00
);

/****** Dados de Exemplo ******/
INSERT INTO [Usuarios] VALUES 
('Admin', 'admin@biblioteca.com', '11999999999', '$2a$10$N9qo8uLOickgx2ZMRZoMy.MQDqUh2yQrIVjH8ZgHX3pzX8JQ1FqK6', 1, NULL, NULL),
('João Silva', 'joao@email.com', '11988887777', '$2a$10$VuXZ5v2Yx7UzO7bq9WqB.uQD5n/sJzZsLcRw5U9Jk5tLW0LQ1JQeW', 0, NULL, NULL);

INSERT INTO [Livros] VALUES
('Domain-Driven Design', 'Eric Evans', '9788550800653', 1),
('Clean Code', 'Robert Martin', '9780132350884', 1);

INSERT INTO [Locacoes] VALUES
(1, 2, '2025-08-01', '2025-08-15', NULL, 0, 0.00);

/****** Stored Procedures ******/
CREATE PROCEDURE sp_CalcularMultasAtrasadas
AS
BEGIN
    UPDATE Locacoes
    SET Multa = DATEDIFF(day, DataDevolucaoPrevista, GETDATE()) * 2.50,
        Status = 2 -- Atrasado
    WHERE Status = 0
    AND DataDevolucaoPrevista < GETDATE()
    AND DataDevolucaoReal IS NULL
END
GO