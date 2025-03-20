using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Book.Shared.Models.Modelos;

namespace Book.Shared.Data.Banco
{
    public class BookApiContext : DbContext
    {
        public DbSet<BookClass> TblBook { get; set; }
        public DbSet<AuthorClass> TblAuthor { get; set; }
        public DbSet<GenreClass> TblGenre { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: "Server=localhost;Database=SistBook;User Id=sa;Password=1234;TrustServerCertificate=True;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorClass>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorID);

            modelBuilder.Entity<GenreClass>()
                .HasMany(g => g.Books)
                .WithMany(b => b.Genres)
                .UsingEntity(j => j.ToTable("TblBookGenres"));
        }
    }
}
