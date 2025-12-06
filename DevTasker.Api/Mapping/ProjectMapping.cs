using DevTasker.Domain.Classes;
using DevTasker.Api.DTO;

namespace DevTasker.Api.Mapping
{
    public static class ProjectMapping
    {
        public static ProjectDto ToDto(this Project project)
        {
            return new ProjectDto
            {
                Id          = project.Id,
                Name        = project.Name,
                Description = project.Description,
                CreatedAt   = project.CreatedAt,
                IsArchived  = project.IsArchived
            };
        }
        public static ProjectDetailsDto ToDetailsDto(this Project project)
        {
            var taskItemsDto = project.TaskItems
                .Select(ti => ti.ToDetailsDto())
                .ToList();

            return new ProjectDetailsDto
            {
                Id          = project.Id,
                Name        = project.Name,
                Description = project.Description,
                CreatedAt   = project.CreatedAt,
                IsArchived  = project.IsArchived,
                TaskItems   = taskItemsDto
            };
        }
    }
}
