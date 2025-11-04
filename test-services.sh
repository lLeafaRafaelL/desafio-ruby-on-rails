#!/bin/bash

echo "========================================="
echo "  Sistema CNAB - Verificação de Saúde"
echo "========================================="
echo ""

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

check_service() {
    local service_name=$1
    local url=$2
    local description=$3
    
    echo -n "Testando $description... "
    
    if curl -s -o /dev/null -w "%{http_code}" "$url" | grep -q "200\|202"; then
        echo -e "${GREEN}✓ OK${NC}"
        return 0
    else
        echo -e "${RED}✗ FALHOU${NC}"
        return 1
    fi
}

echo "Verificando serviços Docker..."
echo ""

# Verificar se os containers estão rodando
if ! docker compose ps | grep -q "Up"; then
    echo -e "${RED}Erro: Serviços não estão rodando!${NC}"
    echo "Execute: docker compose up -d"
    exit 1
fi

echo -e "${GREEN}✓ Containers estão rodando${NC}"
echo ""

# Aguardar alguns segundos para garantir que os serviços estejam prontos
echo "Aguardando serviços iniciarem (10s)..."
sleep 10
echo ""

# Verificar cada serviço
check_service "postgres" "http://localhost:5432" "PostgreSQL"
check_service "api" "http://localhost:5000/health/liveness" "API Backend"
check_service "frontend" "http://localhost:3000/health" "Frontend React"
check_service "worker" "http://localhost:8081/metrics" "Worker"

echo ""
echo "========================================="
echo "  Resumo"
echo "========================================="
echo ""
echo "Frontend:   http://localhost:3000"
echo "API Swagger: http://localhost:5000"
echo "Worker Metrics: http://localhost:8081/metrics"
echo ""
echo "Para ver logs:"
echo "  docker compose logs -f"
echo ""
echo "Para testar upload:"
echo "  curl -X POST http://localhost:5000/api/Files -F \"file=@cnab-react/exemplo-cnab.txt\""
echo ""
