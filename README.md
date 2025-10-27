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

🏗️ Arquitetura do Projeto
Este projeto implementa um sistema de processamento de arquivos CNAB (Centro Nacional de Automação Bancária) utilizando Clean Architecture e Domain-Driven Design (DDD).

Estrutura de Camadas
┌─────────────────────────────────────────┐
│         ByCoders.CNAB.API               │  ← Camada de Apresentação
│         ByCoders.CNAB.Worker            │  ← Background Service
├─────────────────────────────────────────┤
│      ByCoders.CNAB.Application          │  ← Camada de Aplicação
├─────────────────────────────────────────┤
│        ByCoders.CNAB.Domain             │  ← Camada de Domínio
├─────────────────────────────────────────┤
│     ByCoders.CNAB.Infrastructure        │  ← Camada de Infraestrutura
├─────────────────────────────────────────┤
│         ByCoders.CNAB.Core              │  ← Camada Base
└─────────────────────────────────────────┘
Componentes
API (REST): Endpoints para upload de arquivos e consulta de transações
Worker: Serviço em background que processa arquivos CNAB de forma assíncrona
PostgreSQL: Banco de dados relacional para persistência
Docker Compose: Orquestração de containers
🚀 Como Funciona
1. Upload de Arquivo CNAB
Cliente → POST /api/files → API → Salva arquivo → Retorna 202 Accepted
                                        ↓
                                   Arquivo pendente
                                        ↓
                              Worker (background)
                                        ↓
                    ┌──────────────────────────────┐
                    │ 1. Lê arquivo linha por linha│
                    │ 2. Parser (80 caracteres)    │
                    │ 3. Valida dados              │
                    │ 4. Cria transações           │
                    │ 5. Bulk insert no DB         │
                    └──────────────────────────────┘

2. Consulta de Transações
Cliente → GET /api/transactions/store/{storeName}?fromDate=...&toDate=...
            ↓
       API consulta DB
            ↓
       Retorna JSON com:
       - Lista de transações
       - Saldo acumulado (entradas - saídas)
       - Total de transações
🔧 Como Configurar e Executar
Pré-requisitos
Docker & Docker Compose
.NET 9.0 SDK (apenas para desenvolvimento local)
Opção 1: Usando Docker Compose (Recomendado)
# 1. Clone o repositório
git clone <repository-url>
cd desafio-ruby-on-rails

# 2. Build das imagens
docker-compose build

# 3. Iniciar os serviços
docker-compose up -d

# 4. Verificar status
docker-compose ps

# 5. Acompanhar logs
docker-compose logs -f
Opção 2: Usando Script de Automação
# Build
./run.sh docker-build

# Start
./run.sh docker-up

# Logs
./run.sh docker-logs

# Stop
./run.sh docker-down
Endpoints Disponíveis
Após iniciar os containers:

API REST: http://localhost:5000
Swagger UI: http://localhost:5000/swagger
Health Check (Liveness): http://localhost:5000/health/liveness
Health Check (Readiness): http://localhost:5000/health/readiness
Métricas Prometheus: http://localhost:5000/metrics
PostgreSQL: localhost:5432
🧪 Como Testar
1. Upload de Arquivo CNAB
Via cURL
curl -X POST "http://localhost:5000/api/files" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@CNAB.txt"
Resposta Esperada (202 Accepted)
{
  "fileId": "01938e6f-7890-7abc-b123-456789abcdef",
  "fileName": "CNAB.txt",
  "status": 1
}
Status:

0 = Pending (aguardando processamento)
1 = Processing (sendo processado)
2 = Processed (concluído)
3 = Failed (falha)
2. Aguardar Processamento
O Worker processa arquivos a cada 30 segundos. Acompanhe os logs:

