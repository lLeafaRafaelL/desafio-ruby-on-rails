#!/bin/bash

# Script para iniciar o sistema CNAB completo
# Frontend + API + Worker + PostgreSQL

echo "Iniciando Sistema CNAB..."
echo ""

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "Docker não encontrado. Por favor, instale o Docker primeiro."
    exit 1
fi

# Verificar se Docker Compose está instalado
if ! command -v docker compose &> /dev/null; then
    echo "Docker Compose não encontrado. Por favor, instale o Docker Compose primeiro."
    exit 1
fi

echo "Docker e Docker Compose encontrados"
echo ""

# Parar containers anteriores se existirem
echo "Parando containers anteriores..."
docker compose down

echo ""
echo "Buildando imagens Docker..."
docker compose build --no-cache

echo ""
echo "Iniciando containers..."
docker compose up -d

echo ""
echo "Aguardando inicialização dos serviços..."
sleep 10

# Verificar status dos containers
echo ""
echo "Status dos serviços:"
docker compose ps

echo ""
echo "Sistema iniciado com sucesso!"
echo ""
echo "   URLs disponíveis:"
echo "   Frontend:  http://localhost:3000"
echo "   API:       http://localhost:5000"
echo "   Swagger:   http://localhost:5000/swagger"
echo "   Postgres:  localhost:5432"
echo ""
echo "Para ver os logs:"
echo "   docker compose logs -f"
echo ""
echo "Para parar o sistema:"
echo "   docker compose down"
echo ""
