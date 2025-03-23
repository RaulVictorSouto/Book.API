using System.Text.Json.Serialization;
using Book.API.Routes;
using Book.Shared.Data.Banco;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

//adicionando o BookApiContext
builder.Services.AddDbContext<BookApiContext>();

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


app.Run();
