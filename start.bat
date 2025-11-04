@echo off
REM Script para iniciar o sistema CNAB completo no Windows
REM Frontend + API + Worker + PostgreSQL

echo ========================================
echo   Sistema CNAB - Inicializacao
echo ========================================
echo.

REM Verificar se Docker esta instalado
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ERRO] Docker nao encontrado. Por favor, instale o Docker Desktop primeiro.
    pause
    exit /b 1
)

echo [OK] Docker encontrado
echo.

REM Parar containers anteriores se existirem
echo [INFO] Parando containers anteriores...
docker compose down
echo.

REM Buildar imagens
echo [INFO] Buildando imagens Docker...
docker compose build --no-cache
echo.

REM Iniciar containers
echo [INFO] Iniciando containers...
docker compose up -d
echo.

REM Aguardar inicializacao
echo [INFO] Aguardando inicializacao dos servicos...
timeout /t 10 /nobreak >nul
echo.

REM Mostrar status
echo [INFO] Status dos servicos:
docker compose ps
echo.

echo ========================================
echo   Sistema iniciado com sucesso!
echo ========================================
echo.
echo URLs disponiveis:
echo   Frontend:  http://localhost:3000
echo   API:       http://localhost:5000
echo   Swagger:   http://localhost:5000/swagger
echo   Postgres:  localhost:5432
echo.
echo Para ver os logs:
echo   docker compose logs -f
echo.
echo Para parar o sistema:
echo   docker compose down
echo.
pause
