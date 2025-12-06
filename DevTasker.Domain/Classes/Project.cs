using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevTasker.Domain.Classes
{
    [Table("projects")]
    public class Project
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("is_archived")]
        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("task_items")]
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
