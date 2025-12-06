using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using DevTasker.Domain.Utilities;
using Microsoft.Extensions.Logging;

namespace DevTasker.Domain.ServiceLayer
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IProjectRepository projectRepository, ILogger<ProjectService> logger) 
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }

        public async Task<Project> CreateProject(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                throw new ValidationException("Name is required and must be at least 3 characters.");

            Project project = new Project
            {
                Name        = name,
                Description = description,
                IsArchived  = false
            };

            var res = await _projectRepository.CreateProjectAsync(project);

            _logger.LogInformation("Created project {ProjectId}", res.Id);
            return res;
        }

        public async Task<IEnumerable<Project>> GetAllProjects(bool withDetails = false)
        {
            return await _projectRepository.GetAllProjectsAsync(withDetails);
        }

        public async Task<Project> GetProjectById(int id, bool withDetails = false)
        {
            var project = await _projectRepository.GetProjectAsync(id, withDetails);

            if (project == null)
            {
                _logger.LogWarning("Project {ProjectId} not found", id);
                throw new NotFoundException($"Project {id} not found.");
            }

            return project;
        }

        public async Task<Project> Archive(int id)
        {
            var project = await _projectRepository.ArchiveProjectAsync(id);

            if (project == null)
            {
                _logger.LogWarning("Project {ProjectId} not found when toggling archive", id);
                throw new NotFoundException($"Project {id} not found when toggling archive.");
            }

            _logger.LogInformation("Toggled archive for project {ProjectId} to {IsArchived}",
                project.Id, project.IsArchived);

            return project;
        }
    }
}
