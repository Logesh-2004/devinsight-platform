using System.ComponentModel.DataAnnotations;

namespace DevInsightAPI.DTOs
{
    public class UpdateTaskDTO
    {
        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1200, MinimumLength = 5)]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }

        public int? AssignedUserId { get; set; }

        [Required]
        [StringLength(40)]
        public string Priority { get; set; } = "Medium";

        [Required]
        [StringLength(40)]
        public string Status { get; set; } = "Todo";

        public DateTime DueDate { get; set; }
    }
}
