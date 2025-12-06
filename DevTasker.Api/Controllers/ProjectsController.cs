using DevTasker.Api.Mapping;
using DevTasker.Api.DTO;
using DevTasker.Api.Dto.Project.Requests;
using DevTasker.Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DevTasker.Api.Controllers
{

    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private IProjectService _service;

        public ProjectsController (IProjectService service)
        {
            _service = service;
        }

        // GET: api/projects/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAllProjects()
        {
            var res = await _service.GetAllProjects();
            var projects = res.Select(val => val.ToDto());

            return Ok(projects);
        }

        // GET: api/projects/all/details
        [HttpGet("all/details")]
        public async Task<ActionResult<IEnumerable<ProjectDetailsDto>>> GetAllProjectDetails()
        {
            var res = await _service.GetAllProjects(true);
            var projects = res.Select(val => val.ToDetailsDto());

            return Ok(projects);
        }

        // GET: api/projects/{id:int}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var project = await _service.GetProjectById(id);
            
            return Ok(project.ToDto());
        }

        // GET: api/projects/{id:int}/details
        [HttpGet("{id:int}/details")]
        public async Task<ActionResult<ProjectDetailsDto>> GetProjectDetailsById(int id)
        {
            var projectDetails = await _service.GetProjectById(id, true);

            return Ok(projectDetails.ToDetailsDto());
        }

        // POST: api/projects/create
        [HttpPost("create")]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request)
        {
            var project = await _service.CreateProject(request.Name, request.Description);

            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project.ToDto());
        }

        // POST: api/projects/update
        [HttpPut("update")]
        public async Task<ActionResult<ProjectDto>> UpdateProject([FromBody] UpdateProjectRequest request)
        {
            var project = await _service.UpdateProject(request.ProjectId, request.Name, request.Description);

            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project.ToDto());
        }

        // PUT: api/projects/{id:int}/archive
        [HttpPatch("{id:int}/archive")]
        public async Task<ActionResult<ProjectDto>> ChangeProjectArchived(int id)
        {
            var project = await _service.Archive(id);

            return Ok(project.ToDto()); 
        }
    }
}
