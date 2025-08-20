using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input
{
    public class ChoreMessageInput
    {
        [Required]
        public Guid ColocationId { get; set; }

        [Required]
        public Guid ChoreId { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public string Content { get; set; } = null!;
    }
}
