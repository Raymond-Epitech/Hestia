using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models;

public class ExpenseAutomation
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [Range(1, 31)]
    public int DayOfTheMonth { get; set; }

    public ICollection<Expense> Expenses { get; set; } = null!;
}

