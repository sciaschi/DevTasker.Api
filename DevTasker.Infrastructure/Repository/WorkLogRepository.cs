using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace DevTasker.Infrastructure.Repository
{
    public class WorkLogRepository : IWorkLogRepository
    {
        private readonly DevTaskerDbContext _db;
        public WorkLogRepository(DevTaskerDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WorkLog>> GetLogsForTaskAsync(int taskId)
        {
            return await _db.WorkLogs.Where(x => x.TaskItemId == taskId).ToListAsync();
        }

        public async Task<WorkLog?> GetWorkLogById(int workLogId)
        {
            return await _db.WorkLogs.SingleOrDefaultAsync(x => x.Id == workLogId);
        }

        public async Task<WorkLog> CreateWorkLogAsync(WorkLog workLog)
        {
            await _db.WorkLogs.AddAsync(workLog);
            await _db.SaveChangesAsync();

            return workLog;
        }
    }
}
