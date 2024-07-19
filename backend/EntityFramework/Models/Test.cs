
namespace BookStoreApi.Models;

public class Book
{
    public Guid? Id { get; set; }

    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!;
}