using DevTasker.Domain.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DevTasker.Domain.Dto.TaskItem.Requests
{
    public class CreateTaskItemRequest
    {
        [Required(ErrorMessage = "project_id is required.")]
        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [Required(ErrorMessage = "title is required.")]
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
    }
}
