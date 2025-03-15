using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Context
{
    public class HestiaContext : DbContext
    {
        public virtual DbSet<Colocation> Colocations { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; } = null!;
        public virtual DbSet<Chore> Chores { get; set; } = null!;
        public virtual DbSet<ChoreMessage> ChoreMessages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<ChoreEnrollment> ChoreEnrollments { get; set; } = null!;
        public virtual DbSet<Expense> Expenses { get; set; } = null!;
        public virtual DbSet<Entry> Entries { get; set; } = null!;
        public virtual DbSet<Balance> Balances { get; set; } = null!;

        public HestiaContext(DbContextOptions<HestiaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Colocation>(c =>
            {
                c.HasMany(x => x.Users)
                    .WithOne(x => x.Colocation)
                    .HasForeignKey(x => x.ColocationId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                c.HasMany(x => x.Chores)
                    .WithOne(x => x.Colocation)
                    .HasForeignKey(x => x.ColocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                c.HasMany(x => x.Reminders)
                    .WithOne(x => x.Colocation)
                    .HasForeignKey(x => x.ColocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                c.HasMany(x => x.Expenses)
                    .WithOne(x => x.Colocation)
                    .HasForeignKey(x => x.ColocationId)
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

                c.HasMany(x => x.Balances)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                c.HasMany(x => x.Entries)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ChoreEnrollment>(c =>
            {
                c.HasKey(x => new { x.UserId, x.ChoreId });
            });
            modelBuilder.Entity<Expense>(c =>
            {
                c.HasMany(x => x.Entries)
                    .WithOne(x => x.Expense)
                    .HasForeignKey(x => x.ExpenseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}