using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Text;
using Sisand.Airlines.Infrastructure.Context;
using Sisand.Airlines.Infrastructure;
using Sisand.Airlines.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Adiciona Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger com suporte a JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sisand Airlines API",
        Version = "v1",
        Description = "API para compra de passagens Sisand Airlines (Curitiba → São Paulo)"
    });

    // Configuração de autenticação no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost"); 
    });
});

// Configuração JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Registrar conexão com o banco
builder.Services.AddScoped<NpgsqlConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("Postgres");
    return new NpgsqlConnection(connectionString);
});

var app = builder.Build();

// ✅ Ativa o CORS antes de autenticação
app.UseCors("AllowFrontend");

// ✅ Swagger sempre habilitado, independentemente do ambiente
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sisand Airlines API v1");
    c.RoutePrefix = string.Empty;
});

// app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
app.Run();
