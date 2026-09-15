using Microsoft.EntityFrameworkCore;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Core entities
        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Entities.Transaction> Transactions => Set<Entities.Transaction>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // Billing / product entities
        public DbSet<Biller> Billers => Set<Biller>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductItem> ProductItems => Set<ProductItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Account
            modelBuilder.Entity<Account>().HasKey(a => a.Id);

            // Transaction
            modelBuilder.Entity<Entities.Transaction>().HasKey(t => t.Id);

            // RefreshToken
            modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);

            // Biller
            modelBuilder.Entity<Biller>().HasKey(b => b.Id);
            modelBuilder.Entity<Biller>().Property(b => b.Category).HasConversion<string>();
            modelBuilder.Entity<Biller>().HasIndex(b => b.Code).IsUnique();

            // Product
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<Product>().Property(p => p.Category).HasConversion<string>();
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Biller)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BillerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductItem
            modelBuilder.Entity<ProductItem>().HasKey(pi => pi.Id);
            modelBuilder.Entity<ProductItem>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductItems)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
