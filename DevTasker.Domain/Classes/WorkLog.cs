using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevTasker.Domain.Classes
{
    [Table("work_logs")]
    public class WorkLog
    {
        public int Id { get; set; }

        [Required]
        [Column("task_item_id")]
        [JsonPropertyName("task_item_id")]
        public int TaskItemId { get; set; }

        [Column("started_at")]
        [JsonPropertyName("started_at")]
        public DateTime? StartedAt { get; set; }

        [Column("ended_at")]
        [JsonPropertyName("ended_at")]
        public DateTime? EndedAt { get; set; }

        [Column("comment")]
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}
