# Sistema de Gerenciamento CNAB

Sistema completo para upload e consulta de arquivos CNAB (Centro Nacional de Automação Bancária) com frontend React e backend .NET.

## Arquitetura

```
┌─────────────────┐      ┌──────────────┐      ┌─────────────┐
│  React Frontend │─────▶│  Nginx Proxy │─────▶│  .NET API   │
│   (Port 3000)   │      │  (Port 80)   │      │ (Port 5000) │
└─────────────────┘      └──────────────┘      └─────────────┘
                                                       │
                                                       ▼
                         ┌──────────────┐      ┌─────────────┐
                         │    Worker    │─────▶│  PostgreSQL │
                         │ (Port 8081)  │      │ (Port 5432) │
                         └──────────────┘      └─────────────┘
```

## Componentes

### Frontend (React + TypeScript)
- **Porta**: 3000 (mapeada para 80 no container)
- **Tecnologias**: React 18, TypeScript, Axios, React Router
- **Features**:
  - Upload de arquivos CNAB com drag & drop
  - Visualização prévia das transações
  - Consulta de transações por loja e período
  - Interface moderna com tema futurista

### Backend (.NET 8)
- **API**: Porta 5000 (mapeada de 8080 no container)
- **Worker**: Porta 8081 (processamento assíncrono)
- **Tecnologias**: .NET 8, Entity Framework Core, PostgreSQL
- **Features**:
  - Upload e processamento de arquivos CNAB
  - Consulta de transações com filtros
  - Processamento assíncrono via Worker
  - Documentação com Swagger

### Banco de Dados
- **PostgreSQL 16**: Porta 5432
- **Volumes persistentes** para dados e arquivos

## Pré-requisitos

- **Docker**: versão 20.10 ou superior
- **Docker Compose**: versão 2.0 ou superior
- **Portas disponíveis**: 3000, 5000, 5432, 8081

## Instalação e Execução

### Opção 1: Docker Compose (Recomendado)

```bash
# Clone o repositório
git clone <url-do-repositorio>
cd desafio-ruby-on-rails

# Inicie todos os serviços
docker compose up -d

# Aguarde alguns segundos para os serviços inicializarem
# Verifique o status
docker compose ps

# Logs em tempo real
docker compose logs -f
```

### Opção 2: Build e Run Manual

```bash
# Build das imagens
docker compose build

# Inicie os serviços
docker compose up -d

# Pare os serviços
docker compose down

# Pare e remova volumes (cuidado: apaga dados)
docker compose down -v
```

## Acessando a Aplicação

Após iniciar os serviços:

- **Frontend**: http://localhost:3000
- **API (Swagger)**: http://localhost:5000
- **Métricas do Worker**: http://localhost:8081/metrics
- **Health Check API**: http://localhost:5000/health/liveness
- **Health Check Frontend**: http://localhost:3000/health

## Formato do Arquivo CNAB

O arquivo CNAB deve seguir o formato especificado com 80 caracteres por linha:

```
Tipo (1) | Data (8) | Valor (10) | CPF (11) | Cartão (12) | Hora (6) | Dono Loja (14) | Nome Loja (19)
```

**Exemplo de linha válida:**
```
32022030512340001234567890123456123456789012123456JOÃO SILVA      LOJA DO JOÃO       
```

### Tipos de Transação

| Tipo | Descrição            | Natureza |
|------|---------------------|----------|
| 1    | Débito              | Entrada  |
| 2    | Boleto              | Saída    |
| 3    | Financiamento       | Saída    |
| 4    | Crédito             | Entrada  |
| 5    | Recebimento Empréstimo | Entrada |
| 6    | Vendas              | Entrada  |
| 7    | Recebimento TED     | Entrada  |
| 8    | Recebimento DOC     | Entrada  |
| 9    | Aluguel             | Saída    |

## Comunicação entre Serviços

### Frontend → API
O frontend faz requisições através do proxy reverso do Nginx:
- **URL em desenvolvimento**: `http://localhost:5000/api`
- **URL em produção (Docker)**: `/api` (proxy do Nginx redireciona para `cnab-api:8080`)

### Endpoints da API

#### Upload de Arquivo CNAB
```http
POST /api/Files
Content-Type: multipart/form-data

file: <arquivo.txt>
```

#### Consultar Transações
```http
GET /api/Transactions/store/{storeName}?fromDate=2025-11-01T00:00:00Z&toDate=2025-11-02T00:00:00Z
```

## Configuração do Docker

### Variáveis de Ambiente

#### Frontend
```env
REACT_APP_API_URL=
# Vazio significa que usará o proxy reverso do Nginx (/api → cnab-api:8080)
```

#### API
```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=cnab_db
```

### Volumes

- **cnab-files**: Armazena arquivos CNAB enviados
- **postgres-data**: Persiste dados do PostgreSQL

## Testando a Aplicação

### Via Interface Web

1. Acesse http://localhost:3000
2. Clique em "Upload CNAB"
3. Arraste um arquivo ou clique para selecionar
4. Visualize o preview das transações
5. Clique em "Enviar para API"
6. Vá para "Consulta CNAB"
7. Digite o nome da loja e selecione o período
8. Clique em "Buscar"

## Monitoramento

### Health Checks

```bash
# API
curl http://localhost:5000/health/liveness

# Frontend
curl http://localhost:3000/health
```

### Logs

```bash
# Todos os serviços
docker compose logs -f

# Serviço específico
docker compose logs -f cnab-api
docker compose logs -f cnab-frontend
```

##  Troubleshooting

### Frontend não consegue conectar na API

1. Verifique se a API está rodando: `docker compose ps`
2. Verifique os logs da API: `docker compose logs cnab-api`
3. Teste o endpoint: `curl http://localhost:5000/health/liveness`

### Worker não está processando arquivos

1. Verifique os logs: `docker compose logs cnab-worker`
2. Reinicie o Worker: `docker compose restart cnab-worker`

## Estrutura do Projeto

```
.
├── ByCoders.CNAB.API/              # API REST
├── ByCoders.CNAB.Worker/           # Processamento assíncrono
├── cnab-react/                     # Frontend React
│   ├── src/
│   │   ├── components/
│   │   ├── services/
│   │   │   └── cnabService.ts     # Integração com API
│   │   └── types/
│   ├── nginx.conf                  # Proxy reverso
│   └── .env.docker                 # Config Docker
├── docker-compose.yml              # Orquestração
├── Dockerfile.API
├── Dockerfile.Worker
└── Dockerfile.Frontend
```
