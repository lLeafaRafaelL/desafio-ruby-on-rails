# Desafio programação - para vaga desenvolvedor Ruby on Rails

Por favor leiam este documento do começo ao fim, com muita atenção.
O intuito deste teste é avaliar seus conhecimentos técnicos, para ser mais específico em Ruby on Rails.
O teste consiste em parsear [este arquivo de texto(CNAB)](https://github.com/ByCodersTec/desafio-ruby-on-rails/blob/master/CNAB.txt) e salvar suas informações(transações financeiras) em uma base de dados a critério do candidato.
Este desafio deve ser feito por você em sua casa. Gaste o tempo que você quiser, porém normalmente você não deve precisar de mais do que algumas horas.

# Instruções de entrega do desafio

1. Primeiro, faça um fork deste projeto para sua conta no Github (crie uma se você não possuir).
2. Em seguida, implemente o projeto tal qual descrito abaixo, em seu clone local.
3. Por fim, envie via email o projeto ou o fork/link do projeto para seu contato Bycoders_.

# Descrição do projeto

Você recebeu um arquivo CNAB com os dados das movimentações finanaceira de várias lojas.
Precisamos criar uma maneira para que estes dados sejam importados para um banco de dados.

Sua tarefa é criar uma interface web que aceite upload do [arquivo CNAB](https://github.com/ByCodersTec/desafio-ruby-on-rails/blob/master/CNAB.txt), normalize os dados e armazene-os em um banco de dados relacional e exiba essas informações em tela.

**Sua aplicação web DEVE:**

1. Ter uma tela (via um formulário) para fazer o upload do arquivo(pontos extras se não usar um popular CSS Framework )
2. Interpretar ("parsear") o arquivo recebido, normalizar os dados, e salvar corretamente a informação em um banco de dados relacional, **se atente as documentações** que estão logo abaixo.
3. Exibir uma lista das operações importadas por lojas, e nesta lista deve conter um totalizador do saldo em conta
4. Ser escrita obrigatoriamente em Ruby 2.0+ e Rails 5+
5. Ser simples de configurar e rodar, funcionando em ambiente compatível com Unix (Linux ou Mac OS X). Ela deve utilizar apenas linguagens e bibliotecas livres ou gratuitas.
6. Git com commits bem descritos
7. PostgreSQL
8. RUBOCOP
9. RSPEC
10. Simplecov para disponibilizar o code coverage
11. Docker compose (Pontos extras se utilizar)
12. Readme file descrevendo bem o projeto e seu setup
13. Incluir informação descrevendo como consumir o endpoint da API

**Sua aplicação web não precisa:**

1. Lidar com autenticação ou autorização (pontos extras se ela fizer, mais pontos extras se a autenticação for feita via OAuth).
2. Ser escrita usando algum framework específico (mas não há nada errado em usá-los também, use o que achar melhor).
3. Documentação da api.(Será um diferencial e pontos extras se fizer)

# Documentação do CNAB

| Descrição do campo  | Inicio | Fim | Tamanho | Comentário
| ------------- | ------------- | -----| ---- | ------
| Tipo  | 1  | 1 | 1 | Tipo da transação
| Data  | 2  | 9 | 8 | Data da ocorrência
| Valor | 10 | 19 | 10 | Valor da movimentação. *Obs.* O valor encontrado no arquivo precisa ser divido por cem(valor / 100.00) para normalizá-lo.
| CPF | 20 | 30 | 11 | CPF do beneficiário
| Cartão | 31 | 42 | 12 | Cartão utilizado na transação 
| Hora  | 43 | 48 | 6 | Hora da ocorrência atendendo ao fuso de UTC-3
| Dono da loja | 49 | 62 | 14 | Nome do representante da loja
| Nome loja | 63 | 81 | 19 | Nome da loja

# Documentação sobre os tipos das transações

| Tipo | Descrição | Natureza | Sinal |
| ---- | -------- | --------- | ----- |
| 1 | Débito | Entrada | + |
| 2 | Boleto | Saída | - |
| 3 | Financiamento | Saída | - |
| 4 | Crédito | Entrada | + |
| 5 | Recebimento Empréstimo | Entrada | + |
| 6 | Vendas | Entrada | + |
| 7 | Recebimento TED | Entrada | + |
| 8 | Recebimento DOC | Entrada | + |
| 9 | Aluguel | Saída | - |

# Avaliação

Seu projeto será avaliado de acordo com os seguintes critérios.

1. Sua aplicação preenche os requerimentos básicos?
2. Você documentou a maneira de configurar o ambiente e rodar sua aplicação?
3. Você seguiu as instruções de envio do desafio?
4. Qualidade e cobertura dos testes unitários.

Adicionalmente, tentaremos verificar a sua familiarização com as bibliotecas padrões (standard libs), bem como sua experiência com programação orientada a objetos a partir da estrutura de seu projeto.

# Referência

Este desafio foi baseado neste outro desafio: https://github.com/lschallenges/data-engineering

---

Boa sorte!

---

# Sistema de Processamento de Arquivos CNAB

Sistema desenvolvido em .NET 9.0 para processamento de arquivos CNAB (Centro Nacional de Automação Bancária) com arquitetura baseada em Clean Architecture e Domain-Driven Design.

---

## Índice

- [Visão Geral](#visão-geral)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Arquitetura](#arquitetura)
- [Como Funciona](#como-funciona)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Endpoints da API](#endpoints-da-api)
- [Como Testar](#como-testar)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Banco de Dados](#banco-de-dados)
- [Monitoramento](#monitoramento)
- [Desenvolvimento](#desenvolvimento)
- [Troubleshooting](#troubleshooting)

---

## Visão Geral

Este sistema processa arquivos CNAB contendo transações financeiras de múltiplas lojas, armazenando os dados em um banco PostgreSQL e disponibilizando APIs REST para consulta.

### Principais Características

- Upload de arquivos CNAB via API REST
- Processamento assíncrono em background worker
- Parser de arquivos com posições fixas (80 caracteres por linha)
- Cálculo automático de saldo (entradas - saídas)
- API REST para consulta de transações por loja e período
- Health checks e métricas Prometheus
- Logs estruturados com Serilog
- Containerização com Docker Compose

---

## Tecnologias Utilizadas

### Backend
- **.NET 9.0** - Framework principal
- **ASP.NET Core** - API REST
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados relacional

### Qualidade e Testes
- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions fluentes
- **NSubstitute** - Mocks e stubs

### Infraestrutura
- **Docker** - Containerização
- **Docker Compose** - Orquestração de containers
- **Serilog** - Logging estruturado
- **Prometheus** - Métricas
- **Swagger/OpenAPI** - Documentação da API

---

## Arquitetura

### Estrutura de Camadas

O projeto segue os princípios de Clean Architecture com separação clara de responsabilidades:

┌─────────────────────────────────────────┐ │ ByCoders.CNAB.API (REST API) │ │ ByCoders.CNAB.Worker (Background) │ ├─────────────────────────────────────────┤ │ ByCoders.CNAB.Application (Use Cases) │ ├─────────────────────────────────────────┤ │ ByCoders.CNAB.Domain (Entities) │ ├─────────────────────────────────────────┤ │ ByCoders.CNAB.Infrastructure (Data) │ ├─────────────────────────────────────────┤ │ ByCoders.CNAB.Core (Shared) │ └─────────────────────────────────────────┘


### Componentes

**ByCoders.CNAB.API**
- Controllers REST
- Middlewares (Correlation, Exception Filter)
- Health checks e métricas

**ByCoders.CNAB.Worker**
- Background service para processamento assíncrono
- Polling de arquivos pendentes a cada 30 segundos

**ByCoders.CNAB.Application**
- Handlers (Upload, Consulta de transações)
- Validators (FluentValidation)
- Parsers (CNAB line parser)
- Factories (Transaction factory)

**ByCoders.CNAB.Domain**
- Entidades (Transaction, CNABFile)
- Value Objects (Beneficiary, Card, Store)
- Tipos de transação (9 tipos)
- Interfaces de repositório

**ByCoders.CNAB.Infrastructure**
- Entity Framework DbContext
- Repositórios concretos
- Migrations
- File Storage Service

**ByCoders.CNAB.Core**
- Padrões base (Result, Handler)
- Validators abstratos
- Extensões HTTP

---

## Como Funciona

### Fluxo de Processamento

Cliente envia arquivo CNAB │ ▼
API recebe e salva arquivo localmente │ ▼
API retorna 202 Accepted (FileId + Status) │ ▼
Worker detecta arquivo pendente (polling 30s) │ ▼
Worker processa linha por linha │ ├─ Parser extrai dados (80 chars posições fixas) ├─ Validator valida formato ├─ Factory cria entidades Transaction └─ Repository faz bulk insert │ ▼
Arquivo marcado como "Processed" │ ▼
Cliente consulta transações via API

### Formato do Arquivo CNAB

Cada linha do arquivo possui exatamente 80 caracteres com posições fixas:

| Campo          | Início | Fim | Tamanho | Exemplo         | Descrição                    |
|----------------|--------|-----|---------|-----------------|------------------------------|
| Tipo           | 1      | 1   | 1       | 3               | Tipo da transação (1-9)      |
| Data           | 2      | 9   | 8       | 20190301        | Formato YYYYMMDD             |
| Valor          | 10     | 19  | 10      | 0000014200      | Valor em centavos (142.00)   |
| CPF            | 20     | 30  | 11      | 09620676017     | CPF do beneficiário          |
| Cartão         | 31     | 42  | 12      | 4753****3153    | Número do cartão             |
| Hora           | 43     | 48  | 6       | 153453          | Formato HHMMSS (UTC-3)       |
| Dono da Loja   | 49     | 62  | 14      | JOÃO MACEDO     | Nome do representante        |
| Nome da Loja   | 63     | 80  | 18      | BAR DO JOÃO     | Nome da loja                 |

**Exemplo de linha completa:**
3201903010000014200096206760174753****3153153453JOÃO MACEDO BAR DO JOÃO


### Tipos de Transação

| Tipo | Descrição              | Natureza | Sinal | Impacto no Saldo |
|------|------------------------|----------|-------|------------------|
| 1    | Débito                 | Entrada  | +     | Aumenta          |
| 2    | Boleto                 | Saída    | -     | Diminui          |
| 3    | Financiamento          | Saída    | -     | Diminui          |
| 4    | Crédito                | Entrada  | +     | Aumenta          |
| 5    | Recebimento Empréstimo | Entrada  | +     | Aumenta          |
| 6    | Vendas                 | Entrada  | +     | Aumenta          |
| 7    | Recebimento TED        | Entrada  | +     | Aumenta          |
| 8    | Recebimento DOC        | Entrada  | +     | Aumenta          |
| 9    | Aluguel                | Saída    | -     | Diminui          |

---

## Pré-requisitos

- **Docker** versão 20.10 ou superior
- **Docker Compose** versão 1.29 ou superior

Para desenvolvimento local (opcional):
- **.NET 9.0 SDK**
- **PostgreSQL 16**

---

## Instalação e Execução

### Método 1: Docker Compose (Recomendado)

```bash
# 1. Clonar o repositório
git clone <repository-url>
cd desafio-ruby-on-rails

# 2. Build das imagens Docker
docker-compose build

# 3. Iniciar todos os serviços
docker-compose up -d

# 4. Verificar status dos containers
docker-compose ps

# 5. Acompanhar logs
docker-compose logs -f
Método 2: Script de Automação
# Dar permissão de execução (se necessário)
chmod +x run.sh

# Build das imagens
./run.sh docker-build

# Iniciar serviços
./run.sh docker-up

# Ver logs
./run.sh docker-logs

# Parar serviços
./run.sh docker-down
Comandos Disponíveis no Script
./run.sh help              # Exibe ajuda
./run.sh docker-build      # Build das imagens
./run.sh docker-up         # Inicia containers
./run.sh docker-down       # Para containers
./run.sh docker-logs       # Exibe logs
./run.sh test              # Executa testes
./run.sh build             # Build da solution
./run.sh clean             # Limpa artifacts
Serviços Disponíveis
Após iniciar os containers, os seguintes endpoints estarão disponíveis:

| Serviço | URL | Descrição | |----------------------|----------------------------------------|----------------------------------| | API REST | http://localhost:5000 | API principal | | Swagger UI | http://localhost:5000/swagger | Documentação interativa | | Health (Liveness) | http://localhost:5000/health/liveness | Status do serviço | | Health (Readiness) | http://localhost:5000/health/readiness | Prontidão para requisições | | Prometheus Metrics | http://localhost:5000/metrics | Métricas da aplicação | | PostgreSQL | localhost:5432 | Banco de dados |

Credenciais do PostgreSQL:

Database: cnab_db
Username: cnab_user
Password: cnab_pass123
Endpoints da API
1. Upload de Arquivo CNAB
Endpoint: POST /api/files

Content-Type: multipart/form-data

Parâmetros:

file (form-data): Arquivo CNAB (.txt)
Resposta de Sucesso (202 Accepted):

{
  "fileId": "01938e6f-7890-7abc-b123-456789abcdef",
  "fileName": "CNAB.txt",
  "status": 1
}
Status do Arquivo:

0 = Pending (aguardando processamento)
1 = Processing (sendo processado)
2 = Processed (concluído com sucesso)
3 = Failed (falha no processamento)
Exemplo com cURL:

curl -X POST "http://localhost:5000/api/files" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@CNAB.txt"
2. Consultar Transações por Loja
Endpoint: GET /api/transactions/store/{storeName}

Parâmetros de Query:

fromDate (DateTimeOffset, obrigatório): Data inicial no formato ISO 8601
toDate (DateTimeOffset, obrigatório): Data final no formato ISO 8601
Parâmetros de Path:

storeName (string, obrigatório): Nome exato da loja (case sensitive)
Resposta de Sucesso (200 OK):

{
  "startDate": "2019-03-01T00:00:00+00:00",
  "endDate": "2019-03-31T23:59:59+00:00",
  "totalTrsanctions": 5,
  "accumulatedValue": 125.00,
  "transactions": [
    {
      "id": "01938e6f-1111-7abc-b123-456789abcdef",
      "createdOn": "2025-10-27T10:30:00+00:00",
      "cnabFileId": "01938e6f-7890-7abc-b123-456789abcdef",
      "transactionType": {
        "id": 3,
        "description": "Funding",
        "nature": 2
      },
      "transactionDateTime": "2019-03-01T15:34:53+00:00",
      "amountCNAB": 14200,
      "transactionValue": 142.00,
      "beneficiary": {
        "document": "09620676017"
      },
      "card": {
        "number": "4753****3153"
      },
      "store": {
        "name": "BAR DO JOÃO",
        "owner": "JOÃO MACEDO"
      }
    }
  ]
}
Resposta sem Dados (204 No Content): Retornado quando não há transações para a loja no período especificado.

Exemplo com cURL:

curl -X GET "http://localhost:5000/api/transactions/store/BAR%20DO%20JO%C3%83O?fromDate=2019-03-01T00:00:00Z&toDate=2019-03-31T23:59:59Z"
Lojas disponíveis no arquivo de exemplo:

BAR DO JOÃO
LOJA DO Ó - MATRIZ
LOJA DO Ó - FILIAL
MERCADO DA AVENIDA
MERCEARIA 3 IRMÃOS
Como Testar
Teste Completo Passo a Passo
1. Verificar se a API está rodando

curl http://localhost:5000/health/liveness
Resposta esperada:

{
  "status": "Healthy"
}
2. Fazer upload do arquivo CNAB

curl -X POST "http://localhost:5000/api/files" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@CNAB.txt"
Resposta esperada (202 Accepted):

{
  "fileId": "01938e6f-7890-7abc-b123-456789abcdef",
  "fileName": "CNAB.txt",
  "status": 1
}
3. Aguardar processamento (aproximadamente 30 segundos)

Acompanhar logs do worker:

docker-compose logs -f cnab-worker
Você verá mensagens indicando o processamento:

[10:30:00 INF] Processing CNAB file: CNAB.txt
[10:30:01 INF] Parsed 21 transactions
[10:30:01 INF] File processed successfully
4. Consultar transações processadas

curl -X GET "http://localhost:5000/api/transactions/store/BAR%20DO%20JO%C3%83O?fromDate=2019-03-01T00:00:00Z&toDate=2019-03-31T23:59:59Z"
5. Testar outras lojas

# MERCADO DA AVENIDA
curl -X GET "http://localhost:5000/api/transactions/store/MERCADO%20DA%20AVENIDA?fromDate=2019-01-01T00:00:00Z&toDate=2019-12-31T23:59:59Z"

# LOJA DO Ó - MATRIZ
curl -X GET "http://localhost:5000/api/transactions/store/LOJA%20DO%20%C3%93%20-%20MATRIZ?fromDate=2019-01-01T00:00:00Z&toDate=2019-12-31T23:59:59Z"
Usando Postman
Importar Collection:

Importe o arquivo CNAB-API.postman_collection.json no Postman
A collection já vem com variáveis configuradas (baseUrl = http://localhost:5000)
Execute as requisições na ordem:
Health Checks
Upload CNAB File
Get Transactions by Store
Testes Automatizados:

Cada requisição possui scripts de teste que validam:

Status code correto
Estrutura da resposta
Tipos de dados
Regras de negócio
Execute a collection completa usando o Runner do Postman.

Usando Swagger UI
Acesse: http://localhost:5000/swagger
Navegue até POST /api/files
Clique em "Try it out"
Faça upload do arquivo CNAB.txt
Aguarde processamento
Teste o endpoint GET /api/transactions/store/{storeName}
Estrutura do Projeto
desafio-ruby-on-rails/
│
├── ByCoders.CNAB.API/                  # REST API
│   ├── Controllers/                    # Controllers REST
│   ├── Middlewares/                    # Middlewares customizados
│   ├── Filters/                        # Exception filters
│   └── Program.cs                      # Configuração da aplicação
│
├── ByCoders.CNAB.Worker/               # Background Worker
│   ├── Files/                          # Processador de arquivos
│   ├── Configurations/                 # Configurações
│   └── Program.cs                      # Host configuration
│
├── ByCoders.CNAB.Application/          # Casos de Uso
│   ├── Files/CNAB/                     # Upload e processamento
│   │   ├── Upload/                     # Handler de upload
│   │   └── Process/                    # Parser e processamento
│   ├── Transactions/                   # Consulta de transações
│   │   ├── FindTransactions/           # Handler de consulta
│   │   └── Factories/                  # Transaction factory
│   └── DI/                             # Dependency Injection
│
├── ByCoders.CNAB.Domain/               # Entidades de Domínio
│   ├── Files/Models/                   # CNABFile aggregate
│   ├── Transactions/Models/            # Transaction aggregate
│   │   ├── Transaction.cs              # Classe base abstrata
│   │   ├── Debit.cs, Credit.cs, etc.   # Tipos concretos
│   │   └── TransactionType.cs          # Value object
│   └── Transactions/                   # Interfaces de repositório
│
├── ByCoders.CNAB.Infrastructure/       # Infraestrutura
│   ├── EntityFrameworkCore/            # EF Core configuration
│   │   ├── CNABFileDbContext.cs        # DbContext
│   │   └── Builders/                   # Entity builders
│   ├── Repositories/                   # Implementação de repositórios
│   ├── Migrations/                     # Database migrations
│   └── DI/                             # Dependency Injection
│
├── ByCoders.CNAB.Core/                 # Camada Base
│   ├── Handlers/                       # Handler pattern
│   ├── Results/                        # Result pattern
│   ├── Validators/                     # Validators base
│   ├── Http/                           # HTTP extensions
│   └── Prometheus/                     # Prometheus integration
│
├── ByCoders.CNAB.UnitTests/            # Testes Unitários
│   ├── Application/                    # Testes de application
│   ├── Domain/                         # Testes de domínio
│   └── Infrastructure/                 # Testes de infraestrutura
│
├── docker-compose.yml                  # Orquestração Docker
├── Dockerfile.API                      # Build da API
├── Dockerfile.Worker                   # Build do Worker
├── run.sh                              # Script de automação
├── CNAB.txt                            # Arquivo de exemplo
└── README.md                           # Este arquivo
Banco de Dados
Schema
O banco de dados possui 3 tabelas principais:

cnabfiles

CREATE TABLE cnabfiles (
    id UUID PRIMARY KEY,
    filename VARCHAR(255) NOT NULL,
    filepath VARCHAR(500) NOT NULL UNIQUE,
    filesize BIGINT NOT NULL,
    uploadedon TIMESTAMP NOT NULL,
    processingstartedon TIMESTAMP,
    processedon TIMESTAMP,
    failedon TIMESTAMP,
    errormessage VARCHAR(2000),
    transactioncount INTEGER NOT NULL DEFAULT 0
);

-- Índices
CREATE INDEX idx_cnabfiles_uploadedon ON cnabfiles(uploadedon);
CREATE INDEX idx_cnabfiles_processedon ON cnabfiles(processedon);
CREATE INDEX idx_cnabfiles_failedon ON cnabfiles(failedon);
transactiontypes (dados pré-carregados)

CREATE TABLE transactiontypes (
    id INTEGER PRIMARY KEY,
    description VARCHAR(30) NOT NULL,
    nature SMALLINT NOT NULL  -- 1=Entrada, 2=Saída
);

-- Dados iniciais
INSERT INTO transactiontypes VALUES
(1, 'Debit', 1),
(2, 'Bank Slip', 2),
(3, 'Funding', 2),
(4, 'Credit', 1),
(5, 'Loan Receipt', 1),
(6, 'Sales', 1),
(7, 'TED Receipt', 1),
(8, 'DOC Receipt', 1),
(9, 'Rent', 2);
transactions

CREATE TABLE transactions (
    id UUID PRIMARY KEY,
    createdon TIMESTAMPTZ NOT NULL,
    cnabfileid UUID,
    transactiontypeid INTEGER NOT NULL,
    transactiondatetime TIMESTAMPTZ NOT NULL,
    amountcnab NUMERIC NOT NULL,
    beneficiary_document VARCHAR(11) NOT NULL,
    card_number VARCHAR(12) NOT NULL,
    store_name VARCHAR(19) NOT NULL,
    store_owner VARCHAR(14) NOT NULL,
    FOREIGN KEY (transactiontypeid) REFERENCES transactiontypes(id)
);

-- Índices
CREATE INDEX idx_transactions_cnabfileid ON transactions(cnabfileid);
CREATE INDEX idx_transactions_store_name ON transactions(store_name);
CREATE INDEX idx_transactions_datetime ON transactions(transactiondatetime DESC);
Conectar ao Banco
Via Docker:

docker exec -it cnab-postgres psql -U cnab_user -d cnab_db
Via Client Externo:

Host: localhost
Port: 5432
Database: cnab_db
Username: cnab_user
Password: cnab_pass123
Consultas Úteis:

-- Ver todos os arquivos processados
SELECT * FROM cnabfiles ORDER BY uploadedon DESC;

-- Ver transações de uma loja
SELECT * FROM transactions WHERE store_name = 'BAR DO JOÃO';

-- Saldo por loja
SELECT 
    store_name,
    COUNT(*) as total_transactions,
    SUM(amountcnab / 100.0) as accumulated_value
FROM transactions
GROUP BY store_name;

-- Ver tipos de transação
SELECT * FROM transactiontypes;
Monitoramento
Health Checks
Liveness Probe - Verifica se a aplicação está rodando

curl http://localhost:5000/health/liveness
Readiness Probe - Verifica se está pronta para receber requisições

curl http://localhost:5000/health/readiness
Ambos retornam:

{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456"
}
Métricas Prometheus
Endpoint de métricas:

curl http://localhost:5000/metrics
Métricas disponíveis:

Requisições HTTP (contadores e histogramas)
Latência de endpoints
Status codes
Métricas de runtime .NET
Logs Estruturados
Ver logs em tempo real:

# API
docker-compose logs -f cnab-api

# Worker
docker-compose logs -f cnab-worker

# PostgreSQL
docker-compose logs -f postgres

# Todos os serviços
docker-compose logs -f
Formato dos logs (Serilog):

[10:30:00 INF] SourceContext: Message
[10:30:01 WRN] SourceContext: Warning message
[10:30:02 ERR] SourceContext: Error message
Exception details...
Status dos Containers
# Ver status
docker-compose ps

# Ver recursos utilizados
docker stats

# Inspecionar container
docker inspect cnab-api
Desenvolvimento
Executar Testes Unitários
# Todos os testes
dotnet test

# Com saída detalhada
dotnet test --verbosity detailed

# Testes de um projeto específico
dotnet test ByCoders.CNAB.UnitTests/

# Com cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
Build Local
# Restaurar dependências
dotnet restore

# Build completo
dotnet build

# Build em modo Release
dotnet build -c Release

# Publicar
dotnet publish -c Release -o ./publish
Executar Localmente (sem Docker)
Requisitos:

.NET 9.0 SDK
PostgreSQL 16 rodando na porta 5432
1. Atualizar connection string em appsettings.json

2. Executar migrations:

cd ByCoders.CNAB.Infrastructure
dotnet ef database update --context CNABFileDbContext
3. Iniciar API:

cd ByCoders.CNAB.API
dotnet run
4. Iniciar Worker (em outro terminal):

cd ByCoders.CNAB.Worker
dotnet run
Adicionar Nova Migration
cd ByCoders.CNAB.Infrastructure
dotnet ef migrations add NomeDaMigration --context CNABFileDbContext
dotnet ef database update
Troubleshooting
Problema: Containers não iniciam
Solução:

# Ver logs detalhados
docker-compose logs

# Rebuild completo
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
Problema: API retorna 503 (Service Unavailable)
Causa: PostgreSQL não está pronto

Solução:

# Verificar saúde do PostgreSQL
docker-compose ps

# Esperar até health status = healthy
docker exec -it cnab-postgres pg_isready -U cnab_user
Problema: Worker não processa arquivos
Diagnóstico:

# 1. Verificar logs do worker
docker-compose logs cnab-worker

# 2. Verificar arquivos pendentes no banco
docker exec -it cnab-postgres psql -U cnab_user -d cnab_db \
  -c "SELECT * FROM cnabfiles WHERE processedon IS NULL;"

# 3. Verificar se worker está rodando
docker-compose ps cnab-worker
Possíveis causas:

Worker não tem acesso ao diretório de storage
Permissões incorretas no volume
Banco de dados inacessível
Problema: Erro 400 ao fazer upload
Causa comum: Arquivo vazio ou formato inválido

Verificar:

Arquivo tem extensão .txt
Arquivo não está vazio
Cada linha tem exatamente 80 caracteres
Formato segue especificação CNAB
Problema: Transações não aparecem na consulta
Checklist:

# 1. Arquivo foi processado?
docker exec -it cnab-postgres psql -U cnab_user -d cnab_db \
  -c "SELECT filename, processedon FROM cnabfiles;"

# 2. Transações foram inseridas?
docker exec -it cnab-postgres psql -U cnab_user -d cnab_db \
  -c "SELECT COUNT(*) FROM transactions;"

# 3. Nome da loja está correto? (case sensitive)
docker exec -it cnab-postgres psql -U cnab_user -d cnab_db \
  -c "SELECT DISTINCT store_name FROM transactions;"

# 4. Período de datas está correto?
# Verifique se as datas no arquivo estão no intervalo consultado
Problema: Banco de dados não aceita conexão
Solução:

# Reiniciar apenas o PostgreSQL
docker-compose restart postgres

# Verificar logs
docker-compose logs postgres

# Testar conexão
docker exec -it cnab-postgres psql -U cnab_user -d cnab_db -c "SELECT 1;"
Limpar Tudo e Recomeçar
# Parar e remover containers, volumes e networks
docker-compose down -v

# Remover imagens
docker-compose down --rmi all

# Rebuild e reiniciar
docker-compose build --no-cache
docker-compose up -d
Documentação Adicional
Swagger/OpenAPI
Acesse a documentação interativa da API em: http://localhost:5000/swagger

A documentação inclui:

Todos os endpoints disponíveis
Schemas de request/response
Exemplos de uso
Possibilidade de testar diretamente no navegador
Postman Collection
Importe o arquivo CNAB-API.postman_collection.json para ter acesso a:

Requisições pré-configuradas
Testes automatizados
Variáveis de ambiente
Exemplos de uso
Arquitetura de Decisão
Por que Clean Architecture?
Separação de responsabilidades: Cada camada tem uma responsabilidade clara
Testabilidade: Domain e Application são independentes de framework
Manutenibilidade: Mudanças em uma camada não afetam outras
Flexibilidade: Fácil trocar banco de dados ou framework
Por que Processamento Assíncrono?
Performance: API responde imediatamente sem bloquear
Escalabilidade: Worker pode processar múltiplos arquivos em paralelo
Resiliência: Falhas no processamento não afetam a API
Monitoramento: Status do processamento pode ser consultado
Por que Result Pattern?
Evita exceptions: Usa Result<T> para fluxo de negócio
Código mais limpo: Fica explícito quando operação pode falhar
Performance: Exceptions são caras em termos de performance
Rastreabilidade: Failure details fornecem contexto rico
Licença
Este projeto foi desenvolvido como desafio técnico.

Contato
Para dúvidas ou sugestões sobre este projeto, entre em contato através do repositório.