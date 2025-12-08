using DevTasker.Api.DTO;
using DevTasker.Api.Mapping;
using DevTasker.Domain.Classes;
using DevTasker.Api.Dto.TaskItem.Requests;
using DevTasker.Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DevTasker.Api.Hubs;

namespace DevTasker.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskItemsController : ControllerBase
    {
        private ITaskItemService _service;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TaskItemsController(ITaskItemService service, IHubContext<NotificationHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        // GET: api/tasks/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTasks()
        {
            var res = await _service.GetAllTasks();
            var taskItems = res.Select(val => val.ToDto());

            return Ok(taskItems);
        }

        // GET: api/tasks/all
        [HttpGet("all/details")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTasksWithDetails()
        {
            var res = await _service.GetAllTasks(true);
            var taskItems = res.Select(val => val.ToDetailsDto());

            return Ok(taskItems);
        }


        // GET: api/tasks/project/{projectId}
        [HttpGet("project/{projectId:int}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTasksForProject(int projectId)
        {
            var res = await _service.GetAllTasksForProject(projectId);
            var taskItems = res.Select(val => val.ToDto());

            return Ok(taskItems);
        }

        // GET: api/tasks/project/{projectId}
        [HttpGet("project/{projectId:int}/details")]
        public async Task<ActionResult<IEnumerable<TaskItemDetailsDto>>> GetAllTaskDetailsForProject(int projectId)
        {
            var res = await _service.GetAllTasksForProject(projectId, true);
            var taskItems = res.Select(val => val.ToDetailsDto());

            return Ok(taskItems);
        }

        // GET: api/tasks/{taskId}
        [HttpGet("{taskId:int}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskById(int taskId)
        {
            var taskItem = await _service.GetTaskById(taskId);

            return Ok(taskItem.ToDto());
        }

        // GET: api/tasks/{taskId}/details
        [HttpGet("{taskId:int}/details")]
        public async Task<ActionResult<TaskItemDto>> GetTaskDetailsById(int taskId)
        {
            var taskItem = await _service.GetTaskById(taskId, true);

            return Ok(taskItem.ToDetailsDto());
        }

        // POST: api/tasks/create
        [HttpPost("create")]
        public async Task<ActionResult<TaskItemDto>> CreateTaskItem([FromBody] CreateTaskItemRequest request)
        {
            var taskItem = await _service.CreateTask(request.ProjectId, request.Title, request.Description,
                                                        request.Status, request.Priority, request.DueDate,
                                                        request.CompletedAt);

            await _hubContext.Clients.All.SendAsync("TaskCreated", taskItem.ToDto());

            return CreatedAtAction(nameof(GetTaskById), new { taskId = taskItem.Id }, taskItem);
        }

        // POST: api/tasks/update
        [HttpPut("update")]
        public async Task<ActionResult<TaskItemDto>> UpdateTaskItem([FromBody] UpdateTaskItemRequest request)
        {
            var taskItem = await _service.UpdateTask(request.Id, request.Title, request.Description,
                                                        request.Status, request.Priority, request.DueDate,
                                                        request.CompletedAt);

            await _hubContext.Clients.All.SendAsync("TaskUpdated", taskItem.ToDto());

            return CreatedAtAction(nameof(GetTaskById), new { taskId = taskItem.Id }, taskItem.ToDto());
        }

        // PATCH: api/tasks/{taskId}/status
        [HttpPatch("{taskId:int}/status")]
        public async Task<ActionResult<TaskItemDto>> UpdateTaskStatus(int taskId, [FromBody] UpdateTaskStatusRequest request)
        {
            Enum.TryParse<TaskItemStatus>(request.Status.ToString(), out var taskStatus);
            var res = await _service.UpdateTaskStatus(taskId, taskStatus);

            await _hubContext.Clients.All.SendAsync("TaskUpdated", res.ToDto());

            return Ok(res.ToDto());
        }
    }
}
