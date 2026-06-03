# Insertar Usuarios de Prueba en MongoDB

## Opción 1: Usando MongoDB Shell (mongosh)

### Ejecutar el script directamente:
```bash
mongosh mongodb://localhost:27017/ServiceAppDb insert-users.js
```

### O manualmente en MongoDB Shell:
```bash
mongosh mongodb://localhost:27017
```

Luego ejecuta:
```javascript
use ServiceAppDb

db.users.insertMany([
  {
    Id: "6747a1b5c8e4f12345678901",
    Name: "María González",
    PhoneNumber: "+573001234567",
    MessageCount: 3,
    CreatedAt: new Date("2026-05-15T10:30:00Z"),
    LastMessageAt: new Date("2026-05-28T14:20:00Z")
  },
  {
    Id: "6747a1b5c8e4f12345678902",
    Name: "Carlos Rodríguez",
    PhoneNumber: "+573007654321",
    MessageCount: 1,
    CreatedAt: new Date("2026-05-20T08:15:00Z"),
    LastMessageAt: new Date("2026-05-25T16:45:00Z")
  }
])

// Verificar
db.users.find().pretty()
```

## Opción 2: Usando mongoimport

```bash
mongoimport --uri="mongodb://localhost:27017/ServiceAppDb" --collection=users --file=seed-data.json --jsonArray
```

## Opción 3: Usando MongoDB Compass

1. Abre MongoDB Compass
2. Conéctate a `mongodb://localhost:27017`
3. Selecciona la base de datos `ServiceAppDb`
4. Crea o selecciona la colección `users`
5. Click en "ADD DATA" → "Import File"
6. Selecciona `seed-data.json`

## Opción 4: Usando curl con la API (después de ejecutar la API)

```bash
# Primero asegúrate de tener MongoDB corriendo
# Luego ejecuta la API:
# dotnet run --project ServiceApp.Api

# Los usuarios deben insertarse directamente en MongoDB
# La API solo consulta y actualiza usuarios existentes
```

## Verificar Usuarios Insertados

### Desde MongoDB Shell:
```javascript
use ServiceAppDb
db.users.find().pretty()
```

### Desde C# (agregar en Program.cs para seed automático):
```csharp
// Al final de Program.cs, antes de app.Run()
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    
    if (!await context.Users.Find(_ => true).AnyAsync())
    {
        var users = new[]
        {
            new User
            {
                Id = "6747a1b5c8e4f12345678901",
                Name = "María González",
                PhoneNumber = "+573001234567",
                MessageCount = 3,
                CreatedAt = DateTime.Parse("2026-05-15T10:30:00Z"),
                LastMessageAt = DateTime.Parse("2026-05-28T14:20:00Z")
            },
            new User
            {
                Id = "6747a1b5c8e4f12345678902",
                Name = "Carlos Rodríguez",
                PhoneNumber = "+573007654321",
                MessageCount = 1,
                CreatedAt = DateTime.Parse("2026-05-20T08:15:00Z"),
                LastMessageAt = DateTime.Parse("2026-05-25T16:45:00Z")
            }
        };
        
        await context.Users.InsertManyAsync(users);
        Console.WriteLine("Usuarios de prueba insertados");
    }
}
```

## Datos de los Usuarios

### Usuario 1: María González
- **ID**: 6747a1b5c8e4f12345678901
- **Nombre**: María González
- **Teléfono**: +573001234567
- **Mensajes enviados**: 3
- **Fecha creación**: 2026-05-15
- **Último mensaje**: 2026-05-28

### Usuario 2: Carlos Rodríguez
- **ID**: 6747a1b5c8e4f12345678902
- **Nombre**: Carlos Rodríguez
- **Teléfono**: +573007654321
- **Mensajes enviados**: 1 ⭐ (tiene menos mensajes, será seleccionado primero)
- **Fecha creación**: 2026-05-20
- **Último mensaje**: 2026-05-25

## Probar la API

Una vez insertados los usuarios:

```bash
# Enviar un mensaje (irá a Carlos porque tiene 1 mensaje, menos que María con 3)
curl -X POST https://localhost:5001/api/message/send \
  -H "Content-Type: application/json" \
  -d '{"message":"Hola! Este es un mensaje de prueba"}' \
  -k

# Respuesta esperada:
# {
#   "success": true,
#   "message": "Message sent successfully",
#   "userId": "6747a1b5c8e4f12345678902",
#   "userName": "Carlos Rodríguez",
#   "phoneNumber": "+573007654321"
# }
```

Después de enviar, Carlos tendrá 2 mensajes y María seguirá con 3.
El próximo mensaje irá a Carlos de nuevo (2 < 3).
Cuando ambos tengan la misma cantidad, se enviará al primero que encuentre.
