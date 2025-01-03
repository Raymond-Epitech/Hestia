using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Context
{
    public class HestiaContext : DbContext
    {
        public virtual DbSet<Collocation> Collocation { get; set; }
        public virtual DbSet<Reminder> Reminder { get; set; } = null!;
        public virtual DbSet<Chore> Chore { get; set; } = null!;
        public virtual DbSet<ChoreMessage> ChoreMessage { get; set; } = null!;
        public virtual DbSet<User> User { get; set; } = null!;
        public virtual DbSet<ChoreEnrollment> ChoreEnrollments { get; set; } = null!;

        public HestiaContext(DbContextOptions<HestiaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collocation>(c =>
            {
                c.HasMany(x => x.Users)
                    .WithOne(x => x.Collocation)
                    .HasForeignKey(x => x.CollocationId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                c.HasMany(x => x.Chores)
                    .WithOne(x => x.Collocation)
                    .HasForeignKey(x => x.CollocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                c.HasMany(x => x.Reminders)
                    .WithOne(x => x.Collocation)
                    .HasForeignKey(x => x.CollocationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Chore>(c =>
            {
                c.HasMany(x => x.ChoreMessages)
                    .WithOne(x => x.Chore)
                    .HasForeignKey(x => x.ChoreId)
                    .OnDelete(DeleteBehavior.Cascade);

                c.HasMany(x => x.ChoreEnrollments)
                    .WithOne(x => x.Chore)
                    .HasForeignKey(x => x.ChoreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<User>(c =>
            {
                c.HasMany(x => x.ChoreEnrollments)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ChoreEnrollment>(c =>
            {
                c.HasKey(x => new { x.UserId, x.ChoreId });
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}