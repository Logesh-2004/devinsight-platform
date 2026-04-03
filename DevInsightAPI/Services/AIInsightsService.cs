using DevInsightAPI.Models;
using DevInsightAPI.Repositories;
using DevInsightAPI.Services.AIInsights;

namespace DevInsightAPI.Services
{
    public class AIInsightsService : IAIInsightsService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEnumerable<IAIInsightRule> _rules;
        private readonly ICurrentUserContext _currentUserContext;

        public AIInsightsService(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IEnumerable<IAIInsightRule> rules,
            ICurrentUserContext currentUserContext)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _rules = rules;
            _currentUserContext = currentUserContext;
        }

        public async Task<List<string>> GetInsightsAsync()
        {
            var context = await BuildContextAsync();

            return _rules
                .SelectMany(rule => rule.Evaluate(context))
                .Where(insight => !string.IsNullOrWhiteSpace(insight))
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        private async Task<AIInsightsContext> BuildContextAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();
            var projects = await _projectRepository.GetAllAsync();

            if (_currentUserContext.IsDeveloper && _currentUserContext.UserId.HasValue)
            {
                tasks = tasks
                    .Where(task => task.AssignedUserId == _currentUserContext.UserId.Value)
                    .ToList();

                var visibleProjectIds = tasks
                    .Select(task => task.ProjectId)
                    .Distinct()
                    .ToHashSet();

                projects = projects
                    .Where(project => visibleProjectIds.Contains(project.Id))
                    .ToList();

                users = users
                    .Where(user => user.Id == _currentUserContext.UserId.Value)
                    .ToList();
            }

            var todayUtc = DateTime.UtcNow.Date;
            var startOfCurrentWeekUtc = GetStartOfWeek(todayUtc);

            return new AIInsightsContext
            {
                Tasks = tasks,
                Users = users,
                Projects = projects,
                TodayUtc = todayUtc,
                StartOfCurrentWeekUtc = startOfCurrentWeekUtc,
                StartOfPreviousWeekUtc = startOfCurrentWeekUtc.AddDays(-7),
                StartOfNextWeekUtc = startOfCurrentWeekUtc.AddDays(7)
            };
        }

        private static DateTime GetStartOfWeek(DateTime dateUtc)
        {
            var daysSinceMonday = ((int)dateUtc.DayOfWeek + 6) % 7;
            return dateUtc.AddDays(-daysSinceMonday);
        }
    }
}
