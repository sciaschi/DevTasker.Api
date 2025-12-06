using DevTasker.Domain.Classes;
using DevTasker.Domain.Utilities;
using DevTasker.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevTasker.Infrastructure.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DevTaskerDbContext _db;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(DevTaskerDbContext db, ILogger<ProjectRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Project?> GetProjectAsync(int id, bool withDetails = false)
        {
            return withDetails ? await _db.Projects.Include(x => x.TaskItems)
                .ThenInclude(x => x.WorkLogs)
                .SingleOrDefaultAsync(x => x.Id == id) : 
                await _db.Projects.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync(bool withDetails = false)
        {
            return withDetails ? await _db.Projects.Include(x => x.TaskItems)
                .ThenInclude(x => x.WorkLogs).ToListAsync() :
                await _db.Projects.ToListAsync(); 
               
        }

        public async Task<Project> UpdateProjectAsync(int projectId, string name, string? description)
        {
            var existingProject =  await _db.Projects.SingleOrDefaultAsync(x => x.Id == projectId);
            if (existingProject == null)
                throw new NotFoundException($"Project {projectId} not found.");

            existingProject.Name = name;
            existingProject.Description = description;

            _db.Projects.Update(existingProject);
            await _db.SaveChangesAsync();

            return existingProject;
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();

            return project;
        }

        public async Task<Project?> ArchiveProjectAsync(int id)
        {
            var project = await _db.Projects.SingleOrDefaultAsync(x => x.Id == id);

            if (project == null)
                throw new NotFoundException($"Project {id} not found.");

            project.IsArchived = !project.IsArchived;

            await _db.SaveChangesAsync();

            return project;
        }
    }
}
