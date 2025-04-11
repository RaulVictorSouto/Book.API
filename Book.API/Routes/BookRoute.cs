﻿using System.Data.Common;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Book.Shared.Data.Banco;
using Book.Shared.Models.Modelos;
using Book.Shared.Models.Requisicoes;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

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

                    // Verifica se o autor existe
                    var authors = await context.TblAuthor
                        .Where(a => req.AuthorIDs.Contains(a.AuthorID))
                        .ToListAsync();

                    if (authors.Count != req.AuthorIDs.Count)
                        return Results.BadRequest("Um ou mais autores não foram encontrados.");

                    // Busca os gêneros no banco de dados
                    var genres = await context.TblGenre
                        .Where(g => req.GenreIDs.Contains(g.GenreID))
                        .ToListAsync();

                    if (genres.Count != req.GenreIDs.Count)
                        return Results.BadRequest("Um ou mais gêneros não foram encontrados.");



                    // Processando a imagem da capa
                    byte[]? coverPageBytes = null;
                    if (req.BookCoverPage != null && req.BookCoverPage.Length > 0)
                    {
                        //verifica a extensão do arquivo
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(req.BookCoverFileName).ToLower();

                        if (!allowedExtensions.Any(ext => string.Equals(ext, fileExtension, StringComparison.OrdinalIgnoreCase)))
                        {
                            return Results.BadRequest("Formato de imagem inválido. Use JPG, JPEG ou PNG");
                        }

                        //processa o arquivo
                        coverPageBytes = req.BookCoverPage;

                        //verifica o tamanho
                        if (coverPageBytes.Length > 5 * 1024 * 1024)
                        {
                            return Results.BadRequest("A imagem da capa não pode exceder 5MB");
                        }
                    }

                    // Cria o livro
                    var book = new BookClass(
                        req.BookTitle,
                        req.BookLanguage,
                        req.BookPublisher,
                        req.BookISBN,
                        req.BookRating,
                        coverPageBytes);

                    //Associa os autores ao livro
                    book.Authors = authors;

                    // Associa os gêneros ao livro
                    book.Genres = genres;

                    //Associa as tags
                    book.BookTagsList = req.BookTags;

                    // Salva o livro no banco de dados
                    await context.AddAsync(book);
                    await context.SaveChangesAsync();

                    return Results.Created($"/book/{book.BookID}", book);
                })
                .Produces<BookClass>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);


            //GET
            route.MapGet("",
                async (IDbConnection dbConnection) =>
                {
                    var query = @"
                                    SELECT 
                                    b.BookID, 
                                    b.BookTitle, 
                                    b.BookLanguage, 
                                    b.BookPublisher, 
                                    b.BookISBN, 
                                    b.BookRating, 
                                    b.BookCoverPage,
                                    b.BookTags,
                                    a.AuthorID, 
                                    a.AuthorName, 
                                    g.GenreID, 
                                    g.GenreName 
                                    FROM TblBook b
                                    LEFT JOIN TblBookAuthors ba ON b.BookID = ba.BooksBookID
                                    LEFT JOIN TblAuthor a ON ba.AuthorsAuthorID = a.AuthorID
                                    LEFT JOIN TblBookGenres bg ON b.BookID = bg.BooksBookID
                                    LEFT JOIN TblGenre g ON bg.GenresGenreID = g.GenreID";

                    var bookDictionary = new Dictionary<Guid, BookClass>();

                    var result = await dbConnection.QueryAsync<BookClass, AuthorClass, GenreClass, BookClass>(
                        query,
                        (book, author, genre) =>
                        {
                            // Verifica se o livro já foi adicionado ao dicionário
                            if (!bookDictionary.TryGetValue(book.BookID, out var bookEntry))
                            {
                                bookEntry = book;
                                bookEntry.Authors = new List<AuthorClass>();
                                bookEntry.Genres = new List<GenreClass>();
                                bookDictionary.Add(book.BookID, bookEntry);
                            }

                            // Adiciona o autor ao livro, se existir e não estiver duplicado
                            if (author != null && !bookEntry.Authors.Any(a => a.AuthorID == author.AuthorID))
                            {
                                bookEntry.Authors.Add(author);
                            }

                            // Adiciona o gênero ao livro, se existir e não estiver duplicado
                            if (genre != null && !bookEntry.Genres.Any(g => g.GenreID == genre.GenreID))
                            {
                                bookEntry.Genres.Add(genre);
                            }

                            return bookEntry;
                        },
                        splitOn: "AuthorID,GenreID"
                    );

                    // Converte o dicionário para uma lista de livros
                    var books = bookDictionary.Values.ToList();

                    if (books == null || !books.Any())
                        return Results.NotFound("Nenhum livro encontrado.");

                    return Results.Ok(books);
                })
            .Produces<List<BookClass>>(StatusCodes.Status200OK);

            //GET BY ID
            route.MapGet("{BookID:guid}",
                async (Guid BookID, IDbConnection dbConnection) =>
                {
                    var query = @"
                    SELECT 
                        b.BookID, 
                        b.BookTitle, 
                        b.BookLanguage, 
                        b.BookPublisher, 
                        b.BookISBN, 
                        b.BookRating, 
                        b.BookCoverPage,
                        b.BookTags,
                        a.AuthorID, 
                        a.AuthorName, 
                        g.GenreID, 
                        g.GenreName 
                    FROM TblBook b
                    LEFT JOIN TblBookAuthors ba ON b.BookID = ba.BooksBookID
                    LEFT JOIN TblAuthor a ON ba.AuthorsAuthorID = a.AuthorID
                    LEFT JOIN TblBookGenres bg ON b.BookID = bg.BooksBookID
                    LEFT JOIN TblGenre g ON bg.GenresGenreID = g.GenreID
                    WHERE b.BookID = @BookID";

                    var bookDictionary = new Dictionary<Guid, BookClass>();

                    var result = await dbConnection.QueryAsync<BookClass, AuthorClass, GenreClass, BookClass>(
                        query,
                        (book, author, genre) =>
                        {
                            if (!bookDictionary.TryGetValue(book.BookID, out var bookEntry))
                            {
                                bookEntry = book;
                                bookEntry.Authors = new List<AuthorClass>();
                                bookEntry.Genres = new List<GenreClass>();
                                bookDictionary.Add(book.BookID, bookEntry);
                            }

                            if (author != null && !bookEntry.Authors.Any(a => a.AuthorID == author.AuthorID))
                            {
                                bookEntry.Authors.Add(author);
                            }

                            if (genre != null && !bookEntry.Genres.Any(g => g.GenreID == genre.GenreID))
                            {
                                bookEntry.Genres.Add(genre);
                            }

                            return bookEntry;
                        },
                        new { BookID }, // Add this parameter
                        splitOn: "AuthorID,GenreID"
                    );

                    // Get the single book from dictionary
                    if (!bookDictionary.TryGetValue(BookID, out var book))
                        return Results.NotFound($"Livro com ID {BookID} não encontrado.");

                    return Results.Ok(book);
                })
            .Produces<BookClass>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);



            //PUT
            route.MapPut("{BookID:guid}",
                async (Guid BookID, BookRequest req, BookApiContext context) =>
                {
                    // Validação da requisição
                    if (req == null)
                        return Results.BadRequest("Request inválida.");

                    if (string.IsNullOrEmpty(req.BookTitle))
                        return Results.BadRequest("O título do livro é obrigatório.");

                    // Busca o livro no banco de dados, incluindo os gêneros associados
                    var book = await context.TblBook
                        .Include(b => b.Genres) // Carrega os gêneros associados ao livro
                        .FirstOrDefaultAsync(x => x.BookID == BookID);

                    if (book == null)
                        return Results.NotFound("Livro não encontrado.");

                    // Busca os novos gêneros no banco de dados
                    var newAuthors = await context.TblAuthor
                        .Where(a => req.AuthorIDs.Contains(a.AuthorID))
                        .ToListAsync();

                    if (newAuthors == null || newAuthors.Count != req.AuthorIDs.Count)
                        return Results.BadRequest("Um ou mais autores não foram encontrados.");
                    
                    // Busca os novos gêneros no banco de dados
                    var newGenres = await context.TblGenre
                        .Where(g => req.GenreIDs.Contains(g.GenreID))
                        .ToListAsync();

                    if (newGenres == null || newGenres.Count != req.GenreIDs.Count)
                        return Results.BadRequest("Um ou mais gêneros não foram encontrados.");

                    // Atualiza as propriedades básicas do livro
                    book.BookTitle = req.BookTitle;
                    book.BookLanguage = req.BookLanguage;
                    book.BookPublisher = req.BookPublisher;
                    book.BookRating = req.BookRating;
                    book.BookCoverPage = req.BookCoverPage;

                    // Atualiza os autores associados ao livro
                    book.Authors = newAuthors;

                    // Atualiza os gêneros associados ao livro
                    book.Genres = newGenres;

                    //  Atualiza as tags
                    book.BookTags = string.Join(",", req.BookTags ?? new List<string>());


                    // Salva as alterações no banco de dados
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