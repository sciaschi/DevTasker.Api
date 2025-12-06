using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTasker.Domain.Interface
{
    public interface IWorkLogService
    {
        Task<WorkLog> CreateWorkLog(int taskItemId, DateTime? startedAt, DateTime? endedAt, string? comment);
        Task<IEnumerable<WorkLog>> GetWorkLogsForTask(int taskId);
        Task<WorkLog> GetWorkLogById(int taskId);
    }
}
