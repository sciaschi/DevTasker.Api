using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTasker.Domain.Interface
{
    public interface IProjectRepository
    {
        Task<Project> CreateProjectAsync(Project project);
        Task<Project?> GetProjectAsync(int id, bool withDetails = false);
        Task<IEnumerable<Project>> GetAllProjectsAsync(bool withDetails = false);
        Task<Project?> ArchiveProjectAsync(int id);
    }
}
