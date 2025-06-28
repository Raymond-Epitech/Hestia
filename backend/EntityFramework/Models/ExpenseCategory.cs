using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class ExpenseCategory
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ColocationId { get; set; }

    [ForeignKey("ColocationId")]
    public Colocation Colocation { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

<<<<<<<< HEAD:backend/EntityFramework/Models/ExpenseCategory.cs
    public ICollection<Expense> Expenses { get; set; } = null!;
========
    public ICollection<ShoppingItem> ShoppingItems { get; set; } = null!;
>>>>>>>> develop:backend/EntityFramework/Models/ShoppingList.cs
}

