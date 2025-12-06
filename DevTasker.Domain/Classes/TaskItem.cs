using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevTasker.Domain.Classes
{
    public enum TaskItemStatus
    {
        Todo,
        InProgress,
        Done,
        Blocked,
    }

    public enum TaskItemPriority
    {
        VeryHigh,
        High,
        Medium,
        Low,
        VeryLow,
    }

    [Table("task_items")]
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "project_id is required.")]
        [Column("project_id")]
        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status")]
        public TaskItemStatus Status { get; set; }

        [Column("priority")]
        public TaskItemPriority Priority { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("is_archived")]
        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("work_logs")]
        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
