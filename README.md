# ServiceApp - Microservicio con Clean Architecture

Este proyecto es un microservicio construido con .NET 8.0 siguiendo los principios de Clean Architecture. El servicio permite enviar mensajes de WhatsApp al usuario que tiene menos mensajes registrados en una base de datos MongoDB.

## Estructura del Proyecto

```
ServiceApp/
├── ServiceApp.Domain/          # Capa de Dominio (sin dependencias)
│   ├── Entities/              # Entidades del dominio
│   └── Interfaces/            # Interfaces/contratos
├── ServiceApp.Application/     # Capa de Aplicación (lógica de negocio)
│   ├── Services/              # Servicios de aplicación
│   └── DTOs/                  # Data Transfer Objects
├── ServiceApp.Infrastructure/  # Capa de Infraestructura (implementaciones)
│   ├── Persistence/           # Repositorios y contexto de BD
│   └── Services/              # Implementaciones de servicios externos
└── ServiceApp.Api/            # Capa de Presentación (Web API)
    └── Controllers/           # Controladores REST
```

## Arquitectura Clean Architecture

Este proyecto sigue el patrón de Clean Architecture con las siguientes dependencias:

```
Domain (núcleo, sin dependencias)
   ↑
Application (depende de Domain)
   ↑
Infrastructure (depende de Domain y Application)
   ↑
API (depende de Application e Infrastructure)
```

## CaracterísticasUsersGirlAgency

- **MongoDB** - Base de datos NoSQL para almacenar usuarios
- **WhatsApp API** - Integración para envío de mensajes
- **Clean Architecture** - Separación clara de responsabilidades
- **Swagger/OpenAPI** - Documentación automática de la API
- **.NET 8.0** - Framework moderno y de alto rendimiento

## Requisitos Previos

- .NET 8.0 SDK
- MongoDB (local o remoto)
- Cuenta de WhatsApp Business API (opcional para testing)

## Configuración

### 1. Configurar MongoDB

Edita `ServiceApp.Api/appsettings.json`:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ServiceAppDb"
  }
}
```

### 2. Configurar WhatsApp API

Edita `ServiceApp.Api/appsettings.json`:

```json
{
  "WhatsApp": {
    "ApiUrl": "https://api.whatsapp.example.com",
    "ApiToken": "tu-token-aqui"
  }
}
```

**Nota:** La implementación actual de WhatsApp es un ejemplo. Debes adaptarla según tu proveedor (Twilio, WhatsApp Business API, etc.)

## Instalación y Ejecución

### Compilar el proyecto

```bash
cd /home/john-p-inilla/Documentos/developement/ZcechGirlAgency/serviceApp
dotnet build
```

### Ejecutar la API

```bash
dotnet run --project ServiceApp.Api
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Uso de la API

### Endpoint: Enviar Mensaje

**POST** `/api/message/send`

**Request Body:**
```json
{
  "message": "Hola! Este es un mensaje de prueba"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Message sent successfully",
  "userId": "507f1f77bcf86cd799439011",
  "userName": "Juan Pérez",
  "phoneNumber": "+57300123456"
}
```

### Ejemplo con curl

```bash
curl -X POST https://localhost:5001/api/message/send \
  -H "Content-Type: application/json" \
  -d '{"message":"Hola desde la API!"}'
```

## Crear Nuevos Microservicios

Incluye un script de automatización para crear nuevos microservicios con la misma estructura:

```bash
./create-microservice.sh NuevoMicroservicio
```

Este script creará automáticamente:
- Solución .NET
- 4 proyectos con Clean Architecture
- Referencias correctas entre proyectos
- Estructura de carpetas
- Paquetes NuGet básicos

## Datos de Prueba MongoDB

Para probar la API, necesitas insertar usuarios en MongoDB:

```javascript
// Conectarse a MongoDB
use ServiceAppDb

// Insertar usuarios de prueba
db.users.insertMany([
  {
    "Name": "Juan Pérez",
    "PhoneNumber": "+57300123456",
    "MessageCount": 5,
    "Status" false,
    "CreatedAt": new Date(),
    "LastMessageAt": new Date()
  },
  {
    "Name": "María García",
    "PhoneNumber": "+57300654321",
    "MessageCount": 2,
    "Status" true,
    "CreatedAt": new Date(),
    "LastMessageAt": new Date()
  },
  {
    "Name": "Carlos López",
    "PhoneNumber": "+57300789012",
    "MessageCount": 8,
    "Status" true,
    "CreatedAt": new Date(),
    "LastMessageAt": new Date()
  }
])
```

## Próximos Pasos

1. **Implementar WhatsApp real** - Adaptar `WhatsAppService` según tu proveedor
2. **Autenticación** - Agregar JWT o API Keys
3. **Logging** - Implementar Serilog o similar
4. **Testing** - Agregar pruebas unitarias e integración
5. **Docker** - Crear Dockerfile para containerización
6. **CI/CD** - Configurar pipelines de despliegue

## Estructura de Servicios

### MessageService
Servicio principal que coordina la lógica de negocio:
- Busca el usuario con menos mensajes
- Envía el mensaje de WhatsApp
- Actualiza el contador de mensajes

### UserRepository
Repositorio para operaciones con usuarios en MongoDB:
- `GetUserWithLeastMessagesAsync()` - Obtiene usuario con menos mensajes
- `GetAllUsersAsync()` - Lista todos los usuarios
- `GetByIdAsync()` - Busca por ID
- `UpdateAsync()` - Actualiza un usuario

### WhatsAppService
Servicio para integración con WhatsApp API:
- `SendMessageAsync()` - Envía mensaje a un número de teléfono

## Tecnologías Utilizadas

- **.NET 8.0** - Framework
- **ASP.NET Core** - Web API
- **MongoDB.Driver** - Cliente de MongoDB
- **Swagger/Swashbuckle** - Documentación API
- **Clean Architecture** - Patrón de arquitectura

## Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

## Soporte

Para preguntas o problemas, por favor crea un issue en el repositorio.
