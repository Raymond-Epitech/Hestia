namespace Shared.Models.Output;

public class RefundOutput
{
    public Guid From { get; set; }
    public string FromUsername { get; set; } = "";
    public Guid To { get; set; }
    public string ToUsername { get; set; } = "";
    public decimal Amount { get; set; }
}

