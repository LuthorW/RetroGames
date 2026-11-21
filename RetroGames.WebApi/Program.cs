using FluentValidation;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
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

// Controllers, com o filtro de validação valendo para todas as actions.
builder.Services
    .AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        // JSON malformado ou tipo errado (o [ApiController] recusa antes do filtro):
        // mesma resposta da validação, 400 com o código 3 e os erros por campo.
        opcoes.InvalidModelStateResponseFactory = contexto =>
            FluentValidationFilter.CriarRespostaErrosDeCampo(
                contexto.ModelState
                    .Where(item => item.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        item => FluentValidationFilter.ParaCamelCase(item.Key),
                        item => new[] { "Valor inválido ou ausente." }));
    });

// Registra todos os validators deste projeto (classes que herdam de AbstractValidator).
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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
