using DevInsightAPI.DTOs;
using DevInsightAPI.Models;

namespace DevInsightAPI.Mappings
{
    public static class ApiMappings
    {
        public static UserSummaryDTO ToSummaryDto(this User user)
        {
            return new UserSummaryDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        public static UserDTO ToDto(this User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                AssignedTaskCount = user.AssignedTasks.Count,
                CreatedProjectCount = user.CreatedProjects.Count
            };
        }

        public static ProjectDTO ToDto(this Project project)
        {
            return new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                CreatedByUserId = project.CreatedByUserId,
                CreatedByUser = project.CreatedByUser?.ToSummaryDto(),
                TaskCount = project.Tasks.Count,
                CompletedTaskCount = project.Tasks.Count(task => task.Status == "Done")
            };
        }

        public static NotificationDTO ToDto(this Notification notification)
        {
            return new NotificationDTO
            {
                Id = notification.Id,
                Message = notification.Message,
                UserId = notification.UserId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }

        public static TaskDTO ToDto(this TaskItem task)
        {
            return new TaskDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name ?? string.Empty,
                AssignedUserId = task.AssignedUserId,
                AssignedUser = task.AssignedUser?.ToSummaryDto(),
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate
            };
        }
    }
}
