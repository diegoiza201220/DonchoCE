using ComprobantesElectronicos.Services;
using EFModel.Context;
using EFModel.Interfaces;
using EFModel.Repositories;
using EnvioCorreos.Extensions;
using EnvioCorreos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiDonCho;
using WebApiDonCho.Services;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Retry;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    // Esperar 60 segundos antes de iniciar la aplicación para asegurarse de que la base de datos esté lista, se usa solo para Producción
    Thread.Sleep(60000);
}

// 1. Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Environment.IsDevelopment()? builder.Configuration.GetConnectionString("PostgresConnectionTest"): builder.Configuration.GetConnectionString("PostgresConnectionProd");

// 2. Registrar el DbContext con el proveedor de PostgreSQL
builder.Services.AddDbContext<DonchoContext>(options =>
    options.UseNpgsql(connectionString, 
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3, 
            maxRetryDelay: TimeSpan.FromSeconds(2), 
            errorCodesToAdd: null)
        )
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// 3. Registrar Unit of Work (incluye todos los repositorios)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<OrdenService>();
builder.Services.AddScoped<ComprobanteService>();
builder.Services.AddScoped<SriService>();
builder.Services.AddScoped<InfowareFirmaService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<DailyConfigurationValidatorService>();
builder.Services.AddScoped<CacheWarmupService>();

builder.Services.AddSRIDocumentosElectronicos();

builder.Services.AddEnvioCorreos(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

//builder.Services.AddHttpClient<SriService>(client =>
//{
//    client.Timeout = TimeSpan.FromMinutes(5);
//    client.DefaultRequestHeaders.Add("Accept", "text/xml");
//})
//.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
//{
//    ServerCertificateCustomValidationCallback =
//        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
//    AutomaticDecompression =
//        System.Net.DecompressionMethods.GZip |
//        System.Net.DecompressionMethods.Deflate
//});

builder.Services.AddHttpClient<SriService>(client =>
{
    // El control de tiempos lo hace el pipeline de resiliencia
    client.Timeout = Timeout.InfiniteTimeSpan;
    client.DefaultRequestHeaders.Add("Accept", "text/xml");
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    // Con 1 envío por minuto, la conexión no debe sobrevivir entre llamadas
    PooledConnectionIdleTimeout = TimeSpan.FromSeconds(5),
    PooledConnectionLifetime = TimeSpan.FromSeconds(30),
    ConnectTimeout = TimeSpan.FromSeconds(15),
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
    SslOptions = new System.Net.Security.SslClientAuthenticationOptions
    {
        // Deuda técnica: acepta cualquier certificado
        RemoteCertificateValidationCallback = (_, _, _, _) => true
    }
})
.AddResilienceHandler("sri-pipeline", pipeline =>
{
    // 1. Timeout total (incluye todos los reintentos y esperas)
    pipeline.AddTimeout(TimeSpan.FromMinutes(3));

    // 2. Reintentos con espera exponencial y jitter
    pipeline.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 4,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true
        // ShouldHandle por defecto: HttpRequestException (incluye Connection reset),
        // timeouts por intento, 5xx, 408 y 429
    });

    // 3. Timeout por intento
    pipeline.AddTimeout(TimeSpan.FromSeconds(30));
});


var app = builder.Build();

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"[ERROR INTERNO SERILOG] {msg}"));
var rutaLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "app-{Date}.txt");


var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
loggerFactory.AddFile(rutaLogs);

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await AppStartup.EjecutarAsync(app.Services);

app.UseRouting();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
