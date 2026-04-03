using DevInsightAPI.Data;
using DevInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DevInsightAPI.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DevInsightDbContext _context;

        public ProjectRepository(DevInsightDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(project => project.CreatedByUser)
                .Include(project => project.Tasks)
                .OrderByDescending(project => project.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(project => project.CreatedByUser)
                .Include(project => project.Tasks)
                .FirstOrDefaultAsync(project => project.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Projects.AnyAsync(project => project.Id == id);
        }

        public async Task<Project> CreateAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(project.Id))!;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(project.Id))!;
        }

        public async Task DeleteAsync(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
