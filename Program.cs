using FirstAPI.Infrastructure;    // Para o ConnectionContext
using FirstAPI.Infraestrutura; // Para CarroRepository e ICarroRepository
using Microsoft.EntityFrameworkCore; // Para o UseNpgsql

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO DOS SERVIÇOS ---

// 1. Adiciona os serviços de controladores (API)
builder.Services.AddControllers();

// 2. Adiciona o Swagger para documentação e teste da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


// --- FIM DA CONFIGURAÇÃO DOS SERVIÇOS ---


var app = builder.Build();

// --- INÍCIO DO PIPELINE DA APLICAÇÃO ---

// Configure o pipeline de requisições HTTP.
// Em ambiente de desenvolvimento, mostre a UI do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona HTTP para HTTPS
//app.UseHttpsRedirection();

// Adiciona autorização (você usará isso mais tarde)
app.UseAuthorization();

// Mapeia as rotas para os seus Controllers
app.MapControllers();

// --- FIM DO PIPELINE DA APLICAÇÃO ---

// Inicia a aplicação
app.Run();
