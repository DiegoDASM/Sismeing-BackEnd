using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sismeing.API.Middleware;
using Sismeing.Infrestructura.Persistence;
using System.Text;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Operaciones;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Services.Email;
using Sismeing.Service.Services.Operaciones;
using Sismeing.Service.Services.Catalogo;
using Sismeing.API.Extensions;
var builder = WebApplication.CreateBuilder(args);

// ── Conexión a la base de datos Supabase (PostgreSQL) ────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SupaBaseDBcontext>(options =>
    options.UseNpgsql(connectionString));

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey not found."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // true en producción
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── CORS ─────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("SismeingCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Dependency Injection (Servicios) ──────────────────────────────────────────

// Registramos todos los servicios de la aplicación usando el Extension Method
builder.Services.AddApplicationServices();

// ── Controllers & Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Evitar ciclos de referencia en entidades con navegación
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sismeing API",
        Version = "v1",
        Description = "API de gestión de instalaciones y mantenimiento de equipos de climatización."
    });

    // Soporte de Bearer Token en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el prefijo Bearer: **Bearer {token}**"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sismeing API v1");
        c.RoutePrefix = string.Empty; // Swagger en raíz: http://localhost:5xxx/
    });
}

app.UseHttpsRedirection();
app.UseCors("SismeingCors");

// JWT Middleware personalizado (extrae claims al HttpContext.Items)
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ── Verificación de conexión a la Base de Datos ─────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SupaBaseDBcontext>();
    try
    {
        dbContext.Database.OpenConnection();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("✅ CONEXIÓN A LA BASE DE DATOS SUPABASE EXITOSA!");
        Console.WriteLine("=======================================================\n");
        Console.ResetColor();
        dbContext.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n=======================================================");
        Console.WriteLine($"❌ EXCEPCIÓN CONECTANDO A LA BD: {ex.Message}");
        Console.WriteLine("=======================================================\n");
        Console.ResetColor();
    }
}

app.Run();

