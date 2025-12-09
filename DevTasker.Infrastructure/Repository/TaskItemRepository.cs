using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using DevTasker.Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DevTasker.Infrastructure.Repository
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly DevTaskerDbContext _db;

        public TaskItemRepository(DevTaskerDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksForProjectAsync(int projectId, bool withDetails = false)
        {
            return withDetails ? await _db.TaskItems.Include(x => x.WorkLogs).Where(x => x.ProjectId == projectId).ToListAsync() :
                await _db.TaskItems.Where(x => x.ProjectId == projectId).ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(bool withDetails = false)
        {
            return withDetails ? await _db.TaskItems.Include(x => x.WorkLogs).ToListAsync() :await _db.TaskItems.ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId, bool withDetails = false)
        {
            return withDetails ? await _db.TaskItems.Include(x => x.WorkLogs).SingleOrDefaultAsync(x => x.Id == taskId) :
                await _db.TaskItems.SingleOrDefaultAsync(x => x.Id == taskId);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem taskItem)
        {
            var project = await _db.Projects.SingleOrDefaultAsync(x => x.Id == taskItem.ProjectId);

            if (project == null)
                throw new NotFoundException($"Project {taskItem.ProjectId} was not found.");

            if (project.IsArchived)
                throw new ForbiddenException(
                    $"You cannot add a task to an archived project. (ProjectId: {taskItem.ProjectId})");

            await _db.TaskItems.AddAsync(taskItem);
            await _db.SaveChangesAsync();

            return taskItem;
        }

        public async Task<TaskItem> UpdateTaskAsync(int taskId, TaskItem taskItemNew)
        {
            var taskItemOld = await _db.TaskItems.SingleOrDefaultAsync(x => x.Id == taskId);

            if (taskItemOld == null)
                throw new NotFoundException($"Task {taskId} was not found.");

            taskItemOld.Title       = taskItemNew.Title;
            taskItemOld.Description = taskItemNew.Description;
            taskItemOld.Status      = taskItemNew.Status;
            taskItemOld.Priority    = taskItemNew.Priority;
            taskItemOld.DueDate     = taskItemNew.DueDate;
            taskItemOld.CompletedAt = taskItemNew.CompletedAt;

            _db.TaskItems.Update(taskItemOld);
            await _db.SaveChangesAsync();

            return taskItemOld;
        }

        public async Task<TaskItem?> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus)
        {
            var taskItem = await _db.TaskItems.SingleOrDefaultAsync(x => x.Id == taskId);

            if (taskItem == null)
                throw new NotFoundException($"Task {taskId} was not found.");

            taskItem.Status = newStatus;

            _db.TaskItems.Update(taskItem);
            await _db.SaveChangesAsync();

            return taskItem;
        }

        public async Task<TaskItem?> UpdateTaskPriorityAsync(int taskId, TaskItemPriority newPriority)
        {
            var taskItem = await _db.TaskItems.SingleOrDefaultAsync(x => x.Id == taskId);

            if (taskItem == null)
                throw new NotFoundException($"Task {taskId} was not found.");

            taskItem.Priority = newPriority;

            _db.TaskItems.Update(taskItem);
            await _db.SaveChangesAsync();

            return taskItem;
        }
    }
}
