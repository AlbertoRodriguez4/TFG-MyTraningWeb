@echo off
:: Script para compilar y subir imagenes de Docker a Docker Hub
:: Guardado en la raiz del proyecto

echo =======================================================
echo    BUILD & PUSH DOCKER IMAGES - THE TRAINING HUB
echo =======================================================
echo.

:: 1. Verificar si docker esta corriendo
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Docker no se esta ejecutando. Abre Docker Desktop y vuelve a intentarlo.
    pause
    exit /b 1
)

:: 2. Autenticacion en Docker Hub
echo [1/4] Comprobando inicio de sesion en Docker Hub...
echo Si ya has iniciado sesion, presiona ENTER. Si no, introduce tus datos:
docker login
if %errorlevel% neq 0 (
    echo [ERROR] Error al iniciar sesion en Docker Hub.
    pause
    exit /b 1
)

:: 3. Definir URL del Backend para el Frontend
echo.
echo [2/4] Configuracion del Frontend:
echo Introduce la URL publica de tu backend en AWS (ejemplo: http://a7364826d.us-east-1.elb.amazonaws.com:6873^)
set /p VITE_URL="URL Backend: "
if "%VITE_URL%"=="" (
    echo [ADVERTENCIA] No has introducido ninguna URL. Se utilizara http://localhost:6873 por defecto.
    set VITE_URL=http://localhost:6873
)

:: 4. Construir y Subir Backend
echo.
echo [3/4] Compilando y subiendo BACKEND...
echo Ejecutando: docker build -t albertorodriguezpenalva/back-aws:latest ./Backend
docker build -t albertorodriguezpenalva/back-aws:latest ./Backend
if %errorlevel% neq 0 (
    echo [ERROR] Error compilando la imagen del Backend.
    pause
    exit /b 1
)

echo Subiendo imagen del Backend a Docker Hub...
docker push albertorodriguezpenalva/back-aws:latest
if %errorlevel% neq 0 (
    echo [ERROR] Error al subir la imagen del Backend.
    pause
    exit /b 1
)

:: 5. Construir y Subir Frontend
echo.
echo [4/4] Compilando y subiendo FRONTEND...
echo Inyectando variable VITE_API_BASE_URL=%VITE_URL%
echo Ejecutando: docker build --build-arg VITE_API_BASE_URL=%VITE_URL% -t albertorodriguezpenalva/front-aws:latest ./Frontend
docker build --build-arg VITE_API_BASE_URL=%VITE_URL% -t albertorodriguezpenalva/front-aws:latest ./Frontend
if %errorlevel% neq 0 (
    echo [ERROR] Error compilando la imagen del Frontend.
    pause
    exit /b 1
)

echo Subiendo imagen del Frontend a Docker Hub...
docker push albertorodriguezpenalva/front-aws:latest
if %errorlevel% neq 0 (
    echo [ERROR] Error al subir la imagen del Frontend.
    pause
    exit /b 1
)

echo.
echo =======================================================
echo    PROCESO COMPLETADO CON EXITO
echo =======================================================
echo Las imagenes se han compilado y subido correctamente:
echo  - albertorodriguezpenalva/back-aws:latest
echo  - albertorodriguezpenalva/front-aws:latest
echo.
pause
