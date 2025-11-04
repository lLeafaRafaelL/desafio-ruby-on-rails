# Arquitetura e Comunicação - Sistema CNAB

## Diagrama de Arquitetura

```
┌─────────────────────────────────────────────────────────────────────┐
│                         USUÁRIO (Browser)                            │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    │ http://localhost:3000
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    Frontend Container (cnab-frontend)                │
│                                                                       │
│  ┌────────────────────┐              ┌──────────────────────┐      │
│  │   React App        │              │   Nginx (Port 80)     │      │
│  │   - Upload CNAB    │              │   - Serve React       │      │
│  │   - Consulta       │              │   - Proxy /api/*      │      │
│  └────────────────────┘              └──────────────────────┘      │
│                                              │                        │
└──────────────────────────────────────────────┼───────────────────────┘
                                                │
                    /api/* →                   │
                    cnab-api:8080              │
                                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      API Container (cnab-api)                        │
│                                                                       │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │              .NET 8 Web API (Port 8080)                     │    │
│  │                                                              │    │
│  │  Endpoints:                                                  │    │
│  │  • POST /api/Files          → Upload de arquivo CNAB       │    │
│  │  • GET /api/Transactions/store/{name} → Consultar          │    │
│  │  • GET /health/liveness     → Health check                  │    │
│  │  • GET /swagger             → Documentação                  │    │
│  └────────────────────────────────────────────────────────────┘    │
│                               │              │                        │
└───────────────────────────────┼──────────────┼───────────────────────┘
                                │              │
                        Save File│              │Query DB
                                │              │
                                ▼              ▼
              ┌──────────────────────┐  ┌──────────────────────┐
              │  Volume: cnab-files  │  │  PostgreSQL (5432)   │
              │  /app/storage        │  │  Database: cnab_db   │
              └──────────────────────┘  └──────────────────────┘
                         ▲                        ▲
                         │                        │
                         │                        │
┌────────────────────────┼────────────────────────┼───────────────────┐
│                 Worker Container (cnab-worker)  │                    │
│                                                  │                    │
│  ┌─────────────────────────────────────────────┴─────────┐         │
│  │        Background Service (Port 8081)                   │         │
│  │                                                          │         │
│  │  • Poll database every 30s                             │         │
│  │  • Process pending CNAB files                          │         │
│  │  • Parse and save transactions                         │         │
│  │  • Update file status                                  │         │
│  └────────────────────────────────────────────────────────┘         │
│                                                                       │
└───────────────────────────────────────────────────────────────────────┘
```

## Fluxo de Upload

```
1. Usuário seleciona arquivo CNAB no browser
   └─→ React component (UploadCnab.tsx)

2. Frontend faz requisição HTTP
   POST http://localhost:3000/api/Files
   └─→ Nginx intercepta /api/*

3. Nginx faz proxy reverso
   POST http://cnab-api:8080/api/Files
   └─→ .NET API recebe a requisição

4. API salva arquivo e cria registro
   └─→ FilesController.UploadAsync()
       └─→ UploadCNABFileHandler
           ├─→ Salvar arquivo em /app/storage
           └─→ Criar registro no PostgreSQL (status: Pending)

5. API retorna resposta (HTTP 202)
   └─→ { fileId, fileName, status: "Pending" }

6. Worker detecta arquivo pendente (polling)
   └─→ CNABFileProcessor
       ├─→ Ler arquivo de /app/storage
       ├─→ Parsear linhas CNAB
       ├─→ Criar transações no banco
       └─→ Atualizar status para "Processed"
```

## Fluxo de Consulta

