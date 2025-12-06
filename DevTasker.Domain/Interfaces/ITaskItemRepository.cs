using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTasker.Domain.Interface
{
    public interface ITaskItemRepository
    {
        Task<TaskItem> CreateTaskAsync(TaskItem taskItem);
        Task<TaskItem?> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus);
        Task<IEnumerable<TaskItem>> GetAllTasksForProjectAsync(int projectId, bool withDetails = false);
        Task<TaskItem?> GetTaskByIdAsync(int taskId, bool withDetails = false);
    }
}
