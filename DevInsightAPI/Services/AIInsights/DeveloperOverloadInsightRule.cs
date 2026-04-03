using DevInsightAPI.Constants;

namespace DevInsightAPI.Services.AIInsights
{
    public class DeveloperOverloadInsightRule : IAIInsightRule
    {
        public IEnumerable<string> Evaluate(AIInsightsContext context)
        {
            var developerNames = context.Users
                .Where(user => string.Equals(user.Role, UserRoles.Developer, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(user => user.Id, user => user.Name);

            return context.Tasks
                .Where(task => task.AssignedUserId.HasValue && !IsCompleted(task.Status))
                .GroupBy(task => task.AssignedUserId!.Value)
                .Where(group => group.Count() > 3 && IsDeveloperGroup(group, developerNames))
                .Select(group => ResolveDeveloperName(group.Key, group.First().AssignedUser?.Name, developerNames))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .Select(name => $"Dev {name} is overloaded")
                .ToList();
        }

        private static bool IsCompleted(string status)
        {
            return string.Equals(status, "Done", StringComparison.Ordinal);
        }

        private static bool IsDeveloperGroup(
            IGrouping<int, DevInsightAPI.Models.TaskItem> group,
            IReadOnlyDictionary<int, string> developerNames)
        {
            if (developerNames.ContainsKey(group.Key))
            {
                return true;
            }

            return string.Equals(
                group.First().AssignedUser?.Role,
                UserRoles.Developer,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolveDeveloperName(
            int userId,
            string? assignedUserName,
            IReadOnlyDictionary<int, string> developerNames)
        {
            if (developerNames.TryGetValue(userId, out var name) &&
                !string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            if (!string.IsNullOrWhiteSpace(assignedUserName))
            {
                return assignedUserName;
            }

            return $"#{userId}";
        }
    }
}
