using DevInsightAPI.DTOs;
using DevInsightAPI.Repositories;

namespace DevInsightAPI.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public DashboardService(
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            ITaskRepository taskRepository,
            ICurrentUserContext currentUserContext)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<DashboardDTO> GetDashboardData()
        {
            var projects = await _projectRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();
            var tasks = await _taskRepository.GetAllAsync();

             if (_currentUserContext.IsDeveloper && _currentUserContext.UserId.HasValue)
            {
                tasks = tasks
                    .Where(task => task.AssignedUserId == _currentUserContext.UserId.Value)
                    .ToList();
            }

            int totalProjects = _currentUserContext.IsDeveloper
                ? tasks.Select(task => task.ProjectId).Distinct().Count()
                : projects.Count;

            int totalUsers = _currentUserContext.IsDeveloper ? 1 : users.Count;
            int totalTasks = tasks.Count;

            int activeTasks = tasks.Count(task => task.Status == "InProgress");

            int completedTasks = tasks.Count(task => task.Status == "Done");

            int overdueTasks = tasks.Count(task => task.DueDate < DateTime.UtcNow && task.Status != "Done");

            double completionRate = totalTasks == 0
                ? 0
                : (double)completedTasks / totalTasks * 100;

            return new DashboardDTO
            {
                TotalProjects = totalProjects,
                TotalUsers = totalUsers,
                TotalTasks = totalTasks,
                ActiveTasks = activeTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                CompletionRate = completionRate
            };
        }
    }
}
