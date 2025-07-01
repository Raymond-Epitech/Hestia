using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update
{
    public class ColocationUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;
    }
}
