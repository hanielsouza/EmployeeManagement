# Employee Management API

Uma API RESTful para gerenciamento de funcionários desenvolvida com .NET 8.

## Tecnologias e Padrões

### Backend Framework
- .NET 8.0
- ASP.NET Core Web API
- C#

### Arquitetura e Padrões
- Clean Architecture
  - Domain Layer: Entidades e regras de negócio core
  - Application Layer: Casos de uso e interfaces
  - Infrastructure Layer: Implementações técnicas
  - API Layer: Controllers e configurações da API
- Dependency Injection
- Repository Pattern
- DTO Pattern

### Banco de Dados
- Microsoft SQL Server 2022
- Entity Framework Core 8.0
  - Code First
  - Migrations
  - Fluent API para configuração de entidades

### Autenticação e Autorização
- JWT (JSON Web Tokens)
- Role-based Authorization
- BCrypt para hash de senhas

### Logging
- Serilog

### Documentação da API
- Swagger / OpenAPI

### Docker Support
- Dockerfile multi-stage para build e runtime
- Docker Compose para orquestração de serviços:
  - API
  - SQL Server
- Volumes para persistência de dados e logs

## Usuários de Teste

A aplicação inclui os seguintes usuários pré-cadastrados para teste:

| Nome | Email | Senha | Cargo |
|------|--------|-------|-------|
| João Silva | diretor@empresa.com | 123456 | Diretor |
| Maria Santos | lider@empresa.com | 123456 | Líder |
| Pedro Oliveira | pedro@empresa.com | 123456 | Funcionário |
| Ana Costa | ana@empresa.com | 123456 | Funcionário |

**Observação**: Todos os usuários utilizam a senha padrão "123456" para facilitar os testes.

## Requisitos

- Docker
- Docker Compose

## Executando com Docker

1. Clone o repositório
2. Navegue até a pasta do projeto
3. Execute o seguinte comando para iniciar a API e o banco de dados:

```bash
docker-compose up -d
```

A API estará disponível em:
- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081

O SQL Server estará disponível em:
- Host: localhost
- Porta: 1433
- Usuário: sa
- Senha: YourStrong!Passw0rd

## Funcionalidades Principais

### Gerenciamento de Funcionários
- CRUD completo de funcionários
- Suporte a múltiplos números de telefone
- Hierarquia de gerentes
- Validação de idade mínima (18 anos)

### Controle de Acesso
- Três níveis de acesso:
  - Employee: Acesso básico
  - Leader: Pode criar e atualizar funcionários
  - Director: Acesso total

## Endpoints da API

A documentação completa da API está disponível através do Swagger UI em:
- http://localhost:8080/swagger
- https://localhost:8081/swagger

## Logs

Os logs da aplicação são armazenados em:
- Console do container (visualizável através de `docker-compose logs -f api`)
- Pasta `logs` no diretório do projeto (mapeada como volume)
