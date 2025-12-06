using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using DevTasker.Domain.Utilities;
using Microsoft.Extensions.Logging;

namespace DevTasker.Domain.ServiceLayer
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IWorkLogRepository _workLogRepository;
        private readonly ILogger<WorkLogService> _logger;


        public WorkLogService(IWorkLogRepository workLogRepository, ILogger<WorkLogService> logger)
        {
            _workLogRepository = workLogRepository;
            _logger = logger;
        }

        public async Task<WorkLog> CreateWorkLog(int taskItemId, DateTime? startedAt, DateTime? endedAt, string? comment)
        {
            var workLog = new WorkLog
            {
                TaskItemId = taskItemId,
                StartedAt  = startedAt,
                EndedAt    = endedAt,
                Comment    = comment
            };

            var res = await _workLogRepository.CreateWorkLogAsync(workLog);

            _logger.LogInformation("Created WorkLog with ID {WorkLogId}.", res.Id);
            return res;
        }

        public async Task<IEnumerable<WorkLog>> GetWorkLogsForTask(int taskId)
        {
            return await _workLogRepository.GetLogsForTaskAsync(taskId);
        }

        public async Task<WorkLog> GetWorkLogById(int workLogId)
        {
            var workLog = await _workLogRepository.GetWorkLogById(workLogId);

            if(workLog == null)
            {
                _logger.LogWarning("WorkLog with ID {WorkLogId} not found.", workLogId);
                throw new NotFoundException($"WorkLog with ID {workLogId} not found.");
            }

            return workLog;
        }
    }
}
