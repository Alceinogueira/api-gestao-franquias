-- Script de criacao do banco de dados (SQLite) - Sistema de Gestao de Franquias
-- Observacao: a API cria o banco automaticamente na primeira execucao (EnsureCreated) e
-- ja insere dados de exemplo. Este script serve como documentacao do modelo relacional
-- e pode ser usado para criar o banco manualmente, se preferir.

PRAGMA foreign_keys = ON;

CREATE TABLE Franqueadoras (
    Id                       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    RazaoSocial              TEXT    NOT NULL,
    NomeFantasia             TEXT    NULL,
    Cnpj                     TEXT    NOT NULL,
    Email                    TEXT    NULL,
    Telefone                 TEXT    NULL,
    PercentualRoyaltyPadrao  decimal(5,2) NOT NULL,
    DataCadastro             TEXT    NOT NULL
);

CREATE TABLE Unidades (
    Id                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Nome               TEXT    NOT NULL,
    Cnpj               TEXT    NOT NULL,
    Cidade             TEXT    NULL,
    Estado             TEXT    NULL,
    Endereco           TEXT    NULL,
    Telefone           TEXT    NULL,
    Email              TEXT    NULL,
    DataInicio         TEXT    NOT NULL,
    Ativa              INTEGER NOT NULL,
    PercentualRoyalty  decimal(5,2) NOT NULL,
    FranqueadoraId     INTEGER NOT NULL,
    CONSTRAINT FK_Unidades_Franqueadoras FOREIGN KEY (FranqueadoraId) REFERENCES Franqueadoras (Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Unidades_Cnpj ON Unidades (Cnpj);

CREATE TABLE Responsaveis (
    Id                   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Nome                 TEXT    NOT NULL,
    Cpf                  TEXT    NULL,
    Email                TEXT    NULL,
    Telefone             TEXT    NULL,
    Franqueado           INTEGER NOT NULL,
    UnidadeFranqueadaId  INTEGER NOT NULL,
    CONSTRAINT FK_Responsaveis_Unidades FOREIGN KEY (UnidadeFranqueadaId) REFERENCES Unidades (Id) ON DELETE CASCADE
);

CREATE TABLE Usuarios (
    Id                   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Nome                 TEXT    NOT NULL,
    Email                TEXT    NOT NULL,
    SenhaHash            TEXT    NOT NULL,
    Perfil               INTEGER NOT NULL,
    Ativo                INTEGER NOT NULL,
    DataCadastro         TEXT    NOT NULL,
    UnidadeFranqueadaId  INTEGER NULL,
    CONSTRAINT FK_Usuarios_Unidades FOREIGN KEY (UnidadeFranqueadaId) REFERENCES Unidades (Id) ON DELETE RESTRICT
);
CREATE UNIQUE INDEX IX_Usuarios_Email ON Usuarios (Email);

CREATE TABLE Categorias (
    Id     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Nome   TEXT    NOT NULL,
    Ativa  INTEGER NOT NULL
);

CREATE TABLE Fornecedores (
    Id        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Nome      TEXT    NOT NULL,
    Cnpj      TEXT    NOT NULL,
    Email     TEXT    NULL,
    Telefone  TEXT    NULL,
    Ativo     INTEGER NOT NULL
);
CREATE UNIQUE INDEX IX_Fornecedores_Cnpj ON Fornecedores (Cnpj);

CREATE TABLE Produtos (
    Id            INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Nome          TEXT    NOT NULL,
    Descricao     TEXT    NULL,
    PrecoBase     decimal(10,2) NOT NULL,
    EhServico     INTEGER NOT NULL,
    Ativo         INTEGER NOT NULL,
    CategoriaId   INTEGER NOT NULL,
    FornecedorId  INTEGER NULL,
    CONSTRAINT FK_Produtos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Produtos_Fornecedores FOREIGN KEY (FornecedorId) REFERENCES Fornecedores (Id) ON DELETE RESTRICT
);

CREATE TABLE Estoques (
    Id                   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Quantidade           INTEGER NOT NULL,
    QuantidadeMinima     INTEGER NOT NULL,
    UnidadeFranqueadaId  INTEGER NOT NULL,
    ProdutoServicoId     INTEGER NOT NULL,
    CONSTRAINT FK_Estoques_Unidades FOREIGN KEY (UnidadeFranqueadaId) REFERENCES Unidades (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Estoques_Produtos FOREIGN KEY (ProdutoServicoId) REFERENCES Produtos (Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Estoques_Unidade_Produto ON Estoques (UnidadeFranqueadaId, ProdutoServicoId);

CREATE TABLE MovimentacoesEstoque (
    Id          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Tipo        INTEGER NOT NULL,
    Quantidade  INTEGER NOT NULL,
    Observacao  TEXT    NULL,
    Data        TEXT    NOT NULL,
    EstoqueId   INTEGER NOT NULL,
    CONSTRAINT FK_MovimentacoesEstoque_Estoques FOREIGN KEY (EstoqueId) REFERENCES Estoques (Id) ON DELETE CASCADE
);

CREATE TABLE Vendas (
    Id                   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Data                 TEXT    NOT NULL,
    ValorTotal           decimal(10,2) NOT NULL,
    Status               INTEGER NOT NULL,
    UnidadeFranqueadaId  INTEGER NOT NULL,
    UsuarioId            INTEGER NOT NULL,
    CONSTRAINT FK_Vendas_Unidades FOREIGN KEY (UnidadeFranqueadaId) REFERENCES Unidades (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Vendas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios (Id) ON DELETE RESTRICT
);

CREATE TABLE ItensVenda (
    Id                INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Quantidade        INTEGER NOT NULL,
    PrecoUnitario     decimal(10,2) NOT NULL,
    Subtotal          decimal(10,2) NOT NULL,
    VendaId           INTEGER NOT NULL,
    ProdutoServicoId  INTEGER NOT NULL,
    CONSTRAINT FK_ItensVenda_Vendas FOREIGN KEY (VendaId) REFERENCES Vendas (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ItensVenda_Produtos FOREIGN KEY (ProdutoServicoId) REFERENCES Produtos (Id) ON DELETE CASCADE
);

CREATE TABLE Royalties (
    Id                   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Ano                  INTEGER NOT NULL,
    Mes                  INTEGER NOT NULL,
    FaturamentoPeriodo   decimal(12,2) NOT NULL,
    PercentualAplicado   decimal(5,2) NOT NULL,
    ValorDevido          decimal(12,2) NOT NULL,
    StatusPagamento      INTEGER NOT NULL,
    DataGeracao          TEXT    NOT NULL,
    DataPagamento        TEXT    NULL,
    UnidadeFranqueadaId  INTEGER NOT NULL,
    CONSTRAINT FK_Royalties_Unidades FOREIGN KEY (UnidadeFranqueadaId) REFERENCES Unidades (Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Royalties_Unidade_Ano_Mes ON Royalties (UnidadeFranqueadaId, Ano, Mes);

CREATE TABLE Chamados (
    Id                   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Titulo               TEXT    NOT NULL,
    Categoria            TEXT    NULL,
    Descricao            TEXT    NULL,
    Prioridade           INTEGER NOT NULL,
    Status               INTEGER NOT NULL,
    DataAbertura         TEXT    NOT NULL,
    DataEncerramento     TEXT    NULL,
    Resolucao            TEXT    NULL,
    UnidadeFranqueadaId  INTEGER NOT NULL,
    CONSTRAINT FK_Chamados_Unidades FOREIGN KEY (UnidadeFranqueadaId) REFERENCES Unidades (Id) ON DELETE CASCADE
);
