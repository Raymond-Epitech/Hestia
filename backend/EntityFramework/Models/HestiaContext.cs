using System;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyConsoleApp.Models
{
    public partial class entitycoreContext : DbContext
    {
        public virtual DbSet<Book> Book { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Host=localhost;Database=entitycore;Username=postgres;Password=mypassword");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("book", "account");
                entity.Property(e => e.Id)
                                    .HasColumnName("id")
                                    .HasDefaultValueSql("nextval('account.item_id_seq'::regclass)");
                entity.Property(e => e.BookName).HasColumnName("Name");
            });
            modelBuilder.HasSequence("item_id_seq", "account");
        }
    }
}