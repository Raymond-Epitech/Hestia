using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input
{
    public class ChoreMessageInput
    {
        [Required]
        public Guid ChoreId { get; set; }

        [Required]
        public string CreatedBy { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
    }
}
