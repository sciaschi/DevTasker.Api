using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTasker.Domain.Interface
{
    public interface ITaskItemService
    {
        Task<TaskItem> CreateTask(int? projectId, string title, string? description,
            TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate, DateTime? CompletedAt);
        Task<TaskItem> UpdateTask(int taskId, string title, string? description,
            TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate, DateTime? CompletedAt);
        Task<IEnumerable<TaskItem>> GetAllTasksForProject(int projectId, bool withDetails = false);
        Task<IEnumerable<TaskItem>> GetAllTasks(bool withDetails = false);
        Task<TaskItem> GetTaskById(int taskId, bool withDetails = false);
        Task<TaskItem> UpdateTaskStatus(int taskId, TaskItemStatus newStatus);
    }
}
