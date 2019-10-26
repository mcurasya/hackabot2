using Microsoft.EntityFrameworkCore;

namespace hackabot
{
    public class TelegramContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
<<<<<<< HEAD
        
=======
        public DbSet<Board> Boards { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<WorkerToBoard> WorkerToBoards { get; set; }
>>>>>>> 243067e4ed20f99b098eddad7c4b6458e24e99e1
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