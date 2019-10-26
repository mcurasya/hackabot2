using hackabot2.Db.Model;
using Microsoft.EntityFrameworkCore;

namespace hackabot2.Db
{
    public class TelegramContext : DbContext
    {
        public DbSet<Account>          Accounts  { get; set; }
        public DbSet<Channel>          Channels  { get; set; }
        public DbSet<Meme>             Memes     { get; set; }
        public DbSet<Order>            Orders    { get; set; }
        public DbSet<Image>            Images    { get; set; }
        public DbSet<ShoppingCart>     Carts     { get; set; }
        public DbSet<ShoppingCartItem> CartItems { get; set; }
        public DbSet<Text>             Texts     { get; set; }
        public DbSet<Like>             Likes     { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseNpgsql(@"host=localhost;database=telegram;user id=postgres;password=139742685Aa;");
            //optionsBuilder.UseNpgsql(@"host=localhost;database=telegram;user id=kvasdimas;password=yeba;");
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                        .HasKey(
                            a => a.Id
                        );
            modelBuilder.Entity<Text>()
                        .HasKey(c => new {c.Key, c.Language});
        }
    }
}