using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using DevTasker.Domain.ServiceLayer;
using DevTasker.Domain.Utilities;
using DevTasker.Infrastructure;
using DevTasker.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace DevTasker.Test
{
    public class TaskItemsTests
    {
        private readonly Mock<ITaskItemRepository> _mockRepository;
        private readonly TaskItemService _service;
        private readonly Mock<ILogger<TaskItemService>> _mockLogger;

        public TaskItemsTests(ITestOutputHelper output) {
            _mockRepository        = new Mock<ITaskItemRepository>();
            _mockLogger            = new Mock<ILogger<TaskItemService>>();
            _service               = new TaskItemService(_mockRepository.Object, _mockLogger.Object);
        }

        private DevTaskerDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DevTaskerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new DevTaskerDbContext(options);
        }

        [Fact]
        public async Task CreateTaskAsync_Test()
        {
            var taskItem = new TaskItem
            {
                Id = 1,
                ProjectId = 1001,
                Title = "Test Task",
                Status = TaskItemStatus.Todo,
                Priority = TaskItemPriority.Medium
            };

            _mockRepository.Setup(r => r.CreateTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync(taskItem);

            var result = await _service.CreateTask(taskItem.ProjectId, taskItem.Title, taskItem.Description, 
                taskItem.Status, taskItem.Priority, taskItem.DueDate, taskItem.CompletedAt);

            Assert.NotNull(result);
            Assert.Equal(taskItem.Id, result.Id);
            _mockRepository.Verify(r => r.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAsync_ThrowsException_MissingTitle()
        {
            var taskItem = new TaskItem
            {
                ProjectId = 1001,
                Status = TaskItemStatus.Todo,
                Priority = TaskItemPriority.Medium
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateTask(taskItem.ProjectId, taskItem.Title, taskItem.Description,
                taskItem.Status, taskItem.Priority, taskItem.DueDate, taskItem.CompletedAt)
            );

            Assert.Contains("Title is required and must be at least 3 characters.", ex.Message);
        }

        [Fact]
        public async Task CreateTaskAsync_ThrowsException_MissingProjectId()
        {
            var taskItem = new TaskItem
            {
                Title = "Mock Task 1",
                Status = TaskItemStatus.Todo,
                Priority = TaskItemPriority.Medium
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateTask(taskItem.ProjectId, taskItem.Title, taskItem.Description,
                taskItem.Status, taskItem.Priority, taskItem.DueDate, taskItem.CompletedAt)
            );
            Assert.Contains("Project ID is required.", ex.Message);
        }

        [Fact]
        public async Task CreateTaskAsync_ThrowsException_ProjectIsArchived()
        {
            using var db = CreateInMemoryContext();

            db.Projects.Add(new Project
            {
                Id = 1001,
                Name = "Archived project",
                IsArchived = true
            });
            await db.SaveChangesAsync();

            var taskItemRepository = new TaskItemRepository(db);

            var taskItem = new TaskItem
            {
                ProjectId = 1001,
                Title = "Mock Task 1",
                Status = TaskItemStatus.Todo,
                Priority = TaskItemPriority.Medium
            };

            var ex = await Assert.ThrowsAsync<ForbiddenException>(
                () => taskItemRepository.CreateTaskAsync(taskItem)
            );

            Assert.Equal("You cannot add a task to an archived project. (ProjectId: 1001)", ex.Message);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_Success()
        {
            var expectedStatus = TaskItemStatus.Done;
            var taskItem = new TaskItem
            {
                Id = 1,
                ProjectId = 1001,
                Title = "Test Task",
                Status = expectedStatus
            };

            _mockRepository.Setup(r => r.UpdateTaskStatusAsync(taskItem.Id, expectedStatus)).ReturnsAsync(taskItem);


            var result = await _service.UpdateTaskStatus(taskItem.Id, expectedStatus);


            Assert.NotNull(result);
            Assert.Equal(expectedStatus, result.Status);

            _mockRepository.Verify(r => r.UpdateTaskStatusAsync(taskItem.Id, expectedStatus), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_TaskNotFound_ThrowsNotFoundException()
        {
            var taskId = 1;
            var expectedStatus = TaskItemStatus.Done;

            var ex = await Assert.ThrowsAsync<NotFoundException>(
                () => _service.UpdateTaskStatus(taskId, expectedStatus)
            );

            Assert.Contains("Task 1", ex.Message);
        }
    }
}


