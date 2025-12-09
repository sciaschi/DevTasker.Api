using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTasker.Domain.Interface
{
    public interface ITaskItemRepository
    {
        Task<TaskItem> CreateTaskAsync(TaskItem taskItem);
        Task<TaskItem> UpdateTaskAsync(int taskId, TaskItem taskItemNew);
        Task<TaskItem?> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus);
        Task<TaskItem?> UpdateTaskPriorityAsync(int taskId, TaskItemPriority newPriority);
        Task<IEnumerable<TaskItem>> GetAllTasksForProjectAsync(int projectId, bool withDetails = false);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync(bool withDetails = false);
        Task<TaskItem?> GetTaskByIdAsync(int taskId, bool withDetails = false);
        Task<bool> DeleteTaskAsync(int taskId);
    }
}
