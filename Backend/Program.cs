using AA2_CS.Service;
using Microsoft.EntityFrameworkCore;
using AA2_CS.Database;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AA2_CS.JWT;
using AA2_CS.Services;
using AA2_CS.Repository;
using AA2_CS.Middleware;

var builder = WebApplication.CreateBuilder(args);
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:6873");

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en la configuración.");
}

var jwtKey = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Falta JwtSettings:Key en la configuración.");
}

// Agregar servicios al contenedor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registrar los servicios específicos
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<PlanService>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<JWTConfigurer>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<EmailVerificationRepository, EmailVerificationRepository>();
builder.Services.AddScoped<ItemRepository, ItemRepository>();
builder.Services.AddScoped<PlanRepository, PlanRepository>();
builder.Services.AddScoped<PurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<RoomRepository, RoomRepository>();
builder.Services.AddScoped<UserRepository, UserRepository>();
builder.Services.AddScoped<TasksRepository, TasksRepository>();
builder.Services.AddScoped<TasksService, TasksService>();
builder.Services.AddScoped<SubscriptionRepository, SubscriptionRepository>();
builder.Services.AddHttpClient<GeocodificacionRepository, GeocodificacionRepository>();
builder.Services.AddScoped<GeocodificacionService, GeocodificacionService>();
builder.Services.AddScoped<UserRoomRepository, UserRoomRepository>();
builder.Services.AddScoped<UserRoomService, UserRoomService>();
builder.Services.AddScoped<NotificationPreferenceRepository, NotificationPreferenceRepository>();
builder.Services.AddScoped<NotificationPreferenceService, NotificationPreferenceService>();
builder.Services.AddScoped<NotificationService, NotificationService>();
builder.Services.AddHostedService<NotificationBackgroundService>();

// Nuevos servicios y repositorios
builder.Services.AddScoped<AchievementRepository, AchievementRepository>();
builder.Services.AddScoped<AchievementService, AchievementService>();
builder.Services.AddScoped<BodyMetricRepository, BodyMetricRepository>();
builder.Services.AddScoped<BodyMetricService, BodyMetricService>();
builder.Services.AddScoped<ExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<ExerciseService, ExerciseService>();
builder.Services.AddHttpClient<CoachAIService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
});
builder.Services.AddHttpClient<ExerciseDbClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddSingleton<ExerciseImageFallbackService>();

// Evitar que JwtSecurityTokenHandler transforme los claim types (ej: role -> http://schemas...)
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Configuración de JWT para autenticación
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var config = builder.Configuration;
        options.MapInboundClaims = false; // .NET 8: evitar mapeo automático de claims
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //verificar que el issuer sea válido
            ValidateAudience = true, //verificar que el audience sea valido
            ValidateLifetime = true, //verificar que el token no haya expirado
            ValidateIssuerSigningKey = true, //verificar la clave de firma sea válida
            ValidIssuer = config["JwtSettings:Issuer"], //el emisor que se espera (The Training Hub)
            ValidAudience = config["JwtSettings:Audience"], //la audiencia que se espera (The Training Hub Users)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization(); // Agregar el servicio de autorización

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();  // Aplica migraciones pendientes
}
// Configurar el pipeline de la solicitud HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS antes de los controladores
app.UseCors("AllowFrontend");
app.Urls.Add("http://0.0.0.0:6873");

app.UseHttpsRedirection();
app.UseAuthentication(); // Usar la autenticación para JWT y los tokens
app.UseAuthorization();

// Middleware global de manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Mapeo de controladores
app.MapControllers();

app.Run();

