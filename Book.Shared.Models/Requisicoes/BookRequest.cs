using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book.Shared.Models.Modelos;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Book.Shared.Models.Requisicoes
{
    public record BookRequest(
       string BookTitle,
       string BookLanguage,
       string BookPublisher,
       string BookISBN,
       string BookRating,
       int AuthorID,
       byte[] BookCoverPage,
       List<int>? GenreIDs = null, // Usar null como valor padrão
       List<string>? BookTags = null
   )
    { 
        // Inicializa GenreIDs e BookTags se for null
        public List<int> GenreIDs { get; init; } = GenreIDs ?? new List<int>();
        public List<string> BookTags { get; init; } = BookTags ?? new List<string>();
    }
}