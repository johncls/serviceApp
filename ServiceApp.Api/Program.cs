using ServiceApp.Application.Services;
using ServiceApp.Domain.Interfaces;
using ServiceApp.Infrastructure.Persistence;
using ServiceApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB Configuration
var mongoConnectionString = builder.Configuration.GetSection("MongoDB:ConnectionString").Value 
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration.GetSection("MongoDB:DatabaseName").Value 
    ?? "UsersGirlAgency";

builder.Services.AddSingleton(new MongoDbContext(mongoConnectionString, mongoDatabaseName));
builder.Services.AddScoped<IUserRepository, UserRepository>();

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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Allow all origins
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
