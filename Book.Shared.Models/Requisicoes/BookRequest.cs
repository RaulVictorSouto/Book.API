using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Book.Shared.Models.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Book.Shared.Models.Requisicoes
{
    public record BookRequest(
       string BookTitle,
       string BookLanguage,
       string BookPublisher,
       string BookISBN,
       string BookRating,
       byte[] BookCoverPage,
       //string? BookCoverPage,
       List<int>? AuthorIDs = null,
       List<int>? GenreIDs = null, 
       List<string>? BookTags = null
   )
    { 
        // Inicializa AuthorIDs, GenreIDs e BookTags se for null

        public List<int> AuthorIDs { get; init; } = AuthorIDs ?? new List<int>();
        public List<int> GenreIDs { get; init; } = GenreIDs ?? new List<int>();
        public List<string> BookTags { get; init; } = BookTags ?? new List<string>();

    }
}