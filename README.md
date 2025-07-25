# INDT_Hexagonal

## 1. Visão Geral da Arquitetura
A arquitetura será baseada em microserviços com comunicação explícita, onde cada serviço é construído seguindo o padrão de Arquitetura Hexagonal (Ports & Adapters).

### Componentes Principais:

*   **Cliente (Usuário/Frontend):** Interage com os serviços através de APIs REST.

*   **PropostaService:** Microserviço responsável pelo ciclo de vida da proposta.
    *   **Hexágono (Core):** Contém a lógica de negócio pura (entidades, regras, casos de uso) sem dependências de infraestrutura.
    *   **Adapters de Entrada (Driving):** A API REST que recebe requisições do cliente.
    *   **Adapters de Saída (Driven):** A implementação do repositório que se comunica com o banco de dados.

*   **ContratacaoService:** Microserviço responsável por efetivar a contratação.
    *   **Hexágono (Core):** Lógica de negócio para contratação.
    *   **Adapters de Entrada (Driving):** A API REST que recebe requisições de contratação.
    *   **Adapters de Saída (Driven):**
        *   Implementação do repositório para salvar os dados da contratação.
        *   Um cliente HTTP para se comunicar com o PropostaService.

*   **Banco de Dados:** Cada serviço terá seu próprio banco de dados (ou seu próprio schema) para garantir o baixo acoplamento.

*   **(Bônus) Message Broker:** Para comunicação assíncrona, um broker como RabbitMQ ou Azure Service Bus poderia ser usado. Por exemplo, quando uma proposta é aprovada, PropostaService publica um evento `PropostaAprovadaEvent`, e ContratacaoService reage a esse evento. Para o teste, a comunicação síncrona via REST é suficiente e mais simples.

## 2. Estrutura dos Projetos (.NET)
A organização dos projetos dentro da solution é crucial para refletir a arquitetura. Recomendo a seguinte estrutura para cada microserviço:

```
Seguros.sln
├───src
│   ├───PropostaService
│   │   ├───PropostaService.Api             // Adapter de Entrada (Controllers, Program.cs, DI)
│   │   ├───PropostaService.Application     // Casos de Uso (Hexágono)
│   │   ├───PropostaService.Domain          // Entidades, Enums, Interfaces de Repositório (Ports) (Hexágono)
│   │   └───PropostaService.Infrastructure  // Adapters de Saída (Repositórios, Migrations)
│   │
│   └───ContratacaoService
│       ├───ContratacaoService.Api
│       ├───ContratacaoService.Application
│       ├───ContratacaoService.Domain
│       └───ContratacaoService.Infrastructure // Incluirá o cliente HTTP para o PropostaService
│
├───tests
│   ├───PropostaService.UnitTests           // Testes para Domain e Application
│   └───ContratacaoService.UnitTests        // Testes para Domain e Application
│
└───docker-compose.yml                      // (Bônus) Orquestração dos contêineres
```

### Detalhes dos Projetos:

*   **.Domain:** O coração do serviço. Não tem dependências com outros projetos da solução, apenas com pacotes do .NET. Contém:
    *   Entidades: Proposta (com ID, Cliente, Valor, Status, etc.) e Contratacao.
    *   Regras de Negócio: Métodos dentro das entidades (ex: `proposta.Aprovar()`).
    *   Ports (Interfaces): `IPropostaRepository`, `IContratacaoRepository`.

*   **.Application:** Orquestra o fluxo de dados. Depende do `.Domain`.
    *   Casos de Uso/Serviços de Aplicação: `PropostaAppService`.
    *   DTOs (Data Transfer Objects): `PropostaInputModel`, `PropostaViewModel`.
    *   Recebe as interfaces (ports) por injeção de dependência e as utiliza para interagir com a infraestrutura.

*   **.Infrastructure:** Implementação dos adapters de saída. Depende do `.Application`.
    *   Implementações dos Repositórios: `PropostaRepository` (usando Entity Framework Core).
    *   Contexto do EF Core: `PropostaDbContext`.
    *   Migrations do banco de dados.
    *   Cliente HTTP: Para a comunicação entre serviços.