```
1. Usuário preenche filtros (loja, datas)
   └─→ React component (ConsultaCnab.tsx)

2. Frontend faz requisição HTTP
   GET http://localhost:3000/api/Transactions/store/LOJA?fromDate=...&toDate=...
   └─→ Nginx intercepta /api/*

3. Nginx faz proxy reverso
   GET http://cnab-api:8080/api/Transactions/store/LOJA?...
   └─→ .NET API recebe a requisição

4. API consulta banco de dados
   └─→ TransactionsController.GetByStore()
       └─→ TransactionStatementHandler
           └─→ TransactionRepository.GetByStoreAndDateRange()
               └─→ Query SQL com filtros

5. API retorna transações (HTTP 200)
   └─→ { storeName, totalEntries, totalExits, balance, transactions[] }

6. Frontend exibe resultados
   └─→ Tabela com transações
   └─→ Resumo financeiro
```

## Configuração de Rede

### Docker Compose Network: `cnab-network`

```
┌─────────────────────────────────────────────────────┐
│              cnab-network (bridge)                   │
│                                                      │
│  cnab-frontend:80    ←→  cnab-api:8080             │
│         ↕                     ↕                      │
│  cnab-worker:8081    ←→  postgres:5432              │
│                                                      │
└─────────────────────────────────────────────────────┘
         │              │              │              │
         │              │              │              │
    Host:3000      Host:5000      Host:8081      Host:5432
```

### Mapeamento de Portas

| Container       | Porta Interna | Porta Host | Acessível via        |
|-----------------|---------------|------------|----------------------|
| cnab-frontend   | 80            | 3000       | localhost:3000       |
| cnab-api        | 8080          | 5000       | localhost:5000       |
| cnab-worker     | 8081          | 8081       | localhost:8081       |
| postgres        | 5432          | 5432       | localhost:5432       |

### Comunicação Interna (Docker)

Dentro da rede Docker, os serviços se comunicam pelos **nomes dos containers**:

- Frontend → API: `http://cnab-api:8080`
- Worker → Database: `Host=postgres;Port=5432`
- API → Database: `Host=postgres;Port=5432`

### Comunicação Externa (Browser)

Do navegador, o acesso é feito pelas **portas do host**:

- Frontend: `http://localhost:3000`
- API: `http://localhost:5000`

**Importante**: O Nginx no frontend faz proxy reverso de `/api/*` para `cnab-api:8080`, permitindo que o browser acesse a API através do frontend sem problemas de CORS.

## Segurança

### CORS (Cross-Origin Resource Sharing)

A API está configurada com CORS permissivo para desenvolvimento:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Proxy Reverso Nginx

O Nginx está configurado para fazer proxy das requisições `/api/*`:

```nginx
location /api/ {
    proxy_pass http://cnab-api:8080/api/;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

Isso resolve problemas de CORS, pois o browser vê tudo como originando de `localhost:3000`.

## Monitoramento

### Health Checks

Todos os serviços possuem health checks configurados:

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:PORT/health"]
  interval: 30s
  timeout: 10s
  retries: 3
```

### Logs Centralizados

```bash
# Ver todos os logs
docker compose logs -f

# Ver logs de um serviço específico
docker compose logs -f cnab-api
docker compose logs -f cnab-frontend
docker compose logs -f cnab-worker
```

## Variáveis de Ambiente

### Frontend (.env.docker)
```env
REACT_APP_API_URL=
# Vazio = usa proxy reverso do Nginx
```

### API
```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=cnab_db;Username=cnab_user;Password=cnab_pass123
FileStorage__StoragePath=/app/storage
```

### Worker
```env
DOTNET_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=cnab_db;Username=cnab_user;Password=cnab_pass123
FileStorage__StoragePath=/app/storage
CNABFileProcessor__PoolingIntervalSeconds=30
```

## Volumes Persistentes

### cnab-files
- **Propósito**: Armazenar arquivos CNAB enviados
- **Localização**: `/app/storage` (dentro dos containers API e Worker)
- **Tipo**: Volume Docker local

### postgres-data
- **Propósito**: Persistir dados do PostgreSQL
- **Localização**: `/var/lib/postgresql/data` (dentro do container Postgres)
- **Tipo**: Volume Docker local

**Nota**: Os volumes persistem mesmo após `docker compose down`. Use `docker compose down -v` para removê-los.