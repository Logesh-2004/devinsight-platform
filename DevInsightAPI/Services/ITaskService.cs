using DevInsightAPI.DTOs;

namespace DevInsightAPI.Services
{
    public interface ITaskService
    {
        Task<List<TaskDTO>> GetAllTasks();

        Task<TaskDTO?> GetTaskById(int id);

        Task<TaskDTO> CreateTask(CreateTaskDTO dto);

        Task<TaskDTO?> UpdateTask(int id, UpdateTaskDTO dto);

        Task<TaskDTO?> UpdateTaskStatus(int id, string status);

        Task<bool> DeleteTask(int id);
    }
}
