using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Book.Shared.Models.Modelos
{
    public class GenreClass
    {
        [Key]
        public int GenreID { get; set; }
        [Required(ErrorMessage = "O nome do gênero é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome do gênero não pode exceder 50 caracteres")]
        public string GenreName { get; set; }


        //Relaccionamento com livro
        [JsonIgnore]
        public ICollection<BookClass> Books { get; set; }

        //Construtor
        public GenreClass(string name) 
        { 
            GenreName = name;
            Books = new List<BookClass>();
        }

        // Construtor sem parâmetros (obrigatório para o EF Core)
        public GenreClass()
        {
            Books = new List<BookClass>();
        }

    }
}
