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

using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

ILogger logger = factory.CreateLogger("Program");

var builder = WebApplication.CreateBuilder(args);

// 1. Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// 2. Registrar el DbContext con el proveedor de PostgreSQL
builder.Services.AddDbContext<DonchoContext>(options =>
    options.UseNpgsql(connectionString)
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
);

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

builder.Services.AddHttpClient<SriService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
    client.DefaultRequestHeaders.Add("Accept", "text/xml");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
    AutomaticDecompression =
        System.Net.DecompressionMethods.GZip |
        System.Net.DecompressionMethods.Deflate
});

var app = builder.Build();

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
