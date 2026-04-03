namespace DevInsightAPI.DTOs
{
    public class TaskMovedRealtimeEventDTO : TaskRealtimeEventDTO
    {
        public string PreviousStatus { get; set; } = string.Empty;

        public string CurrentStatus { get; set; } = string.Empty;
    }
}
