using DevTasker.Domain.Classes;
using System.Text.Json.Serialization;

namespace DevTasker.Api.DTO
{
    public class ProjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }
    }

    public class ProjectDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("tasks")]
        public ICollection<TaskItemDetailsDto> TaskItems { get; set; } = new List<TaskItemDetailsDto>();

    }
}
