using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Context
{
    public class HestiaContext : DbContext
    {
        public virtual DbSet<Reminder> Reminder { get; set; } = null!;
        public virtual DbSet<Chore> Chore { get; set; } = null!;
        public virtual DbSet<ChoreMessage> ChoreMessage { get; set; } = null!;

        public HestiaContext(DbContextOptions<HestiaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chore>(c =>
            {
                c.HasMany(x => x.ChoreMessages)
                    .WithOne(x => x.Chore)
                    .HasForeignKey(x => x.ChoreId);
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}