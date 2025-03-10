using Microsoft.EntityFrameworkCore;

namespace EF_Core
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        // list of root entities
        public DbSet<Recipe> Recipes { get; set; }
    }
}
