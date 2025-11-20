using Shared.Models.DTO;

namespace Business.Interfaces;

public interface IReceiptScannerService
{
    Task<ReceiptScanResult> ScanReceiptAsync(Stream imageStream, string contentType);
}