using Microsoft.EntityFrameworkCore;
using Task_TrueTech.Models;

namespace Task_TrueTech.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Item> items { get; set; }
    public DbSet<Stock> stocks { get; set; }
    public DbSet<StockItem> stockItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>()
           .HasOne(bi => bi.stock)
           .WithMany(b => b.StockItems)
           .HasForeignKey(bi => bi.StockId);

        modelBuilder.Entity<StockItem>()
            .HasOne(bi => bi.item)
            .WithMany(i => i.StockItems)
            .HasForeignKey(bi => bi.ItemId);
    }
}

