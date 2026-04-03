using System.ComponentModel.DataAnnotations;

namespace DevInsightAPI.DTOs
{
    public class UpdateUserDTO
    {
        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(180)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Role { get; set; } = string.Empty;

        public string? Password { get; set; }
    }
}
