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
    public class ProjectTests
    {
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly ProjectService _service;
        private readonly Mock<ILogger<ProjectService>> _mockLogger;

        public ProjectTests(ITestOutputHelper output) {
            _mockRepository        = new Mock<IProjectRepository>();
            _mockLogger            = new Mock<ILogger<ProjectService>>();
            _service               = new ProjectService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateProjectAsync_Successful()
        {
            Project project = new Project
            {
                Id = 1001,
                Name = "Mock Project 1",
                Description = "Test Description",
                IsArchived = false
            };

            _mockRepository.Setup(r => r.CreateProjectAsync(It.IsAny<Project>())).ReturnsAsync(project);

            var result = await _service.CreateProject(project.Name, project.Description);

            Assert.NotNull(result);
            Assert.Equal(project.Id, result.Id);
            _mockRepository.Verify(r => r.CreateProjectAsync(It.IsAny<Project>()), Times.Once);
        }

        [Fact]
        public async Task CreateProjectAsync_NameMissing_ThrowsValidationError()
        {
            Project project = new Project
            {
                Description = "Test Description",
                IsArchived = false
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateProject(project.Name, project.Description)
            );

            Assert.Contains("Name is required and must be at least 3 characters.", ex.Message);
        }

        [Fact]
        public async Task ArchiveProjectAsync_Success()
        {
            var project = new Project
            {
                Id = 1001,
                Name = "Mock Project 1",
                Description = "Test Description",
                IsArchived = false
            };

            _mockRepository.Setup(r => r.ArchiveProjectAsync(project.Id)).ReturnsAsync(project);


            var result = await _service.Archive(project.Id);

            Assert.NotNull(result);
            Assert.Equal(project.IsArchived, result.IsArchived);

            _mockRepository.Verify(r => r.ArchiveProjectAsync(project.Id), Times.Once);
        }

        [Fact]
        public async Task ArchiveProjectAsync_ProjectId_ThrowsNotFoundException()
        {
            var projectId = 1;

            var ex = await Assert.ThrowsAsync<NotFoundException>(
                () => _service.Archive(projectId)
            );

            Assert.Contains("Project 1 not found when toggling archive.", ex.Message);
        }
    }
}


