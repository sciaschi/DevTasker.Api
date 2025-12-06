using DevTasker.Domain.Classes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DevTasker.Api.DTO
{
    public class TaskItemDto
    {
        public int Id { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public TaskItemStatus Status { get; set; }

        [JsonPropertyName("priority")]
        public TaskItemPriority Priority { get; set; }

        [JsonPropertyName("due_date")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }
    }

    public class TaskItemDetailsDto
    {
        public int Id { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public TaskItemStatus Status { get; set; }

        [JsonPropertyName("priority")]
        public TaskItemPriority Priority { get; set; }

        [JsonPropertyName("due_date")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("work_logs")]
        public ICollection<WorkLogDto> WorkLogs { get; set; } = new List<WorkLogDto>();

    }
}
