namespace Shared.Models.DTO;

public class ReceiptScanResult
{
    public decimal TotalAmount { get; set; }
    public DateTime? DateOfPurchase { get; set; }
    public List<ReceiptItemResult> Items { get; set; } = new();
}

public class ReceiptItemResult
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}