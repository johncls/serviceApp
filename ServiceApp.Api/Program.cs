using ServiceApp.Application.Services;
using ServiceApp.Domain.Interfaces;
using ServiceApp.Infrastructure.Persistence;
using ServiceApp.Infrastructure.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// MongoDB Configuration
var mongoConnectionString = builder.Configuration.GetSection("MongoDB:ConnectionString").Value
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration.GetSection("MongoDB:DatabaseName").Value
    ?? "UsersGirlAgency";
var collectionUserLogin = builder.Configuration.GetSection("MongoDB:CollectionUserLogin").Value
    ?? "UserLogin";
var collectionUser = builder.Configuration.GetSection("MongoDB:CollectionUser").Value
    ?? "Users";

builder.Services.AddSingleton(new MongoDbContext(mongoConnectionString, mongoDatabaseName, collectionUserLogin, collectionUser));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserLoginRepository, UserLoginRepository>();

// WhatsApp Configuration
var whatsAppApiUrl = builder.Configuration.GetSection("WhatsApp:ApiUrl").Value ?? "";
var whatsAppApiToken = builder.Configuration.GetSection("WhatsApp:ApiToken").Value ?? "";

builder.Services.AddHttpClient();
builder.Services.AddLogging();
builder.Services.AddScoped<IWhatsAppService>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    var logger = sp.GetRequiredService<ILogger<WhatsAppService>>();
    var userRepository = sp.GetRequiredService<IUserRepository>();
    return new WhatsAppService(whatsAppApiUrl, whatsAppApiToken, httpClient, logger, userRepository);
});

// Application Services
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserLoginService>();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Pega el token: Bearer {tu-token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Allow all origins
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
