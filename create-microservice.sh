#!/bin/bash

# Script para crear estructura de microservicio con Clean Architecture
# Uso: ./create-microservice.sh <NombreDelMicroservicio>

if [ -z "$1" ]; then
    echo "Error: Debe proporcionar un nombre para el microservicio"
    echo "Uso: ./create-microservice.sh <NombreDelMicroservicio>"
    exit 1
fi

SERVICE_NAME=$1
BASE_DIR=$(pwd)
SERVICE_DIR="${BASE_DIR}/${SERVICE_NAME}"

echo "=========================================="
echo "Creando microservicio: ${SERVICE_NAME}"
echo "=========================================="

# Crear directorio del servicio
mkdir -p "${SERVICE_DIR}"
cd "${SERVICE_DIR}"

# Crear solución
echo "Creando solución..."
dotnet new sln -n "${SERVICE_NAME}"

# Crear proyectos
echo "Creando proyecto Domain..."
dotnet new classlib -n "${SERVICE_NAME}.Domain" -f net8.0

echo "Creando proyecto Application..."
dotnet new classlib -n "${SERVICE_NAME}.Application" -f net8.0

echo "Creando proyecto Infrastructure..."
dotnet new classlib -n "${SERVICE_NAME}.Infrastructure" -f net8.0

echo "Creando proyecto API..."
dotnet new webapi -n "${SERVICE_NAME}.Api" -f net8.0

# Agregar proyectos a la solución
echo "Agregando proyectos a la solución..."
dotnet sln add "${SERVICE_NAME}.Domain/${SERVICE_NAME}.Domain.csproj"
dotnet sln add "${SERVICE_NAME}.Application/${SERVICE_NAME}.Application.csproj"
dotnet sln add "${SERVICE_NAME}.Infrastructure/${SERVICE_NAME}.Infrastructure.csproj"
dotnet sln add "${SERVICE_NAME}.Api/${SERVICE_NAME}.Api.csproj"

# Configurar referencias entre proyectos
echo "Configurando referencias entre proyectos..."
dotnet add "${SERVICE_NAME}.Application/${SERVICE_NAME}.Application.csproj" reference "${SERVICE_NAME}.Domain/${SERVICE_NAME}.Domain.csproj"
dotnet add "${SERVICE_NAME}.Infrastructure/${SERVICE_NAME}.Infrastructure.csproj" reference "${SERVICE_NAME}.Domain/${SERVICE_NAME}.Domain.csproj"
dotnet add "${SERVICE_NAME}.Infrastructure/${SERVICE_NAME}.Infrastructure.csproj" reference "${SERVICE_NAME}.Application/${SERVICE_NAME}.Application.csproj"
dotnet add "${SERVICE_NAME}.Api/${SERVICE_NAME}.Api.csproj" reference "${SERVICE_NAME}.Application/${SERVICE_NAME}.Application.csproj"
dotnet add "${SERVICE_NAME}.Api/${SERVICE_NAME}.Api.csproj" reference "${SERVICE_NAME}.Infrastructure/${SERVICE_NAME}.Infrastructure.csproj"

# Eliminar archivos Class1.cs por defecto
echo "Limpiando archivos por defecto..."
rm -f "${SERVICE_NAME}.Domain/Class1.cs"
rm -f "${SERVICE_NAME}.Application/Class1.cs"
rm -f "${SERVICE_NAME}.Infrastructure/Class1.cs"

# Crear estructura de carpetas
echo "Creando estructura de carpetas..."
mkdir -p "${SERVICE_NAME}.Domain/Entities"
mkdir -p "${SERVICE_NAME}.Domain/Interfaces"
mkdir -p "${SERVICE_NAME}.Application/Services"
mkdir -p "${SERVICE_NAME}.Application/DTOs"
mkdir -p "${SERVICE_NAME}.Infrastructure/Persistence"
mkdir -p "${SERVICE_NAME}.Infrastructure/Services"
mkdir -p "${SERVICE_NAME}.Api/Controllers"

# Agregar paquetes NuGet comunes
echo "Agregando paquetes NuGet..."
dotnet add "${SERVICE_NAME}.Infrastructure/${SERVICE_NAME}.Infrastructure.csproj" package MongoDB.Driver

echo "=========================================="
echo "Microservicio creado exitosamente!"
echo "Ubicación: ${SERVICE_DIR}"
echo "=========================================="
echo ""
echo "Próximos pasos:"
echo "1. cd ${SERVICE_NAME}"
echo "2. Editar appsettings.json en ${SERVICE_NAME}.Api"
echo "3. Crear tus entidades en ${SERVICE_NAME}.Domain/Entities"
echo "4. Crear tus interfaces en ${SERVICE_NAME}.Domain/Interfaces"
echo "5. Implementar tus servicios en ${SERVICE_NAME}.Application/Services"
echo "6. Implementar repositorios en ${SERVICE_NAME}.Infrastructure/Persistence"
echo "7. Crear controladores en ${SERVICE_NAME}.Api/Controllers"
echo "8. dotnet build"
echo "9. dotnet run --project ${SERVICE_NAME}.Api"
echo ""
