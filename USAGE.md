# Guía de Uso - ServiceApp

## Uso del Script de Automatización

El script `create-microservice.sh` permite crear nuevos microservicios con la misma estructura de Clean Architecture de forma automatizada.

### Sintaxis

```bash
./create-microservice.sh <NombreDelMicroservicio>
```

### Ejemplo

```bash
# Crear un nuevo microservicio llamado "PaymentService"
./create-microservice.sh PaymentService
```

Esto creará:
```
PaymentService/
├── PaymentService.sln
├── PaymentService.Domain/
├── PaymentService.Application/
├── PaymentService.Infrastructure/
└── PaymentService.Api/
```

### Después de crear el microservicio

```bash
cd PaymentService
dotnet build
dotnet run --project PaymentService.Api
```

## Ejemplos de Implementación

### 1. Agregar una Nueva Entidad

**Domain/Entities/Message.cs**
```csharp
namespace ServiceApp.Domain.Entities;

public class Message
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsDelivered { get; set; }
}
```

### 2. Crear un Nuevo Repositorio

**Domain/Interfaces/IMessageRepository.cs**
```csharp
namespace ServiceApp.Domain.Interfaces;

public interface IMessageRepository
{
    Task<Message> CreateAsync(Message message);
    Task<IEnumerable<Message>> GetByUserIdAsync(string userId);
}
```

**Infrastructure/Persistence/MessageRepository.cs**
```csharp
public class MessageRepository : IMessageRepository
{
    private readonly MongoDbContext _context;

    public MessageRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Message> CreateAsync(Message message)
    {
        await _context.Messages.InsertOneAsync(message);
        return message;
    }

    public async Task<IEnumerable<Message>> GetByUserIdAsync(string userId)
    {
        return await _context.Messages
            .Find(m => m.UserId == userId)
            .ToListAsync();
    }
}
```

### 3. Crear un Nuevo Servicio

**Application/Services/StatisticsService.cs**
```csharp
public class StatisticsService
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;

    public StatisticsService(
        IUserRepository userRepository,
        IMessageRepository messageRepository)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
    }

    public async Task<UserStatisticsDto> GetUserStatisticsAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var messages = await _messageRepository.GetByUserIdAsync(userId);

        return new UserStatisticsDto
        {
            UserId = userId,
            UserName = user?.Name ?? "Unknown",
            TotalMessages = messages.Count(),
            LastMessageDate = messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt
        };
    }
}
```

### 4. Crear un Nuevo Controlador

**Api/Controllers/StatisticsController.cs**
```csharp
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly StatisticsService _statisticsService;

    public StatisticsController(StatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserStatisticsDto>> GetUserStats(string userId)
    {
        var stats = await _statisticsService.GetUserStatisticsAsync(userId);
        return Ok(stats);
    }
}
```

### 5. Registrar en Program.cs

```csharp
// Agregar al Program.cs
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<StatisticsService>();
```

## Integraciones Recomendadas

### Twilio WhatsApp API

```csharp
public class TwilioWhatsAppService : IWhatsAppService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;

    public async Task<bool> SendMessageAsync(string phoneNumber, string message)
    {
        var client = new TwilioRestClient(_accountSid, _authToken);
        
        var messageResource = await MessageResource.CreateAsync(
            to: new PhoneNumber($"whatsapp:{phoneNumber}"),
            from: new PhoneNumber($"whatsapp:{_fromNumber}"),
            body: message
        );

        return messageResource.Status != MessageResource.StatusEnum.Failed;
    }
}
```

### WhatsApp Business API

```csharp
public class WhatsAppBusinessService : IWhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
    private readonly string _phoneNumberId;

    public async Task<bool> SendMessageAsync(string phoneNumber, string message)
    {
        var request = new
        {
            messaging_product = "whatsapp",
            to = phoneNumber,
            type = "text",
            text = new { body = message }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"https://graph.facebook.com/v18.0/{_phoneNumberId}/messages",
            request
        );

        return response.IsSuccessStatusCode;
    }
}
```

## Testing

### Crear Tests Unitarios

```bash
# Crear proyecto de tests
dotnet new xunit -n ServiceApp.Tests
dotnet sln add ServiceApp.Tests/ServiceApp.Tests.csproj
dotnet add ServiceApp.Tests/ServiceApp.Tests.csproj reference ServiceApp.Application/ServiceApp.Application.csproj
dotnet add ServiceApp.Tests/ServiceApp.Tests.csproj package Moq
```

### Ejemplo de Test

```csharp
public class MessageServiceTests
{
    [Fact]
    public async Task SendMessage_WhenNoUsers_ReturnsFailure()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(r => r.GetUserWithLeastMessagesAsync())
            .ReturnsAsync((User)null);

        var mockWhatsApp = new Mock<IWhatsAppService>();
        var service = new MessageService(mockUserRepo.Object, mockWhatsApp.Object);

        // Act
        var result = await service.SendMessageToUserWithLeastMessagesAsync("Test");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("No users found in database", result.Message);
    }
}
```

## Docker

### Crear Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ServiceApp.Api/ServiceApp.Api.csproj", "ServiceApp.Api/"]
COPY ["ServiceApp.Application/ServiceApp.Application.csproj", "ServiceApp.Application/"]
COPY ["ServiceApp.Domain/ServiceApp.Domain.csproj", "ServiceApp.Domain/"]
COPY ["ServiceApp.Infrastructure/ServiceApp.Infrastructure.csproj", "ServiceApp.Infrastructure/"]
RUN dotnet restore "ServiceApp.Api/ServiceApp.Api.csproj"
COPY . .
WORKDIR "/src/ServiceApp.Api"
RUN dotnet build "ServiceApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceApp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceApp.Api.dll"]
```

### Docker Compose

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - MongoDB__ConnectionString=mongodb://mongo:27017
      - MongoDB__DatabaseName=ServiceAppDb
    depends_on:
      - mongo

  mongo:
    image: mongo:latest
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db

volumes:
  mongo_data:
```

## Variables de Entorno

Para producción, usa variables de entorno en lugar de appsettings.json:

```bash
export MongoDB__ConnectionString="mongodb://user:pass@host:27017"
export MongoDB__DatabaseName="ServiceAppDb"
export WhatsApp__ApiUrl="https://api.whatsapp.com"
export WhatsApp__ApiToken="your-token"
```

## Mejores Prácticas

1. **Siempre usa interfaces** - Facilita testing y cambios futuros
2. **Inyección de dependencias** - Usa el contenedor de DI de .NET
3. **Async/Await** - Todas las operaciones I/O deben ser asíncronas
4. **DTOs** - Nunca expongas entidades de dominio directamente
5. **Validación** - Valida inputs en los controladores
6. **Manejo de errores** - Implementa middleware de manejo de errores global
7. **Logging** - Usa ILogger para registrar eventos importantes
8. **Documentación** - Mantén Swagger actualizado con comentarios XML

## Recursos Adicionales

- [Clean Architecture en .NET](https://docs.microsoft.com/aspnet/core/fundamentals/architecture)
- [MongoDB .NET Driver](https://mongodb.github.io/mongo-csharp-driver/)
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api)
- [Twilio WhatsApp API](https://www.twilio.com/whatsapp)
- [WhatsApp Business API](https://developers.facebook.com/docs/whatsapp)
