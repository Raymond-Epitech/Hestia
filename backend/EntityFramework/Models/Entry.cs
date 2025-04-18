﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Entry
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid ColocationId { get; set; }

        [ForeignKey("ColocationId")]
        public Colocation Colocation { get; set; } = null!;

        [Required]
        public Guid ExpenseId { get; set; }

        [ForeignKey("ExpenseId")]
        public Expense Expense { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(19,2)")]
        public decimal Amount { get; set; }
    }
}
