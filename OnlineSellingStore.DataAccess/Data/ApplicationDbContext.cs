using Microsoft.EntityFrameworkCore;
using OnlineSellingStore.Models;

namespace OnlineSellingStore.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 2, Name="Action", DisplayOrder = 2 },
                new Category { Id = 3, Name="Sci-Fi", DisplayOrder = 3 },
                new Category { Id = 4, Name="History", DisplayOrder = 4 }
                );
        }

    }
}