*   **.Api:** O ponto de entrada. Depende do `.Infrastructure` e `.Application` para configurar a injeção de dependência.
    *   Controllers da API REST.
    *   Configuração de Injeção de Dependência (DI) em `Program.cs`.

## 3. Implementação da Infraestrutura do PropostaService

Nesta etapa, foram implementados os componentes de infraestrutura para o `PropostaService`, seguindo os princípios de DDD e Arquitetura Hexagonal.

*   **`PropostaDbContext.cs`**: Define o contexto do banco de dados usando Entity Framework Core. Mapeia a entidade `Proposta` para a tabela correspondente no banco de dados, configurando suas propriedades e chaves.
*   **`PropostaRepository.cs`**: Implementa a interface `IPropostaRepository` (definida no domínio), fornecendo a lógica de persistência para a entidade `Proposta`. Utiliza o `PropostaDbContext` para interagir com o banco de dados (operações de adicionar, obter, listar e atualizar).
*   **`DataSeeder.cs`**: Uma classe estática responsável por popular o banco de dados com dados iniciais, útil para ambientes de desenvolvimento e testes. Garante que os dados sejam inseridos apenas se a tabela estiver vazia.
*   **`Program.cs` (PropostaService.Api)**: Configurado para:
    *   Registrar o `PropostaDbContext` com o provedor Npgsql para PostgreSQL, utilizando a string de conexão definida nos arquivos de configuração.
    *   Registrar as dependências `IPropostaRepository` e `PropostaAppService` para injeção de dependência.
    *   Aplicar automaticamente as migrações do banco de dados na inicialização da aplicação.
    *   Executar o `DataSeeder` para popular o banco de dados, mas apenas em ambiente de desenvolvimento.
*   **`appsettings.Development.json` e `appsettings.Production.json`**: Arquivos de configuração que definem as strings de conexão para os bancos de dados de desenvolvimento e produção, respectivamente. Isso permite configurar diferentes credenciais e hosts para cada ambiente.
*   **`docker-compose.yml`**: Atualizado para incluir dois serviços de banco de dados PostgreSQL (`proposta-db` e `proposta-db-prod`), permitindo simular ambientes de desenvolvimento e produção com diferentes configurações de acesso.

## 4. Implementação do ContratacaoService

Nesta etapa, o `ContratacaoService` foi inicializado e seus componentes principais foram implementados, seguindo a mesma arquitetura hexagonal e princípios de DDD.

*   **Estrutura de Projetos**: Foram criados os projetos `ContratacaoService.Domain`, `ContratacaoService.Application`, `ContratacaoService.Infrastructure`, `ContratacaoService.Api` e `ContratacaoService.Tests` dentro da solução `Seguros/src/ContratacaoService`, e as referências entre eles foram configuradas.
*   **Domínio (`ContratacaoService.Domain`)**:
    *   **`Contratacao.cs`**: Entidade que representa uma contratação, contendo `Id`, `PropostaId` e `DataContratacao`.
    *   **`IContratacaoRepository.cs`**: Interface de repositório para operações de persistência da entidade `Contratacao`.
*   **Aplicação (`ContratacaoService.Application`)**:
    *   **`ContratacaoAppService.cs`**: Serviço de aplicação que orquestra a lógica de negócio para efetivar uma contratação. Ele interage com o repositório de contratações e com o `PropostaService` para verificar o status da proposta.
    *   **`IPropostaServiceClient.cs`**: Interface que define o contrato para comunicação com o `PropostaService`, permitindo obter o status de uma proposta.
*   **Infraestrutura (`ContratacaoService.Infrastructure`)**:
    *   **`ContratacaoDbContext.cs`**: Contexto do Entity Framework Core para o `ContratacaoService`, mapeando a entidade `Contratacao`.
    *   **`ContratacaoRepository.cs`**: Implementação do `IContratacaoRepository`, utilizando o `ContratacaoDbContext` para persistir os dados da contratação.
    *   **`PropostaServiceClient.cs`**: Implementação do `IPropostaServiceClient`, utilizando `HttpClient` para realizar chamadas HTTP ao `PropostaService` e obter o status de uma proposta.
