using System.ComponentModel.DataAnnotations;

namespace DevInsightAPI.DTOs
{
    public class UpdateTaskStatusDTO
    {
        [Required]
        [StringLength(40)]
        public string Status { get; set; } = string.Empty;
    }
}
