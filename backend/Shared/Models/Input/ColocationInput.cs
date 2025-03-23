using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input
{
    public class ColocationInput
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public Guid CreatedBy { get; set; }
    }
}
