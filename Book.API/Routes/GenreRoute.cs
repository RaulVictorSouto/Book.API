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
            var route = app.MapGroup("genre")
                .RequireAuthorization();

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
                var genres = await dbConnection.QueryAsync<GenreClass>(query);
                if (genres == null || !genres.Any())
                    return Results.NotFound("Nenhum genero encontrado.");
                return Results.Ok(genres);

            })
            .Produces<List<AuthorClass>>(StatusCodes.Status200OK);


            //GET BY ID
            route.MapGet("{GenreID:int}",
               async (int GenreID, IDbConnection dbConnection) =>
               {
                   var query = @"Select * from TblGenre WHERE GenreID = @GenreID";
                   var genre = await dbConnection.QueryFirstOrDefaultAsync<GenreClass>(query, new { GenreID });
                   if (genre == null)
                       return Results.NotFound($"O Gênero com ID {GenreID} não encontrado.");

                   return Results.Ok(genre);

               })
               .Produces<List<GenreClass>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound);



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


            //PESQUISA COM BASE EM NOME
            route.MapGet("search/",
            async (IDbConnection dbConnection, string? name) =>
            {
                var query = @"Select * from TblGenre WHERE (@Name IS NULL OR GenreName LIKE CONCAT('%', @Name, '%'))";
                var genres = await dbConnection.QueryAsync<GenreClass>(query, new { Name = name});
                if (genres == null || !genres.Any())
                    return Results.NotFound("Nenhum genero encontrado.");
                return Results.Ok(genres);

            })
            .Produces<List<AuthorClass>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
