using FirstAPI.Infrastructure;    // Para o ConnectionContext
using FirstAPI.Infraestrutura; // Para CarroRepository e ICarroRepository
using Microsoft.EntityFrameworkCore; // Para o UseNpgsql

var builder = WebApplication.CreateBuilder(args);

// --- IN�CIO DA CONFIGURA��O DOS SERVI�OS ---

// 1. Adiciona os servi�os de controladores (API)
builder.Services.AddControllers();

// 2. Adiciona o Swagger para documenta��o e teste da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


var app = builder.Build();

// --- IN�CIO DO PIPELINE DA APLICA��O ---

// Configure o pipeline de requisi��es HTTP.
// Em ambiente de desenvolvimento, mostre a UI do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
