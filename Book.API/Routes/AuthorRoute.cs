using System.Data.Common;
using System.Data;
using Book.Shared.Data.Banco;
using Book.Shared.Models.Modelos;
using Book.Shared.Models.Requisicoes;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Book.API.Routes
{
    public static class AuthorRoute
    {
        public static void AuthorRoutes(this WebApplication app) 
        {
            var route = app.MapGroup("author");

            //POST
            route.MapPost("",
                async (AuthorRequest req, BookApiContext context) =>
                {
                    if(req == null)
                        return Results.BadRequest("Request inválida");

                    if (string.IsNullOrEmpty(req.AuthorName))
                        return Results.BadRequest("O nome do autor é obrigatório!");

                    var author = new AuthorClass(req.AuthorName);

                    await context.AddAsync(author);
                    await context.SaveChangesAsync();

                    return Results.Created($"/author/{author.AuthorID}", author);
                })
                .Produces<AuthorClass>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);


            //GET
            route.MapGet("",
                async (IDbConnection dbConnection) =>
                {
                    var query = @"Select * from TblAuthor";
                    var authors = await dbConnection.QueryAsync<AuthorClass>(query);
                    if (authors == null || !authors.Any())
                        return Results.NotFound("Nenhum autor encontrado.");
                    return Results.Ok(authors);

                })
                .Produces<List<AuthorClass>>(StatusCodes.Status200OK);


            //GET BY ID
            route.MapGet("{AuthorID:int}",
               async (int AuthorID, IDbConnection dbConnection) =>
               {
                   var query = @"Select * from TblAuthor WHERE AuthorID = @AuthorID";
                   var author = await dbConnection.QueryFirstOrDefaultAsync<AuthorClass>(query, new { AuthorID });
                   if (author == null)
                       return Results.NotFound($"Autor com ID {AuthorID} não encontrado.");

                   return Results.Ok(author);

               })
               .Produces<List<AuthorClass>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound);


            //PUT
            route.MapPut("{AuthorID:int}",
                async(int AuthorID, AuthorRequest req, BookApiContext context) =>
                {
                    var author = await context.TblAuthor.FirstOrDefaultAsync(x => x.AuthorID == AuthorID);

                    if (author == null)
                        return Results.NotFound();

                    author.AuthorName = req.AuthorName;

                    await context.SaveChangesAsync();

                    return Results.Ok(author);

                })
                .Produces<AuthorClass>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);


            //DELETE
            route.MapDelete("{AuthorID:int}",
                async (int AuthorID, BookApiContext context) =>
                {
                    var author = context.TblAuthor.FirstOrDefault(x => x.AuthorID == AuthorID);

                    if (author == null)
                        return Results.NotFound();

                    context.TblAuthor.Remove(author);
                    await context.SaveChangesAsync();
                    return Results.NoContent();

                })
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);


            //PESQUISAR POR NOME
            route.MapGet("search/",
                async (IDbConnection dbConnection, string? name) =>
                {
                    var query = @"Select * from TblAuthor WHERE (@Name IS NULL OR AuthorName LIKE CONCAT('%', @Name, '%'))";
                    var authors = await dbConnection.QueryAsync<AuthorClass>(query, new { Name = name});
                    if (authors == null || !authors.Any())
                        return Results.NotFound("Nenhum autor encontrado.");
                    return Results.Ok(authors);

                })
                .Produces<List<AuthorClass>>(StatusCodes.Status200OK);
        }
    }
}
