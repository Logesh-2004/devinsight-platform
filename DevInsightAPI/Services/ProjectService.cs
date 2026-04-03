using DevInsightAPI.Repositories;
using DevInsightAPI.DTOs;
using DevInsightAPI.Mappings;
using DevInsightAPI.Models;

namespace DevInsightAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repository;
        private readonly IUserRepository _userRepository;

        public ProjectService(IProjectRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<List<ProjectDTO>> GetAllProjects()
        {
            var projects = await _repository.GetAllAsync();
            return projects.Select(project => project.ToDto()).ToList();
        }

        public async Task<ProjectDTO?> GetProjectById(int id)
        {
            var project = await _repository.GetByIdAsync(id);
            return project?.ToDto();
        }

        public async Task<ProjectDTO> CreateProject(CreateProjectDTO dto)
        {
            await ValidateCreator(dto.CreatedByUserId);

            var project = new Project
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                CreatedByUserId = dto.CreatedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            var createdProject = await _repository.CreateAsync(project);
            return createdProject.ToDto();
        }

        public async Task<ProjectDTO?> UpdateProject(int id, UpdateProjectDTO dto)
        {
            var existingProject = await _repository.GetByIdAsync(id);

            if (existingProject == null)
            {
                return null;
            }

            await ValidateCreator(dto.CreatedByUserId);

            existingProject.Name = dto.Name.Trim();
            existingProject.Description = dto.Description.Trim();
            existingProject.CreatedByUserId = dto.CreatedByUserId;

            var updatedProject = await _repository.UpdateAsync(existingProject);
            return updatedProject.ToDto();
        }

        public async Task<bool> DeleteProject(int id)
        {
            var existingProject = await _repository.GetByIdAsync(id);

            if (existingProject == null)
            {
                return false;
            }

            await _repository.DeleteAsync(existingProject);
            return true;
        }

        private async Task ValidateCreator(int? createdByUserId)
        {
            if (createdByUserId.HasValue && !await _userRepository.ExistsAsync(createdByUserId.Value))
            {
                throw new InvalidOperationException("The selected project owner does not exist.");
            }
        }
    }
}
