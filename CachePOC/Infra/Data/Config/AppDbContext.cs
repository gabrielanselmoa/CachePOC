using CachePOC.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CachePOC.Infra.Data.Config;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(100).IsRequired();
        modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(255);
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>().Property(p => p.Quantity).HasDefaultValue(0);
    }
}