using System.Data.Entity;
using WebApplicationExercise.Core.Models;

namespace WebApplicationExercise.Infrastructure.Data
{
    public class MainDataContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}