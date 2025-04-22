using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Book.Shared.Data.Banco;

namespace Book.Shared.Data
{
    public class BookApiContextFactory : IDesignTimeDbContextFactory<BookApiContext>
    {
        public BookApiContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.GetFullPath(Path.Combine("..", "Book.API", "appsettings.json")))
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BookApiContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new BookApiContext(optionsBuilder.Options, configuration);
        }
    }

}
