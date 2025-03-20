using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.Shared.Models.Modelos
{
    public class AuthorClass
    {
        [Key]
        public int AuthorID { get; set; } 
        public string AuthorName { get; set; }


        //Relaccionamento com livro
        public ICollection<BookClass> Books { get; set; }


        //Construtor
        public AuthorClass(string name) 
        {
            AuthorName = name;
            Books = new List<BookClass>();
        }

        // Construtor sem parâmetros (obrigatório para o EF Core)
        public AuthorClass()
        {
            Books = new List<BookClass>();
        }

    }
}
