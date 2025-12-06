using DevTasker.Domain.Classes;
using DevTasker.Api.DTO;

namespace DevTasker.Api.Mapping
{
    public static class WorkLogMapping
    {
        public static WorkLogDto ToDto(this WorkLog workLog)
        {
            return new WorkLogDto
            {
                Id         = workLog.Id,
                TaskItemId = workLog.TaskItemId,
                StartedAt  = workLog.StartedAt,
                EndedAt    = workLog.EndedAt,
                Comment    = workLog.Comment
            };
        }
    }
}
