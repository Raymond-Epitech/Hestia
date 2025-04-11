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
        public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; } = null!;
        public virtual DbSet<Entry> Entries { get; set; } = null!;
        public virtual DbSet<SplitBetween> SplitBetweens { get; set; } = null!;
        public virtual DbSet<ShoppingList> ShoppingList { get; set; } = null!;
        public virtual DbSet<ShoppingItem> ShoppingItems { get; set; } = null!;

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

                c.HasMany(x => x.ExpenseCategories)
                    .WithOne(x => x.Colocation)
                    .HasForeignKey(x => x.ColocationId)
                    .OnDelete(DeleteBehavior.Cascade);
                c.HasMany(x => x.ShoppingLists)
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

                c.HasMany(x => x.Entries)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                c.HasMany(x => x.SplitBetweens)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<ChoreEnrollment>(c =>
            {
                c.HasKey(x => new { x.UserId, x.ChoreId });
            });
            modelBuilder.Entity<SplitBetween>(c =>
            {
                c.HasKey(x => new { x.UserId, x.ExpenseId });
            });
            modelBuilder.Entity<Expense>(c =>
            {
                c.HasMany(x => x.Entries)
                    .WithOne(x => x.Expense)
                    .HasForeignKey(x => x.ExpenseId)
                    .OnDelete(DeleteBehavior.Cascade);
                c.HasMany(x => x.SplitBetweens)
                    .WithOne(x => x.Expense)
                    .HasForeignKey(x => x.ExpenseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ExpenseCategory>(c =>
            {
                c.HasMany(x => x.Expenses)
                    .WithOne(x => x.ExpenseCategory)
                    .HasForeignKey(x => x.ExpenseCategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ShoppingList>(c =>
            {
                c.HasMany(x => x.ShoppingItems)
                    .WithOne(x => x.ShoppingList)
                    .HasForeignKey(x => x.ShoppingListId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}