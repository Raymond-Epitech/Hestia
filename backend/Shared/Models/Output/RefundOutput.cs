namespace Shared.Models.Output;

public class RefundOutput
{
    public Guid From { get; set; }
    public Guid To { get; set; }
    public decimal Amount { get; set; }
}

