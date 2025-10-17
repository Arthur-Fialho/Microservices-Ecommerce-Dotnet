# Desafio Técnico: Arquitetura de Microserviços para E-commerce

Projeto de um sistema de e-commerce back-end com arquitetura de microserviços, focado em resiliência, escalabilidade e segurança. O sistema simula o gerenciamento de Estoque e Vendas, utilizando comunicação síncrona e assíncrona, um API Gateway para acesso centralizado e autenticação via JWT.

Este projeto foi construído seguindo os princípios de Clean Architecture, SOLID e Domain-Driven Design (DDD).

## Arquitetura da Solução

O sistema é composto por 3 serviços principais que rodam de forma independente:

*   **Microserviço de Estoque:** Gerencia o cadastro, consulta e atualização de produtos.
*   **Microserviço de Vendas:** Gerencia a criação e consulta de pedidos.
*   **API Gateway:** Ponto de entrada único para todos os clientes. Responsável pela autenticação, roteamento e segurança.

### Fluxo de Comunicação:

*   **Cliente → API Gateway:** Todas as requisições externas passam pelo Gateway e devem conter um Token JWT válido.
*   **Gateway → Microserviços:** O Gateway (construído com YARP) roteia as requisições para o serviço interno correto (`/api/produtos` para Estoque, `/api/pedidos` para Vendas).
*   **Vendas → Estoque (Síncrono):** Ao criar um pedido, o serviço de Vendas faz uma chamada HTTP direta ao serviço de Estoque para validar a disponibilidade do produto em tempo real.
*   **Vendas → RabbitMQ → Estoque (Assíncrono):** Após o pedido ser validado e salvo, o serviço de Vendas publica um evento `PedidoCriadoEvent`. O serviço de Estoque consome esta mensagem e debita o estoque do seu banco de dados, garantindo resiliência e desacoplamento.

## Funcionalidades Implementadas

*   \[x] **Microserviço 1 (Gestão de Estoque)**
    *   Cadastro, consulta e atualização de produtos.
*   \[x] **Microserviço 2 (Gestão de Vendas)**
    *   Criação de pedidos com validação síncrona de estoque.
    *   Consulta de pedidos.
*   \[x] **API Gateway (YARP)**
    *   Roteamento de requisições para os serviços corretos.
    *   Ponto de entrada único para o sistema.
*   \[x] **Comunicação Assíncrona (RabbitMQ)**
    *   Publicação de eventos `PedidoCriadoEvent` pelo serviço de Vendas.
    *   Consumo de eventos no serviço de Estoque para debitar o estoque de forma resiliente.
*   \[x] **Autenticação e Autorização (JWT)**
    *   Endpoint de login (`/api/auth/login`) no Gateway para geração de tokens JWT.
    *   Validação de token no Gateway e em cada microserviço.
    *   Endpoints de negócio protegidos com `[Authorize]`.
*   \[x] **Persistência de Dados (Entity Framework)**
    *   Uso do EF Core com SQL Server (rodando em Docker).
    *   Separação de bancos de dados (EstoqueDB e VendasDB).
*   \[x] **Testes Unitários (xUnit e Moq)**
    *   Testes para a lógica de negócio no Domínio (Ex: `Produto.DebitarEstoque`).
    *   Testes para a lógica de orquestração na Aplicação (Ex: `PedidoService`) usando mocks.
*   \[x] **Boas Práticas**
    *   Implementação de Clean Architecture em todos os serviços.
    *   Uso do Repository Pattern para abstração do acesso a dados.
    *   Validações de negócio dentro das entidades do Domínio.

## Stack de Tecnologias

