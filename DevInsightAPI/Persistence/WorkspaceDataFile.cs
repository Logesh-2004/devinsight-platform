namespace DevInsightAPI.Persistence
{
    public class WorkspaceDataFile
    {
        public List<StoredUser> Users { get; set; } = [];

        public List<StoredProject> Projects { get; set; } = [];

        public List<StoredTask> Tasks { get; set; } = [];

        public List<StoredNotification> Notifications { get; set; } = [];
    }

    public class StoredUser
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    public class StoredProject
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int? CreatedByUserId { get; set; }
    }

    public class StoredTask
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Todo";

        public string Priority { get; set; } = "Medium";

        public int ProjectId { get; set; }

        public int? AssignedUserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class StoredNotification
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public int UserId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
