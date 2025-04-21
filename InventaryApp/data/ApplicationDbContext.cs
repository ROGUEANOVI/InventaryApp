using InventaryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventaryApp.data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {}

        public DbSet<Product> Products { get; set; }
    }
}
