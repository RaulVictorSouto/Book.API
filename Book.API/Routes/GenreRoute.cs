using System.Data;
using System.Net;
using Book.Shared.Data.Banco;
using Book.Shared.Models.Modelos;
using Book.Shared.Models.Requisicoes;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Book.API.Routes
{
    public static class GenreRoute
    {
        public static void GenreRoutes(this WebApplication app) 
        {
            var route = app.MapGroup("genre");

            //POST
            route.MapPost("",
                async (GenreRequest req, BookApiContext context) =>
                {
                    if (req == null)
                        return Results.BadRequest("Request inválida.");

                    if (string.IsNullOrEmpty(req.GenreName))
                        return Results.BadRequest("O nome do genero é obrigatório.");

                    var genre = new GenreClass(req.GenreName);

                    await context.AddAsync(genre);
                    await context.SaveChangesAsync();

                    return Results.Created($"/genre/{genre.GenreID}", genre);
                })
                .Produces<GenreClass>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);


            //GET
            route.MapGet("",
            async (IDbConnection dbConnection) =>
            {
                var query = @"Select * from TblGenre";
                var authors = await dbConnection.QueryAsync<GenreClass>(query);
                if (authors == null || !authors.Any())
                    return Results.NotFound("Nenhum autor encontrado.");
                return Results.Ok(authors);

            })
            .Produces<List<AuthorClass>>(StatusCodes.Status200OK);



            //PUT
            route.MapPut("{GenreID:int}",
                async (int GenreID, GenreRequest req, BookApiContext context) =>
                {
                    var genre = await context.TblGenre.FirstOrDefaultAsync(x => x.GenreID == GenreID);

                    if (genre == null)
                        return Results.NotFound();

                    genre.GenreName = req.GenreName;

                    await context.SaveChangesAsync();

                    return Results.Ok(genre);
                })
                .Produces<GenreClass>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status400BadRequest);


            //DELETE
            route.MapDelete("{GenreID:int}",
                async (int GenreID, BookApiContext context) =>
                {
                    var genre = await context.TblGenre.FirstOrDefaultAsync(x => x.GenreID == GenreID);

                    if (genre == null)
                        return Results.NotFound();

                    context.TblGenre.Remove(genre);
                    await context.SaveChangesAsync();
                    return Results.NoContent();
                })
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}