*   **Back-end:** .NET 9 (C#)
*   **Frameworks:** ASP.NET Core, Entity Framework Core
*   **Banco de Dados:** SQL Server (rodando em Docker)
*   **Mensageria:** RabbitMQ (rodando em Docker)
*   **API Gateway:** YARP (Yet Another Reverse Proxy)
*   **Segurança:** JSON Web Tokens (JWT)
*   **Testes:** xUnit, Moq
*   **Containerização:** Docker

## Como Executar o Projeto

Siga os passos abaixo para configurar e executar a aplicação completa na sua máquina.

### 1. Pré-requisitos

*   .NET SDK 9.0 (ou superior)
*   Docker Desktop
*   Uma ferramenta de API (Postman, Insomnia) ou um cliente de banco de dados (DBeaver, Azure Data Studio).

### 2. Clonar o Repositório

```bash
git clone https://github.com/Arthur-Fialho/Microservices-Ecommerce-Dotnet
cd Microservices-Ecommerce-Dotnet
```

### 3. Configurar o Ambiente (Docker)

Os bancos de dados e o message broker rodam em contêineres.

a) Iniciar o SQL Server: (Lembre-se de definir uma senha forte e usá-la nos passos seguintes)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=suaSenhaForte(!)123" \
-p 1433:1433 --name sql-server-dev -d \
mcr.microsoft.com/mssql/server:2022-latest
```

b) Iniciar o RabbitMQ:

```bash
docker run -d --hostname my-rabbitmq-server --name rabbitmq-dev \
-p 5672:5672 -p 15672:15672 \
rabbitmq:3-management
```

Acesse a interface de gerenciamento em http://localhost:15672 (login: `guest` / senha: `guest`).

### 4. Configurar as `appsettings.json`

Você precisa atualizar as connection strings e as configurações de JWT em três locais. Substitua `suaSenhaForte(!)123` pela senha que você definiu para o Docker.

a) `src/Estoque.API/appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=EstoqueDB;User Id=sa;Password=suaSenhaForte(!)123;TrustServerCertificate=True;"
},
"JwtSettings": {
  "Secret": "SuaSenhaForteAqui",
  "Issuer": "MeuEcommerceApi",
  "Audience": "ClientesDoEcommerce"
}
```

b) `src/Vendas.API/appsettings.json` (Verifique se a porta 5001 corresponde à porta HTTP do seu serviço de Estoque)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=VendasDB;User Id=sa;Password=suaSenhaForte(!)123;TrustServerCertificate=True;"
},
"JwtSettings": { ... (igual ao de Estoque) ... },
"ServicesUrl": {
  "EstoqueApi": "http://localhost:5001"
}
```

c) `src/ApiGateway/appsettings.json` (Verifique se as portas 5001 e 5002 correspondem às portas HTTP dos seus serviços de Estoque e Vendas)

```json
"JwtSettings": { ... (igual ao de Estoque) ... },
"ReverseProxy": {
  "Routes": { ... },
  "Clusters": {
    "estoque-cluster": {
      "Destinations": { "destination1": { "Address": "http://localhost:5001" } }
    },
    "vendas-cluster": {
      "Destinations": { "destination1": { "Address": "http://localhost:5002" } }
    }
  }
}
```

### 5. Criar os Bancos de Dados (Migrations)

Com os contêineres rodando e as configurações prontas, execute os comandos a seguir na pasta raiz do projeto (Ecommerce):

```bash
# Criar o banco de dados e tabelas do Estoque
dotnet ef database update --startup-project src/Estoque.API

# Criar o banco de dados e tabelas de Vendas
dotnet ef database update --startup-project src/Vendas.API
```

### 6. Executar a Aplicação

Você precisará de 3 terminais abertos na pasta raiz.

**Terminal 1: Rodar Serviço de Estoque**

```bash
dotnet run --project src/Estoque.API
```

**Terminal 2: Rodar Serviço de Vendas**

```bash
dotnet run --project src/Vendas.API
```

**Terminal 3: Rodar API Gateway**

```bash
dotnet run --project src/ApiGateway
```

A aplicação está no ar! O API Gateway estará rodando (provavelmente em `http://localhost:5003` ou `http://localhost:5004` - verifique o console).

### 7. Executar os Testes Unitários

Para garantir que toda a lógica de negócio está correta, rode os testes:

```bash
dotnet test
```

## Fluxo de Uso da API

Todas as requisições devem ser feitas para o endereço do API Gateway.

### 1. Obter Token de Autenticação

Faça uma requisição `POST` para o endpoint de login com o usuário e senha simulados.

**`POST http://localhost:5003/api/auth/login`** (use a porta do seu Gateway)

**Body (JSON):**

```json
{
  "username": "usuario",
  "password": "senha123"
}
```

**Resposta:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

Copie o token. Você precisará dele para todos os passos seguintes.

### 2. Cadastrar um Produto

**`POST http://localhost:5003/api/produtos`**

**Headers:**

*   `Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**Body (JSON):**

```json
{
  "nome": "Livro Clean Architecture",
  "descricao": "Um livro sobre arquitetura de software",
  "preco": 50,
  "quantidadeEmEstoque": 20
}
```

Copie o `id` do produto retornado na resposta.

### 3. Criar um Pedido

**`POST http://localhost:5003/api/pedidos`**

**Headers:**

*   `Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**Body (JSON):** (Use o `id` do produto criado no passo anterior)

```json
{
  "itens": [
    {
      "produtoId": "a1b2c3d4-…",
      "quantidade": 2
    }
  ]
}
```

### 4. Verificar o Resultado

Se tudo deu certo, você receberá um `200 OK`. Quase que instantaneamente, o serviço de Estoque receberá a mensagem do RabbitMQ e debitará o estoque.

**`GET http://localhost:5003/api/produtos/a1b2c3d4-...`**

**Headers:**

*   `Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

Na resposta, você verá que a `quantidadeEmEstoque` foi atualizada (de 20 para 18).

## 👨‍💻 Autor

*   [Arthur Fialho](https://arthurfialho.com.br)
*   [GitHub](https://github.com/Arthur-Fialho)
*   [LinkedIn](https://www.linkedin.com/in/arthurfialho/)