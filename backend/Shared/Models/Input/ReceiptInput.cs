using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class ReceiptInput
{
    [Required]
    public Guid ColocationId { get; set; }

    [Required]
    public IFormFile ReceiptPicture { get; set; }
}
