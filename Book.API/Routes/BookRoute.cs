using System.Runtime.CompilerServices;
using Book.Shared.Data.Banco;
using Book.Shared.Models.Modelos;
using Book.Shared.Models.Requisicoes;
using Microsoft.EntityFrameworkCore;

namespace Book.API.Routes
{
    public static class BookRoute
    {
        public static void BookRoutes(this WebApplication app)
        {
            var route = app.MapGroup("book");


            //POST
            route.MapPost("",
                async (BookRequest req, BookApiContext context) =>
                {
                    if (req == null)
                        return Results.BadRequest("Request inválida.");

                    if (string.IsNullOrEmpty(req.BookTitle))
                        return Results.BadRequest("O título do livro é obrigatório.");

                    var book = new BookClass(
                        req.BookTitle,
                        req.BookLanguage,
                        req.BookPublisher,
                        req.BookISBN,
                        req.BookRating,
                        req.AuthorID,
                        req.BookCoverPage);

                    await context.AddAsync(book);
                    await context.SaveChangesAsync();

                    return Results.Created($"/book/{book.BookID}", book);
                })
                .Produces<BookClass>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);


            //GET
            route.MapGet("",
               async (BookApiContext context) =>
               {
                   var books = await context.TblBook.ToListAsync();
                   return Results.Ok(books);
               })
               .Produces<List<BookClass>>(StatusCodes.Status200OK);


            // PUT
            route.MapPut("{BookID:guid}",
                async (Guid BookID, BookRequest req, BookApiContext context) =>
                {
                    var book = await context.TblBook.FirstOrDefaultAsync(x => x.BookID == BookID);

                    if (book == null)
                        return Results.NotFound();

                    book.BookTitle = req.BookTitle;
                    book.BookLanguage = req.BookLanguage;
                    book.BookPublisher = req.BookPublisher;
                    book.BookRating = req.BookRating;
                    book.AuthorID = req.AuthorID;
                    book.BookCoverPage = req.BookCoverPage;

                    await context.SaveChangesAsync();

                    return Results.Ok(book);
                })
                .Produces<BookClass>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);


            // DELETE
            route.MapDelete("{BookID:guid}",
                async (Guid BookID, BookApiContext context) =>
                {
                    var book = await context.TblBook.FirstOrDefaultAsync(x => x.BookID == BookID);

                    if (book == null)
                        return Results.NotFound();

                    context.TblBook.Remove(book);
                    await context.SaveChangesAsync();
                    return Results.NoContent();
                })
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}