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
        public virtual DbSet<ShoppingItem> ShoppingItems { get; set; } = null!;
        public virtual DbSet<FCMDevice> FCMDevices { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<PollVote> PollVotes { get; set; } = null!;
        public virtual DbSet<Reaction> Reactions { get; set; } = null!;

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

                c.HasMany(x => x.Messages)
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

                c.HasMany(x => x.Messages)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.SentBy)
                    .OnDelete(DeleteBehavior.NoAction);

                c.HasMany(x => x.Chores)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                c.HasMany(x => x.ChoreMessages)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull);

                c.HasMany(u => u.FCMDevices)
                    .WithMany(d => d.Users)
                    .UsingEntity(j => j.ToTable("UserFCMDevices"));

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

            modelBuilder.Entity<Reminder>(c =>
            {
                c.HasMany(x => x.Reactions)
                    .WithOne(x => x.Reminder)
                    .HasForeignKey(x => x.ReminderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Reminder>()
                .HasDiscriminator<string>("ReminderType")
                .HasValue<TextReminder>("Text")
                .HasValue<ImageReminder>("Image")
                .HasValue<ShoppingListReminder>("ShoppingList")
                .HasValue<PollReminder>("Poll");

            modelBuilder.Entity<PollReminder>(c =>
            {
                c.HasMany(x => x.PollVotes)
                    .WithOne(x => x.PollReminder)
                    .HasForeignKey(x => x.PollReminderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ShoppingListReminder>(c =>
            {
                c.HasMany(x => x.ShoppingItems)
                    .WithOne(x => x.ShoppingListReminder)
                    .HasForeignKey(x => x.ShoppingListReminderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}