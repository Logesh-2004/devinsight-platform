namespace DevInsightAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();

        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
