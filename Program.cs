using FirstAPI.Infrastructure;    // Para o ConnectionContext
using Microsoft.EntityFrameworkCore;
using System.Text; // Para o UseNpgsql
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using FirstAPI.Infraestrutura.Repositories;
using FirstAPI.Domain.Model.CarroAggregate;
using WebApi.Application.Swagger;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO DOS SERVIÇOS ---

// 1. Adiciona os serviços de controladores (API)
builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(DomainToDTOMapping));
// 2. Adiciona o Swagger para documentação e teste da API
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true; 
});


builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerDefaultValues>();

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

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

// 3. Pegar a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 4. Adicionar o DbContext aos serviços da aplicação
// Isso permite que o 'ConnectionContext' seja injetado em outras classes
builder.Services.AddDbContext<ConnectionContext>(options =>
    options.UseNpgsql(connectionString)
);

// 5. REGISTRAR O REPOSITÓRIO (Esta era a parte que faltava)
// Isso "ensina" o .NET: "Quando um construtor pedir um ICarroRepository,
// entregue para ele uma nova instância de CarroRepository"
// Usamos AddScoped para que o repositório viva por toda a requisição web.
builder.Services.AddScoped<ICarroRepository, CarroRepository>();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod();
        });
});

// --- FIM DA CONFIGURAÇÃO DOS SERVIÇOS ---

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

// --- INÍCIO DO PIPELINE DA APLICAÇÃO ---

// Configure o pipeline de requisições HTTP.
// Em ambiente de desenvolvimento, mostre a UI do Swagger

var versionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();


if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error") ;
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"Web APi - {description.GroupName.ToUpper()}");
        }
    });
}
else
{
    app.UseExceptionHandler("/error");
}

// Redireciona HTTP para HTTPS
//app.UseHttpsRedirection();

app.UseCors("MyPolicy");

// Adiciona autorização (você usará isso mais tarde)
app.UseAuthorization();

// Mapeia as rotas para os seus Controllers
app.MapControllers();

// --- FIM DO PIPELINE DA APLICAÇÃO ---

// Inicia a aplicação
app.Run();
