using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class Expense
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(19,2)")]
    public decimal Amount { get; set; }

    [Required]
    public Guid PaidBy { get; set; }

    [ForeignKey("PaidBy")]
    public User User { get; set; } = null!;

    [Required]
    public string SplitType { get; set; } = "Evenly";

    [Required]
    public DateTime DateOfPayment { get; set; }

    [Required]
    public Guid ExpenseCategoryId { get; set; }

    [ForeignKey("ExpenseCategoryId")]
    public ExpenseCategory ExpenseCategory { get; set; } = null!;

    public ICollection<SplitBetween> SplitBetweens { get; set; } = null!;
    public ICollection<Entry> Entries { get; set; } = null!;
}
