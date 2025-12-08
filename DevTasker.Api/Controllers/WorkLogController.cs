using DevTasker.Api.Dto.WorkLog.Requests;
using DevTasker.Api.DTO;
using DevTasker.Api.Hubs;
using DevTasker.Api.Mapping;
using DevTasker.Domain.Classes;
using DevTasker.Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DevTasker.Api.Controllers
{
    [ApiController]
    [Route("api/worklogs")]
    public class WorkLogController : ControllerBase
    {
        private IWorkLogService _service;
        private readonly IHubContext<NotificationHub> _hubContext;

        public WorkLogController(IWorkLogService service, IHubContext<NotificationHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        // POST: api/worklogs/create
        [HttpPost("create")]
        public async Task<ActionResult<WorkLogDto>> Create([FromBody] CreateWorkLogRequest request)
        {
            var workLog = await _service.CreateWorkLog(request.TaskItemId, request.StartedAt,
                                                       request.EndedAt, request.Comment);

            await _hubContext.Clients.All.SendAsync("WorkLogCreated", workLog.ToDto());

            return CreatedAtAction(nameof(GetWorkLogById), new { workLogId = workLog.Id }, workLog.ToDto());
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
