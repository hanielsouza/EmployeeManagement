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
- SOLID Principles
- Dependency Injection
- Repository Pattern
- DTO Pattern

### Estrutura do Projeto
```
EmployeeManagement/
├── EmployeeManagement.Domain/        # Camada de domínio
│   ├── Entities/                     # Entidades de domínio
│   ├── Interfaces/                   # Interfaces de repositório
│   └── Enums/                        # Enumerações
│
├── EmployeeManagement.Application/    # Camada de aplicação
│   ├── DTOs/                         # Objetos de transferência de dados
│   ├── Interfaces/                   # Interfaces de serviço
│   └── Services/                     # Implementação dos casos de uso
│
├── EmployeeManagement.Infrastructure/ # Camada de infraestrutura
│   ├── Data/                         # Contexto e configurações do EF Core
│   ├── Repositories/                 # Implementação dos repositórios
│   └── Services/                     # Serviços de infraestrutura
│
└── EmployeeManagement.Api/           # Camada de apresentação
    └── Controllers/                  # Controllers da API
```

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
- Claims-based Authentication

### Logging
- Serilog
- Múltiplos sinks:
  - Console
  - Arquivo (com rotação diária)
- Estruturado com suporte a contexto
- Diferentes níveis de log por namespace

### Documentação da API
- Swagger / OpenAPI
- Documentação detalhada dos endpoints

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
  - Director: Acesso total, incluindo deleção

### Logging
- Logs de operações CRUD
- Logs de autenticação
- Logs de erros e exceções
- Logs de performance

## Endpoints da API

A documentação completa da API está disponível através do Swagger UI em:
- http://localhost:8080/swagger
- https://localhost:8081/swagger

## Logs

Os logs da aplicação são armazenados em:
- Console do container (visualizável através de `docker-compose logs -f api`)
- Pasta `logs` no diretório do projeto (mapeada como volume)

## Comandos Docker úteis

- Iniciar os serviços:
```bash
docker-compose up -d
```

- Parar os serviços:
```bash
docker-compose down
```

- Visualizar logs da API:
```bash
docker-compose logs -f api
```

- Visualizar logs do banco de dados:
```bash
docker-compose logs -f db
```

- Reconstruir a imagem da API (após alterações no código):
```bash
docker-compose build api
docker-compose up -d api
```

## Volumes

- `sqlserver_data`: Armazena os dados do SQL Server
- `./logs`: Armazena os logs da aplicação

## Portas

- API:
  - 8080: HTTP
  - 8081: HTTPS
- SQL Server:
  - 1433: Porta padrão do SQL Server

## Contribuindo

1. Fork o projeto
2. Crie sua branch de feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.
