using Microsoft.EntityFrameworkCore;

namespace hackabot
{
    public class TelegramContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasKey(
                    a => a.Id
                );
        }
    }
}