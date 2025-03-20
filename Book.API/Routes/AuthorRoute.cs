using Book.Shared.Data.Banco;
using Book.Shared.Models.Modelos;
using Book.Shared.Models.Requisicoes;
using Microsoft.EntityFrameworkCore;

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
                async (BookApiContext context) =>
                {
                    var authos = await context.TblAuthor.ToListAsync();
                    return authos;
                })
                .Produces<List<AuthorClass>>(StatusCodes.Status200OK);


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
        }
    }
}
