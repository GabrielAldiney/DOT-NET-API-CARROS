using FirstAPI.Infrastructure;    // Para o ConnectionContext
using FirstAPI.Infraestrutura; // Para CarroRepository e ICarroRepository
using Microsoft.EntityFrameworkCore;
using System.Text; // Para o UseNpgsql
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// --- IN�CIO DA CONFIGURA��O DOS SERVI�OS ---

// 1. Adiciona os servi�os de controladores (API)
builder.Services.AddControllers();

// 2. Adiciona o Swagger para documenta��o e teste da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
    {
        new OpenApiSecurityScheme
        {
        Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,

        },
        new List<string>()
        }
    });


});

// 3. Pegar a string de conex�o do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 4. Adicionar o DbContext aos servi�os da aplica��o
// Isso permite que o 'ConnectionContext' seja injetado em outras classes
builder.Services.AddDbContext<ConnectionContext>(options =>
    options.UseNpgsql(connectionString)
);

// 5. REGISTRAR O REPOSIT�RIO (Esta era a parte que faltava)
// Isso "ensina" o .NET: "Quando um construtor pedir um ICarroRepository,
// entregue para ele uma nova inst�ncia de CarroRepository"
// Usamos AddScoped para que o reposit�rio viva por toda a requisi��o web.
builder.Services.AddScoped<ICarroRepository, CarroRepository>();


// --- FIM DA CONFIGURA��O DOS SERVI�OS ---

var key = Encoding.ASCII.GetBytes(FirstAPI.Key.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// --- IN�CIO DO PIPELINE DA APLICA��O ---

// Configure o pipeline de requisi��es HTTP.
// Em ambiente de desenvolvimento, mostre a UI do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error") ;
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

// Redireciona HTTP para HTTPS
//app.UseHttpsRedirection();

// Adiciona autoriza��o (voc� usar� isso mais tarde)
app.UseAuthorization();

// Mapeia as rotas para os seus Controllers
app.MapControllers();

// --- FIM DO PIPELINE DA APLICA��O ---

// Inicia a aplica��o
app.Run();
