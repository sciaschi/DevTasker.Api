using DevTasker.Api.Mapping;
using DevTasker.Domain.Dto.WorkLog.Requests;
using DevTasker.Api.DTO;
using DevTasker.Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DevTasker.Api.Controllers
{
    [ApiController]
    [Route("api/worklogs")]
    public class WorkLogController : ControllerBase
    {
        private IWorkLogService _service;

        public WorkLogController(IWorkLogService service)
        {
            _service = service;
        }

        // POST: api/worklogs/create
        [HttpPost("create")]
        public async Task<ActionResult<WorkLogDto>> Create([FromBody] CreateWorkLogRequest request)
        {
            var workLog = await _service.CreateWorkLog(request.TaskItemId, request.StartedAt,
                                                       request.EndedAt, request.Comment);

            return CreatedAtAction(nameof(GetWorkLogById), new { workLogId = workLog.Id }, workLog);
        }

        // GET: api/worklogs/task/{taskItemId}
        [HttpGet("task/{taskItemId:int}")]
        public async Task<ActionResult<IEnumerable<WorkLogDto>>> GetWorkLogsForTask(int taskItemId)
        {
            var res = await _service.GetWorkLogsForTask(taskItemId);
            var workLogs = res.Select(val => val.ToDto());

            return Ok(workLogs);
        }

        // GET: api/worklogs/{taskItemId}
        [HttpGet("{workLogId:int}")]
        public async Task<ActionResult<WorkLogDto>> GetWorkLogById(int workLogId)
        {
            var workLog = await _service.GetWorkLogById(workLogId);

            return Ok(workLog.ToDto());
        }
    }
}
