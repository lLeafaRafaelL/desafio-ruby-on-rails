# Guia de Início Rápido - Sistema CNAB

## Opção 1: Executar com Docker (Mais Fácil)

### Linux/Mac:
```bash
./start.sh
```

### Windows:
```bash
start.bat
```

Ou manualmente:
```bash
docker compose up --build
```

### Acessar:
- **Frontend**: http://localhost:3000
- **API/Swagger**: http://localhost:5000/swagger
- **API Health**: http://localhost:5000/health/liveness

---

## Opção 2: Desenvolvimento Local

### Backend (.NET)

```bash
# Terminal 1 - API
cd ByCoders.CNAB.API
dotnet run

# Terminal 2 - Worker
cd ByCoders.CNAB.Worker
dotnet run
```

**Pré-requisitos:**
- .NET 8.0 SDK
- PostgreSQL rodando (ou via Docker: `docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=cnab_pass123 -e POSTGRES_USER=cnab_user -e POSTGRES_DB=cnab_db postgres:16-alpine`)

### 2️⃣ Frontend (React)

```bash
cd cnab-react

# Instalar dependências (primeira vez)
npm install

# Criar arquivo .env
echo "REACT_APP_API_URL=http://localhost:5000" > .env

# Executar
npm start
```

**Pré-requisitos:**
- Node.js 18+
- npm ou yarn

---

## Testar a Aplicação

### 1. Upload de Arquivo CNAB

Use o arquivo de exemplo `cnab-react/exemplo-cnab.txt` ou crie um com o formato:

```
3201903010000014200096206760174753****3153153453JOSE COSTA      MERCEARIA 3 IRMÃOS
```

### 2. Consultar Transações

- Selecione uma loja (exemplo: "MERCEARIA 3 IRMÃOS")
- Escolha um intervalo de datas (máximo 24 horas)
- Clique em "Consultar"

---

## Parar o Sistema

### Docker:
```bash
docker compose down
```

### Local:
`Ctrl+C` em cada terminal

---

## Monitoramento

- **Logs Docker**: `docker compose logs -f`
- **Logs API**: Terminal onde rodou `dotnet run`
- **Status Containers**: `docker compose ps`

---

## Problemas Comuns

### Porta já em uso
```bash
# Verificar processos usando as portas
# Linux/Mac
lsof -i :3000
lsof -i :5000

# Windows
netstat -ano | findstr :3000
netstat -ano | findstr :5000
```

### Erro de conexão com banco
```bash
# Verificar se PostgreSQL está rodando
docker compose ps postgres

# Ver logs do banco
docker compose logs postgres
```

### Frontend não conecta com API
1. Verifique se a API está rodando: `curl http://localhost:5000/health/liveness`
2. Verifique o arquivo `.env` do frontend
3. Verifique o console do navegador (F12)

---

## Endpoints Principais

### Upload
```bash
curl -X POST http://localhost:5000/api/Files \
  -F "file=@exemplo-cnab.txt"
```

### Consulta
```bash
curl "http://localhost:5000/api/Transactions/store/MERCEARIA%203%20IRMÃOS?fromDate=2019-03-01T00:00:00Z&toDate=2019-03-02T00:00:00Z"
```

---

Para documentação completa, veja [README.md](README.md)
