using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> Options) : base(Options) 
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Entities.Transaction> Transactions => Set<Entities.Transaction>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Biller> Billers => Set<Biller>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Account>().HasKey(a => a.Id);
            modelBuilder.Entity<Entities.Transaction>().HasKey(t => t.Id);
            modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);
            modelBuilder.Entity<Biller>().HasKey(b => b.Id);
            modelBuilder.Entity<Biller>().Property(b => b.Category).HasConversion<string>();
            modelBuilder.Entity<Biller>().HasIndex(b => b.Code).IsUnique();
           
          
        }


    }
}
