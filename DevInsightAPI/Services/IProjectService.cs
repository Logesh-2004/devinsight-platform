using DevInsightAPI.DTOs;

namespace DevInsightAPI.Services
{
    public interface IProjectService
    {
        Task<List<ProjectDTO>> GetAllProjects();

        Task<ProjectDTO?> GetProjectById(int id);

        Task<ProjectDTO> CreateProject(CreateProjectDTO dto);

        Task<ProjectDTO?> UpdateProject(int id, UpdateProjectDTO dto);

        Task<bool> DeleteProject(int id);
    }
}