*   **API (`ContratacaoService.Api`)**:
    *   **`Program.cs`**: Configurado para:
        *   Registrar o `ContratacaoDbContext` com o provedor Npgsql para PostgreSQL.
        *   Registrar as dependências `IContratacaoRepository` e `ContratacaoAppService`.
        *   Configurar o `HttpClient` para `IPropostaServiceClient`, definindo a `BaseUrl` a partir das configurações.
        *   Aplicar automaticamente as migrações do banco de dados na inicialização.
    *   **`appsettings.Development.json` e `appsettings.json`**: Arquivos de configuração que definem a string de conexão para o banco de dados do `ContratacaoService` e a URL base para o `PropostaService`.
    *   **`Dockerfile`**: Define a imagem Docker para o `ContratacaoService.Api`.
*   **`docker-compose.yml`**: Atualizado para incluir o serviço `contratacao-service` e seu banco de dados (`contratacao-db`), garantindo que ambos os microserviços possam ser orquestrados juntos.

## 5. Exposição e Documentação das APIs (Controllers e Swagger)

Nesta etapa, os endpoints REST de ambos os serviços foram criados e documentados com Swagger.

*   **`PropostasController.cs` (`PropostaService.Api`)**:
    *   `POST /api/propostas`: Cria uma nova proposta.
    *   `GET /api/propostas`: Lista todas as propostas.
    *   `GET /api/propostas/{id}`: Busca uma proposta por ID.
    *   `PATCH /api/propostas/{id}/status`: Altera o status de uma proposta (aceita "Aprovada" ou "Recusada").
*   **`ContratacoesController.cs` (`ContratacaoService.Api`)**:
    *   `POST /api/contratacoes`: Efetiva a contratação de uma proposta, recebendo o `propostaId` no corpo da requisição.
*   **Documentação com Swagger**:
    *   O pacote `Swashbuckle.AspNetCore` foi adicionado a ambos os projetos de API.
    *   Os serviços foram configurados em `Program.cs` para gerar a documentação Swagger em ambiente de desenvolvimento.
    *   As APIs podem ser exploradas e testadas através das seguintes URLs:
        *   **PropostaService**: `http://localhost:5001/swagger`
        *   **ContratacaoService**: `http://localhost:5002/swagger`

## 6. Comandos Essenciais

Para gerenciar o ambiente e o banco de dados:

*   **Instalar a ferramenta `dotnet-ef` (se ainda não tiver):**
    ```bash
    dotnet tool install --global dotnet-ef
    ```
    Este comando instala a ferramenta de linha de comando do Entity Framework Core, necessária para gerenciar migrações.

*   **Gerar a Migração Inicial para PropostaService:**
    ```bash
    cd Seguros/src/PropostaService.Api
    dotnet ef migrations add InitialCreate --context PropostaDbContext --output-dir ../PropostaService.Infrastructure/Migrations
    ```
    Este comando cria uma nova migração chamada `InitialCreate` no diretório `PropostaService.Infrastructure/Migrations`, baseada no estado atual do `PropostaDbContext` e suas entidades.

*   **Gerar a Migração Inicial para ContratacaoService:**
    ```bash
    cd Seguros/src/ContratacaoService.Api
    dotnet ef migrations add InitialCreate --context ContratacaoDbContext --output-dir ../ContratacaoService.Infrastructure/Migrations
    ```
    Este comando cria uma nova migração chamada `InitialCreate` no diretório `ContratacaoService.Infrastructure/Migrations`, baseada no estado atual do `ContratacaoDbContext` e suas entidades.

*   **Subir o Ambiente Docker:**
    ```bash
    cd Seguros
    docker-compose up --build
    ```
    Este comando constrói as imagens Docker dos serviços (`PropostaService.Api`, `ContratacaoService.Api`) e inicia os contêineres, incluindo os bancos de dados PostgreSQL. As migrações serão aplicadas automaticamente e o banco de dados de desenvolvimento será populado (se `ASPNETCORE_ENVIRONMENT` for `Development`).

*   **Rodar os Testes:**
    ```bash
    cd Seguros
    dotnet test
    ```
    Este comando executa todos os testes unitários definidos nos projetos de teste.
