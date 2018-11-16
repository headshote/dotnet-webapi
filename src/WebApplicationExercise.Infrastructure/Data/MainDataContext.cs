using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using WebApplicationExercise.Core.Models;

namespace WebApplicationExercise.Infrastructure.Data
{
    public class MainDataContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id)
                .Property(o => o.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Product>()
                .HasKey(p => p.Id)
                .Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithRequired(p => p.Order)
                .WillCascadeOnDelete(true);
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}