namespace DevInsightAPI.Services.AIInsights
{
    public class ProjectRiskInsightRule : IAIInsightRule
    {
        public IEnumerable<string> Evaluate(AIInsightsContext context)
        {
            var projectNames = context.Projects
                .ToDictionary(project => project.Id, project => project.Name);

            return context.Tasks
                .GroupBy(task => task.ProjectId)
                .Select(group => new
                {
                    ProjectId = group.Key,
                    TotalTasks = group.Count(),
                    CompletedTasks = group.Count(task => string.Equals(task.Status, "Done", StringComparison.Ordinal))
                })
                .Where(project => project.TotalTasks > 0)
                .Select(project => new
                {
                    project.ProjectId,
                    CompletionRate = project.CompletedTasks / (double)project.TotalTasks
                })
                .Where(project => project.CompletionRate < 0.5d)
                .Select(project => ResolveProjectName(project.ProjectId, projectNames))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .Select(name => $"Project {name} is at risk")
                .ToList();
        }

        private static string ResolveProjectName(
            int projectId,
            IReadOnlyDictionary<int, string> projectNames)
        {
            if (projectNames.TryGetValue(projectId, out var name) &&
                !string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return $"#{projectId}";
        }
    }
}
