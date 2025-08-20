namespace Shared.Models.Output;

public class RefundOutput
{
    public Guid From { get; set; }
    public string FromUrlPP { get; set; } = "default.jpg";
    public Guid To { get; set; }
    public string ToUrlPP { get; set; } = "default.jpg";
    public decimal Amount { get; set; }
}

