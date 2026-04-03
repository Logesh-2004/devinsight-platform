namespace DevInsightAPI.Constants
{
    public static class TaskMetadata
    {
        public static readonly string[] AllowedStatuses = ["Todo", "InProgress", "Done"];

        public static readonly string[] AllowedPriorities = ["Low", "Medium", "High", "Critical"];

        public static bool IsValidStatus(string status)
        {
            return AllowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidPriority(string priority)
        {
            return AllowedPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase);
        }

        public static string NormalizeStatus(string status)
        {
            return AllowedStatuses.First(allowed =>
                allowed.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        public static string NormalizePriority(string priority)
        {
            return AllowedPriorities.First(allowed =>
                allowed.Equals(priority, StringComparison.OrdinalIgnoreCase));
        }
    }
}
