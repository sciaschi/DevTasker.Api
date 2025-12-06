using DevTasker.Domain.Classes;
using DevTasker.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTasker.Domain.Interface
{
    public interface IProjectService
    {
        Task<Project> CreateProject(string name, string? description);
        Task<IEnumerable<Project>> GetAllProjects(bool withDetails = false);
        Task<Project> GetProjectById(int id, bool withDetails = false);
        Task<Project> Archive(int id);
    }
}
