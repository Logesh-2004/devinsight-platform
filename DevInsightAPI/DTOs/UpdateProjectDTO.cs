using System.ComponentModel.DataAnnotations;

namespace DevInsightAPI.DTOs
{
    public class UpdateProjectDTO
    {
        [Required]
        [StringLength(140, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(600, MinimumLength = 5)]
        public string Description { get; set; } = string.Empty;

        public int? CreatedByUserId { get; set; }
    }
}
