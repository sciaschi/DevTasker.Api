using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using DevTasker.Domain.ServiceLayer;
using DevTasker.Domain.Utilities;
using DevTasker.Infrastructure;
using DevTasker.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DevTasker.Test
{
    public class WorkLogTests
    {
        private readonly Mock<IWorkLogRepository> _mockRepository;
        private readonly WorkLogService _service;
        private readonly Mock<ILogger<WorkLogService>> _mockLogger;

        public WorkLogTests(ITestOutputHelper output) {
            _mockRepository = new Mock<IWorkLogRepository>();
            _mockLogger     = new Mock<ILogger<WorkLogService>>();
            _service        = new WorkLogService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateWorkLog_Successful()
        {
            WorkLog workLog = new WorkLog
            {
                Id         = 1,
                TaskItemId = 1,
                StartedAt  = DateTime.UtcNow.AddHours(-2),
                EndedAt    = DateTime.UtcNow,
                Comment    = "Worked on task"
            };

            _mockRepository.Setup(r => r.CreateWorkLogAsync(It.IsAny<WorkLog>())).ReturnsAsync(workLog);

            var result = await _service.CreateWorkLog(workLog.TaskItemId, workLog.StartedAt, workLog.EndedAt, workLog.Comment);

            Assert.NotNull(result);
            Assert.Equal(workLog.Id, result.Id);
            _mockRepository.Verify(r => r.CreateWorkLogAsync(It.IsAny<WorkLog>()), Times.Once);
        }

        [Fact]
        public async Task GetWorkLogByID_ID_ThrowsNotFoundException()
        {
            var workLogId = 1;

            var ex = await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetWorkLogById(workLogId)
            );

            Assert.Contains($"WorkLog with ID {workLogId} not found.", ex.Message);
        }
    }
}


