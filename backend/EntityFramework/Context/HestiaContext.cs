using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Context
{
    public class HestiaContext : DbContext
    {
        public virtual DbSet<Reminder> Reminder { get; set; } = null!;

        public HestiaContext(DbContextOptions<HestiaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}