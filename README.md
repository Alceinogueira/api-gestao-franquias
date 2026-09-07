# Sistema de Gestão de Franquias

API REST em C# (.NET 8 / ASP.NET Core Web API) para gerenciar uma rede de franquias:
usuários e perfis, unidades franqueadas, catálogo de produtos e serviços, fornecedores,
estoque por unidade, vendas, royalties, chamados de suporte e relatórios gerenciais.

Trabalho acadêmico da disciplina de Desenvolvimento Web Back-end.

## Tecnologias utilizadas

- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8 (provedor SQLite)
- Autenticação JWT (Bearer)
- BCrypt para hash de senha
- Swagger / OpenAPI (Swashbuckle)

## Requisitos para executar

- SDK do .NET 8 instalado (`dotnet --version` deve mostrar 8.x)
- Nenhum banco externo é necessário: o SQLite é criado em um arquivo local (`franquias.db`)

## Como executar

```bash
cd Franquias.Api
dotnet restore
dotnet run
```

Ao iniciar, a aplicação cria o banco `franquias.db` automaticamente e insere dados de
exemplo (franqueadora, unidades, usuários, produtos, estoque, vendas, chamados e royalties).

A API sobe em `http://localhost:5080`.
A documentação Swagger fica em `http://localhost:5080/swagger`.

Para recriar o banco do zero, basta apagar o arquivo `Franquias.Api/franquias.db` e rodar de novo.

## Usuários de teste (já cadastrados pelo seed)

| Perfil                     | E-mail                         | Senha         |
|----------------------------|--------------------------------|---------------|
| Administrador da franqueadora | admin@franquias.com         | Admin@123     |
| Gestor de unidade (Centro) | gestor.centro@franquias.com    | Gestor@123    |
| Operador (Centro)          | operador.centro@franquias.com  | Operador@123  |
| Gestor de unidade (Zona Sul) | gestor.zonasul@franquias.com | Gestor@123    |

## Autenticação

1. `POST /api/auth/login` com e-mail e senha retorna um token JWT.
2. Nas demais chamadas, envie o header `Authorization: Bearer <token>`.
3. No Swagger, use o botão **Authorize** e informe apenas o token.

Perfis de acesso:

- **AdministradorFranqueadora**: acesso total (cadastros, geração de royalties, usuários).
- **GestorUnidade**: estoque, vendas, chamados e relatórios da própria unidade.
- **Operador**: registro de vendas e consultas.

## Principais endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/auth/login | Autenticação e geração do token |
| POST/GET | /api/auth/usuarios | Cadastro e listagem de usuários (admin) |
| PATCH | /api/auth/usuarios/{id}/situacao | Ativa/inativa usuário |
| GET/POST/PUT | /api/unidades | Unidades franqueadas |
| PATCH | /api/unidades/{id}/situacao | Ativa/inativa unidade |
| POST | /api/unidades/{id}/responsaveis | Adiciona responsável/franqueado |
| GET/POST | /api/categorias | Categorias de produtos |
| GET/POST/PUT | /api/produtos | Produtos e serviços |
| GET/POST/PUT | /api/fornecedores | Fornecedores |
| GET | /api/estoques/unidade/{id} | Estoque de uma unidade |
| GET | /api/estoques/abaixo-do-minimo | Itens abaixo do estoque mínimo |
| POST | /api/estoques | Cria registro de estoque |
| POST | /api/estoques/movimentacoes | Entrada/saída de estoque |
| GET/POST | /api/vendas | Vendas e itens |
| POST | /api/vendas/{id}/confirmar | Confirma a venda e baixa o estoque |
| POST | /api/vendas/{id}/cancelar | Cancela a venda (estorna estoque) |
| POST | /api/royalties/gerar | Gera o royalty do período |
| PATCH | /api/royalties/{id}/pagamento | Registra o pagamento |
| GET/POST/PUT | /api/chamados | Chamados de suporte |
| GET | /api/relatorios/faturamento | Faturamento por unidade e período |
| GET | /api/relatorios/ranking-unidades | Ranking por faturamento |
| GET | /api/relatorios/produtos-mais-vendidos | Produtos/serviços mais vendidos |
| GET | /api/relatorios/royalties | Total de royalties gerados/pagos/pendentes |
| GET | /api/relatorios/estoque-critico | Itens abaixo do mínimo em toda a rede |
| GET | /api/relatorios/chamados-por-status | Quantidade de chamados por status |

## Regras de negócio implementadas

- CNPJ único por unidade e por fornecedor; e-mail único por usuário.
- Unidade inativa não registra novas vendas.
- Venda pertence a uma única unidade e precisa de pelo menos um item.
- Valor total da venda calculado a partir dos itens (quantidade x preço).
- Estoque nunca fica negativo em vendas ou movimentações.
- Royalty calculado pelo percentual da unidade sobre o faturamento confirmado do período.
- Autorização por perfil em todos os endpoints.
- Exclusão lógica (ativo/inativo) em usuários, unidades, categorias, produtos e fornecedores.
- Erros de validação e recursos inexistentes retornam 400 / 404 com mensagem.

## Testes

- `Franquias.Api/Franquias.Api.http`: requisições prontas para o VS Code / Rider.
- `Franquias.Api.postman_collection.json`: coleção do Postman (a request de login já salva o token).
- `script_criacao_banco.sql`: script de criação do banco (documentação do modelo).

## Estrutura do projeto

```
Franquias.Api/
├── Controllers/     endpoints REST
├── Services/        regras de negócio (interface + implementação)
├── Data/            AppDbContext e seed de dados
├── Models/          entidades do domínio
├── DTOs/            objetos de entrada e saída da API
├── Middleware/      tratamento global de erros
├── Common/          exceções e constantes de perfil
└── Program.cs       configuração da aplicação
```

## Repositório

Link do GitHub: _COLOQUE AQUI O LINK DO SEU REPOSITÓRIO_
