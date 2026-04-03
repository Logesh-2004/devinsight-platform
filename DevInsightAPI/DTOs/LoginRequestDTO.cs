using System.ComponentModel.DataAnnotations;

namespace DevInsightAPI.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        [StringLength(180)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
