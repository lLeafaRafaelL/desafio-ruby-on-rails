#!/bin/bash


set -e

echo "🚀 CNAB System - Automation Script"
echo "===================================="

# Função de help
show_help() {
    echo ""
    echo "Uso: ./run.sh [comando]"
    echo ""
    echo "Comandos disponíveis:"
    echo "  docker-build    - Build das imagens Docker"
    echo "  docker-up       - Start dos containers"
    echo "  docker-down     - Stop dos containers"
    echo "  docker-logs     - Ver logs dos containers"
    echo "  migrate         - Executar migrations"
    echo "  test            - Executar testes"
    echo "  test-coverage   - Executar testes com cobertura"
    echo "  add-projects    - Adicionar projetos à solution"
    echo "  restore         - Restaurar dependências"
    echo "  build           - Build da solution"
    echo "  clean           - Limpar build artifacts"
    echo "  help            - Mostrar esta mensagem"
    echo ""
}

# Docker Build
docker_build() {
    echo "📦 Building Docker images..."
    docker-compose build
    echo "✅ Build completed!"
}

# Docker Up
docker_up() {
    echo "🐳 Starting containers..."
    docker-compose up -d
    echo ""
    echo "✅ Containers started!"
    echo ""
    echo "📍 Endpoints:"
    echo "   API: http://localhost:5000"
    echo "   Swagger: http://localhost:5000/swagger"
    echo "   PostgreSQL: localhost:5432"
    echo ""
    echo "📊 Check status: docker-compose ps"
    echo "📋 View logs: docker-compose logs -f"
}

# Docker Down
docker_down() {
    echo "Stopping containers..."
    docker-compose down
    echo "Containers stopped!"
}

# Docker Logs
docker_logs() {
    echo "Showing logs (Ctrl+C to exit)..."
    docker-compose logs -f
}

# Migrations
run_migrations() {
    echo "🗄️  Running migrations..."
    cd ByCoders.CNAB.Infrastructure
    
    echo "Creating CNABFile migration..."
    dotnet ef migrations add AddCNABFileAggregate --context CNABFileDbContext
    
    echo "Applying CNABFile migration..."
    dotnet ef database update --context CNABFileDbContext
    
    echo "Creating Transaction migration..."
    dotnet ef migrations add AddTransactionAggregate --context TransactionDbContext
    
    echo "Applying Transaction migration..."
    dotnet ef database update --context TransactionDbContext
    
    cd ..
    echo "Migrations completed!"
}

# Tests
run_tests() {
    echo "Running tests..."
    dotnet test
}

# Test Coverage
run_test_coverage() {
    echo "Running tests with coverage..."
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
}

# Add Projects
add_projects() {
    echo "Adding projects to solution..."
    dotnet sln add ByCoders.CNAB.API/ByCoders.CNAB.API.csproj
    dotnet sln add ByCoders.CNAB.Worker/ByCoders.CNAB.Worker.csproj
    echo "Projects added!"
}

# Restore
restore_deps() {
    echo "Restoring dependencies..."
    dotnet restore
    echo "Dependencies restored!"
}

# Build
build_solution() {
    echo "Building solution..."
    dotnet build
    echo "Build completed!"
}

# Clean
clean_build() {
    echo "Cleaning build artifacts..."
    dotnet clean
    find . -type d -name "bin" -o -name "obj" | xargs rm -rf
    echo "Clean completed!"
}

# Main
case "$1" in
    docker-build)
        docker_build
        ;;
    docker-up)
        docker_up
        ;;
    docker-down)
        docker_down
        ;;
    docker-logs)
        docker_logs
        ;;
    migrate)
        run_migrations
        ;;
    test)
        run_tests
        ;;
    test-coverage)
        run_test_coverage
        ;;
    add-projects)
        add_projects
        ;;
    restore)
        restore_deps
        ;;
    build)
        build_solution
        ;;
    clean)
        clean_build
        ;;
    help|*)
        show_help
        ;;
esac
