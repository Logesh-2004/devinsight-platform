namespace DevInsightAPI.Constants
{
    public static class HubEventNames
    {
        public const string TaskCreated = "TaskCreated";
        public const string TaskUpdated = "TaskUpdated";
        public const string TaskMoved = "TaskMoved";
        public const string NotificationCreated = "NotificationCreated";
        public const string NotificationRead = "NotificationRead";
        public const string AdminGroup = "role:admin";

        public static string UserGroup(int userId) => $"user:{userId}";
    }
}
