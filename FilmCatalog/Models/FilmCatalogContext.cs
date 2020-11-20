using Microsoft.EntityFrameworkCore;

namespace FilmCatalog.Models
{
    public class FilmCatalogContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Film> Films { get; set; }

        public FilmCatalogContext(DbContextOptions<FilmCatalogContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