docker-compose logs -f cnab-worker
3. Consultar Transações por Loja
Via cURL
curl -X GET "http://localhost:5000/api/transactions/store/BAR%20DO%20JO%C3%83O?fromDate=2019-03-01T00:00:00Z&toDate=2019-03-31T23:59:59Z"
Resposta Esperada (200 OK)
{
  "startDate": "2019-03-01T00:00:00+00:00",
  "endDate": "2019-03-31T23:59:59+00:00",
  "totalTrsanctions": 5,
  "accumulatedValue": 125.00,
  "transactions": [
    {
      "id": "01938e6f-1111-7abc-b123-456789abcdef",
      "createdOn": "2025-10-27T10:30:00+00:00",
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
4. Usando Postman
Importe a collection CNAB-API.postman_collection.json (veja seção abaixo) para testar todos os endpoints com exemplos prontos.

📊 Monitoramento
Health Checks
# Liveness (serviço está rodando?)
curl http://localhost:5000/health/liveness

# Readiness (serviço está pronto para receber requisições?)
curl http://localhost:5000/health/readiness
Métricas Prometheus
curl http://localhost:5000/metrics
Logs Estruturados (Serilog)
# Ver logs da API
docker-compose logs cnab-api

# Ver logs do Worker
docker-compose logs cnab-worker

# Ver logs do PostgreSQL
docker-compose logs postgres
🗄️ Acesso ao Banco de Dados
# Conectar via psql
docker exec -it cnab-postgres psql -U cnab_user -d cnab_db

# Comandos úteis
\dt                          # Listar tabelas
SELECT * FROM cnabfiles;     # Ver arquivos
SELECT * FROM transactions;  # Ver transações
SELECT * FROM transactiontypes; # Ver tipos
🛠️ Desenvolvimento Local
Executar Testes Unitários
# Via script
./run.sh test

# Ou diretamente
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true
Restaurar Dependências
./run.sh restore
# ou
dotnet restore
Build da Solution
./run.sh build
# ou
dotnet build
Limpar Artifacts
./run.sh clean
🐛 Troubleshooting
Container não inicia
# Verificar logs
docker-compose logs

# Rebuild forçado
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
Banco de dados não conecta
# Verificar se o PostgreSQL está saudável
docker-compose ps

# Testar conexão manual
docker exec -it cnab-postgres pg_isready -U cnab_user
Worker não processa arquivos
# Verificar se há arquivos pendentes no banco
docker exec -it cnab-postgres psql -U cnab_user -d cnab_db -c "SELECT * FROM cnabfiles WHERE processedon IS NULL;"

# Verificar logs do worker
docker-compose logs -f cnab-worker
📦 Estrutura de Diretórios
desafio-ruby-on-rails/
├── ByCoders.CNAB.API/           # REST API
├── ByCoders.CNAB.Worker/        # Background Service
├── ByCoders.CNAB.Application/   # Casos de Uso
├── ByCoders.CNAB.Domain/        # Entidades e Regras de Negócio
├── ByCoders.CNAB.Infrastructure/# Repositórios e DbContext
├── ByCoders.CNAB.Core/          # Padrões Base
├── ByCoders.CNAB.UnitTests/     # Testes Unitários
├── docker-compose.yml           # Orquestração
├── Dockerfile.API               # Build da API
├── Dockerfile.Worker            # Build do Worker
├── run.sh                       # Script de automação
└── CNAB.txt                     # Arquivo de exemplo
📮 Postman Collection

Utilize o arquivo chamado postman_collection.json:

📝 Como Usar a Collection do Postman
1. Importar a Collection
Abra o Postman
Clique em Import
Selecione o arquivo CNAB-API.postman_collection.json
A collection será importada com todos os endpoints
2. Configurar Environment (Opcional)
A collection já vem com a variável baseUrl configurada como http://localhost:5000. Se precisar alterar:

Clique no ícone de "olho" no canto superior direito
Edite a variável baseUrl
3. Ordem de Testes Recomendada
Health Checks → Verificar se a API está rodando
Upload CNAB File → Fazer upload do arquivo CNAB.txt
Aguardar ~30 segundos → Worker processa o arquivo
Get Transactions by Store → Consultar transações processadas
4. Testes Automatizados
Cada requisição tem scripts de teste que verificam:

Status code correto
Estrutura da resposta
Tipos de dados
Regras de negócio
Execute a collection completa clicando em Run para ver todos os testes passarem.