using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DevTasker.Api.Dto.Project.Requests
{
    public class UpdateProjectRequest
    {
        [Required]
        [JsonPropertyName("project_id")]
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
