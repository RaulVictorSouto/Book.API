using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Book.Shared.Models.Modelos
{
    public class BookClass
    {
        [Key]
        public Guid BookID { get; set; }
        [Required(ErrorMessage = "O título do livro é obrigatório")]
        [StringLength(150, ErrorMessage = "O título não pode exceder 100 caracteres")]
        public string BookTitle { get; set; }
        [Required(ErrorMessage = "O idioma do livro é obrigatório")]
        [StringLength(50, ErrorMessage = "O idioma não pode exceder 50 caracteres")]
        public string BookLanguage { get; set; }
        [Required(ErrorMessage = "A editora do livro é obrigatória")]
        [StringLength(50, ErrorMessage = "A editora não pode exceder 50 caracteres")]
        public string BookPublisher { get; set; }
        [Required(ErrorMessage = "O ISBN do livro é obrigatório")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN deve ter entre 10 e 13 caracteres")]
        public string BookISBN
        {
            get => _bookISBN;
            set => _bookISBN = LimparISBN(value);
        }

        // Campo privado para armazenar o ISBN limpo
        private string _bookISBN;


        [Required(ErrorMessage = "A classificação do livro é obrigatória")]
        public string BookRating { get; set; }
        public byte[]? BookCoverPage { get; set; }


        [JsonIgnore]
        public string BookTags { get; set; }

        //armazenamento e tratamento de tags
        [NotMapped]
        public List<string> BookTagsList
        {
            get => string.IsNullOrWhiteSpace(BookTags)
                ? new List<string>() 
                : BookTags.Split(',').Select(tag => tag.Trim().Trim('\'')).ToList();

            set => BookTags = value != null
                 ? string.Join(", ", value.Select(tag => $"'{tag}'")) 
                : "";
        }

        ////FK para autor
        //public int AuthorID { get; set; } 
        //public AuthorClass Author { get; set; }

        //Relacionamento para autores
        public ICollection<AuthorClass> Authors { get; set; }

        //Relacionamento para genero
        public ICollection<GenreClass> Genres { get; set; }

        //Construtor
        public BookClass(string title, string language, string publisher, string isbn, string rating, byte[] coverPage)
        {
            BookID = Guid.NewGuid();
            BookTitle = title;
            BookLanguage = language;
            BookPublisher = publisher;
            BookISBN = isbn;
            BookRating = rating;
            BookCoverPage = coverPage ?? Array.Empty<byte>();
            Authors = new List<AuthorClass>();
            Genres = new List<GenreClass>();
            BookTagsList = new List<string>();
        }

        // Construtor sem parâmetros (obrigatório para o EF Core)
        public BookClass()
        {
            Authors = new List<AuthorClass>();
            Genres = new List<GenreClass>();
            BookTagsList = new List<string>();
        }

        // Método utilitário para limpar o ISBN (remover pontos, traços, espaços, etc.)
        private string LimparISBN(string isbn)
        {
            return new string(isbn?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        }

    }
}
