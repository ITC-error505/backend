using Microsoft.EntityFrameworkCore;

namespace Pizza_Games_Endpoints.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Score> Scores { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.id);

                entity.Property(a => a.username).IsRequired();

                entity.HasIndex(a => a.username).IsUnique();

                entity.Property(a => a.password).IsRequired();
            });

            builder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.id);
                entity.Property(g => g.name).IsRequired();
                entity.HasIndex(g => g.name).IsUnique();
            });

            builder.Entity<Score>(entity =>
            {
                entity.HasKey(s => s.id);
                entity.Property(s => s.accountId).IsRequired();
                entity.Property(s => s.gameId).IsRequired();
                entity.Property(s => s.score).IsRequired();
                entity.HasOne(s => s.Account).WithMany().HasForeignKey(s => s.accountId);
                entity.HasOne(s => s.Game).WithMany().HasForeignKey(s => s.gameId);
            });
        }
    }
}
