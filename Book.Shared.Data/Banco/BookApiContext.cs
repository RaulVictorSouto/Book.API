using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Book.Shared.Models.Modelos;
using Microsoft.Extensions.Configuration;

namespace Book.Shared.Data.Banco
{
    public class BookApiContext : DbContext
    {
        public DbSet<BookClass> TblBook { get; set; }
        public DbSet<AuthorClass> TblAuthor { get; set; }
        public DbSet<GenreClass> TblGenre { get; set; }

        private readonly IConfiguration _configuration;


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(connectionString: "Server=localhost;Database=SistBook;User Id=sa;Password=1234;TrustServerCertificate=True;");
        //    base.OnConfiguring(optionsBuilder);
        //}

        public BookApiContext(DbContextOptions<BookApiContext> options, IConfiguration configuration)
           : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            // Relacionamento muitos-para-muitos entre BookClass e AuthorClass
            modelBuilder.Entity<BookClass>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity(j => j.ToTable("TblBookAuthors"));

            // Relacionamento muitos-para-muitos entre BookClass e GenreClass
            modelBuilder.Entity<BookClass>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity(j => j.ToTable("TblBookGenres"));
        }
    }
}
