using System.Text.Json.Serialization;

namespace DevTasker.Api.DTO
{
    public class WorkLogDto
    {
        public int Id { get; set; }

        [JsonPropertyName("task_item_id")]
        public int TaskItemId { get; set; }

        [JsonPropertyName("started_at")]
        public DateTime? StartedAt { get; set; }

        [JsonPropertyName("ended_at")]
        public DateTime? EndedAt { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}
