using DevTasker.Api.DTO;
using DevTasker.Domain.Classes;
using System.Globalization;

namespace DevTasker.Api.Mapping
{
    public static class TaskItemsMapping
    {
        public static TaskItemDto ToDto(this TaskItem taskItem)
        {
            return new TaskItemDto
            {
                Id          = taskItem.Id,
                ProjectId   = taskItem.ProjectId,
                Title       = taskItem.Title,
                Description = taskItem.Description,
                Status      = taskItem.Status,
                Priority    = taskItem.Priority,
                DueDate     = taskItem.DueDate,
                CompletedAt = taskItem.CompletedAt,
                CreatedAt   = taskItem.CreatedAt
            };
        }
        public static TaskItemDetailsDto ToDetailsDto(this TaskItem taskItem)
        {
            var workLogsDto = taskItem.WorkLogs
                .Select(wl => wl.ToDto())
                .ToList();

            return new TaskItemDetailsDto
            {
                Id          = taskItem.Id,
                ProjectId   = taskItem.ProjectId,
                Title       = taskItem.Title,
                Description = taskItem.Description,
                Status      = taskItem.Status,
                Priority    = taskItem.Priority,
                DueDate     = taskItem.DueDate,
                CompletedAt = taskItem.CompletedAt,
                CreatedAt   = taskItem.CreatedAt,
                WorkLogs    = workLogsDto
            };
        }
    }
}
