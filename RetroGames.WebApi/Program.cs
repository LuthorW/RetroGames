using FluentValidation;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
using RetroGames.Services;
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

// Administrador inicial: e-mail e senha vêm do .env (AdminInicial__Email e AdminInicial__Senha).
var emailAdministrador = builder.Configuration["AdminInicial:Email"];
var senhaAdministrador = builder.Configuration["AdminInicial:Senha"];
if (string.IsNullOrWhiteSpace(emailAdministrador) || string.IsNullOrWhiteSpace(senhaAdministrador))
{
    throw new InvalidOperationException(
        "Administrador inicial não configurado. Acrescente ao .env, na raiz do repositório, " +
        "as linhas AdminInicial__Email=\"...\" e AdminInicial__Senha=\"...\".");
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
        {
            // O nome do parâmetro da action (ex.: "request") não é um campo do JSON:
            // aparece só porque o corpo não pôde ser lido. Fica de fora da lista.
            var parametros = contexto.ActionDescriptor.Parameters
                .Select(parametro => parametro.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var erros = contexto.ModelState
                .Where(item => item.Value is { Errors.Count: > 0 } && !parametros.Contains(item.Key))
                .ToDictionary(
                    item => FluentValidationFilter.ParaCamelCase(item.Key),
                    item => new[] { "Valor inválido ou ausente." });

            if (erros.Count == 0)
            {
                erros["corpo"] = new[] { "Valor inválido ou ausente." };
            }

            return FluentValidationFilter.CriarRespostaErrosDeCampo(erros);
        };
    });

// Registra todos os validators deste projeto (classes que herdam de AbstractValidator).
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddOpenApi();

// Tratamento global de erros
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Cria o administrador inicial, se ainda não houver nenhum. Os Services são "scoped"
// (um por requisição); fora de uma requisição, é preciso abrir um escopo à mão.
using (var escopo = app.Services.CreateScope())
{
    var autenticacaoService = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacaoService.GarantirAdministradorInicialAsync(emailAdministrador, senhaAdministrador);
}

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
