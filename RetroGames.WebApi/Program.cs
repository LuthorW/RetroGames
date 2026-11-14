using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

// Carrega o .env (procurando a partir da pasta atual e subindo até a raiz do
// repositório). Cada linha vira variável de ambiente: ConnectionStrings__Default
// passa a ser lida como ConnectionStrings:Default. Precisa vir antes do builder.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string não encontrada. Crie o arquivo .env na raiz do repositório " +
        "(copie o .env.example) com a linha ConnectionStrings__Default=\"...\".");
}

// Injeção de dependência
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Tratamento global de erros
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Pipeline: por onde toda requisição passa.
// Primeira etapa: captura qualquer exceção das etapas seguintes.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Descrição da API em /openapi/v1.json e página de testes em /swagger
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API"));
}

app.MapControllers();

app.Run();
