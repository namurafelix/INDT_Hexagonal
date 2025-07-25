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
A organização dos projetos dentro da solution é crucial para refletir a arquitetura

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
