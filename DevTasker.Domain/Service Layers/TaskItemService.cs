using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using DevTasker.Domain.Utilities;
using Microsoft.Extensions.Logging;

namespace DevTasker.Domain.ServiceLayer
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly ILogger<TaskItemService> _logger;

        public TaskItemService(ITaskItemRepository taskItemRepository, ILogger<TaskItemService> logger)
        {
            _taskItemRepository = taskItemRepository;
            _logger = logger;
        }

        public async Task<TaskItem> CreateTask(int? projectId, string title, string? description,
            TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate, DateTime? CompletedAt)
        {
            if (projectId == null)
                throw new ValidationException("Project ID is required.");

            if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
                throw new ValidationException("Title is required and must be at least 3 characters.");

            var task = new TaskItem
            {
                ProjectId   = projectId,
                Title       = title,
                Description = description,
                Status      = status,
                Priority    = priority,
                DueDate     = dueDate,
                CompletedAt = CompletedAt
            };

            _logger.LogInformation("Creating new task for project {ProjectId} with title {Title}", projectId, title);
            return await _taskItemRepository.CreateTaskAsync(task);
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasks(bool withDetails = false)
        {
            return await _taskItemRepository.GetAllTasksAsync(withDetails);
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksForProject(int projectId, bool withDetails = false)
        {
            return await _taskItemRepository.GetAllTasksForProjectAsync(projectId, withDetails);
        }

        public async Task<TaskItem> GetTaskById(int taskId, bool withDetails = false)
        {
            var task = await _taskItemRepository.GetTaskByIdAsync(taskId, withDetails);

            if(task == null)
            {
                _logger.LogWarning("Task {taskId} not found", taskId);
                throw new NotFoundException($"Task {taskId} not found.");
            }

            return task;
        }

        public async Task<TaskItem> UpdateTask(int taskId, string title, string? description,
            TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate, DateTime? CompletedAt)
        {
            var taskItemNew = new TaskItem
            {
                Title       = title,
                Description = description,
                Status      = status,
                Priority    = priority,
                DueDate     = dueDate,
                CompletedAt = CompletedAt
            };

            var task = await _taskItemRepository.UpdateTaskAsync(taskId, taskItemNew);

            if (task == null)
            {
                _logger.LogWarning("Task {taskId} not found when trying to update", taskId);
                throw new NotFoundException($"Task {taskId} not found.");
            }

            _logger.LogInformation("Updated task {taskId}", task.Id);

            return task;
        }

        public async Task<TaskItem> UpdateTaskStatus(int taskId, TaskItemStatus newStatus)
        {
            var task = await _taskItemRepository.UpdateTaskStatusAsync(taskId, newStatus);

            if (task == null)
            {
                _logger.LogWarning("Task {taskId} not found when toggling archive", taskId);
                throw new NotFoundException($"Task {taskId} not found.");
            }

            _logger.LogInformation("Changed status for task {taskId} to {}", task.Id, task.IsArchived);

            return task;
        }

        public async Task<TaskItem> UpdateTaskPriority(int taskId, TaskItemPriority newPriority)
        {
            var task = await _taskItemRepository.UpdateTaskPriorityAsync(taskId, newPriority);

            if (task == null)
            {
                _logger.LogWarning("Task {taskId} not found when setting priority {newPriority}", taskId, newPriority);
                throw new NotFoundException($"Task {taskId} not found.");
            }

            _logger.LogInformation("Changed priority for task {taskId}", task.Id);

            return task;
        }
    }
}
