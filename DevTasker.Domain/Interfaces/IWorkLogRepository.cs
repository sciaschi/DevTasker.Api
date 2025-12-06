using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTasker.Domain.Interface
{
    public interface IWorkLogRepository
    {
        Task<WorkLog> CreateWorkLogAsync(WorkLog workLog);
        Task<WorkLog?> GetWorkLogById(int workLogId);
        Task<IEnumerable<WorkLog>> GetLogsForTaskAsync(int taskId);
    }
}
