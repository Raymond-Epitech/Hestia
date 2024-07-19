using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace MyConsoleApp.Models
{
    public class HestiaContext : DbContext
    {
        public virtual DbSet<Reminder> Reminder { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Reminder>(e =>
            {
                e.HasMany(x => x.Roles)
                    .WithMany(x => x.Users)
                    .UsingEntity<RoleUser>(
                        x => x.HasOne(c => c.Role).WithMany(c => c.RoleUsers),
                        x => x.HasOne(c => c.User).WithMany(c => c.RoleUsers));
                e.HasMany(x => x.Candidates).WithOne(x => x.Recruiter);
                e.HasMany(x => x.Interviews).WithOne(x => x.Responsible).HasForeignKey(x => x.ResponsibleId);
            });*/
        }
    }
}