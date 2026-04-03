using DevInsightAPI.Models;

namespace DevInsightAPI.Persistence
{
    public class WorkspaceGraph
    {
        public List<User> Users { get; init; } = [];

        public List<Project> Projects { get; init; } = [];

        public List<TaskItem> Tasks { get; init; } = [];

        public List<Notification> Notifications { get; init; } = [];

        public static WorkspaceGraph From(WorkspaceDataFile data)
        {
            var users = data.Users
                .Select(user => new User
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    PasswordHash = user.PasswordHash,
                    CreatedAt = user.CreatedAt
                })
                .ToDictionary(user => user.Id);

            var projects = data.Projects
                .Select(project => new Project
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt,
                    CreatedByUserId = project.CreatedByUserId
                })
                .ToDictionary(project => project.Id);

            var tasks = data.Tasks
                .Select(task => new TaskItem
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    ProjectId = task.ProjectId,
                    AssignedUserId = task.AssignedUserId,
                    CreatedAt = task.CreatedAt,
                    DueDate = task.DueDate,
                    CompletedAt = task.CompletedAt
                })
                .ToList();

            var notifications = data.Notifications
                .Select(notification => new Notification
                {
                    Id = notification.Id,
                    Message = notification.Message,
                    UserId = notification.UserId,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt
                })
                .ToList();

            foreach (var project in projects.Values)
            {
                if (project.CreatedByUserId.HasValue &&
                    users.TryGetValue(project.CreatedByUserId.Value, out var createdByUser))
                {
                    project.CreatedByUser = createdByUser;
                    createdByUser.CreatedProjects.Add(project);
                }
            }

            foreach (var task in tasks)
            {
                if (projects.TryGetValue(task.ProjectId, out var project))
                {
                    task.Project = project;
                    project.Tasks.Add(task);
                }

                if (task.AssignedUserId.HasValue &&
                    users.TryGetValue(task.AssignedUserId.Value, out var assignedUser))
                {
                    task.AssignedUser = assignedUser;
                    assignedUser.AssignedTasks.Add(task);
                }
            }

            foreach (var notification in notifications)
            {
                if (users.TryGetValue(notification.UserId, out var user))
                {
                    notification.User = user;
                    user.Notifications.Add(notification);
                }
            }

            return new WorkspaceGraph
            {
                Users = users.Values.ToList(),
                Projects = projects.Values.ToList(),
                Tasks = tasks,
                Notifications = notifications
            };
        }
    }
}
