using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.Shared.Models.Modelos
{
    public class BookClass
    {
        [Key]
        public Guid BookID { get; set; }
        public string BookTitle { get; set; }
        public string BookLanguage { get; set; }
        public string BookPublisher { get; set; }
        public string BookISBN { get; set; }
        public string BookRating { get; set; }
        public byte[] BookCoverPage { get; set; }

        //FK para autor
        public int AuthorID { get; set; } 
        public AuthorClass Author { get; set; }

        //Relacionamento para genero
        public ICollection<GenreClass> Genres { get; set; }

        //Construtor
        public BookClass(string title, string language, string publisher, string isbn, string rating, int authorId, byte[] coverPage)
        {
            BookID = Guid.NewGuid();
            BookTitle = title;
            BookLanguage = language;
            BookPublisher = publisher;
            BookISBN = isbn;
            BookRating = rating;
            AuthorID = authorId;
            BookCoverPage = coverPage ?? Array.Empty<byte>();
            Genres = new List<GenreClass>();
        }

        // Construtor sem parâmetros (obrigatório para o EF Core)
        public BookClass()
        {
            Genres = new List<GenreClass>();
        }
    }
}
