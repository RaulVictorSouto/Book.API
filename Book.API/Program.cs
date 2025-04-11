using System.Data;
using System.Text.Json.Serialization;
using Book.API.Routes;
using Book.Shared.Data.Banco;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

//adicionando o BookApiContext
builder.Services.AddDbContext<BookApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar DatabaseConnection como um serviço
builder.Services.AddSingleton<DapperConnection>();

// Registrar IDbConnection usando a classe DatabaseConnection
builder.Services.AddScoped<IDbConnection>(sp =>
    sp.GetRequiredService<DapperConnection>().CreateConnection());

// serviços de autorização
builder.Services.AddAuthorization();

// Outras configurações de serviço
builder.Services.AddControllers();

//adicionando o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(doc =>
{
    doc.SwaggerDoc("v1", new OpenApiInfo() 
    { 
        Title = "Book API", 
        Version = "v1",
        Description = "API mapeamento de livros cadastrados",
        Contact = new OpenApiContact()
        {
            Name = "Raul Souto"
        }
    });
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuração para uploads grandes
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50MB
});

// Configuração do Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50MB
});

// Logging detalhado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

//mapeando as routes
app.BookRoutes();
app.AuthorRoutes();
app.GenreRoutes();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book API v1");
});

app.UseCors("AllowAll");

// Tratamento de erros
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
