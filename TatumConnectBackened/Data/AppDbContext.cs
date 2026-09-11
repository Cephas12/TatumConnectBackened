using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Transactions;
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
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Account>().HasKey(a => a.Id);
            modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);
           
          
        }


    }
}
