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
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:6873"); // Permitir que la aplicación escuche en todas las interfaces de red en el puerto 6873

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173") // Permitir solo el origen del frontend de Vite
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Obtener la cadena de conexión desde la configuración de appsettings.json
if (string.IsNullOrWhiteSpace(connectionString)) // Verificar que la cadena de conexión no esté vacía o nula
{
    throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en la configuración.");
}

var jwtKey = builder.Configuration["JwtSettings:Key"]; // Obtener la clave secreta para JWT desde la configuración de appsettings.json
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
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // Permitir que los nombres de las propiedades sean insensibles a mayúsculas/minúsculas
    });
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)); // Configurar Entity Framework Core para usar PostgreSQL con la cadena de conexión obtenida desde la configuración de appsettings.json

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
builder.Services.AddScoped<NotificationRepository, NotificationRepository>();
builder.Services.AddScoped<NotificationService, NotificationService>();
builder.Services.AddHostedService<NotificationBackgroundService>();
builder.Services.AddScoped<AchievementRepository, AchievementRepository>();
builder.Services.AddScoped<AchievementService, AchievementService>();
builder.Services.AddScoped<BodyMetricRepository, BodyMetricRepository>();
builder.Services.AddScoped<BodyMetricService, BodyMetricService>();
builder.Services.AddScoped<ExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<ExerciseService, ExerciseService>();
builder.Services.AddScoped<AuthRepository, AuthRepository>();
builder.Services.AddHttpClient<CoachAIService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60); // Establecer un tiempo de espera más largo para las solicitudes a la API de CoachAI, ya que las respuestas pueden tardar más en generarse para no saturar el sistema 
});
builder.Services.AddHttpClient<ExerciseDbClient>(client => // Lo mismo para ExerciseDB, que también puede tardar en responder debido a la cantidad de datos que maneja
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddSingleton<ExerciseImageFallbackService>(); // Servicio singleton para manejar las imágenes de ejercicio, ya que no tiene estado y se puede compartir entre solicitudes


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // Evitar que el sistema mapee automáticamente los claims estándar de JWT a tipos de claim específicos de .NET, para recuperarlos mas facilmente en el front

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
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Obtener el contexto de la base de datos desde el contenedor de servicios
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

