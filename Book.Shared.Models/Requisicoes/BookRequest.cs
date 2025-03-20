using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book.Shared.Models.Modelos;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Book.Shared.Models.Requisicoes
{
    public record BookRequest (
        string BookTitle, 
        string BookLanguage, 
        string BookPublisher,
        string BookISBN,
        string BookRating,
        int AuthorID,
        byte[] BookCoverPage);
}